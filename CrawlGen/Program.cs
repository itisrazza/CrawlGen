using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace CrawlGen
{
    /**
     * This is not meant to be a particularily good program
     * with a good user interface.
     * 
     * If you want me to do that. Leave an issue on
     * https://github.com/thegreatrazz/CrawlGen/
     * and I'll maybe consider it.
     */

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                var errOut = Console.Error;
                errOut.WriteLine("Usage: CrawlGen [credits file]\n");
                errOut.WriteLine("A credits file (*.txt) must be accompanied by a config file (*.xml).");
                errOut.WriteLine("Example: CrawlGen credits.txt (will use credits.xml as config)");

                Environment.ExitCode = 1;
                return;
            }

            // load config
            var config = Config.LoadXml(Path.Combine(
                Path.GetDirectoryName(args[0]),
                Path.GetFileNameWithoutExtension(args[0]) + ".xml")
            );

            // load credits
            var rawCreditLines = File.ReadAllLines(args[0], Encoding.UTF8);
            var creditLines = new CreditLine[rawCreditLines.Length];
            for (int i = 0; i < rawCreditLines.Length; i++)
                creditLines[i] = CreditLine.Parse(rawCreditLines[i], config);

            // get the entire height of the bitmap
            var height = 0;
            foreach (var line in creditLines) height += line.GetHeight();
            if (config.TitleImage != null) height += config.TitleImage.Height;
            if (config.FooterImage != null) height += config.FooterImage.Height;

            // make it
            Bitmap bmp = new Bitmap(config.Width, height);
            Graphics gfx = Graphics.FromImage(bmp);
            gfx.TextRenderingHint = config.TextRendering;
            //gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            int offsetY = 0;

            // draw the header
            if (config.TitleImage != null)
            {
                gfx.DrawImage(config.TitleImage, config.Width / 2 - config.TitleImage.Width / 2, offsetY);
                offsetY += config.TitleImage.Height;
            }

            foreach (var line in creditLines)
            {
                Console.WriteLine(line.Text);
                Console.WriteLine("> Font: {0}, Color: {1}", line.Font, line.Color);

                var measurements = gfx.MeasureString(line.Text, line.Font);
                gfx.DrawString(line.Text, line.Font, new SolidBrush(line.Color), new PointF(config.Width / 2 - measurements.Width / 2, offsetY));
                offsetY += line.GetHeight();
            }

            // draw the footer
            if (config.FooterImage != null)
            {
                gfx.DrawImage(config.FooterImage, config.Width / 2 - config.FooterImage.Width / 2, offsetY);
            }

            // save it
            bmp.Save(Path.Combine(
                Path.GetDirectoryName(args[0]),
                Path.GetFileNameWithoutExtension(args[0]) + ".png")
            );
        }
    }
}
