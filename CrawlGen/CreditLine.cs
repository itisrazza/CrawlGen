using System.Drawing;

namespace CrawlGen
{
    class CreditLine
    {
        /// <summary>Line font</summary>
        public Font Font { get; private set; }

        public Color Color { get; private set; }

        /// <summary>Text to be drawn</summary>
        public string Text { get; private set; }

        private int emptyHeight = 0;

        public static CreditLine Parse(string line, Config config)
        {
            // select the right font
            Font font = config.Fonts['\0'];
            Color color = config.Colors['\0'];
            if (line.Length > 0)
            {
                // potential key
                char potKey = line[0];
                bool keyFlag = false;   // flag to trim key if found

                // Take the first char and look it up in the font and color thing
                if (config.Fonts.ContainsKey(potKey))
                {
                    font = config.Fonts[potKey];
                    keyFlag = true;
                }
                if (config.Colors.ContainsKey(potKey))
                {
                    color = config.Colors[potKey];
                    keyFlag = true;
                }

                if (keyFlag) line = line.TrimStart(potKey);
                line = line.Trim();
            }

            // create creditLine
            return new CreditLine()
            {
                Font = font,
                Text = line,
                Color = color,
                emptyHeight = config.EmptyHeight
            };
        }

        public int GetHeight()
        {
            return Text.Length > 0 ? Font.Height : emptyHeight;
        }
    }
}
