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
        [STAThread]

        static void Main(string[] args)
        {
            var parserResult = Parser.Default.ParseArguments<Options>(args);

            parserResult.WithParsed(options =>
            {
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog()
                    {
                        Filter = "Images | *.bmp; *.png; *.jpg; *.jpeg"
                    };

                    while (true)
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

                        Console.ReadLine();
                    }
                }
            });
            parserResult.WithNotParsed(errors =>
            {
                if (errors.Any(e => e.Tag != ErrorType.HelpRequestedError) && errors.Any(e => e.Tag != ErrorType.VersionRequestedError))
                {
                    // Обработать ошибки при разборе аргументов командной строки
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Error: {error.Tag}");
                    }
                    Console.WriteLine("Failed to parse command line arguments!");
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
