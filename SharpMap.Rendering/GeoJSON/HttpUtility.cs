//JD: Borrowed from Mono project:

//
// System.Web.HttpUtility
//
// Authors:
// Patrik Torstensson (Click here to reveal e-mail address)
// Wictor Wilén (decode/encode functions) (Click here to reveal e-mail address)
// Tim Coleman (Click here to reveal e-mail address)
// Gonzalo Paniagua Javier (Click here to reveal e-mail address)
//
// 12/30/03 -- Modified by Kevin Z Grey (Click here to reveal e-mail address) to compile
// on the .NET Compact Framework
//
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;

namespace SharpMap.Rendering.GeoJson
{
    public sealed class HttpUtility
    {
        #region Fields

        private const string _chars = "<>;:.?=&@*+%/\\";
        private const string _hex = "0123456789ABCDEF";
        private static readonly Hashtable entities;

        #endregion // Fields

        #region Constructors

        static HttpUtility()
        {
            // Build the hash table of HTML entity references. This list comes
            // from the HTML 4.01 W3C recommendation.
            entities = new Hashtable
                           {
                               {"nbsp", '\u00A0'},
                               {"iexcl", '\u00A1'},
                               {"cent", '\u00A2'},
                               {"pound", '\u00A3'},
                               {"curren", '\u00A4'},
                               {"yen", '\u00A5'},
                               {"brvbar", '\u00A6'},
                               {"sect", '\u00A7'},
                               {"uml", '\u00A8'},
                               {"copy", '\u00A9'},
                               {"ordf", '\u00AA'},
                               {"laquo", '\u00AB'},
                               {"not", '\u00AC'},
                               {"shy", '\u00AD'},
                               {"reg", '\u00AE'},
                               {"macr", '\u00AF'},
                               {"deg", '\u00B0'},
                               {"plusmn", '\u00B1'},
                               {"sup2", '\u00B2'},
                               {"sup3", '\u00B3'},
                               {"acute", '\u00B4'},
                               {"micro", '\u00B5'},
                               {"para", '\u00B6'},
                               {"middot", '\u00B7'},
                               {"cedil", '\u00B8'},
                               {"sup1", '\u00B9'},
                               {"ordm", '\u00BA'},
                               {"raquo", '\u00BB'},
                               {"frac14", '\u00BC'},
                               {"frac12", '\u00BD'},
                               {"frac34", '\u00BE'},
                               {"iquest", '\u00BF'},
                               {"Agrave", '\u00C0'},
                               {"Aacute", '\u00C1'},
                               {"Acirc", '\u00C2'},
                               {"Atilde", '\u00C3'},
                               {"Auml", '\u00C4'},
                               {"Aring", '\u00C5'},
                               {"AElig", '\u00C6'},
                               {"Ccedil", '\u00C7'},
                               {"Egrave", '\u00C8'},
                               {"Eacute", '\u00C9'},
                               {"Ecirc", '\u00CA'},
                               {"Euml", '\u00CB'},
                               {"Igrave", '\u00CC'},
                               {"Iacute", '\u00CD'},
                               {"Icirc", '\u00CE'},
                               {"Iuml", '\u00CF'},
                               {"ETH", '\u00D0'},
                               {"Ntilde", '\u00D1'},
                               {"Ograve", '\u00D2'},
                               {"Oacute", '\u00D3'},
                               {"Ocirc", '\u00D4'},
                               {"Otilde", '\u00D5'},
                               {"Ouml", '\u00D6'},
                               {"times", '\u00D7'},
                               {"Oslash", '\u00D8'},
                               {"Ugrave", '\u00D9'},
                               {"Uacute", '\u00DA'},
                               {"Ucirc", '\u00DB'},
                               {"Uuml", '\u00DC'},
                               {"Yacute", '\u00DD'},
                               {"THORN", '\u00DE'},
                               {"szlig", '\u00DF'},
                               {"agrave", '\u00E0'},
                               {"aacute", '\u00E1'},
                               {"acirc", '\u00E2'},
                               {"atilde", '\u00E3'},
                               {"auml", '\u00E4'},
                               {"aring", '\u00E5'},
                               {"aelig", '\u00E6'},
                               {"ccedil", '\u00E7'},
                               {"egrave", '\u00E8'},
                               {"eacute", '\u00E9'},
                               {"ecirc", '\u00EA'},
                               {"euml", '\u00EB'},
                               {"igrave", '\u00EC'},
                               {"iacute", '\u00ED'},
                               {"icirc", '\u00EE'},
                               {"iuml", '\u00EF'},
                               {"eth", '\u00F0'},
                               {"ntilde", '\u00F1'},
                               {"ograve", '\u00F2'},
                               {"oacute", '\u00F3'},
                               {"ocirc", '\u00F4'},
                               {"otilde", '\u00F5'},
                               {"ouml", '\u00F6'},
                               {"divide", '\u00F7'},
                               {"oslash", '\u00F8'},
                               {"ugrave", '\u00F9'},
                               {"uacute", '\u00FA'},
                               {"ucirc", '\u00FB'},
                               {"uuml", '\u00FC'},
                               {"yacute", '\u00FD'},
                               {"thorn", '\u00FE'},
                               {"yuml", '\u00FF'},
                               {"fnof", '\u0192'},
                               {"Alpha", '\u0391'},
                               {"Beta", '\u0392'},
                               {"Gamma", '\u0393'},
                               {"Delta", '\u0394'},
                               {"Epsilon", '\u0395'},
                               {"Zeta", '\u0396'},
                               {"Eta", '\u0397'},
                               {"Theta", '\u0398'},
                               {"Iota", '\u0399'},
                               {"Kappa", '\u039A'},
                               {"Lambda", '\u039B'},
                               {"Mu", '\u039C'},
                               {"Nu", '\u039D'},
                               {"Xi", '\u039E'},
                               {"Omicron", '\u039F'},
                               {"Pi", '\u03A0'},
                               {"Rho", '\u03A1'},
                               {"Sigma", '\u03A3'},
                               {"Tau", '\u03A4'},
                               {"Upsilon", '\u03A5'},
                               {"Phi", '\u03A6'},
                               {"Chi", '\u03A7'},
                               {"Psi", '\u03A8'},
                               {"Omega", '\u03A9'},
                               {"alpha", '\u03B1'},
                               {"beta", '\u03B2'},
                               {"gamma", '\u03B3'},
                               {"delta", '\u03B4'},
                               {"epsilon", '\u03B5'},
                               {"zeta", '\u03B6'},
                               {"eta", '\u03B7'},
                               {"theta", '\u03B8'},
                               {"iota", '\u03B9'},
                               {"kappa", '\u03BA'},
                               {"lambda", '\u03BB'},
                               {"mu", '\u03BC'},
                               {"nu", '\u03BD'},
                               {"xi", '\u03BE'},
                               {"omicron", '\u03BF'},
                               {"pi", '\u03C0'},
                               {"rho", '\u03C1'},
                               {"sigmaf", '\u03C2'},
                               {"sigma", '\u03C3'},
                               {"tau", '\u03C4'},
                               {"upsilon", '\u03C5'},
                               {"phi", '\u03C6'},
                               {"chi", '\u03C7'},
                               {"psi", '\u03C8'},
                               {"omega", '\u03C9'},
                               {"thetasym", '\u03D1'},
                               {"upsih", '\u03D2'},
                               {"piv", '\u03D6'},
                               {"bull", '\u2022'},
                               {"hellip", '\u2026'},
                               {"prime", '\u2032'},
                               {"Prime", '\u2033'},
                               {"oline", '\u203E'},
                               {"frasl", '\u2044'},
                               {"weierp", '\u2118'},
                               {"image", '\u2111'},
                               {"real", '\u211C'},
                               {"trade", '\u2122'},
                               {"alefsym", '\u2135'},
                               {"larr", '\u2190'},
                               {"uarr", '\u2191'},
                               {"rarr", '\u2192'},
                               {"darr", '\u2193'},
                               {"harr", '\u2194'},
                               {"crarr", '\u21B5'},
                               {"lArr", '\u21D0'},
                               {"uArr", '\u21D1'},
                               {"rArr", '\u21D2'},
                               {"dArr", '\u21D3'},
                               {"hArr", '\u21D4'},
                               {"forall", '\u2200'},
                               {"part", '\u2202'},
                               {"exist", '\u2203'},
                               {"empty", '\u2205'},
                               {"nabla", '\u2207'},
                               {"isin", '\u2208'},
                               {"notin", '\u2209'},
                               {"ni", '\u220B'},
                               {"prod", '\u220F'},
                               {"sum", '\u2211'},
                               {"minus", '\u2212'},
                               {"lowast", '\u2217'},
                               {"radic", '\u221A'},
                               {"prop", '\u221D'},
                               {"infin", '\u221E'},
                               {"ang", '\u2220'},
                               {"and", '\u2227'},
                               {"or", '\u2228'},
                               {"cap", '\u2229'},
                               {"cup", '\u222A'},
                               {"int", '\u222B'},
                               {"there4", '\u2234'},
                               {"sim", '\u223C'},
                               {"cong", '\u2245'},
                               {"asymp", '\u2248'},
                               {"ne", '\u2260'},
                               {"equiv", '\u2261'},
                               {"le", '\u2264'},
                               {"ge", '\u2265'},
                               {"sub", '\u2282'},
                               {"sup", '\u2283'},
                               {"nsub", '\u2284'},
                               {"sube", '\u2286'},
                               {"supe", '\u2287'},
                               {"oplus", '\u2295'},
                               {"otimes", '\u2297'},
                               {"perp", '\u22A5'},
                               {"sdot", '\u22C5'},
                               {"lceil", '\u2308'},
                               {"rceil", '\u2309'},
                               {"lfloor", '\u230A'},
                               {"rfloor", '\u230B'},
                               {"lang", '\u2329'},
                               {"rang", '\u232A'},
                               {"loz", '\u25CA'},
                               {"spades", '\u2660'},
                               {"clubs", '\u2663'},
                               {"hearts", '\u2665'},
                               {"diams", '\u2666'},
                               {"quot", '\u0022'},
                               {"amp", '\u0026'},
                               {"lt", '\u003C'},
                               {"gt", '\u003E'},
                               {"OElig", '\u0152'},
                               {"oelig", '\u0153'},
                               {"Scaron", '\u0160'},
                               {"scaron", '\u0161'},
                               {"Yuml", '\u0178'},
                               {"circ", '\u02C6'},
                               {"tilde", '\u02DC'},
                               {"ensp", '\u2002'},
                               {"emsp", '\u2003'},
                               {"thinsp", '\u2009'},
                               {"zwnj", '\u200C'},
                               {"zwj", '\u200D'},
                               {"lrm", '\u200E'},
                               {"rlm", '\u200F'},
                               {"ndash", '\u2013'},
                               {"mdash", '\u2014'},
                               {"lsquo", '\u2018'},
                               {"rsquo", '\u2019'},
                               {"sbquo", '\u201A'},
                               {"ldquo", '\u201C'},
                               {"rdquo", '\u201D'},
                               {"bdquo", '\u201E'},
                               {"dagger", '\u2020'},
                               {"Dagger", '\u2021'},
                               {"permil", '\u2030'},
                               {"lsaquo", '\u2039'},
                               {"rsaquo", '\u203A'},
                               {"euro", '\u20AC'}
                           };
        }

        #endregion // Constructors

        #region Methods

        private static readonly char[] hexChars = "0123456789ABCDEF".ToCharArray();

        public static void HtmlAttributeEncode(string s, TextWriter output)
        {
            output.Write(HtmlAttributeEncode(s));
        }

        public static string HtmlAttributeEncode(string s)
        {
            if (null == s)
                return null;

            var output = new StringBuilder();

            foreach (char c in s)
                switch (c)
                {
                    case '&':

                        output.Append("&");
                        break;
                    case '"':
                        {
                            output.Append('"');
                            break;
                        }
                    default:

                        output.Append(c);
                        break;
                }

            return output.ToString();
        }

        public static string UrlDecode(string str)
        {
            return UrlDecode(str, Encoding.UTF8);
        }

        private static char[] GetChars(MemoryStream b, Encoding e)
        {
            return e.GetChars(b.GetBuffer(), 0, (int) b.Length);
        }

        public static string UrlDecode(string s, Encoding e)
        {
            if (null == s)
                return null;

            if (e == null)
                e = Encoding.UTF8;

            var output = new StringBuilder();
            long len = s.Length;
            NumberStyles hexa = NumberStyles.HexNumber;
            var bytes = new MemoryStream();

            for (int i = 0; i < len; i++)
            {
                if (s[i] == '%' && i + 2 < len)
                {
                    if (s[i + 1] == 'u' && i + 5 < len)
                    {
                        if (bytes.Length > 0)
                        {
                            output.Append(GetChars(bytes, e));

                            bytes.SetLength(0);
                        }

                        output.Append((char) Int32.Parse(s.Substring(i + 2, 4), hexa));
                        i += 5;
                    }
                    else
                    {
                        bytes.WriteByte((byte) Int32.Parse(s.Substring
                                                               (i + 1, 2), hexa));
                        i += 2;
                    }
                    continue;
                }

                if (bytes.Length > 0)
                {
                    output.Append(GetChars(bytes, e));
                    bytes.SetLength(0);
                }

                if (s[i] == '+')
                {
                    output.Append(' ');
                }
                else
                {
                    output.Append(s[i]);
                }
            }

            if (bytes.Length > 0)
            {
                output.Append(GetChars(bytes, e));
            }

            bytes = null;
            return output.ToString();
        }

        public static string UrlDecode(byte[] bytes, Encoding e)
        {
            if (bytes == null)
                return null;

            return UrlDecode(bytes, 0, bytes.Length, e);
        }

        private static int GetInt(byte b)
        {
            char c = Char.ToUpper((char) b);
            if (c >= '0' && c <= '9')
                return c - '0';

            if (c < 'A' || c > 'F')
                return 0;

            return (c - 'A' + 10);
        }

        private static char GetChar(byte[] bytes, int offset, int length)
        {
            int value = 0;
            int end = length + offset;
            for (int i = offset; i < end; i++)
                value = (value << 4) + GetInt(bytes[offset]);

            return (char) value;
        }

        public static string UrlDecode(byte[] bytes, int offset, int count, Encoding e)
        {
            if (bytes == null || count == 0)
                return null;

            if (bytes == null)
                throw new ArgumentNullException("bytes");

            if (offset < 0 || offset > bytes.Length)
                throw new ArgumentOutOfRangeException("offset");

            if (count < 0 || offset + count > bytes.Length)
                throw new ArgumentOutOfRangeException("count");

            var output = new StringBuilder();
            var acc = new MemoryStream();

            int end = count + offset;
            for (int i = offset; i < end; i++)
            {
                if (bytes[i] == '%' && i + 2 < count)
                {
                    if (bytes[i + 1] == (byte) 'u' && i + 5 < end)
                    {
                        if (acc.Length > 0)
                        {
                            output.Append(GetChars(acc, e));

                            acc.SetLength(0);
                        }

                        output.Append(GetChar(bytes, offset + 2, 4));
                        i += 5;
                    }
                    else
                    {
                        acc.WriteByte((byte) GetChar(bytes, offset + 1, 2));
                        i += 2;
                    }
                    continue;
                }

                if (acc.Length > 0)
                {
                    output.Append(GetChars(acc, e));
                    acc.SetLength(0);
                }

                if (bytes[i] == '+')
                {
                    output.Append(' ');
                }
                else
                {
                    output.Append((char) bytes[i]);
                }
            }

            if (acc.Length > 0)
            {
                output.Append(GetChars(acc, e));
            }

            acc = null;
            return output.ToString();
        }

        public static byte[] UrlDecodeToBytes(byte[] bytes)
        {
            if (bytes == null)
                return null;

            return UrlDecodeToBytes(bytes, 0, bytes.Length);
        }

        public static byte[] UrlDecodeToBytes(string str)
        {
            return UrlDecodeToBytes(str, Encoding.UTF8);
        }

        public static byte[] UrlDecodeToBytes(string str, Encoding e)
        {
            if (str == null)
                return null;

            if (e == null)
                throw new ArgumentNullException("e");

            return UrlDecodeToBytes(e.GetBytes(str));
        }

        public static byte[] UrlDecodeToBytes(byte[] bytes, int offset, int count)
        {
            if (bytes == null)
                return null;

            int len = bytes.Length;
            if (offset < 0 || offset >= len)
                throw new ArgumentOutOfRangeException("offset");

            if (count < 0 || offset <= len - count)
                throw new ArgumentOutOfRangeException("count");

            var result = new ArrayList();
            int end = offset + count;
            for (int i = offset; i < end; i++)
            {
                var c = (char) bytes[i];
                if (c == '+')
                    c = ' ';
                else if (c == '%' && i < end - 2)
                {
                    c = GetChar(bytes, i, 2);
                    i += 2;
                }
                result.Add((byte) c);
            }

            return (byte[]) result.ToArray(typeof (byte));
        }

        public static string UrlEncode(string str)
        {
            return UrlEncode(str, Encoding.UTF8);
        }

        public static string UrlEncode(string s, Encoding Enc)
        {
            if (s == null)
                return null;

            if (s == "")
                return "";

            byte[] bytes = Enc.GetBytes(s);
            bytes = UrlEncodeToBytes(bytes, 0, bytes.Length);
            return Encoding.ASCII.GetString(bytes, 0, bytes.Length);
        }

        public static string UrlEncode(byte[] bytes)
        {
            if (bytes == null)
                return null;

            if (bytes.Length == 0)
                return "";
            bytes = UrlEncodeToBytes(bytes, 0, bytes.Length);
            return Encoding.ASCII.GetString(bytes, 0, bytes.Length);
        }

        public static string UrlEncode(byte[] bytes, int offset, int count)
        {
            if (bytes == null)
                return null;

            if (bytes.Length == 0)
                return "";
            bytes = UrlEncodeToBytes(bytes, offset, count);
            return Encoding.ASCII.GetString(bytes, 0, bytes.Length);
        }

        public static byte[] UrlEncodeToBytes(string str)
        {
            return UrlEncodeToBytes(str, Encoding.UTF8);
        }

        public static byte[] UrlEncodeToBytes(string str, Encoding e)
        {
            if (str == null)
                return null;

            if (str == "")
                return new byte[0];

            byte[] bytes = e.GetBytes(str);
            return UrlEncodeToBytes(bytes, 0, bytes.Length);
        }

        public static byte[] UrlEncodeToBytes(byte[] bytes)
        {
            if (bytes == null)
                return null;

            if (bytes.Length == 0)
                return new byte[0];

            return UrlEncodeToBytes(bytes, 0, bytes.Length);
        }

        public static byte[] UrlEncodeToBytes(byte[] bytes, int offset, int count)
        {
            if (bytes == null)
                return null;

            int len = bytes.Length;
            if (len == 0)
                return new byte[0];

            if (offset < 0 || offset >= len)
                throw new ArgumentOutOfRangeException("offset");

            if (count < 0 || offset < len - count)
                throw new ArgumentOutOfRangeException("count");

            var result = new ArrayList();
            int end = offset + count;
            for (int i = offset; i < end; i++)
            {
                var c = (char) bytes[i];
                if (c == ' ')
                    result.Add((byte) '+');
                else if ((c < '0' && c != '-' && c != '.')
                         || (c < 'A' && c > '9')
                         || (c > 'Z' && c < 'a' && c != '_')
                         || (c > 'z'))
                {
                    result.Add((byte) '%');
                    int idx = (c) >> 4;
                    result.Add((byte) hexChars[idx]);
                    idx = (c) & 0x0F;
                    result.Add((byte) hexChars[idx]);
                }
                else
                {
                    result.Add((byte) c);
                }
            }

            return (byte[]) result.ToArray(typeof (byte));
        }

        public static string UrlEncodeUnicode(string str)
        {
            if (str == null)
                return null;

            var result = new StringBuilder();
            int end = str.Length;
            for (int i = 0; i < end; i++)
            {
                int idx;
                char c = str[i];
                if (c == ' ')
                {
                    result.Append('+');
                    continue;
                }

                if (c > 255)
                {
                    result.Append("%u");
                    idx = (c) >> 24;
                    result.Append(hexChars[idx]);
                    idx = ((c) >> 16) & 0x0F;
                    result.Append(hexChars[idx]);
                    idx = ((c) >> 8) & 0x0F;
                    result.Append(hexChars[idx]);
                    idx = (c) & 0x0F;
                    result.Append(hexChars[idx]);
                    continue;
                }

                if ((c < '0' && c != '-' && c != '.')
                    || (c < 'A' && c > '9')
                    || (c > 'Z' && c < 'a' && c != '_')
                    || (c > 'z'))
                {
                    result.Append('%');
                    idx = (c) >> 4;
                    result.Append(hexChars[idx]);
                    idx = (c) & 0x0F;
                    result.Append(hexChars[idx]);
                    continue;
                }

                result.Append(c);
            }

            return result.ToString();
        }

        public static byte[] UrlEncodeUnicodeToBytes(string str)
        {
            if (str == null)
                return null;

            if (str == "")
                return new byte[0];

            return Encoding.ASCII.GetBytes(UrlEncodeUnicode(str));
        }

        /// <summary>
        /// Decodes an HTML-encoded string and  returns the decoded string.
        /// </summary>
        /// <param name="s">The HTML string to decode. </param>
        /// <returns>The decoded text.</returns>
        public static string HtmlDecode(string s)
        {
            if (s == null)
                throw new ArgumentNullException("s");

            bool insideEntity = false; // used to indicate that we are in a potential entity
            string entity = String.Empty;
            var output = new StringBuilder();

            foreach (char c in s)
            {
                switch (c)
                {
                    case '&':

                        output.Append(entity);
                        entity = "&";

                        insideEntity = true;
                        break;
                    case ';':
                        if (!insideEntity)
                        {
                            output.Append(c);

                            break;
                        }

                        entity += c;
                        int length = entity.Length;
                        if (length >= 2 && entity[1] == '#' && entity[2] != ';')

                            entity = ((char) Int32.Parse(entity.Substring(2, entity.Length - 3))).ToString();
                        else if (length > 1 && entities.ContainsKey(entity.Substring(1, entity.Length - 2)))

                            entity = entities[entity.Substring(1, entity.Length - 2)].ToString();


                        output.Append(entity);
                        entity = String.Empty;

                        insideEntity = false;
                        break;
                    default:
                        if (insideEntity)

                            entity += c;
                        else

                            output.Append(c);
                        break;
                }
            }
            output.Append(entity);
            return output.ToString();
        }

        /// <summary>
        /// Decodes an HTML-encoded string and sends the resulting output to a TextWriter output stream.
        /// </summary>
        /// <param name="s">The HTML string to decode</param>
        /// <param name="output">The TextWriter output stream containing the decoded string. </param>
        public static void HtmlDecode(string s, TextWriter output)
        {
            output.Write(HtmlDecode(s));
        }

        /// <summary>
        /// HTML-encodes a string and returns the encoded string.
        /// </summary>
        /// <param name="s">The text string to encode. </param>
        /// <returns>The HTML-encoded text.</returns>
        public static string HtmlEncode(string s)
        {
            if (s == null)
                throw new ArgumentNullException("s");

            var output = new StringBuilder();

            foreach (char c in s)
                switch (c)
                {
                    case '&':

                        output.Append("&");
                        break;
                    case '>':

                        output.Append(">");
                        break;
                    case '<':

                        output.Append("<");
                        break;
                    case '"':

                        output.Append('"');
                        break;
                    default:
                        if (c > 128)
                        {
                            output.Append("&#");

                            output.Append(((int) c).ToString());

                            output.Append(";");
                        }
                        else

                            output.Append(c);
                        break;
                }
            return output.ToString();
        }

        /// <summary>
        /// HTML-encodes a string and sends the resulting output to a TextWriter output stream.
        /// </summary>
        /// <param name="s">The string to encode. </param>
        /// <param name="output">The TextWriter output stream containing the encoded string. </param>
        public static void HtmlEncode(string s, TextWriter output)
        {
            output.Write(HtmlEncode(s));
        }

        #endregion // Methods
    }
}