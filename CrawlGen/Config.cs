using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Xml;

namespace CrawlGen
{
    class Config
    {
        /// <summary>Artwork Title Image</summary>
        public Image TitleImage { get; private set; }

        /// <summary>Footer Image</summary>
        public Image FooterImage { get; private set; }

        /// <summary>Fonts and their key</summary>
        public Dictionary<char, Font> Fonts { get; private set; }

        /// <summary>Colors for keys</summary>
        public Dictionary<char, Color> Colors { get; private set; }

        /// <summary>Credits Output Width</summary>
        public int Width { get; private set; }

        /// <summary>Height of empty lines</summary>
        public int EmptyHeight { get; private set; }

        public TextRenderingHint TextRendering { get; private set; }

        /// <summary>Default properties</summary>
        public static readonly Config Defaults = new Config()
        {
            // no title and footer images
            TitleImage = null,
            FooterImage = null,

            // fonts
            Fonts = new Dictionary<char, Font>()
            {
                { '\0', new Font("Arial", 16) },
                { '-', new Font("Arial", 12) },
                { '=', new Font("Times New Roman", 14, FontStyle.Italic) },
            },
            Colors = new Dictionary<char, Color>()
            {
                { '\0', Color.White }
            },

            // sizing
            Width = 640,
            EmptyHeight = 24
        };

        /// <summary>
        /// Loads an XML configuration file and creates a config object
        /// </summary>
        /// <param name="filename"></param>
        public static Config LoadXml(string filename)
        {
            // load xml
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filename);

            // look for credits props
            int width = Defaults.Width;
            int emptyHeight = Defaults.EmptyHeight;
            TextRenderingHint textRendering = TextRenderingHint.SystemDefault;
            if (xmlDoc.DocumentElement.HasAttribute("width"))
                int.TryParse(xmlDoc.DocumentElement.GetAttribute("width"), out width);
            if (xmlDoc.DocumentElement.HasAttribute("empty-height"))
                int.TryParse(xmlDoc.DocumentElement.GetAttribute("empty-height"), out emptyHeight);
            if (xmlDoc.DocumentElement.HasAttribute("text-rendering"))
                Enum.TryParse(xmlDoc.DocumentElement.GetAttribute("text-rendering"), true, out textRendering);

            // load fonts
            Dictionary<char, Font> fonts = new Dictionary<char, Font>(Defaults.Fonts);
            Dictionary<char, Color> colors = new Dictionary<char, Color>(Defaults.Colors);
            foreach (XmlNode font in xmlDoc.GetElementsByTagName("fonts")[0].ChildNodes)
            {
                // reject node if it's not an XML element
                if (!(font is XmlElement)) continue;
                var xmlFont = font as XmlElement;
                if (xmlFont.Name != "font") continue;

                // read in the compulsory values (family and size)
                string fontFamily = xmlFont.GetAttribute("family");
                float fontSize = float.Parse(xmlFont.GetAttribute("size"));

                // read in the key
                char fontKey = '\0';    // 0x00 is the default key
                if (xmlFont.HasAttribute("key"))
                    fontKey = xmlFont.GetAttribute("key")[0];

                // font style
                FontStyle fontStyle = FontStyle.Regular;
                if (xmlFont.HasAttribute("style"))
                    Enum.TryParse(xmlFont.GetAttribute("style"), true, out fontStyle);
                // color
                Color color = Defaults.Colors['\0'];
                if (xmlFont.HasAttribute("color"))
                    color = ColorTranslator.FromHtml(xmlFont.GetAttribute("color"));
                Console.WriteLine("Color for {0} is {1}", fontKey < 0x20 ? (int)fontKey : fontKey, color);

                Font gfxFont = new Font(fontFamily, fontSize, fontStyle);
                fonts[fontKey] = gfxFont;
                colors[fontKey] = color;
            }

            // get images
            string titleFilename = "", footerFilename = "";
            {
                var imageSearch = xmlDoc.GetElementsByTagName("images");
                if (imageSearch.Count == 1)
                {
                    XmlElement images = imageSearch[0] as XmlElement;
                    var titleSearch = images.GetElementsByTagName("title");
                    var footerSearch = images.GetElementsByTagName("footer");
                    if (titleSearch.Count == 1)
                        titleFilename = titleSearch[0].Attributes["file"].Value;
                    if (footerSearch.Count == 1)
                        footerFilename = footerSearch[0].Attributes["file"].Value;
                }
            }

            // actually load said images
            Image titleImage = null;
            Image footerImage = null;
            if (File.Exists(titleFilename)) titleImage = Image.FromFile(titleFilename);
            if (File.Exists(footerFilename)) footerImage = Image.FromFile(footerFilename);

            // return config obj
            return new Config()
            {
                TitleImage = titleImage,
                FooterImage = footerImage,

                Fonts = fonts,
                Colors = colors,
                TextRendering = textRendering,

                Width = width,
                EmptyHeight = emptyHeight
            };
        }
    }
}
