using System;
using System.Collections.Generic;
using SkiaSharp;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters;
using System.Text;

namespace _3DSTTool
{
    internal class Encode
    {
        public static async Task<int> EncodeImage(string input,
                                      short width_given,
                                      short height_given,
                                      string format_output,
                                      bool flip)
        {
            SKBitmap bitmap = SKBitmap.Decode(input);
            short width = (short)bitmap.Width;
            short height = (short)bitmap.Height;

            short new_width = width;
            short new_height = height;
            double width_power;
            double height_power;

            // Check if given width and / or height exists
            if (width_given != 0)
            {
                // If it's a power of 2, save the value to resize the bitmap
                width_power = Math.Log2(width_given);
                if (width_power % 1 == 0)
                {
                    new_width = width_given;
                }
            }
            if (height_given != 0)
            {
                // If it's a power of 2, save the value to resize the bitmap
                height_power = Math.Log2(height_given);
                if (height_power % 1 == 0)
                {
                    new_height = height_given;
                }
            }

            // In case there are resoulution parameters missing, we'll use the image parameters
            if (new_width == width)
            {
                // Check if original resolution is a power of 2
                width_power = Math.Log2(width);
                if (width_power % 1 != 0)
                {
                    // Round resolution to next power of 2
                    new_width = (short)Math.Pow(2, Math.Ceiling(width_power));
                }
            }
            if (new_height == height)
            {
                // Check if original resolution is a power of 2
                height_power = Math.Log2(height);
                if (height_power % 1 != 0)
                {
                    // Round resolution to next power of 2
                    new_height = (short)Math.Pow(2, Math.Ceiling(height_power));
                }
            }

            // Assign the resolution to the bitmap
            SKBitmap new_bitmap = new SKBitmap(new_width, new_height);
            bitmap.ScalePixels(new_bitmap, SKFilterQuality.High);

            // Since the Nintendo 3DS flips the images when loading,
            // give the option to do the same when encoding
            if (flip)
            {
                SKCanvas canvas = new SKCanvas(new_bitmap);
                canvas.Scale(1, -1, 0, new_bitmap.Height / 2);
                canvas.DrawBitmap(new_bitmap, new SKPoint());
            }

            // Encode the image with the proper format
            byte format;
            byte[] bitmap_raw;
            switch (format_output)
            {
                case "rgba8":
                    format = 0;
                    bitmap_raw = new byte[new_bitmap.Width * new_bitmap.Height * 4];
                    RGBA8.Encode(new_bitmap, bitmap_raw);
                    break;
                case "rgb8":
                    format = 1;
                    bitmap_raw = new byte[new_bitmap.Width * new_bitmap.Height * 3];
                    RGB8.Encode(new_bitmap, bitmap_raw);
                    break;
                case "a8":
                    format = 2;
                    bitmap_raw = new byte[new_bitmap.Width * new_bitmap.Height];
                    A8.Encode(new_bitmap, bitmap_raw);
                    break;
                case "rgba4":
                    format = 7;
                    bitmap_raw = new byte[new_bitmap.Width * new_bitmap.Height * 2];
                    RGBA4.Encode(new_bitmap, bitmap_raw);
                    break;
                default:
                    throw new NotImplementedException("Selected format currently not supported!");
            }

            // Get bytearray for word 'texture' for the header
            byte[] magic = Encoding.ASCII.GetBytes("texture");
            
            // Write the header
            byte[] header = new byte[0x80];
            MemoryStream header_memory = new MemoryStream(header);
            var write_header = new BinaryWriter(header_memory);
            write_header.Write(magic);
            write_header.Seek(0x10, SeekOrigin.Begin);
            write_header.Write(new_width);
            write_header.Write(new_height);
            write_header.Write(format);
            write_header.Close();

            // Get file extension for output
            string output = Path.ChangeExtension(input, "3dst");
            
            // Write the header and the data to given output
            var output_file = File.OpenWrite(output);
            var file_write = new BinaryWriter(output_file);
            file_write.Write(header);
            file_write.Write(bitmap_raw);
            file_write.Close();
            Console.WriteLine("We are here!");
            return 0;
        }
    }
}
