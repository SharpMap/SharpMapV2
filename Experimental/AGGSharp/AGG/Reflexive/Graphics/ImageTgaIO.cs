//*********************************************************************************************************************
//
//	     Author: Lars Brubaker, James C. Smith
//	    Started: 11/08/97 - © Reflexive Entertainment Inc.
//     $Modtime: 7/07/99 11:03a $
//    $Revision: 46 $
//																$NoKeywords $
//	   Filename: TGAFileIO.cpp
//
//	Description: Reading add writing from a Targa File
//
//*********************************************************************************************************************
#define TURN_ON_COMPRESSION

using System;
using NPack.Interfaces;

namespace Reflexive.Graphics
{
    static class ImageTgaIO<T>
          where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        //*********************************************************************************************************************
        // Header of a TGA file
        public struct STargaHeader
        {
            public Byte PostHeaderSkip;
            public Byte ColorMapType;		// 0 = RGB, 1 = Palette
            public Byte ImageType;			// 1 = Palette, 2 = RGB, 3 = mono, 9 = RLE Palette, 10 = RLE RGB, 11 RLE mono
            public ushort ColorMapStart;
            public ushort ColorMapLength;
            public Byte ColorMapBits;
            public ushort XStart;				// offsets the image would like to have (ignored)
            public ushort YStart;				// offsets the image would like to have (ignored)
            public ushort Width;
            public ushort Height;
            public Byte BPP;				// bit depth of the image
            public Byte Descriptor;
        };

        private const int TargaHeaderSize = 18;
        const int RGB_BLUE = 2;
        const int RGB_GREEN = 1;
        const int RGB_RED = 0;
        const int RGBA_ALPHA = 3;

        //*********************************************************************************************************************
        // these are used during loading (only valid during load)
        static int TGABytesPerLine;

        //*********************************************************************************************************************
        static void Do24To8Bit(Byte[] Dest, Byte[] Source, int SourceOffset, int Width, int Height)
        {
            throw new System.NotImplementedException();
#if false

	        int i;
	        if (Width) 
	        {
		        i = 0;
		        Dest = &Dest[Height*Width];
		        do 
		        {
			        if(p[RGB_RED] == 0 && p[RGB_GREEN] == 0 && p[RGB_BLUE] == 0)
			        {
				        Dest[i] = 0;
			        }
			        else
			        {
				        // no other color can map to color 0
				        Dest[i] =(Byte) pStaticRemap->GetColorIndex(p[RGB_RED], p[RGB_GREEN], p[RGB_BLUE], 1);
			        }
			        p += 3;
		        } while (++i<Width);
	        }
#endif
        }

        //*********************************************************************************************************************
        static void Do32To8Bit(Byte[] Dest, Byte[] Source, int SourceOffset, int Width, int Height)
        {
            throw new System.NotImplementedException();

#if false
	        int i;
	        if (Width) 
	        {
		        i = 0;
		        Dest = &Dest[Height*Width];
		        do 
		        {
			        if(p[RGB_RED] == 0 && p[RGB_GREEN] == 0 && p[RGB_BLUE] == 0)
			        {
				        Dest[i] = 0;
			        }
			        else
			        {
				        // no other color can map to color 0
				        Dest[i] = (Byte)pStaticRemap->GetColorIndex(p[RGB_RED], p[RGB_GREEN], p[RGB_BLUE], 1);
			        }
			        p += 4;
		        } while (++i < Width);
	        }
#endif
        }

        //*********************************************************************************************************************
        static unsafe void Do24To24Bit(Byte[] Dest, Byte[] Source, int SourceOffset, int Width, int Height)
        {
	        if (Width > 0) 
	        {
                int DestOffset = Height * Width * 3;
                for (int i = 0; i < Width * 3; i++)
                {
                    Dest[DestOffset+i] = Source[SourceOffset+i];
                }
	        }
        }

        //*********************************************************************************************************************
        static unsafe void Do32To24Bit(Byte[] Dest, Byte[] Source, int SourceOffset, int Width, int Height)
        {
	        if (Width > 0) 
	        {
		        int i = 0;
		        int DestOffest = Height * Width * 3;
		        do 
		        {
                    Dest[DestOffest + i * 3 + RGB_BLUE] = Source[SourceOffset + RGB_BLUE];
                    Dest[DestOffest + i * 3 + RGB_GREEN] = Source[SourceOffset + RGB_GREEN];
                    Dest[DestOffest + i * 3 + RGB_RED] = Source[SourceOffset + RGB_RED];
                    SourceOffset += 4;
		        } while (++i < Width);
	        }
        }

        //*********************************************************************************************************************
        static unsafe void Do24To32Bit(Byte[] Dest, Byte[] Source, int SourceOffset, int Width, int Height)
        {
	        if (Width > 0) 
	        {
		        int i = 0;
                int DestOffest = Height * Width * 4;
		        do 
		        {
                    Dest[DestOffest + i * 4 + RGB_BLUE] = Source[SourceOffset + RGB_BLUE];
                    Dest[DestOffest + i * 4 + RGB_GREEN] = Source[SourceOffset + RGB_GREEN];
                    Dest[DestOffest + i * 4 + RGB_RED] = Source[SourceOffset + RGB_RED];
                    Dest[DestOffest + i * 4 + 3] = 255;
			        SourceOffset += 3;
		        } while (++i < Width);
	        }
        }

        //*********************************************************************************************************************
        static unsafe void Do32To32Bit(Byte[] Dest, Byte[] Source, int SourceOffset, int Width, int Height)
        {
	        if (Width > 0) 
	        {
		        int i = 0;
                int DestOffest = Height * Width * 4;
                do 
		        {
                    Dest[DestOffest + i * 4 + RGB_BLUE] = Source[SourceOffset + RGB_BLUE];
                    Dest[DestOffest + i * 4 + RGB_GREEN] = Source[SourceOffset + RGB_GREEN];
                    Dest[DestOffest + i * 4 + RGB_RED] = Source[SourceOffset + RGB_RED];
                    Dest[DestOffest + i * 4 + RGBA_ALPHA] = Source[SourceOffset + RGBA_ALPHA];
			        SourceOffset += 4;
		        } while (++i < Width);
	        }
        }

        //*********************************************************************************************************************
        static unsafe bool ReadTGAInfo(Byte* WorkPtr, out STargaHeader TargaHeader)
        {
	        TargaHeader.PostHeaderSkip = WorkPtr[0];
            TargaHeader.ColorMapType = WorkPtr[1];
            TargaHeader.ImageType = WorkPtr[2];
            TargaHeader.ColorMapStart = *((ushort*)(&WorkPtr[3]));
            TargaHeader.ColorMapLength = *((ushort*)(&WorkPtr[5]));
            TargaHeader.ColorMapBits = WorkPtr[7];
            TargaHeader.XStart = *((ushort*)(&WorkPtr[8]));
            TargaHeader.YStart = *((ushort*)(&WorkPtr[10]));
            TargaHeader.Width = *((ushort*)(&WorkPtr[12]));
            TargaHeader.Height = *((ushort*)(&WorkPtr[14]));
            TargaHeader.BPP = WorkPtr[16];
            TargaHeader.Descriptor = WorkPtr[17];
        	
	        // check the header
            if (TargaHeader.ColorMapType != 0 ||	// 0 = RGB, 1 = Palette
		        // 1 = Palette, 2 = RGB, 3 = mono, 9 = RLE Palette, 10 = RLE RGB, 11 RLE mono
                (TargaHeader.ImageType != 2 && TargaHeader.ImageType != 10 && TargaHeader.ImageType != 9) ||
                (TargaHeader.BPP != 24 && TargaHeader.BPP != 32))
	        {
        #if ASSERTS_ENABLED
		        if ( ((Byte*)pTargaHeader)[0] == 'B' && ((Byte*)pTargaHeader)[1] == 'M' )
		        {
			        assert(!"This TGA's header looks like a BMP!"); //  look at the first two bytes and see if they are 'BM'
			        // if so it's a BMP not a TGA
		        }
		        else
		        {
			        Byte * pColorMapType = NULL;
			        switch (TargaHeader.ColorMapType)
			        {
				        case 0:
					        pColorMapType = "RGB Color Map";
					        break;
				        case 1:
					        pColorMapType = "Palette Color Map";
					        break;
				        default:
					        pColorMapType = "<Illegal Color Map>";
					        break;
			        }
			        Byte * pImageType = NULL;
			        switch (TargaHeader.ImageType)
			        {
				        case 1:
					        pImageType = "Palette Image Type";
					        break;
				        case 2:
					        pImageType = "RGB Image Type";
					        break;
				        case 3:
					        pImageType = "mono Image Type";
					        break;
				        case 9:
					        pImageType = "RLE Palette Image Type";
					        break;
				        case 10:
					        pImageType = "RLE RGB Image Type";
					        break;
				        case 11:
					        pImageType = "RLE mono Image Type";
					        break;
				        default:
					        pImageType = "<Illegal Image Type>";
					        break;
			        }
			        int ColorDepth = TargaHeader.BPP;
			        CJString ErrorString;
			        ErrorString.Format( "Image type %s %s (%u bpp) not supported!", pColorMapType, pImageType, ColorDepth);
			        ShowSystemMessage("TGA File IO Error", ErrorString.GetBytePtr());
		        }
#endif // ASSERTS_ENABLED
                return false;
	        }

	        return true;
        }

        //*********************************************************************************************************************
        const int IS_PIXLE_RUN = 0x80;
        const int RUN_LENGTH_MASK = 0x7f;

        //*********************************************************************************************************************
        static unsafe int Decompress(Byte[] pDecompressBits, Byte[] pBitsToPars, int ParsOffset, int Width, int Depth, int LineBeingRead)
        {
            int DecompressOffset = 0;
            int Total = 0;
            do
            {
                int i;
                int NumPixels = (pBitsToPars[ParsOffset] & RUN_LENGTH_MASK) + 1;
                Total += NumPixels;
                if ((pBitsToPars[ParsOffset++] & IS_PIXLE_RUN) != 0)
                {
                    // decompress the run for NumPixels
                    Byte r, g, b, a;
                    b = pBitsToPars[ParsOffset++];
                    g = pBitsToPars[ParsOffset++];
                    r = pBitsToPars[ParsOffset++];
                    switch (Depth)
                    {
                        case 24:
                            for (i = 0; i < NumPixels; i++)
                            {
                                pDecompressBits[DecompressOffset++] = b;
                                pDecompressBits[DecompressOffset++] = g;
                                pDecompressBits[DecompressOffset++] = r;
                            }
                            break;

                        case 32:
                            a = pBitsToPars[ParsOffset++];
                            for (i = 0; i < NumPixels; i++)
                            {
                                pDecompressBits[DecompressOffset++] = b;
                                pDecompressBits[DecompressOffset++] = g;
                                pDecompressBits[DecompressOffset++] = r;
                                pDecompressBits[DecompressOffset++] = a;
                            }
                            break;

                        default:
                            throw new System.Exception("Bad bit depth.");
                    }
                }
                else // store NumPixels normally
                {
                    switch (Depth)
                    {
                        case 24:
                            for (i = 0; i < NumPixels * 3; i++)
                            {
                                pDecompressBits[DecompressOffset++] = pBitsToPars[ParsOffset++];
                            }
                            break;

                        case 32:
                            for (i = 0; i < NumPixels * 4; i++)
                            {
                                pDecompressBits[DecompressOffset++] = pBitsToPars[ParsOffset++];
                            }
                            break;

                        default:
                            throw new System.Exception("Bad bit depth.");
                    }
                }
            } while (Total < Width);

            if (Total > Width)
            {
                throw new System.Exception("The TGA you loaded is corrupt (line " + LineBeingRead.ToString() + ").");
            }

            return ParsOffset;
        }

        static unsafe int LowLevelReadTGABitsFromBuffer(Image<T> Input, Byte[] WholeFileBuffer, int DestBitDepth)
        {
	        STargaHeader TargaHeader = new STargaHeader();
	        int FileReadOffset;

            fixed (byte* pWorkPtr = WholeFileBuffer)
            {
                if (!ReadTGAInfo(pWorkPtr, out TargaHeader))
                {
                    return 0;
                }
            }

	        // if the frame we are loading is different then the one we have allocated
	        // or we don't have any bits allocated

            if ((Input.Width * Input.Height) != (TargaHeader.Width * TargaHeader.Height)) 
	        {
                Input.Allocate(TargaHeader.Width, TargaHeader.Height, DestBitDepth, TargaHeader.Width * DestBitDepth / 8);
	        }

	        // work out the line width
            switch (TargaHeader.BPP)
            {
                case 24:
                    TGABytesPerLine = Input.Width * 3;
                    break;

                case 32:
                    TGABytesPerLine = Input.Width * 4;
                    break;

                default:
                    throw new System.Exception("Bad bit depth.");
            }

	        if(TGABytesPerLine > 0) 
	        {
		        Byte[] BufferToDecompressTo = null;
		        FileReadOffset = TargaHeaderSize + TargaHeader.PostHeaderSkip;

                if (TargaHeader.ImageType == 10) // 10 is RLE compressed
		        {
			        BufferToDecompressTo = new Byte[TGABytesPerLine*2];
		        }

		        // read all the lines *
		        for(int i=0; i< Input.Height; i++) 
		        {
			        Byte[] BufferToCopyFrom;
                    int CopyOffset = 0;

			        int CurReadLine;

			        // bit 5 tells us if the image is stored top to bottom or bottom to top
			        if((TargaHeader.Descriptor & 0x20) != 0)
			        {
				        // top to bottom
				        CurReadLine = i;
			        }
			        else
			        {
				        // bottom to top
                        CurReadLine = Input.Height - i - 1;
			        }

                    if (TargaHeader.ImageType == 10) // 10 is RLE compressed
			        {
                        FileReadOffset = Decompress(BufferToDecompressTo, WholeFileBuffer, FileReadOffset, Input.Width, TargaHeader.BPP, CurReadLine);
				        BufferToCopyFrom = BufferToDecompressTo;
			        }
			        else
			        {
                        BufferToCopyFrom = WholeFileBuffer;
                        CopyOffset = FileReadOffset;
                    }

			        switch(Input.BitDepth)
			        {
			        case 8:
				        switch(TargaHeader.BPP) 
				        {
				        case 24:
                            Do24To8Bit(Input.ImageBuffer, BufferToCopyFrom, CopyOffset, Input.Width, CurReadLine);
					        break;

				        case 32:
                            Do32To8Bit(Input.ImageBuffer, BufferToCopyFrom, CopyOffset, Input.Width, CurReadLine);
					        break;
				        }
				        break;

			        case 24:
				        switch(TargaHeader.BPP) 
				        {
				        case 24:
                            Do24To24Bit(Input.ImageBuffer, BufferToCopyFrom, CopyOffset, Input.Width, CurReadLine);
					        break;

				        case 32:
                            Do32To24Bit(Input.ImageBuffer, BufferToCopyFrom, CopyOffset, Input.Width, CurReadLine);
					        break;
				        }
				        break;

			        case 32:
				        switch(TargaHeader.BPP) 
				        {
				        case 24:
                            Do24To32Bit(Input.ImageBuffer, BufferToCopyFrom, CopyOffset, Input.Width, CurReadLine);
					        break;

				        case 32:
                            Do32To32Bit(Input.ImageBuffer, BufferToCopyFrom, CopyOffset, Input.Width, CurReadLine);
					        break;
				        }
				        break;

			        default:
                        throw new System.Exception("Bad bit depth");
			        }

                    if (TargaHeader.ImageType != 10) // 10 is RLE compressed
			        {
                        FileReadOffset += TGABytesPerLine;
			        }
		        }
	        }

	        return TargaHeader.Width;
        }

        //*********************************************************************************************************************
        const int MAX_RUN_LENGTH = 127;
        static int memcmp(Byte[] pCheck, int CheckOffset, Byte[] pSource, int SourceOffset, int Width)
        {
            for(int i=0; i<Width; i++)
            {
                if (pCheck[CheckOffset + i] < pSource[SourceOffset + SourceOffset + i])
                {
                    return -1;
                }
                if (pCheck[CheckOffset + i] > pSource[SourceOffset + i])
                {
                    return 1;
                }
            }

            return 0;
        }

        //*********************************************************************************************************************
        static int GetSameLength(Byte[] pCheck, int CheckOffset, Byte[] pSource, int SourceOffset, int Width, int Max)
        {
            int Count = 0;
            while (memcmp(pCheck, CheckOffset, pSource, SourceOffset, Width) == 0 && Count < Max)
	        {
		        Count++;
                SourceOffset += Width;
	        }

	        return Count;
        }

        //*********************************************************************************************************************
        static int GetDifLength(Byte[] pCheck, int CheckOffset, Byte[] pSource, int SourceOffset, int Width, int Max)
        {
            int Count = 0;
            while (memcmp(pCheck, CheckOffset, pSource, SourceOffset, Width) != 0 && Count < Max)
	        {
		        Count++;
                for (int i = 0; i < Width; i++)
		        {
                    pCheck[CheckOffset+i] = pSource[SourceOffset+i];
		        }
                SourceOffset += Width;
            }

	        return Count;
        }

        //*********************************************************************************************************************
        const int MIN_RUN_LENGTH = 2;

        //*********************************************************************************************************************
        //static int GetDifLengthMinRun(Byte[]  /*pCheck*/ , Byte[]  /*pSource*/ , int Max)
        static int GetDifLengthMinRun(int Max)
        {
	        return Max;
        #if false
	        int Count = 0;
	        int RunLength = 0;
	        while((*pCheck != *pSource || RunLength < MIN_RUN_LENGTH-1) && Count < Max)
	        {
		        if(*pCheck != *pSource)
		        {
			        RunLength = 0;
		        }
		        else
		        {
			        RunLength++;
		        }

		        Count++;
		        *pCheck = *pSource++;
	        }

	        if(RunLength == MIN_RUN_LENGTH-1)
	        {
		        return Count - RunLength;
	        }

	        return Count;
#endif
        }

        //*********************************************************************************************************************
        static int CompressLine8(Byte[] pDest, Byte[] pSource, int Width)
        {
            int SourcePos = 0;
            int WritePos = 0;

	        while(SourcePos < Width)
	        {
		        // always get as many as you can that are the same first
                int Max = System.Math.Min(MAX_RUN_LENGTH, (Width - 1) - SourcePos);
                int SameLength = GetSameLength(pSource, SourcePos, pSource, SourcePos + 1, 1, Max);
		        if(SameLength >= MIN_RUN_LENGTH)
		        //if(SameLength)
		        {
			        // write in the count
                    if (SameLength > MAX_RUN_LENGTH)
                    {
                        throw new System.Exception("Bad Length");
                    }
			        pDest[WritePos++] = (Byte)((SameLength) | IS_PIXLE_RUN);

			        // write in the same length pixel Value
			        pDest[WritePos++] = pSource[SourcePos];

			        SourcePos += SameLength + 1;
		        }
		        else
		        {
        #if true
			        Byte CheckPixel = pSource[SourcePos];
                    int DifLength = Max; //  GetDifLengthMinRun(&CheckPixel, &pSource[SourcePos + 1], Max);
        #else
			        Byte CheckPixel = pSource[SourcePos];
			        int DifLength = GetDifLength(&CheckPixel, &pSource[SourcePos+1], 1, Max);
        #endif
			        if(DifLength == 0)
			        {
				        DifLength = 1;
			        }
			        // write in the count (if there is only one the count is 0)
                    if (DifLength > MAX_RUN_LENGTH)
                    {
                        throw new System.Exception("Bad Length");
                    }

			        pDest[WritePos++] = (Byte)(DifLength-1);

			        while(DifLength-- != 0)
			        {
				        // write in the same length pixel Value
				        pDest[WritePos++] = pSource[SourcePos++];
			        }
		        }
	        }

	        return WritePos;
        }

        //*********************************************************************************************************************
        static int CompressLine24(Byte[] pDest, Byte[] pSource, int Width)
        {
            throw new System.NotImplementedException("Write this");
            /*
            int SourcePos = 0;
            int WritePos = 0;

	        while(SourcePos < Width)
	        {
		        // always get as many as you can that are the same first
                int Max = System.Math.Min(MAX_RUN_LENGTH, (Width - 1) - SourcePos);
                int SameLength = GetSameLength((Byte*)&pSource[SourcePos], (Byte*)&pSource[SourcePos + 1], 3, Max);
		        if(SameLength)
		        {
			        // write in the count
			        assert(SameLength<= MAX_RUN_LENGTH);
			        pDest[WritePos++] = (Byte)((SameLength) | IS_PIXLE_RUN);

			        // write in the same length pixel Value
			        pDest[WritePos++] = pSource[SourcePos].Blue;
			        pDest[WritePos++] = pSource[SourcePos].Green;
			        pDest[WritePos++] = pSource[SourcePos].Red;

			        SourcePos += SameLength + 1;
		        }
		        else
		        {
			        Pixel24 CheckPixel = pSource[SourcePos];
			        int DifLength = GetDifLength((Byte*)&CheckPixel, (Byte*)&pSource[SourcePos+1], 3, Max);
			        if(!DifLength)
			        {
				        DifLength = 1;
			        }

			        // write in the count (if there is only one the count is 0)
			        assert(DifLength <= MAX_RUN_LENGTH);
			        pDest[WritePos++] = (Byte)(DifLength-1);

			        while(DifLength--)
			        {
				        // write in the same length pixel Value
				        pDest[WritePos++] = pSource[SourcePos].Blue;
				        pDest[WritePos++] = pSource[SourcePos].Green;
				        pDest[WritePos++] = pSource[SourcePos].Red;
				        SourcePos++;
			        }
		        }
	        }

	        return WritePos;
             */
        }

        //*********************************************************************************************************************
        static int CompressLine32(Byte[] pDest, Byte[] pSource, int Width)
        {
            throw new System.NotImplementedException("Implement this");
            /*
            int SourcePos = 0;
            int WritePos = 0;

	        while(SourcePos < Width)
	        {
		        // always get as many as you can that are the same first
                int Max = System.Math.Min(MAX_RUN_LENGTH, (Width - 1) - SourcePos);
                int SameLength = GetSameLength((Byte*)&pSource[SourcePos], (Byte*)&pSource[SourcePos + 1], 4, Max);
		        if(SameLength)
		        {
			        // write in the count
			        assert(SameLength<= MAX_RUN_LENGTH);
			        pDest[WritePos++] = (Byte)((SameLength) | IS_PIXLE_RUN);

			        // write in the same length pixel Value
			        pDest[WritePos++] = pSource[SourcePos].Blue;
			        pDest[WritePos++] = pSource[SourcePos].Green;
			        pDest[WritePos++] = pSource[SourcePos].Red;
			        pDest[WritePos++] = pSource[SourcePos].Alpha;

			        SourcePos += SameLength + 1;
		        }
		        else
		        {
			        Pixel32 CheckPixel = pSource[SourcePos];
			        int DifLength = GetDifLength((Byte*)&CheckPixel, (Byte*)&pSource[SourcePos+1], 4, Max);
			        if(!DifLength)
			        {
				        DifLength = 1;
			        }

			        // write in the count (if there is only one the count is 0)
			        assert(DifLength <= MAX_RUN_LENGTH);
			        pDest[WritePos++] = (Byte)(DifLength-1);

			        while(DifLength--)
			        {
				        // write in the same length pixel Value
				        pDest[WritePos++] = pSource[SourcePos].Blue;
				        pDest[WritePos++] = pSource[SourcePos].Green;
				        pDest[WritePos++] = pSource[SourcePos].Red;
				        pDest[WritePos++] = pSource[SourcePos].Alpha;
				        SourcePos++;
			        }
		        }
	        }

	        return WritePos;
             */
        }

        /*
        //*********************************************************************************************************************
        static int HEADER_SET_AND_WRITE(int x, int Value)
        {
            x = Value;
            TGAFile.Write(&x, sizeof(x));
        }

        //*********************************************************************************************************************
        bool InternalSave(Image pFrame, String pFileName, int AdditionalOpenFlags)
        {
	        if(pFrame->GetBitDepth() == 16)
	        {
		        CFrame Temp24Frame;
		        Temp24Frame.Initialize(pFrame->GetWidth(), pFrame->GetHeight(), 24);
		        CSmallPoint OldUpperLeft;
		        OldUpperLeft.Set((short)pFrame->GetUpperLeftOffsetX(), (short)pFrame->GetUpperLeftOffsetY());
		        pFrame->SetUpperLeftOffset(0, 0);
		        Temp24Frame.CFrameInterface::Blit(pFrame, (long)0, (long)0, &BlitNormal);
		        pFrame->SetUpperLeftOffset((short)OldUpperLeft.x, (short)OldUpperLeft.y);
		        return InternalSave(&Temp24Frame, pFileName, AdditionalOpenFlags);
	        }

	        STargaHeader TargaHeader;
	        CFile TGAFile;

	        CFrame CompressedHold;
	        int SourceDepth = pFrame->GetBitDepth();

	        if(pFrame->GetFlags ()& CFrameInterface::FLAG_COMPRESSION_BITS)
	        {
		        CompressedHold.Initialize(pFrame->GetWidth(), pFrame->GetHeight(), SourceDepth);
		        CompressedHold.CFrameInterface::Blit(pFrame, -pFrame->GetUpperLeftOffsetX(), -pFrame->GetUpperLeftOffsetY(), &BlitNormal);
		        pFrame = &CompressedHold;
	        }

	        // make sure there is something to save before opening the file
	        if(pFrame->GetBuffer() && 
		        pFrame->GetWidth() &&
		        pFrame->GetHeight() &&
		        TGAFile.Open(pFileName, CFile::modeCreate | CFile::modeWrite | AdditionalOpenFlags))
	        {
		        int SaveWidthBytes;
		        switch(SourceDepth)
		        {
		        case 8:
			        SaveWidthBytes = pFrame->GetWidth();
			        break;

		        case 24:
			        SaveWidthBytes = pFrame->GetWidth() * 3;
			        break;

		        case 32:
			        SaveWidthBytes = pFrame->GetWidth() * 4;
			        break;

		        default:
			        TGAFile.Close();
			        assert(0);
			        return false;
		        }

		        // set up the header
		        HEADER_SET_AND_WRITE(TargaHeader.PostHeaderSkip, 0);	// no skip after the header
		        if(SourceDepth == 8)
		        {
			        HEADER_SET_AND_WRITE(TargaHeader.ColorMapType, 1);		// Color type is Palette
			        HEADER_SET_AND_WRITE(TargaHeader.ImageType, 9);		// 1 = Palette, 9 = RLE Palette
			        HEADER_SET_AND_WRITE(TargaHeader.ColorMapStart, 0);
			        HEADER_SET_AND_WRITE(TargaHeader.ColorMapLength, 256);
			        HEADER_SET_AND_WRITE(TargaHeader.ColorMapBits, 24);
		        }
		        else
		        {
			        HEADER_SET_AND_WRITE(TargaHeader.ColorMapType, 0);		// Color type is RGB
        #if TURN_ON_COMPRESSION
			        HEADER_SET_AND_WRITE(TargaHeader.ImageType, 10);		// RLE RGB
        #else
			        HEADER_SET_AND_WRITE(TargaHeader.ImageType, 2);		// RGB
        #endif
			        HEADER_SET_AND_WRITE(TargaHeader.ColorMapStart, 0);
			        HEADER_SET_AND_WRITE(TargaHeader.ColorMapLength, 0);
			        HEADER_SET_AND_WRITE(TargaHeader.ColorMapBits, 0);
		        }
		        HEADER_SET_AND_WRITE(TargaHeader.XStart, 0);
		        HEADER_SET_AND_WRITE(TargaHeader.YStart, 0);
		        HEADER_SET_AND_WRITE(TargaHeader.Width, (ushort)pFrame->GetWidth());
		        HEADER_SET_AND_WRITE(TargaHeader.Height, (ushort)pFrame->GetHeight());
		        HEADER_SET_AND_WRITE(TargaHeader.BPP, (Byte)SourceDepth);
		        HEADER_SET_AND_WRITE(TargaHeader.Descriptor, 0);	// all 8 bits are used for alpha

		        Byte[] pLineBuffer = new Byte[SaveWidthBytes * 2];

		        int i, BytesToSave;
		        switch(SourceDepth)
		        {
		        case 8:
			        if( pFrame->HasPalette())
			        {
				        for(int i=0; i<256; i++)
				        {
					        TGAFile.Write(pFrame->GetPaletteIfAllocated()->pPalette[i*RGB_SIZE+RGB_BLUE]);
					        TGAFile.Write(pFrame->GetPaletteIfAllocated()->pPalette[i*RGB_SIZE+RGB_GREEN]);
					        TGAFile.Write(pFrame->GetPaletteIfAllocated()->pPalette[i*RGB_SIZE+RGB_RED]);
				        }
			        } 
			        else 
			        {	// there is no palette for this DIB but we should write something
				        for(int i=0; i<256; i++)
				        {
					        TGAFile.Write((Byte)i);
					        TGAFile.Write((Byte)i);
					        TGAFile.Write((Byte)i);
				        }
			        }
			        for(i = pFrame->GetHeight()-1; i>=0; i-- ) 
			        {
        #if TURN_ON_COMPRESSION
				        BytesToSave = CompressLine8(pLineBuffer, pFrame->GetBuffer() + pFrame->GetYTable()[i], 
					        pFrame->GetWidth());
				        TGAFile.Write(pLineBuffer, BytesToSave);
        #else
				        TGAFile.Write(pFrame->GetBuffer() + pFrame->GetYTable()[i], pFrame->GetWidth());
        #endif
			        }
			        break;

		        case 24:
			        for(i = pFrame->GetHeight()-1; i>=0; i-- ) 
			        {
        #if TURN_ON_COMPRESSION
				        BytesToSave = CompressLine24(pLineBuffer, ((Pixel24*)pFrame->GetBuffer()) + pFrame->GetYTable()[i], pFrame->GetWidth());
				        TGAFile.Write(pLineBuffer, BytesToSave);
        #else
				        TGAFile.Write((void*)(((Pixel24*)pFrame->GetBuffer()) + pFrame->GetYTable()[i]), pFrame->GetWidth() * 3);
        #endif
			        }
			        break;

		        case 32:
			        for(i = pFrame->GetHeight()-1; i>=0; i-- ) 
			        {
				        BytesToSave = CompressLine32(pLineBuffer, ((Pixel32*)pFrame->GetBuffer()) + pFrame->GetYTable()[i], pFrame->GetWidth());
				        TGAFile.Write(pLineBuffer, BytesToSave);
			        }
			        break;

		        default:
			        TGAFile.Close();
			        assert(0);
			        return false;
		        }
        		
		        ArrayDeleteAndSetNull(pLineBuffer);

		        TGAFile.Close();
		        return true;
	        }
        	
	        return false;
        }

        //*********************************************************************************************************************
        bool SourceNeedsToBeResaved(String pFileName)
        {
	        CFile TGAFile;
	        if(TGAFile.Open(pFileName, CFile::modeRead))
	        {
		        STargaHeader TargaHeader;
		        Byte[] pWorkPtr = new Byte[sizeof(STargaHeader)];

		        TGAFile.Read(pWorkPtr, sizeof(STargaHeader));
		        TGAFile.Close();

		        if(ReadTGAInfo(pWorkPtr, &TargaHeader))
		        {
			        ArrayDeleteAndSetNull(pWorkPtr);
			        return TargaHeader.ImageType != 10;
		        }

		        ArrayDeleteAndSetNull(pWorkPtr);
	        }

	        return true;
        }
         */

        //*********************************************************************************************************************
        static public int ReadBitsFromBuffer(Image<T> pFrame, Byte[] WorkPtr, int DestBitDepth)
        {
            return LowLevelReadTGABitsFromBuffer(pFrame, WorkPtr, DestBitDepth);
        }

        /*
        //*********************************************************************************************************************
        int GetBitDepth(Stream streamToReadFrom)
        {
	        STargaHeader TargaHeader;
	        if(ReadTGAInfo(streamToReadFrom, &TargaHeader))
	        {
		        return TargaHeader.BPP;
	        }

	        return 0;
        }
         */
    }
}
