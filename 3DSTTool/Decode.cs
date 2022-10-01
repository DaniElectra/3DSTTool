using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace _3DSTTool
{
    internal class Decode
    {
        public static int DecodeImage(string input, string output, string format_output, bool flip)
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
            Bitmap bitmap = new Bitmap(width, height);
            switch (format)
            {
                case 0:
                    RGBA8.Decode(data, width, height, bitmap);
                    break;
                default:
                    throw new NotImplementedException("3DST file format currently not supported!");
            }

            // Since the Nintendo 3DS flips the images when loading,
            // give the option to do the same when decoding
            if (flip)
            {
                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            }

            // Save bitmap based on given output format
            switch (format_output)
            {
                case "bmp":
                    bitmap.Save(output, ImageFormat.Bmp);
                    break;
                case "emf":
                    bitmap.Save(output, ImageFormat.Emf);
                    break;
                case "exif":
                    bitmap.Save(output, ImageFormat.Exif);
                    break;
                case "gif":
                    bitmap.Save(output, ImageFormat.Gif);
                    break;
                case "ico":
                case "icon":
                    bitmap.Save(output, ImageFormat.Icon);
                    break;
                case "png":
                    bitmap.Save(output, ImageFormat.Png);
                    break;
                case "jpg":
                case "jpeg":
                    bitmap.Save(output, ImageFormat.Jpeg);
                    break;
                case "tiff":
                    bitmap.Save(output, ImageFormat.Tiff);
                    break;
                case "wmf":
                    bitmap.Save(output, ImageFormat.Wmf);
                    break;
                default:
                    throw new Exception("Unsupported output format!");
            }
            return 0;
        }
    }
}
