// Copyright 2005, 2006 - Christian Gräfe (www.sharptools.de)
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

namespace SharpMap.Extensions.Layers.GdalRasterLayer
{
	/// <summary>
	/// Types of color interpretation for raster bands.
	/// </summary>
	public enum ColorInterpretation
	{
		/// <summary>
		/// Undefined
		/// </summary>
		Undefined = 0,
		/// <summary>
		/// Greyscale
		/// </summary>
		GrayIndex = 1,
		/// <summary>
		/// Paletted (see associated color table)
		/// </summary>
		PaletteIndex = 2,
		/// <summary>
		/// Red band of RGBA image
		/// </summary>               
		RedBand = 3,
		/// <summary>
		/// Green band of RGBA image
		/// </summary>
		GreenBand = 4,
		/// <summary>
		/// Blue band of RGBA image
		/// </summary>                       
		BlueBand = 5,
		/// <summary>
		/// Alpha (0=transparent, 255=opaque)
		/// </summary>   
		AlphaBand = 6,
		/// <summary>
		/// Hue band of HLS image 
		/// </summary>                 
		HueBand = 7,
		/// <summary>
		/// Saturation band of HLS image 
		/// </summary>       
		SaturationBand = 8,
		/// <summary>
		/// Lightness band of HLS image
		/// </summary>            
		LightnessBand = 9,
		/// <summary>
		/// Cyan band of CMYK image
		/// </summary>                
		CyanBand = 10,
		/// <summary>
		/// Magenta band of CMYK image
		/// </summary>             
		MagentaBand = 11,
		/// <summary>
		/// Yellow band of CMYK image
		/// </summary>             
		YellowBand = 12,
		/// <summary>
		/// Black band of CMYK image
		/// </summary>                      
		BlackBand = 13,
		/// <summary>
		/// Y Luminance
		/// </summary>                             
		YCbCr_YBand = 14,
		/// <summary>
		/// Cb Chroma
		/// </summary>                              
		YCbCr_CbBand = 15,
		/// <summary>
		/// Cr Chroma
		/// </summary>                                          
		YCbCr_CrBand = 16,
		/// <summary>
		/// Max current value
		/// </summary>
		Max = 16
	};
}
