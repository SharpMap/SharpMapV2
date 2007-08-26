// Portions copyright 2005, 2006 - Christian Gräfe (www.sharptools.de)
// Portions copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
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
using OSGeo.GDAL;
using SharpMap.Geometries;
using SharpMap.Layers;

namespace SharpMap.Extensions.Layers.GdalRasterLayer
{
	/// <summary>
	/// Gdal raster image layer
	/// </summary>
	/// <remarks>
	/// <example>
	/// <code lang="C#">
	/// GdalRasterLayer layGdal = new GdalRasterLayer("Blue Marble", @"C:\data\srtm30plus.tif");
	/// myMap.Layers.Add(layGdal);
	/// myMap.ZoomToExtents();
	/// </code>
	/// </example>
	/// </remarks>
	public class GdalRasterLayer : Layer
	{
		private BoundingBox _envelope;
		private Dataset _gdalDataset;
		private System.Drawing.Size _imageSize;
		private string _filename;
		private int? _srid = null;
		private bool _isDisposed = false;

		#region Object construction and disposal

		/// <summary>
		/// initialize a Gdal based raster layer
		/// </summary>
		/// <param name="layerName">Name of layer</param>
		/// <param name="imageFilename">location of image</param>
		public GdalRasterLayer(string layerName, string imageFilename)
		{
			LayerName = layerName;
			Filename = imageFilename;
			_isDisposed = false;

			Gdal.AllRegister();
			try
			{
				_gdalDataset = Gdal.Open(_filename, Access.GA_ReadOnly);
				_imageSize = new Size(_gdalDataset.RasterXSize, _gdalDataset.RasterYSize);

				_envelope = getExtent();
			}
			catch (Exception ex)
			{
				_gdalDataset = null;
				throw new GdalRasterDataLoadFailedException(imageFilename, "Loading raster dataset failed.", ex);
			}
		}

		/// <summary>
		/// initialize a Gdal based raster layer
		/// </summary>
		/// <param name="strLayerName">Name of layer</param>
		/// <param name="imageFilename">location of image</param>
		/// <param name="srid">sets the SRID of the data set</param>
		public GdalRasterLayer(string strLayerName, string imageFilename, int srid)
			: this(strLayerName, imageFilename)
		{
			Srid = srid;
		}

		#region Disposers and finalizers

		/// <summary>
		/// Finalizer
		/// </summary>
		~GdalRasterLayer()
		{
			Dispose(false);
		}

		private void Dispose(bool disposing)
		{
			if (_isDisposed)
			{
				return;
			}

			if (disposing)
			{
				if (_gdalDataset != null)
				{
					try
					{
						_gdalDataset.Dispose();
					}
					finally
					{
						_gdalDataset = null;
					}
				}
			}
		}

		/// <summary>
		/// Disposes the GdalRasterLayer and release the raster file
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
			_isDisposed = true;
		}

		#endregion

		#endregion

		/// <summary>
		/// Gets or sets the filename of the raster file
		/// </summary>
		public string Filename
		{
			get { return _filename; }
			set { _filename = value; }
		}

		#region ILayer Members

		/// <summary>
		/// Renders the layer
		/// </summary>
		/// <param name="g">Graphics object reference</param>
		/// <param name="map">Map which is rendered</param>
		public override void Render(Graphics g, Map map)
		{
			checkDisposed();

			//if (this.Envelope.Intersects(map.Envelope))
			//{
			getPreview(_gdalDataset, map.Size, g, map.Envelope);
			//}
			base.Render(g, map);
		}

		/// <summary>
		/// Returns the extent of the layer
		/// </summary>
		/// <returns>Bounding box corresponding to the extent of the features in the layer</returns>
		public override BoundingBox Envelope
		{
			get { return _envelope; }
		}

		/// <summary>
		/// The spatial reference ID (CRS)
		/// </summary>
		public override int? Srid
		{
			get { return _srid; }
			set { _srid = value; }
		}

		#endregion

		#region ICloneable Members

		/// <summary>
		/// Clones the object
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Private helper methods
		private BoundingBox getExtent()
		{
			if (_gdalDataset != null)
			{
				double[] geoTrans = new double[6];
				_gdalDataset.GetGeoTransform(geoTrans);
				GeoTransform GT = new GeoTransform(geoTrans);

				return new BoundingBox(GT.Left,
									   GT.Top + (GT.VerticalPixelResolution * _gdalDataset.RasterYSize),
									   GT.Left + (GT.HorizontalPixelResolution * _gdalDataset.RasterXSize),
									   GT.Top);
			}

			return null;
		}

		private void getPreview(Dataset dataset, System.Drawing.Size size, Graphics g, BoundingBox bbox)
		{
			double[] geoTrans = new double[6];
			dataset.GetGeoTransform(geoTrans);
			GeoTransform GT = new GeoTransform(geoTrans);

			int DsWidth = dataset.RasterXSize;
			int DsHeight = dataset.RasterYSize;

			Bitmap bitmap = new Bitmap(size.Width, size.Height, PixelFormat.Format24bppRgb);
			int iPixelSize = 3; //Format24bppRgb = byte[b,g,r]

			if (dataset != null)
			{
				/*
                if ((float)size.Width / (float)size.Height > (float)DsWidth / (float)DsHeight)
                    size.Width = size.Height * DsWidth / DsHeight;
                else
                    size.Height = size.Width * DsHeight / DsWidth;
                */


				double left = Math.Max(bbox.Left, _envelope.Left);
				double top = Math.Min(bbox.Top, _envelope.Top);
				double right = Math.Min(bbox.Right, _envelope.Right);
				double bottom = Math.Max(bbox.Bottom, _envelope.Bottom);


				int x1 = (int)GT.PixelX(left);
				int y1 = (int)GT.PixelY(top);
				int x1width = (int)GT.PixelXwidth(right - left);

				int y1height = (int)GT.PixelYwidth(bottom - top);


				bitmap = new Bitmap(size.Width, size.Height, PixelFormat.Format24bppRgb);
				BitmapData bitmapData =
					bitmap.LockBits(new Rectangle(0, 0, size.Width, size.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);

				try
				{
					unsafe
					{
						for (int i = 1; i <= (dataset.RasterCount > 3 ? 3 : dataset.RasterCount); ++i)
						{
							byte[] buffer = new byte[size.Width * size.Height];
							Band band = dataset.GetRasterBand(i);

							//band.ReadRaster(x1, y1, x1width, y1height, buffer, size.Width, size.Height, (int)GT.HorizontalPixelResolution, (int)GT.VerticalPixelResolution);
							band.ReadRaster(x1, y1, x1width, y1height, buffer, size.Width, size.Height, 0, 0);

							int p_indx = 0;
							int ch = 0;

							//#warning Check correspondance between enum and integer values
							if (band.GetRasterColorInterpretation() == ColorInterp.GCI_BlueBand) ch = 0;
							if (band.GetRasterColorInterpretation() == ColorInterp.GCI_GreenBand) ch = 1;
							if (band.GetRasterColorInterpretation() == ColorInterp.GCI_RedBand) ch = 2;
							if (band.GetRasterColorInterpretation() != ColorInterp.GCI_PaletteIndex)
							{
								for (int y = 0; y < size.Height; y++)
								{
									byte* row = (byte*)bitmapData.Scan0 + (y * bitmapData.Stride);
									for (int x = 0; x < size.Width; x++, p_indx++)
									{
										row[x * iPixelSize + ch] = buffer[p_indx];
									}
								}
							}
							else //8bit Grayscale
							{
								for (int y = 0; y < size.Height; y++)
								{
									byte* row = (byte*)bitmapData.Scan0 + (y * bitmapData.Stride);
									for (int x = 0; x < size.Width; x++, p_indx++)
									{
										row[x * iPixelSize] = buffer[p_indx];
										row[x * iPixelSize + 1] = buffer[p_indx];
										row[x * iPixelSize + 2] = buffer[p_indx];
									}
								}
							}
						}
					}
				}
				finally
				{
					bitmap.UnlockBits(bitmapData);
				}
			}

			g.DrawImage(bitmap, new System.Drawing.Point(0, 0));
		}

		private void checkDisposed()
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
		}
		#endregion
	}
}