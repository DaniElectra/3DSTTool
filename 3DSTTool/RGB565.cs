using System;
using System.Collections.Generic;
using SkiaSharp;
using System.Text;

namespace _3DSTTool
{
    // The code used in this encoding is based from the Ohana3DS-Rebirth implementation, made by gdkchan.
    // You can look at the original code here: https://github.com/gdkchan/Ohana3DS-Rebirth/blob/master/Ohana3DS%20Rebirth/Ohana/TextureCodec.cs
    internal class RGB565
    {
        /// <summary>
        /// Encode a given SkiaSharp bitmap into RGB565 color
        /// format, and store the result in a bytearray.
        /// </summary>
        /// <param name="bitmap">The given SkiaSharp bitmap.</param>
        /// <param name="output">The bytearray where storing the result.</param>
        public static void Encode(SKBitmap bitmap, byte[] output)
        {
            int pixelCount = 0;

            // Divide the pixels in blocks
            for (int bY = 0; bY < bitmap.Height / 8; bY++)
            {
                for (int bX = 0; bX < bitmap.Width / 8; bX++)
                {
                    for (int tileCount = 0; tileCount < 64; tileCount++)
                    {
                        // Load pixels in specified order by tileOrder
                        int x = Program.TileOrder[tileCount] % 8;
                        int y = (Program.TileOrder[tileCount] - x) / 8;
                        int pixelX = bX * 8 + x;
                        int pixelY = bY * 8 + y;

                        // Get color parameters for specified pixel
                        SKColor pixelColor = bitmap.GetPixel(pixelX, pixelY);
                        byte red = (byte)(pixelColor.Red >> 3); // 5 bits
                        byte green = (byte)(pixelColor.Green >> 2); // 6 bits
                        byte blue = (byte)(pixelColor.Blue >> 3); // 5 bits

                        // Convert bytes into a short with color bytes aligned
                        short color = (short)((red << 11) | (green << 5) | blue);

                        // Move blue and red to left side of byte
                        output[tileCount * 2 + pixelCount] = (byte)color;
                        output[tileCount * 2 + pixelCount + 1] = (byte)(color >> 8);
                    }
                    pixelCount += 128;
                }
            }
        }

        /// <summary>
        /// Decode a given RGB565 image using given resolution as reference,
        /// and store the result in a SkiaSharp bitmap.
        /// </summary>
        /// <param name="input">The given RGB565 image as bytearray.</param>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="bitmap">The SkiaSharp bitmap where storing the result.</param>
        public static void Decode(byte[] input, short width, short height, SKBitmap bitmap)
        {
            int pixelCount = 0;

            // Divide the pixels in blocks
            for (int bY = 0; bY < height / 8; bY++)
            {
                for (int bX = 0; bX < width / 8; bX++)
                {
                    for (int tileCount = 0; tileCount < 64; tileCount++)
                    {
                        // Load pixels in specified order by tileOrder
                        int x = Program.TileOrder[tileCount] % 8;
                        int y = (Program.TileOrder[tileCount] - x) / 8;
                        int pixelX = bX * 8 + x;
                        int pixelY = bY * 8 + y;

                        // Extract the global color from the two bytes for easier extraction of colors
                        short color = (short)(input[tileCount * 2 + pixelCount] | (input[tileCount * 2 + pixelCount + 1] << 8));

                        // Save the color parameters on integers, extracting the colors from short
                        byte red = (byte)(((color >> 11) & 0x1f) << 3);
                        byte green = (byte)(((color >> 5) & 0x3f) << 2);
                        byte blue = (byte)((color & 0x1f) << 3);

                        // Save the pixel into the bitmap
                        SKColor pixelColor = new SKColor(red, green, blue);
                        bitmap.SetPixel(pixelX, pixelY, pixelColor);
                    }
                    pixelCount += 128;
                }
            }
        }
    }
}
