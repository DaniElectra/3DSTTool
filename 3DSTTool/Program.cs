using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using CommandLine;
using System.Threading.Tasks;
using System.Linq;

namespace _3DSTTool
{
    internal class Program
    {
        // Set possible arguments for encoding and decoding
        [Verb("encode", HelpText = "Convert an image file to a 3DST file.")]
        public class EncodeOptions
        {
            [Value(0, Required = true, MetaName = "input", HelpText = "The image file(s) to convert.")]
            public IEnumerable<string> Input { get; set; }

            [Option('o', "output", HelpText = "The 3DST file save location.")]
            public string Output { get; set; }

            [Option('w', "width", HelpText = "Specify the width of the output image(s). Must be a power of 2.")]
            public short Width { get; set; }

            [Option('h', "height", HelpText = "Specify the height of the output image(s). Must be a power of 2.")]
            public short Height { get; set; }

            [Option('f', "format", Default = "rgba8", HelpText = "The format type to use when saving.\n" +
                "Available options: rgba8, rgb8, a8, rgba5551, rgb565, rgba4")]
            public string Format { get; set; }
            
            [Option('y', "flip", HelpText = "Flip the image(s) when saving.")]
            public bool Flip { get; set; }
        }
        
        [Verb("decode", HelpText = "Convert 3DST file(s) to image file(s).")]
        public class DecodeOptions
        {
            [Value(0, Required = true, MetaName = "input", HelpText = "The 3DST file(s) to convert.")]
            public IEnumerable<string> Input { get; set; }

            [Option('o', "output", HelpText = "The image file save location.")]
            public string Output { get; set; }

            [Option('w', "width", HelpText = "Specify the width of the output image(s).")]
            public short Width { get; set; }

            [Option('h', "height", HelpText = "Specify the height of the output image(s).")]
            public short Height { get; set; }

            [Option('f', "format", Default = "png", HelpText = "The image format to use when saving.\n" +
                "Available options: astc, avif, bmp, dng, gif, heif, ico / icon, jpg / jpeg, ktx, pkm, png, wbmp, webp")]
            public string Format { get; set; }

            [Option('y', "flip", HelpText = "Flip the image(s) when saving.")]
            public bool Flip { get; set; }
        }

        static async Task<int> Main(string[] args)
        {
            // Parse the given arguments with the proper parser
            var result = await Parser.Default.ParseArguments<EncodeOptions, DecodeOptions>(args)
                .MapResult(
                    async (EncodeOptions opts) => await EncodeParser(opts),
                    async (DecodeOptions opts) => await DecodeParser(opts),
                    async errors => await Task.FromResult(1)); // In case an error happens when parsing, return 1 to the command line
            return result;
        }

        static async Task<int> EncodeParser(EncodeOptions opts)
        {
            var input = opts.Input;
            var output = opts.Output;
            var width = opts.Width;
            var height = opts.Height;
            var format = opts.Format;
            var flip = opts.Flip;

            // If there's more than one input, tell the encoder to add the Task ID
            // to the output filename when it's given by the user
            bool use_taskid = false;
            if (input.Count() > 1 && output != null)
            {
                use_taskid = true;
            }

            var tasks = new List<Task>();
            foreach (var i in input)
            {
                tasks.Add(Task.Run(async() => await Encode.EncodeImage(i, output, width, height, format, flip, use_taskid)));
            }
            await Task.WhenAll(tasks);
            return 0;
        }

        static async Task<int> DecodeParser(DecodeOptions opts)
        {
            var input = opts.Input;
            var output = opts.Output;
            var width = opts.Width;
            var height = opts.Height;
            var format = opts.Format;
            var flip = opts.Flip;

            // If there's more than one input, tell the decoder to add the Task ID
            // to the output filename when it's given by the user
            bool use_taskid = false;
            if (input.Count() > 1 && output != null)
            {
                use_taskid = true;
            }

            var tasks = new List<Task>();
            foreach (var i in input)
            {
                tasks.Add(Task.Run(async() => await Decode.DecodeImage(i, output, width, height, format, flip, use_taskid)));
            }
            await Task.WhenAll(tasks);
            return 0;
        }
    }
}
