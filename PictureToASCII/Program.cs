using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using CommandLine;
using CommandLine.Text;

namespace PictureToASCII
{
    class Program
    {
        private const System.ConsoleKey KEY_TERMINATED = ConsoleKey.Escape;

        [STAThread]

        static void Main(string[] args)
        {
            var parserResult = Parser.Default.ParseArguments<Options>(args);

            parserResult.WithParsed(options =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    Filter = "Images | *.bmp; *.png; *.jpg; *.jpeg"
                };

                Console.WriteLine($"Press any key to select an image OR press <{KEY_TERMINATED}> to shut down the program...");

                while (Console.ReadKey(true).Key != KEY_TERMINATED)
                {
                    Console.Clear();

                    if (openFileDialog.ShowDialog() != DialogResult.OK)
                    {
                        continue;
                    }

                    Bitmap bitmap = new Bitmap(openFileDialog.FileName);
                    bitmap = ResizeBitmap(bitmap);
                    bitmap.ToGrayscale();

                    var converter = new BitmapToASCIIConverter(bitmap);
                    var rows = converter.Convert();
                    foreach (var row in rows)
                    {
                        Console.WriteLine(row);
                    }

                    if (Options.File != null)
                    {
                        var rowsReverse = converter.ConvertReverse();
                        File.WriteAllLines(Options.File, rowsReverse.Select(r => new string(r)));
                        Console.WriteLine($"ASCII art has been saved.\n");

                        break;
                    }

                    if (Options.Change)
                    {
                        Options.ChangeOptions();
                    }

                    Console.WriteLine($"Press any key to select an image OR press <{KEY_TERMINATED}> to shut down the program...");
                }

                if (Options.File == null)
                {
                    Console.WriteLine($"<{KEY_TERMINATED}> is pressed! The program has been terminated.\n");
                }
            });
            parserResult.WithNotParsed(errors =>
            {
                if (errors.Any(e => e.Tag != ErrorType.HelpRequestedError) && errors.Any(e => e.Tag != ErrorType.VersionRequestedError))
                {
                    Console.WriteLine("Failed to parse command line arguments!\n");
                }
            });
        }

        private static Bitmap ResizeBitmap(Bitmap bitmap)
        {
            double newHeight = bitmap.Height / Options.WidthOffset * Options.MaxWidth / bitmap.Width;

            if (bitmap.Width > Options.MaxWidth || bitmap.Height > newHeight) 
            {
                bitmap = new Bitmap(bitmap, new Size(Options.MaxWidth, (int)newHeight));
            }
            return bitmap;
        }
    }
}
