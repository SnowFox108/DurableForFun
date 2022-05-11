using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PdfSharp.Fonts;

namespace PdfGenerator
{
    public class AzureFontResolver : IFontResolver
    {
        public FontResolverInfo ResolveTypeface(string familyName,
            bool isBold,
            bool isItalic)
        {
            // Ignore case of font names.
            var name = familyName.ToLower();

            // Add fonts here
            switch (name)
            {
                case "arial":
                    return new FontResolverInfo("Arial#");
            }

            //Return a default font if the font couldn't be found
            //this is not a unicode font 
            return PlatformFontResolver.ResolveTypeface("Arial", isBold, isItalic);
        }

        // Return the font data for the fonts.
        public byte[] GetFont(string faceName)
        {
            switch (faceName)
            {
                case "Arial#": return FontHelper.ARIAL; break;
            }

            return null;
        }

    }

    public static class FontHelper
    {
        public static byte[] ARIAL
        {
            //the font is in the folder "/fonts" in the project
            get { return LoadFontData("PdfGenerator.ARIAL.TTF"); }
        }

        /// Returns the specified font from an embedded resource.
        static byte[] LoadFontData(string name)
        {

            var assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(name))
            {
                if (stream == null)
                    throw new ArgumentException("No resource with name " + name);

                int count = (int)stream.Length;
                byte[] data = new byte[count];
                stream.Read(data, 0, count);
                return data;
            }
        }
    }
}
