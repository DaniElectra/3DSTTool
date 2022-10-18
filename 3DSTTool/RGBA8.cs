using System;
using System.Collections.Generic;
using SkiaSharp;
using System.Text;

namespace _3DSTTool
{
    // The code used in this encoding is based from the Ohana3DS-Rebirth implementation, made by gdkchan.
    // You can look at the original code here: https://github.com/gdkchan/Ohana3DS-Rebirth/blob/master/Ohana3DS%20Rebirth/Ohana/TextureCodec.cs
    internal class RGBA8
    {
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
                        output[tileCount * 4 + pixelCount] = pixelColor.Alpha;
                        output[tileCount * 4 + pixelCount + 1] = pixelColor.Blue;
                        output[tileCount * 4 + pixelCount + 2] = pixelColor.Green;
                        output[tileCount * 4 + pixelCount + 3] = pixelColor.Red;
                    }
                    pixelCount += 256;
                }
            }
        }

        public static void Decode(byte[] input, short width, short height, SKBitmap bitmap)
        {
            int byteCount = 0;
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

                        // Save the color parameters on integers
                        byte alpha = input[tileCount * 4 + pixelCount];
                        byteCount++;
                        byte blue = input[tileCount * 4 + pixelCount + 1];
                        byteCount++;
                        byte green = input[tileCount * 4 + pixelCount + 2];
                        byteCount++;
                        byte red = input[tileCount * 4 + pixelCount + 3];
                        byteCount++;

                        // Save the pixel into the bitmap
                        SKColor pixelColor = new SKColor(red, green, blue, alpha);
                        bitmap.SetPixel(pixelX, pixelY, pixelColor);
                    }
                    pixelCount += 256;
                }
            }
        }
    }
}
