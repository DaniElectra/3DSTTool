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
        public static Task<int> EncodeImage(string input,
                                            string outputGiven,
                                            short widthGiven,
                                            short heightGiven,
                                            string formatOutput,
                                            bool flip,
                                            bool useTaskId)
        {
            SKBitmap bitmap = SKBitmap.Decode(input);
            short width = (short)bitmap.Width;
            short height = (short)bitmap.Height;

            short newWidth = width;
            short newHeight = height;
            double widthPower;
            double heightPower;

            // Check if given width and / or height exists
            if (widthGiven != 0)
            {
                // If it's a power of 2, save the value to resize the bitmap
                widthPower = Math.Log2(widthGiven);
                if (widthPower % 1 == 0)
                {
                    newWidth = widthGiven;
                }
            }
            if (heightGiven != 0)
            {
                // If it's a power of 2, save the value to resize the bitmap
                heightPower = Math.Log2(heightGiven);
                if (heightPower % 1 == 0)
                {
                    newHeight = heightGiven;
                }
            }

            // In case there are resoulution parameters missing, we'll use the image parameters
            if (newWidth == width)
            {
                // Check if original resolution is a power of 2
                widthPower = Math.Log2(width);
                if (widthPower % 1 != 0)
                {
                    // Round resolution to next power of 2
                    newWidth = (short)Math.Pow(2, Math.Ceiling(widthPower));
                }
            }
            if (newHeight == height)
            {
                // Check if original resolution is a power of 2
                heightPower = Math.Log2(height);
                if (heightPower % 1 != 0)
                {
                    // Round resolution to next power of 2
                    newHeight = (short)Math.Pow(2, Math.Ceiling(heightPower));
                }
            }

            // Assign the resolution to the bitmap
            SKBitmap newBitmap = new SKBitmap(newWidth, newHeight);
            bitmap.ScalePixels(newBitmap, SKFilterQuality.High);

            // Since the Nintendo 3DS flips the images when loading,
            // give the option to do the same when encoding
            if (flip)
            {
                SKCanvas canvas = new SKCanvas(newBitmap);
                canvas.Scale(1, -1, 0, newHeight / 2);
                canvas.DrawBitmap(newBitmap, new SKPoint());
            }

            // Encode the image with the proper format
            byte format;
            byte[] bitmapRaw;
            switch (formatOutput)
            {
                case "rgba8":
                    format = 0;
                    bitmapRaw = new byte[newWidth * newHeight * 4];
                    RGBA8.Encode(newBitmap, bitmapRaw);
                    break;
                case "rgb8":
                    format = 1;
                    bitmapRaw = new byte[newWidth * newHeight * 3];
                    RGB8.Encode(newBitmap, bitmapRaw);
                    break;
                case "a8":
                    format = 2;
                    bitmapRaw = new byte[newWidth * newHeight];
                    A8.Encode(newBitmap, bitmapRaw);
                    break;
                case "etc1":
                    // format = 4;
                    // bitmapRaw = new byte[newWidth * newHeight / 2];
                    // ETC1.Encode(newBitmap, bitmapRaw);
                    // break;
                    throw new NotImplementedException("ETC1 format currently not supported!");
                case "rgba5551":
                    format = 5;
                    bitmapRaw = new byte[newWidth * newHeight * 2];
                    RGBA5551.Encode(newBitmap, bitmapRaw);
                    break;
                case "rgb565":
                    format = 6;
                    bitmapRaw = new byte[newWidth * newHeight * 2];
                    RGB565.Encode(newBitmap, bitmapRaw);
                    break;
                case "rgba4":
                    format = 7;
                    bitmapRaw = new byte[newWidth * newHeight * 2];
                    RGBA4.Encode(newBitmap, bitmapRaw);
                    break;
                default:
                    throw new InvalidOperationException("Invalid pixel format!");
            }

            // Get bytearray for word 'texture' for the header
            byte[] magic = Encoding.ASCII.GetBytes("texture");
            
            // Write the header
            byte[] header = new byte[0x80];
            MemoryStream headerMemory = new MemoryStream(header);
            var writeHeader = new BinaryWriter(headerMemory);
            writeHeader.Write(magic);
            writeHeader.Seek(0x10, SeekOrigin.Begin);
            writeHeader.Write(newWidth);
            writeHeader.Write(newHeight);
            writeHeader.Write(format);
            writeHeader.Close();

            string output = outputGiven;
            
            // If output isn't specified, get file extension for output from input
            output ??= Path.ChangeExtension(input, "3dst");

            string outputPath = Path.GetDirectoryName(output);
            string outputFilename = Path.GetFileNameWithoutExtension(output);
            string outputExtension = Path.GetExtension(output);

            // If more than one input was given and output was specified, enumerate the files using Task ID
            if (useTaskId == true)
            {
                output = Path.Combine(outputPath, outputFilename + Task.CurrentId + outputExtension);
            }
            
            // Write the header and the data to given output
            var outputFile = File.OpenWrite(output);
            var fileWrite = new BinaryWriter(outputFile);
            fileWrite.Write(header);
            fileWrite.Write(bitmapRaw);
            fileWrite.Close();

            // If file was encoded without errors, inform about that on the command line
            Console.WriteLine("{0} encoded successfully!\n" +
                "Result {1} file was saved into {2}", input, formatOutput, output);
            return Task.FromResult(0);
        }
    }
}
