using System;
using System.Collections.Generic;
using SkiaSharp;
using System.IO;
using System.Threading.Tasks;
using System.Text;

namespace _3DSTTool
{
    internal class Decode
    {
        public static async Task<int> DecodeImage(string input,
                                                  string output_given,
                                                  short width_given,
                                                  short height_given,
                                                  string format_output,
                                                  bool flip,
                                                  bool use_taskid)
        {
            var input_file = File.OpenRead(input);
            BinaryReader input_read = new BinaryReader(input_file);
            string magic = Encoding.ASCII.GetString(input_read.ReadBytes(7));
            
            // Check if it's a valid 3DST file
            if (magic != "texture")
            {
                Console.WriteLine("Invalid 3DST file!");
                return 1;
            }

            // Skip the null bytes
            input_read.ReadUInt32();
            input_read.ReadUInt32();
            input_read.ReadByte();

            // Read image parameters
            short width = input_read.ReadInt16();
            short height = input_read.ReadInt16();
            byte format = input_read.ReadByte();

            // Read image data
            input_file.Seek(0x80, SeekOrigin.Begin);
            byte[] data = new byte[input_file.Length - 0x80];
            input_file.Read(data, 0, data.Length);
            input_read.Close();

            // Determine which format the image uses, and decode it with the proper tool
            SKBitmap bitmap = new SKBitmap(width, height);
            switch (format)
            {
                case 0:
                    RGBA8.Decode(data, width, height, bitmap);
                    break;
                case 1:
                    RGB8.Decode(data, width, height, bitmap);
                    break;
                case 2:
                    A8.Decode(data, width, height, bitmap);
                    break;
                case 6:
                    RGB565.Decode(data, width, height, bitmap);
                    break;
                case 7:
                    RGBA4.Decode(data, width, height, bitmap);
                    break;
                default:
                    throw new NotImplementedException("3DST file format currently not supported!");
            }

            short new_width = width;
            short new_height = height;
            SKBitmap new_bitmap = bitmap;

            // Check if given width and / or height exists
            if (width_given != 0)
            {
                new_width = width_given;
            }
            if (height_given != 0)
            {
                new_height = height_given;
            }

            // If whether a valid width or height has been given, create a new bitmap using that value(s)
            if (new_width != width)
            {
                if (new_height != height)
                {
                    new_bitmap = new SKBitmap(new_width, new_height);
                    bitmap.ScalePixels(new_bitmap, SKFilterQuality.High);
                }
                else
                {
                    new_bitmap = new SKBitmap(new_width, height);
                    bitmap.ScalePixels(new_bitmap, SKFilterQuality.High);
                }
            }

            if (new_height != height && new_width == width)
            {
                new_bitmap = new SKBitmap(width, new_height);
                bitmap.ScalePixels(new_bitmap, SKFilterQuality.High);
            }

            // Since the Nintendo 3DS flips the images when loading,
            // give the option to do the same when decoding
            if (flip)
            {
                SKCanvas canvas = new SKCanvas(new_bitmap);
                canvas.Scale(1, -1, 0, new_bitmap.Height / 2);
                canvas.DrawBitmap(new_bitmap, new SKPoint());
            }

            string output = output_given;

            // If output isn't specified, get output from input and file extension from format
            output ??= Path.ChangeExtension(input, format_output);

            string output_path = Path.GetDirectoryName(output);
            string output_filename = Path.GetFileNameWithoutExtension(output);
            string output_extension = Path.GetExtension(output);

            // If more than one input was given and output was specified, enumerate the files using Task ID
            if (use_taskid == true)
            {
                output = Path.Combine(output_path, output_filename + Task.CurrentId + output_extension);
            }

            // Save bitmap based on given output format
            SKFileWStream save_file;
            switch (format_output)
            {
                case "astc":
                    save_file = new SKFileWStream(output);
                    new_bitmap.Encode(save_file, SKEncodedImageFormat.Astc, 80);
                    break;
                case "avif":
                    save_file = new SKFileWStream(output);
                    new_bitmap.Encode(save_file, SKEncodedImageFormat.Avif, 80);
                    break;
                case "bmp":
                    save_file = new SKFileWStream(output);
                    new_bitmap.Encode(save_file, SKEncodedImageFormat.Bmp, 80);
                    break;
                case "dng":
                    save_file = new SKFileWStream(output);
                    new_bitmap.Encode(save_file, SKEncodedImageFormat.Dng, 80);
                    break;
                case "gif":
                    save_file = new SKFileWStream(output);
                    new_bitmap.Encode(save_file, SKEncodedImageFormat.Gif, 80);
                    break;
                case "ico":
                case "icon":
                    output = Path.ChangeExtension(output, "ico");
                    save_file = new SKFileWStream(output);
                    new_bitmap.Encode(save_file, SKEncodedImageFormat.Ico, 80);
                    break;
                case "jpg":
                case "jpeg":
                    save_file = new SKFileWStream(output);
                    new_bitmap.Encode(save_file, SKEncodedImageFormat.Jpeg, 80);
                    break;
                case "ktx":
                    save_file = new SKFileWStream(output);
                    new_bitmap.Encode(save_file, SKEncodedImageFormat.Ktx, 80);
                    break;
                case "pkm":
                    save_file = new SKFileWStream(output);
                    new_bitmap.Encode(save_file, SKEncodedImageFormat.Pkm, 80);
                    break;
                case "png":
                    save_file = new SKFileWStream(output);
                    new_bitmap.Encode(save_file, SKEncodedImageFormat.Png, 80);
                    break;
                case "wbmp":
                    save_file = new SKFileWStream(output);
                    new_bitmap.Encode(save_file, SKEncodedImageFormat.Wbmp, 80);
                    break;
                case "webp":
                    save_file = new SKFileWStream(output);
                    new_bitmap.Encode(save_file, SKEncodedImageFormat.Webp, 80);
                    break;
                default:
                    throw new Exception("Unsupported output format!");
            }

            // If file was decoded without errors, inform about that on the command line
            Console.WriteLine("{0} decoded successfully!\n" +
                "Result {1} file was saved into {2}", input, format_output, output);
            return 0;
        }
    }
}
