using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Threading;
using BruTile;
using GeoAPI.Coordinates;
using GeoAPI.Geometries;
using SharpMap.Expressions;
using BruTile.Cache;
using System.Data.SQLite;

namespace SharpMap.Data.Providers
{
    public class BrutileProvider : ProviderBase, IRasterProvider
    {

        #region Fields

        private IGeometryFactory _geometryFactory;
        private ICoordinateFactory _coordinateFactory;

        private ITileSource _tileSource;

        //Cache
        private ITileCache<Byte[]> _tileCache;

        private Int32 _pixelSize;


        #endregion

        public static SQLiteConnection MakeSqLiteConnection(String datasource)
        {
            SQLiteConnection cn = new SQLiteConnection(string.Format("Data Source={0}", datasource));
            cn.Open();
            SQLiteCommand cmd = cn.CreateCommand();
            cmd.CommandText =
                "CREATE TABLE IF NOT EXISTS Tiles (level integer, row integer, col integer, size integer, image blob, primary key (level, row, col));";
            cmd.ExecuteNonQuery();
            cn.Close();
            return cn;
        }

        #region Constructors and disposal


        public BrutileProvider(IGeometryFactory geometryFactory, ITileSource tileSource)
            : this(geometryFactory, tileSource, new DbCache<SQLiteConnection>(MakeSqLiteConnection("test.db"), (p, c) => c, "main", "Tiles"))
        {
            
        }

        public BrutileProvider(IGeometryFactory geometryFactory, ITileSource tileSource, ITileCache<byte[]> tileCache)
        {

            _geometryFactory = geometryFactory;
            _coordinateFactory = geometryFactory.CoordinateFactory;
            _tileSource = tileSource;
            _tileCache = tileCache;

            //System.Diagnostics.Debug.Assert(tileSource.Schema.Srs == geometryFactory.Srid);
        }



        protected override void Dispose(bool disposing)
        {
            return;
        }

        #endregion

        public override IExtents GetExtents()
        {
            return _geometryFactory.CreateExtents2D(
                _tileSource.Schema.Extent.MinX,
                _tileSource.Schema.Extent.MinY,
                _tileSource.Schema.Extent.MaxX,
                _tileSource.Schema.Extent.MaxY);
                
        }

        public override string ConnectionId
        {
            get { return _tileSource.Schema.Name; }
        }

        public override object ExecuteQuery(Expression query)
        {
            return ExecuteRasterQuery(query as RasterQueryExpression);
        }

        public IEnumerable<IRasterRecord> ExecuteRasterQuery(RasterQueryExpression query)
        {
            BrutileRasterQueryExpression bquery = query as BrutileRasterQueryExpression;
            if (bquery == null) yield break;
            
            //Extent extent = new Extent(map.Envelope.Min.X, map.Envelope.Min.Y, map.Envelope.Max.X, map.Envelope.Max.Y);
            IExtents mapExtens = query.SpatialPredicate.SpatialExpression.Extents;

            Extent extent = new Extent(mapExtens.Min[Ordinates.X], mapExtens.Min[Ordinates.Y],
                                       mapExtens.Max[Ordinates.X], mapExtens.Max[Ordinates.Y]);
            int level = BruTile.Utilities.GetNearestLevel(_tileSource.Schema.Resolutions, bquery.Resolution.Value);
            IList<TileInfo> tiles = _tileSource.Schema.GetTilesInView(extent, level);

            IList<WaitHandle> waitHandles = new List<WaitHandle>();

            foreach (TileInfo info in tiles)
            {
                if (_tileCache.Find(info.Index) != null) continue;
                AutoResetEvent waitHandle = new AutoResetEvent(false);
                waitHandles.Add(waitHandle);
                ThreadPool.QueueUserWorkItem(GetTileOnThread, new object[] { _tileSource.Provider, info, _tileCache, waitHandle });
            }

            foreach (WaitHandle handle in waitHandles)
                handle.WaitOne();

            foreach (TileInfo info in tiles)
            {
                Byte[] bitmap = _tileCache.Find(info.Index);
                if (bitmap == null) continue;

                IExtents extents = _geometryFactory.CreateExtents2D(info.Extent.MinX, info.Extent.MinY,
                                                                    info.Extent.MaxX, info.Extent.MaxY);

                yield return new BrutileRasterRecord(new MemoryStream(bitmap), extents);

                /*
                PointF min = map.WorldToImage(new SharpMap.Geometries.Point(info.Extent.MinX, info.Extent.MinY));
                PointF max = map.WorldToImage(new SharpMap.Geometries.Point(info.Extent.MaxX, info.Extent.MaxY));

                min = new PointF((float)Math.Round(min.X), (float)Math.Round(min.Y));
                max = new PointF((float)Math.Round(max.X), (float)Math.Round(max.Y));

                graphics.DrawImage(bitmap,
                    new Rectangle((int)min.X, (int)max.Y, (int)(max.X - min.X), (int)(min.Y - max.Y)),
                    0, 0, source.Schema.Width, source.Schema.Height,
                    GraphicsUnit.Pixel,
                    imageAttributes);
                 */
            }
        }

        private static void GetTileOnThread(object parameter)
        {
            object[] parameters = (object[])parameter;
            if (parameters.Length != 4) throw new ArgumentException("Four parameters expected");
            ITileProvider tileProvider = (ITileProvider)parameters[0];
            TileInfo tileInfo = (TileInfo)parameters[1];
            ITileCache<byte[]> bitmaps = (ITileCache<byte[]>)parameters[2];
            AutoResetEvent autoResetEvent = (AutoResetEvent)parameters[3];

            byte[] bytes;
            try
            {
                bytes = tileProvider.GetTile(tileInfo);
                //Bitmap bitmap = new Bitmap(new MemoryStream(bytes));
                bitmaps.Add(tileInfo.Index, bytes);
            }

                /*
            catch (System.WebException ex)
            {
                if (showErrorInTile)
                {
                    //an issue with this method is that one an error tile is in the memory cache it will stay even
                    //if the error is resolved. PDD.
                    Bitmap bitmap = new Bitmap(this.source.Schema.Width, this.source.Schema.Height);
                    Graphics graphics = Graphics.FromImage(bitmap);
                    graphics.DrawString(ex.Message, new Font(FontFamily.GenericSansSerif, 12), new SolidBrush(Color.Black),
                        new RectangleF(0, 0, this.source.Schema.Width, this.source.Schema.Height));
                    bitmaps.Add(tileInfo.Index, bytes);
                }
            }
                 */
            catch (Exception ex)
            {
                //todo: log and use other ways to report to user.
            }
            finally
            {
                autoResetEvent.Set();
            }
        }

        #region properties

        public Int32 PixelSize
        {
            get { return _pixelSize; }
            set { _pixelSize = value; }
        }
        #endregion
    }
}