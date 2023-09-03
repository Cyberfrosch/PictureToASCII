//#define TEST

using CommandLine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace PictureToASCII
{
    public class Options
    {
        private const double WIDTH_OFFSET_DEFAULT = 1.5;
        private const double WIDTH_OFFSET_MIN = 0.1;

        private const int MAX_WIDTH_DEFAULT = 300;
        private const int MAX_WIDTH_MAX = 599;
        private const int MAX_WIDTH_MIN = 1;

        private const ConsoleColor COLOR_DEFAULT = ConsoleColor.Black;
        private const ConsoleColor COLOR_MIN = ConsoleColor.Black; //0
        private const ConsoleColor COLOR_MAX = ConsoleColor.DarkYellow; //6

        [Option('o', "width-offset", Default = 1.5, HelpText = "Set width offset to image")]
        public static double WidthOffset { get; set; } = WIDTH_OFFSET_DEFAULT;

        [Option('w', "max-width", Default = 300, HelpText = "Set max width to image")]
        public static int MaxWidth { get; set; } = MAX_WIDTH_DEFAULT;

        [Option('c', "change", HelpText = "Set the ability to change parameters")]
        public static bool Change { get; set; } = false;

        [Option('f', "file", HelpText = "Save image to file")]
        public static string File { get; set; } = string.Empty;

        [Option("color", HelpText = "Select the console background color")]
        public static ConsoleColor Color { get; set; } = COLOR_DEFAULT;

#if TEST
        public Options(double widthOffset, int maxWidth, bool change, string file, ConsoleColor color)
        {
            WidthOffset = widthOffset;
            MaxWidth = maxWidth;
            Change = true;
            File = file;
            Color = color;
        }
#else
        public Options(double widthOffset, int maxWidth, bool change, string file, ConsoleColor color) 
        {
            WidthOffset = widthOffset;
            MaxWidth = maxWidth;
            Change = change;
            File = file;
            Color = color;
        }
#endif //TEST

        public static void ChangeOptions()
        {
            bool keyPressed = false;

            while (!keyPressed)
            {
                MessageOptions();

                switch (Console.ReadKey(intercept : true).Key)
                {
                    case ConsoleKey.O:
                        WidthOffset = TryChangeValue($"\nSet width offset (current width-offset = {WidthOffset}): ",
                            WIDTH_OFFSET_DEFAULT, WIDTH_OFFSET_MIN);
                        
                        break;
                    case ConsoleKey.W:
                        MaxWidth = TryChangeValue($"\nSet max width (current max-width = {MaxWidth}): ",
                            MAX_WIDTH_DEFAULT, MAX_WIDTH_MIN, MAX_WIDTH_MAX);
                        
                        break;
                    case ConsoleKey.C:
                        MessageOptions(Color);
                        Color = TryChangeValue($"\nSet console background color (current background color = {Color}): ",
                            COLOR_DEFAULT, COLOR_MIN, COLOR_MAX);

                        Console.BackgroundColor = Color;
                        Console.Clear();

                        break;
                    case ConsoleKey.D:
                        Console.WriteLine($"\nDefault values are set:");
                        Console.WriteLine($"width-offset = {WidthOffset = WIDTH_OFFSET_DEFAULT}");
                        Console.WriteLine($"max-width = {MaxWidth = MAX_WIDTH_DEFAULT}");
                        Console.WriteLine($"color = {Color = COLOR_DEFAULT}\n");

                        break;
                    case ConsoleKey.R:
                        Console.Clear();
                        keyPressed = true;

                        break;

                }
            }
        }

        public static void MessageOptions()
        {
            Console.WriteLine("Press any key from list:");
            Console.WriteLine("o    -     change width offset (min = 0.1)");
            Console.WriteLine("w    -     change max width (min = 1; max = 599)");
            Console.WriteLine("c    -     change console backrground color");
            Console.WriteLine("d    -     set default values for everything parameters");
            Console.WriteLine("r    -     return");
        }

        public static void MessageOptions(in ConsoleColor color)
        {
            Console.Clear();

            Console.WriteLine("Write any number from list:");
            Console.WriteLine("0    -     select black color");
            Console.WriteLine("1    -     select dark blue color");
            Console.WriteLine("2    -     select dark green color");
            Console.WriteLine("3    -     select dark cyan color");
            Console.WriteLine("4    -     select dark red color");
            Console.WriteLine("5    -     select dark magenta color");
            Console.WriteLine("6    -     select dark yellow color");
            Console.WriteLine("15    -    select white color (negative)");
        }

        public static T TryChangeValue<T>(string message, T defaultValue, T minValue, T maxValue = default)
        {
            T option;

            //Pressed keys buffer clean
            while (Console.KeyAvailable)
            {
                Console.ReadKey(false);
            }
            Console.Write(message);

            try
            {
                if (typeof(T) == typeof(ConsoleColor))
                {
                    string userInput = Console.ReadLine();
                    if (Enum.TryParse(userInput, true, out ConsoleColor color))
                    {
                        option = (T)(object)color;
                    }
                    else
                    {
                        option = defaultValue;
                    }
                }
                else
                {
                    T userInput = (T)Convert.ChangeType(Console.ReadLine(), typeof(T));
                    option = userInput;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
                Console.WriteLine($"The default value: {defaultValue} was set forcibly!\n");
                return defaultValue;
            }

            Console.WriteLine();

            if (option is ConsoleColor && Comparer.Default.Compare(option, ConsoleColor.White) == 0)
            {
                return option;
            }

            //Check availability maxValue
            if (EqualityComparer<T>.Default.Equals(maxValue, default))
            {
                if (Comparer.Default.Compare(option, minValue) < 0)
                {
                    option = defaultValue;
                    Console.WriteLine($"Value set output range.\nThe default value: {option} was set forcibly!\n");
                }
            }
            else
            {
                if (Comparer.Default.Compare(option, minValue) < 0 || Comparer.Default.Compare(option, maxValue) > 0)
                {
                    option = defaultValue;
                    Console.WriteLine($"Value set output range.\nThe default value: {option} was set forcibly!\n");
                }
            }

            return option;
        }
    }
}
