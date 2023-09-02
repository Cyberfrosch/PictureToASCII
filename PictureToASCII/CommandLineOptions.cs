using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureToASCII
{
    public class Options
    {
        [Option('o', "width-offset", Default = 1.5, HelpText = "Set width offset to image")]
        public static double WidthOffset { get; set; }

        [Option('w', "max-width", Default = 300, HelpText = "Set max width to image")]
        public static int MaxWidth { get; set; }

        [Value(0, MetaName = "input", HelpText = "Input image file")]
        public string InputFile { get; set; }
    }

}
