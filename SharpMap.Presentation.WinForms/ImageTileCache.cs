// Copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using SharpMap.Geometries;
using GeoPoint = SharpMap.Geometries.Point;

namespace SharpMap.Presentation.WinForms
{
    class ImageTileCache
    {
        private const int CacheRowCount = 3;
        private const int CacheColumnCount = 3;
        private Image[] _cachedImages = new Image[CacheColumnCount * CacheRowCount];
        private BoundingBox[] _imageBoundaries = new BoundingBox[CacheColumnCount * CacheRowCount];
        private double[] _zoomLevels = new double[CacheColumnCount * CacheRowCount];
        private SizeF _maxBoundarySize;
        private GeoPoint _cacheCenter;
        private MapPresenter _presenter;

        private enum CachedImageIndicies
        {
            TopLeft = 0,
            TopCenter = 1,
            TopRight = 2,
            MiddleLeft = 3,
            MiddleCenter = 4,
            MiddleRight = 5,
            BottomLeft = 6,
            BottomCenter = 7,
            BottomRight = 8
        }

        public ImageTileCache(MapPresenter presenter)
        {
            if (presenter == null)
                throw new ArgumentNullException("presenter");

            MapPresenter = presenter;

            for (int i = 0; i < CacheColumnCount * CacheRowCount; i++)
            {
                _zoomLevels[i] = 0.0;
                _imageBoundaries[i] = BoundingBox.Empty;
            }
        }

        ~ImageTileCache()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (Image image in _cachedImages)
                {
                    if (image != null)
                        image.Dispose();
                }
            }
        }

        public MapPresenter MapPresenter
        {
            get { return _presenter; }
            set { _presenter = value; }
        }

        public IEnumerable<BoundingBox> ComputeMissingRegions(BoundingBox region)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<KeyValuePair<BoundingBox, Image>> GetCachedImagesForRegion(BoundingBox worldRegion)
        {
            throw new NotImplementedException();
        }

        #region Cache measurement and management

        private void initializeCache(GeoPoint center, double zoomLevel)
        {
            Size cachedImageSize = getCacheImageSize();
            _cacheCenter = center;

            BoundingBox maxBoundary = _cacheCenter.GetBoundingBox().Grow(
                MapPresenter.PixelWidth * cachedImageSize.Width / 2,
                MapPresenter.PixelHeight * cachedImageSize.Height / 2);

            _maxBoundarySize = new SizeF((float)maxBoundary.Width, (float)maxBoundary.Height);
            initializeCacheIndex((int)CachedImageIndicies.MiddleCenter, zoomLevel);
        }

        private void initializeCacheIndex(int index, double zoomLevel)
        {
            Size cachedImageSize = getCacheImageSize();
            _cachedImages[index] = new Bitmap(cachedImageSize.Width, cachedImageSize.Height);
            _imageBoundaries[index] = BoundingBox.Empty;
            _zoomLevels[index] = zoomLevel;
        }

        private void initializeCachedImages()
        {
            //for (int imageIndex = 0; imageIndex < _cachedImages.Length; imageIndex++)
            //{
            //    _cachedImages[imageIndex] = new Bitmap(Width, Height);
            //    using (Graphics g = Graphics.FromImage(_cachedImages[imageIndex]))
            //    using (SolidBrush brush = new SolidBrush(BackColor))
            //        g.FillRectangle(brush, new Rectangle(new Point(0, 0), _cachedImages[imageIndex].Size));
            //}
        }

        //void fillCache(object state)
        //{
        //    BoundingBox fullRegion = BoundingBox.Empty;
        //    for (int cacheIndex = 0; cacheIndex < CacheColumnCount * CacheRowCount; cacheIndex++)
        //        fullRegion.ExpandToInclude(getMaxRegionForIndex(cacheIndex));

        //    Bitmap cache = new Bitmap(getCacheImageSize().Width * CacheColumnCount, getCacheImageSize().Height * CacheRowCount);

        //    MapPresenter.RenderMap(fullRegion);
        //    //logImage(cache, String.Format("{0:HHmmss_ffff_dd-MM-yyyy}", DateTime.Now));

        //    for (int cacheIndex = 0; cacheIndex < CacheColumnCount * CacheRowCount; cacheIndex++)
        //    {
        //        Size cacheImageSize = getCacheImageSize();
        //        Image cacheImageTile = new Bitmap(cacheImageSize.Width, cacheImageSize.Height);
        //        int col = cacheIndex % CacheColumnCount;
        //        int row = cacheIndex / CacheRowCount;
        //        Rectangle tileRectangle = new Rectangle(0, 0, cacheImageSize.Width, cacheImageSize.Height);
        //        Rectangle areaOfCacheToDraw = new Rectangle(new GdiPoint(col * cacheImageSize.Width, row * cacheImageSize.Height), cacheImageSize);

        //        using (Graphics g = Graphics.FromImage(cacheImageTile))
        //            g.DrawImage(cache, tileRectangle, areaOfCacheToDraw, GraphicsUnit.Pixel);

        //        _cachedImages[cacheIndex] = cacheImageTile;
        //    }
        //}

        private GeoPoint getCenterForIndex(int cacheIndex)
        {
            // get center of cache
            return getMaxRegionForIndex(cacheIndex).GetCentroid();
        }

        private BoundingBox getMaxRegionForIndex(int cacheIndex)
        {
            double xMin, yMin, xMax, yMax;
            GeoPoint center = _cacheCenter;
            xMin = xMax = center.X;
            yMin = yMax = center.Y;

            // move it to correct index
            if (isTop(cacheIndex))
            {
                yMin += _maxBoundarySize.Height;
                yMax += _maxBoundarySize.Height;
            }

            if (isBottom(cacheIndex))
            {
                yMin -= _maxBoundarySize.Height;
                yMax -= _maxBoundarySize.Height;
            }

            if (isLeft(cacheIndex))
            {
                xMin -= _maxBoundarySize.Width;
                xMax -= _maxBoundarySize.Width;
            }

            if (isRight(cacheIndex))
            {
                xMin += _maxBoundarySize.Width;
                xMax += _maxBoundarySize.Width;
            }

            return new BoundingBox(xMin, yMin, xMax, yMax).Grow(_maxBoundarySize.Width / 2, _maxBoundarySize.Height / 2);
        }

        private Size getCacheImageSize()
        {
            return System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size;
        }

        private IEnumerable<int> getIndiciesForRegion(BoundingBox region)
        {
            List<int> indicies = new List<int>();
            for (int cacheIndex = 0; cacheIndex < CacheColumnCount * CacheRowCount; cacheIndex++)
            {
                BoundingBox cacheIndexMaxBounds = getMaxRegionForIndex(cacheIndex);
                if (cacheIndexMaxBounds.Overlaps(region))
                    indicies.Add(cacheIndex);
            }

            return indicies;
        }

        private BoundingBox removeBoundingBox(BoundingBox sourceRegion, BoundingBox regionToRemove)
        {
            if (sourceRegion == BoundingBox.Empty)
                return BoundingBox.Empty;

            if (regionToRemove == BoundingBox.Empty)
                return sourceRegion;

            if (sourceRegion.Left < regionToRemove.Left)
                return new BoundingBox(sourceRegion.Left, sourceRegion.Bottom, regionToRemove.Left, sourceRegion.Top);

            if (sourceRegion.Bottom < regionToRemove.Bottom)
                return new BoundingBox(sourceRegion.Left, sourceRegion.Bottom, sourceRegion.Right, regionToRemove.Bottom);

            if (sourceRegion.Right > regionToRemove.Right)
                return new BoundingBox(regionToRemove.Right, sourceRegion.Bottom, sourceRegion.Right, sourceRegion.Top);

            if (sourceRegion.Top > regionToRemove.Top)
                return new BoundingBox(sourceRegion.Left, regionToRemove.Top, sourceRegion.Right, sourceRegion.Top);

            throw new InvalidOperationException(String.Format("Cannot remove region {0} from {1}", regionToRemove, sourceRegion));
        }

        private void shiftCachedImages(int imageIndexToAcquire)
        {
            if (isTop(imageIndexToAcquire))
            {
                for (int i = CacheColumnCount; i < _cachedImages.Length; i++)
                {
                    _cachedImages[i] = _cachedImages[i - CacheColumnCount];
                    if (i - CacheColumnCount < CacheColumnCount)
                        _cachedImages[i - CacheColumnCount] = null;
                }
            }
            else if (isBottom(imageIndexToAcquire))
            {
                for (int i = _cachedImages.Length - CacheColumnCount; i >= 0; i--)
                {
                    _cachedImages[i] = _cachedImages[i + CacheColumnCount];
                    if (i + CacheColumnCount > _cachedImages.Length - CacheColumnCount)
                        _cachedImages[i + CacheColumnCount] = null;
                }
            }

            if (isLeft(imageIndexToAcquire))
            {
                for (int i = _cachedImages.Length; i >= 0; i--)
                {
                    if (i % CacheColumnCount == 0)  // Left edge
                        _cachedImages[i] = null;
                    else
                        _cachedImages[i] = _cachedImages[i - 1];
                }
            }
            else if (isRight(imageIndexToAcquire))
            {
                for (int i = 0; i < _cachedImages.Length; i++)
                {
                    if (i % CacheColumnCount == CacheColumnCount - 1)  // Right Edge
                        _cachedImages[i] = null;
                    else
                        _cachedImages[i] = _cachedImages[i + 1];
                }
            }
        }

        private bool isRight(int imageIndexToAcquire)
        {
            return imageIndexToAcquire == (int)CachedImageIndicies.BottomRight ||
                imageIndexToAcquire == (int)CachedImageIndicies.MiddleRight ||
                imageIndexToAcquire == (int)CachedImageIndicies.TopRight;
        }

        private bool isLeft(int imageIndexToAcquire)
        {
            return imageIndexToAcquire == (int)CachedImageIndicies.BottomLeft ||
                imageIndexToAcquire == (int)CachedImageIndicies.MiddleLeft ||
                imageIndexToAcquire == (int)CachedImageIndicies.TopLeft;
        }

        private bool isBottom(int imageIndexToAcquire)
        {
            return imageIndexToAcquire == (int)CachedImageIndicies.BottomLeft ||
                imageIndexToAcquire == (int)CachedImageIndicies.BottomCenter ||
                imageIndexToAcquire == (int)CachedImageIndicies.BottomRight;
        }

        private bool isTop(int imageIndexToAcquire)
        {
            return imageIndexToAcquire == (int)CachedImageIndicies.TopRight ||
                imageIndexToAcquire == (int)CachedImageIndicies.TopCenter ||
                imageIndexToAcquire == (int)CachedImageIndicies.TopLeft;
        }
        #endregion

    }
}
