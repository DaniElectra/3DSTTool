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
        public static Task<int> DecodeImage(string input,
                                            string outputGiven,
                                            short widthGiven,
                                            short heightGiven,
                                            string formatOutput,
                                            bool flip,
                                            bool useTaskId)
        {
            var inputFile = File.OpenRead(input);
            BinaryReader inputRead = new BinaryReader(inputFile);
            string magic = Encoding.ASCII.GetString(inputRead.ReadBytes(7));
            
            // Check if it's a valid 3DST file
            if (magic != "texture")
            {
                Console.WriteLine("{0}: Invalid 3DST file!", input);
                return Task.FromResult(1);
            }

            // Skip the null bytes
            inputRead.ReadUInt32();
            inputRead.ReadUInt32();
            inputRead.ReadByte();

            // Read image parameters
            short width = inputRead.ReadInt16();
            short height = inputRead.ReadInt16();
            byte format = inputRead.ReadByte();

            // Read image data
            inputFile.Seek(0x80, SeekOrigin.Begin);
            byte[] data = new byte[inputFile.Length - 0x80];
            inputFile.Read(data, 0, data.Length);
            inputRead.Close();

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
                case 4:
                    // ETC1.Decode(data, width, height, bitmap);
                    // break;
                    throw new NotImplementedException("ETC1 format currently not supported!");
                case 5:
                    RGBA5551.Decode(data, width, height, bitmap);
                    break;
                case 6:
                    RGB565.Decode(data, width, height, bitmap);
                    break;
                case 7:
                    RGBA4.Decode(data, width, height, bitmap);
                    break;
                default:
                    throw new InvalidOperationException("3DST file format is invalid!");
            }

            short newWidth = width;
            short newHeight = height;
            SKBitmap newBitmap = bitmap;

            // Check if given width and / or height exists
            if (widthGiven != 0)
            {
                newWidth = widthGiven;
            }
            if (heightGiven != 0)
            {
                newHeight = heightGiven;
            }

            // If whether a valid width or height has been given, create a new bitmap using that value(s)
            if (newWidth != width)
            {
                if (newHeight != height)
                {
                    newBitmap = new SKBitmap(newWidth, newHeight);
                    bitmap.ScalePixels(newBitmap, SKFilterQuality.High);
                }
                else
                {
                    newBitmap = new SKBitmap(newWidth, height);
                    bitmap.ScalePixels(newBitmap, SKFilterQuality.High);
                }
            }

            if (newHeight != height && newWidth == width)
            {
                newBitmap = new SKBitmap(width, newHeight);
                bitmap.ScalePixels(newBitmap, SKFilterQuality.High);
            }

            // Since the Nintendo 3DS flips the images when loading,
            // give the option to do the same when decoding
            if (flip)
            {
                SKCanvas canvas = new SKCanvas(newBitmap);
                canvas.Scale(1, -1, 0, newBitmap.Height / 2);
                canvas.DrawBitmap(newBitmap, new SKPoint());
            }

            string output = outputGiven;

            // If output isn't specified, get output from input and file extension from format
            output ??= Path.ChangeExtension(input, formatOutput);

            string outputPath = Path.GetDirectoryName(output);
            string outputFilename = Path.GetFileNameWithoutExtension(output);
            string outputExtension = Path.GetExtension(output);

            // If more than one input was given and output was specified, enumerate the files using Task ID
            if (useTaskId == true)
            {
                output = Path.Combine(outputPath, outputFilename + Task.CurrentId + outputExtension);
            }

            // Save bitmap based on given output format
            SKFileWStream saveFile;
            switch (formatOutput)
            {
                case "astc":
                    saveFile = new SKFileWStream(output);
                    newBitmap.Encode(saveFile, SKEncodedImageFormat.Astc, 80);
                    break;
                case "avif":
                    saveFile = new SKFileWStream(output);
                    newBitmap.Encode(saveFile, SKEncodedImageFormat.Avif, 80);
                    break;
                case "bmp":
                    saveFile = new SKFileWStream(output);
                    newBitmap.Encode(saveFile, SKEncodedImageFormat.Bmp, 80);
                    break;
                case "dng":
                    saveFile = new SKFileWStream(output);
                    newBitmap.Encode(saveFile, SKEncodedImageFormat.Dng, 80);
                    break;
                case "gif":
                    saveFile = new SKFileWStream(output);
                    newBitmap.Encode(saveFile, SKEncodedImageFormat.Gif, 80);
                    break;
                case "ico":
                case "icon":
                    output = Path.ChangeExtension(output, "ico");
                    saveFile = new SKFileWStream(output);
                    newBitmap.Encode(saveFile, SKEncodedImageFormat.Ico, 80);
                    break;
                case "jpg":
                case "jpeg":
                    saveFile = new SKFileWStream(output);
                    newBitmap.Encode(saveFile, SKEncodedImageFormat.Jpeg, 80);
                    break;
                case "ktx":
                    saveFile = new SKFileWStream(output);
                    newBitmap.Encode(saveFile, SKEncodedImageFormat.Ktx, 80);
                    break;
                case "pkm":
                    saveFile = new SKFileWStream(output);
                    newBitmap.Encode(saveFile, SKEncodedImageFormat.Pkm, 80);
                    break;
                case "png":
                    saveFile = new SKFileWStream(output);
                    newBitmap.Encode(saveFile, SKEncodedImageFormat.Png, 80);
                    break;
                case "wbmp":
                    saveFile = new SKFileWStream(output);
                    newBitmap.Encode(saveFile, SKEncodedImageFormat.Wbmp, 80);
                    break;
                case "webp":
                    saveFile = new SKFileWStream(output);
                    newBitmap.Encode(saveFile, SKEncodedImageFormat.Webp, 80);
                    break;
                default:
                    throw new Exception("Unsupported output format!");
            }

            // If file was decoded without errors, inform about that on the command line
            Console.WriteLine("{0} decoded successfully!\n" +
                "Result {1} file was saved into {2}", input, formatOutput, output);
            return Task.FromResult(0);
        }
    }
}
