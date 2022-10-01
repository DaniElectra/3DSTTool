using System;
using System.IO;
using System.Collections.Generic;
using CommandLine;

namespace _3DSTTool
{
    internal class Program
    {
        // Set possible arguments for encoding and decoding
        [Verb("encode", HelpText = "Convert an image file to a 3DST file.")]
        public class EncodeOptions
        {
            [Value(0, Required = true, MetaName = "input", HelpText = "The image file to convert.")]
            public string Input { get; set; }

            [Value(1, Required = true, MetaName = "output", HelpText = "The 3DST file save location.")]
            public string Output { get; set; }

            [Option('w', "width", HelpText = "Specify the width of the output image. Must be a power of 2.")]
            public short Width { get; set; }

            [Option('h', "height", HelpText = "Specify the height of the output image. Must be a power of 2.")]
            public short Height { get; set; }

            [Option('f', "format", Default = "rgba8", HelpText = "The format type to use when saving.")]
            public string Format { get; set; }
            
            [Option('y', "flip", HelpText = "Flip the image when saving.")]
            public bool Flip { get; set; }
        }
        
        [Verb("decode", HelpText = "Convert 3DST file to an image file.")]
        public class DecodeOptions
        {
            [Value(0, Required = true, MetaName = "input", HelpText = "The 3DST file to convert.")]
            public string Input { get; set; }

            [Value(1, Required = true, MetaName = "output", HelpText = "The image file save location.")]
            public string Output { get; set; }

            [Option('w', "width", HelpText = "Specify the width of the output image.")]
            public short Width { get; set; }

            [Option('h', "height", HelpText = "Specify the height of the output image.")]
            public short Height { get; set; }

            [Option('f', "format", Default = "png", HelpText = "The image format to use when saving.")]
            public string Format { get; set; }

            [Option('y', "flip", HelpText = "Flip the image when saving.")]
            public bool Flip { get; set; }
        }

        static int Main(string[] args)
        {
            // Parse the given arguments with the proper parser
            var result = Parser.Default.ParseArguments<EncodeOptions, DecodeOptions>(args)
                .MapResult(
                    (EncodeOptions opts) => EncodeParser(opts),
                    (DecodeOptions opts) => DecodeParser(opts),
                    errors => 1); // In case an error happens when parsing, return 1 to the command line
            return result;
        }

        static int EncodeParser(EncodeOptions opts)
        {
            var input = opts.Input;
            var output = opts.Output;
            var width = opts.Width;
            var height = opts.Height;
            var format = opts.Format;
            var flip = opts.Flip;
            var result = Encode.EncodeImage(input, output, width, height, format, flip);
            // If file was encoded without errors, inform about that on the command line
            if (result == 0)
            {
                Console.WriteLine("{0} encoded successfully!", input);
                Console.WriteLine("Result {0} file was saved into {1}", format, output);
            }
            return result;
        }

        static int DecodeParser(DecodeOptions opts)
        {
            var input = opts.Input;
            var output = opts.Output;
            var width = opts.Width;
            var height = opts.Height;
            var format = opts.Format;
            var flip = opts.Flip;
            var result = Decode.DecodeImage(input, output, width, height, format, flip);
            // If file was encoded without errors, inform about that on the command line
            if (result == 0)
            {
                Console.WriteLine("{0} decoded successfully!", input);
                Console.WriteLine("Result {0} file was saved into {1}", format, output);
            }
            return result;
        }
    }
}
