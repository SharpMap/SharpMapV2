#define SourceDepth24

using System;
using pix_format_e = AGG.UI.PlatformSupportAbstract.PixelFormats;
using AGG.PixelFormat;
using AGG.UI;
using AGG.VertexSource;

namespace AGG
{
    class pattern_src_brightness_to_alpha_RGBA_Bytes : PixelFormatProxy
    {
        static byte[] brightness_to_alpha = 
        {
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 254, 254, 254, 254, 254, 254, 
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 
            254, 254, 254, 254, 254, 254, 254, 254, 254, 254, 254, 254, 254, 254, 253, 253, 
            253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 253, 252, 
            252, 252, 252, 252, 252, 252, 252, 252, 252, 252, 252, 251, 251, 251, 251, 251, 
            251, 251, 251, 251, 250, 250, 250, 250, 250, 250, 250, 250, 249, 249, 249, 249, 
            249, 249, 249, 248, 248, 248, 248, 248, 248, 248, 247, 247, 247, 247, 247, 246, 
            246, 246, 246, 246, 246, 245, 245, 245, 245, 245, 244, 244, 244, 244, 243, 243, 
            243, 243, 243, 242, 242, 242, 242, 241, 241, 241, 241, 240, 240, 240, 239, 239, 
            239, 239, 238, 238, 238, 238, 237, 237, 237, 236, 236, 236, 235, 235, 235, 234, 
            234, 234, 233, 233, 233, 232, 232, 232, 231, 231, 230, 230, 230, 229, 229, 229, 
            228, 228, 227, 227, 227, 226, 226, 225, 225, 224, 224, 224, 223, 223, 222, 222, 
            221, 221, 220, 220, 219, 219, 219, 218, 218, 217, 217, 216, 216, 215, 214, 214, 
            213, 213, 212, 212, 211, 211, 210, 210, 209, 209, 208, 207, 207, 206, 206, 205, 
            204, 204, 203, 203, 202, 201, 201, 200, 200, 199, 198, 198, 197, 196, 196, 195, 
            194, 194, 193, 192, 192, 191, 190, 190, 189, 188, 188, 187, 186, 186, 185, 184, 
            183, 183, 182, 181, 180, 180, 179, 178, 177, 177, 176, 175, 174, 174, 173, 172, 
            171, 171, 170, 169, 168, 167, 166, 166, 165, 164, 163, 162, 162, 161, 160, 159, 
            158, 157, 156, 156, 155, 154, 153, 152, 151, 150, 149, 148, 148, 147, 146, 145, 
            144, 143, 142, 141, 140, 139, 138, 137, 136, 135, 134, 133, 132, 131, 130, 129, 
            128, 128, 127, 125, 124, 123, 122, 121, 120, 119, 118, 117, 116, 115, 114, 113, 
            112, 111, 110, 109, 108, 107, 106, 105, 104, 102, 101, 100,  99,  98,  97,  96,  
             95,  94,  93,  91,  90,  89,  88,  87,  86,  85,  84,  82,  81,  80,  79,  78, 
             77,  75,  74,  73,  72,  71,  70,  69,  67,  66,  65,  64,  63,  61,  60,  59, 
             58,  57,  56,  54,  53,  52,  51,  50,  48,  47,  46,  45,  44,  42,  41,  40, 
             39,  37,  36,  35,  34,  33,  31,  30,  29,  28,  27,  25,  24,  23,  22,  20, 
             19,  18,  17,  15,  14,  13,  12,  11,   9,   8,   7,   6,   4,   3,   2,   1
        };

        public pattern_src_brightness_to_alpha_RGBA_Bytes(RasterBuffer rb)
            : base(new FormatRGB(rb, new BlenderBGR()))
        {
        }

        public override RGBA_Bytes pixel(int x, int y)
        {
            RGBA_Bytes c = m_pixf.pixel(x, y);
            c.A_Byte = brightness_to_alpha[c.R_Byte + c.G_Byte + c.B_Byte];
            return c;
        }
    };


    public class line_patterns_application : AGG.UI.win32.PlatformSupport
    {
        RGBA_Bytes m_ctrl_color;
        cbox_ctrl m_approximation_method;
        bezier_ctrl m_curve1 = new bezier_ctrl();
        bezier_ctrl m_curve2 = new bezier_ctrl();
        bezier_ctrl m_curve3 = new bezier_ctrl();
        bezier_ctrl m_curve4 = new bezier_ctrl();
        bezier_ctrl m_curve5 = new bezier_ctrl();
        bezier_ctrl m_curve6 = new bezier_ctrl();
        bezier_ctrl m_curve7 = new bezier_ctrl();
        bezier_ctrl m_curve8 = new bezier_ctrl();
        bezier_ctrl m_curve9 = new bezier_ctrl();
        SliderWidget m_scale_x;
        SliderWidget m_start_x;

        line_patterns_application(PixelFormats format, AGG.UI.PlatformSupportAbstract.ERenderOrigin RenderOrigin)
            : base(format, RenderOrigin)
        {
            m_ctrl_color = new RGBA_Bytes(0, 0.3, 0.5, 0.3);
            m_scale_x = new SliderWidget(5.0,   5.0, 240.0, 12.0);
            m_start_x = new SliderWidget(250.0, 5.0, 495.0, 12.0);
            m_approximation_method = new cbox_ctrl(10, 30, "Approximation Method = curve_div");

            m_curve1.line_color(m_ctrl_color);
            m_curve2.line_color(m_ctrl_color);
            m_curve3.line_color(m_ctrl_color);
            m_curve4.line_color(m_ctrl_color);
            m_curve5.line_color(m_ctrl_color);
            m_curve6.line_color(m_ctrl_color);
            m_curve7.line_color(m_ctrl_color);
            m_curve8.line_color(m_ctrl_color);
            m_curve9.line_color(m_ctrl_color);

            m_curve1.curve(64, 19, 14, 126, 118, 266, 19, 265);
            m_curve2.curve(112, 113, 178, 32, 200, 132, 125, 438);
            m_curve3.curve(401, 24, 326, 149, 285, 11, 177, 77);
            m_curve4.curve(188, 427, 129, 295, 19, 283, 25, 410);
            m_curve5.curve(451, 346, 302, 218, 265, 441, 459, 400);
            m_curve6.curve(454, 198, 14, 13, 220, 291, 483, 283);
            m_curve7.curve(301, 398, 355, 231, 209, 211, 170, 353);
            m_curve8.curve(484, 101, 222, 33, 486, 435, 487, 138);
            m_curve9.curve(143, 147, 11, 45, 83, 427, 132, 197);

            AddChild(m_curve1);
            AddChild(m_curve2);
            AddChild(m_curve3);
            AddChild(m_curve4);
            AddChild(m_curve5);
            AddChild(m_curve6);
            AddChild(m_curve7);
            AddChild(m_curve8);
            AddChild(m_curve9);
            AddChild(m_approximation_method);

            m_scale_x.label("Scale X=%.2f");
            m_scale_x.range(0.2, 3.0);
            m_scale_x.value(1.0);
            AddChild(m_scale_x);

            m_start_x.label("Start X=%.2f");
            m_start_x.range(0.0, 10.0);
            m_start_x.value(0.0);
            AddChild(m_start_x);
        }

        /*
            void draw_curve(Pattern patt, IRasterizer ras, Renderer ren, PatternSource src, IVertexSource vs)
            {
                patt.create(src);
                ren.scale_x(m_scale_x.value());
                ren.start_x(m_start_x.value());
                ras.add_path(vs);
            }
         */

        public override void  OnDraw()
        {
        #if SourceDepth24
                    FormatRGB pf = new FormatRGB(rbuf_window(), new BlenderBGR());
        #else
                    FormatRGBA pf = new FormatRGBA(rbuf_window(), new BlenderBGRA());
        #endif
                    FormatClippingProxy ren_base = new FormatClippingProxy(pf);

            ren_base.clear(new RGBA_Doubles(1.0, 1.0, .95));

            rasterizer_scanline_aa ras = new rasterizer_scanline_aa();
            scanline_packed_8 sl = new scanline_packed_8();

            // Pattern source. Must have an interface:
            // width() const
            // height() const
            // pixel(int x, int y) const
            // Any agg::renderer_base<> or derived
            // is good for the use as a source.
            //-----------------------------------
            pattern_src_brightness_to_alpha_RGBA_Bytes p1 = new pattern_src_brightness_to_alpha_RGBA_Bytes(rbuf_img(0));
            pattern_src_brightness_to_alpha_RGBA_Bytes p2 = new pattern_src_brightness_to_alpha_RGBA_Bytes(rbuf_img(1));
            pattern_src_brightness_to_alpha_RGBA_Bytes p3 = new pattern_src_brightness_to_alpha_RGBA_Bytes(rbuf_img(2));
            pattern_src_brightness_to_alpha_RGBA_Bytes p4 = new pattern_src_brightness_to_alpha_RGBA_Bytes(rbuf_img(3));
            pattern_src_brightness_to_alpha_RGBA_Bytes p5 = new pattern_src_brightness_to_alpha_RGBA_Bytes(rbuf_img(4));
            pattern_src_brightness_to_alpha_RGBA_Bytes p6 = new pattern_src_brightness_to_alpha_RGBA_Bytes(rbuf_img(5));
            pattern_src_brightness_to_alpha_RGBA_Bytes p7 = new pattern_src_brightness_to_alpha_RGBA_Bytes(rbuf_img(6));
            pattern_src_brightness_to_alpha_RGBA_Bytes p8 = new pattern_src_brightness_to_alpha_RGBA_Bytes(rbuf_img(7));
            pattern_src_brightness_to_alpha_RGBA_Bytes p9 = new pattern_src_brightness_to_alpha_RGBA_Bytes(rbuf_img(8));

            //pattern_filter_bilinear_rgba 
            pattern_filter_bilinear_RGBA_Bytes fltr;           // Filtering functor

            // agg::line_image_pattern is the main container for the patterns. It creates
            // a copy of the patterns extended according to the needs of the filter.
            // agg::line_image_pattern can operate with arbitrary image width, but if the 
            // width of the pattern is power of 2, it's better to use the modified
            // version agg::line_image_pattern_pow2 because it works about 15-25 percent
            // faster than agg::line_image_pattern (because of using simple masking instead 
            // of expensive '%' operation). 
            
            //typedef agg::line_image_pattern<agg::pattern_filter_bilinear_RGBA_Bytes> pattern_type;
            //typedef agg::renderer_base<pixfmt> base_ren_type;
            //typedef agg::renderer_outline_image<base_ren_type, pattern_type> renderer_type;
            //typedef agg::rasterizer_outline_aa<renderer_type>                rasterizer_type;

            //-- Create with specifying the source
            //pattern_type patt(fltr, src);   

            //-- Create uninitialized and set the source
            line_image_pattern patt = new line_image_pattern(new pattern_filter_bilinear_RGBA_Bytes());
            //renderer_outline_image ren_img = new renderer_outline_image(, patt);
            /*
            renderer_type ren_img(ren_base, patt);
            rasterizer_type ras_img(ren_img);

            draw_curve(patt, ras_img, ren_img, p1, m_curve1.curve());
            draw_curve(patt, ras_img, ren_img, p2, m_curve2.curve());
            draw_curve(patt, ras_img, ren_img, p3, m_curve3.curve());
            draw_curve(patt, ras_img, ren_img, p4, m_curve4.curve());
            draw_curve(patt, ras_img, ren_img, p5, m_curve5.curve());
            draw_curve(patt, ras_img, ren_img, p6, m_curve6.curve());
            draw_curve(patt, ras_img, ren_img, p7, m_curve7.curve());
            draw_curve(patt, ras_img, ren_img, p8, m_curve8.curve());
            draw_curve(patt, ras_img, ren_img, p9, m_curve9.curve());
             */
            base.OnDraw();
        }

        public override void OnControlChanged()
        {
            base.OnControlChanged();
            Curves.curve_approximation_method_e method = Curves.curve_approximation_method_e.curve_div;
            if (m_approximation_method.status())
            {
                method = Curves.curve_approximation_method_e.curve_inc;
                m_approximation_method.label("Approximation Method = curve_inc");
            }
            else
            {
                m_approximation_method.label("Approximation Method = curve_div");
            }
            m_curve1.curve().approximation_method(method);
            m_curve2.curve().approximation_method(method);
            m_curve3.curve().approximation_method(method);
            m_curve4.curve().approximation_method(method);
            m_curve5.curve().approximation_method(method);
            m_curve6.curve().approximation_method(method);
            m_curve7.curve().approximation_method(method);
            m_curve8.curve().approximation_method(method);
            m_curve9.curve().approximation_method(method);
        }

		public static void StartDemo()
		{
#if SourceDepth24
            line_patterns_application app = new line_patterns_application(pix_format_e.pix_format_rgb24, AGG.UI.PlatformSupportAbstract.ERenderOrigin.OriginBottomLeft);
#else
            line_patterns_application app = new line_patterns_application(PixelFormats.pix_format_rgba32, AGG.UI.PlatformSupportAbstract.ERenderOrigin.OriginBottomLeft);
#endif
            app.Caption = "AGG Example. Drawing Lines with Image Patterns";

            if(!app.load_img(0, "1") ||
               !app.load_img(1, "2") ||
               !app.load_img(2, "3") ||
               !app.load_img(3, "4") ||
               !app.load_img(4, "5") ||
               !app.load_img(5, "6") ||
               !app.load_img(6, "7") ||
               !app.load_img(7, "8") ||
               !app.load_img(8, "9"))
            {
                String buf = "There must be files 1%s...9%s\n"
                             + "Download and unzip:\n"
                             + "http://www.antigrain.com/line_patterns.bmp.zip\n"
                             + "or\n"
                             + "http://www.antigrain.com/line_patterns.ppm.tar.gz\n";
                throw new System.Exception(buf);
            }

            if(app.init(500, 450, (uint)WindowFlags.Risizeable))
            {
                app.run();
            }
        }

		[STAThread]
        public static void Main(string[] args)
        {
        	StartDemo();
        }
    };
}




