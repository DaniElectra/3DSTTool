using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Text;

namespace _3DSTTool
{
    internal class Encode
    {
        public static int EncodeImage(string input, string output, string format_output, bool flip)
        {
            Bitmap bitmap = new Bitmap(input);
            short width = (short)bitmap.Width;
            short height = (short)bitmap.Height;
            
            // Check if resolution is a power of 2
            double width_power = Math.Log2(width);
            double height_power = Math.Log2(height);
            if (width_power % 1 != 0 || height_power % 1 != 0)
            {
                // Round resolution to next power of 2
                width = (short)Math.Pow(2, Math.Ceiling(width_power));
                height = (short)Math.Pow(2, Math.Ceiling(height_power));
                bitmap = new Bitmap(bitmap, width, height);
            }

            // Since the Nintendo 3DS flips the images when loading,
            // give the option to do the same when encoding
            if (flip)
            {
                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            }

            // Encode the image with the proper format
            byte format;
            byte[] bitmap_raw;
            switch (format_output)
            {
                case "rgba8":
                    format = 0;
                    bitmap_raw = new byte[bitmap.Width * bitmap.Height * 4];
                    RGBA8.Encode(bitmap, bitmap_raw);
                    break;
                case "rgb8":
                    format = 1;
                    bitmap_raw = new byte[bitmap.Width * bitmap.Height * 3];
                    RGB8.Encode(bitmap, bitmap_raw);
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
            write_header.Write(width);
            write_header.Write(height);
            write_header.Write(format);
            write_header.Close();

            // Write the header and the data to given output
            var output_file = File.OpenWrite(output);
            var file_write = new BinaryWriter(output_file);
            file_write.Write(header);
            file_write.Write(bitmap_raw);
            file_write.Close();
            return 0;
        }
    }
}
