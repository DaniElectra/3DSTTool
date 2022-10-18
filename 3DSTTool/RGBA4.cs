using System;
using System.Collections.Generic;
using SkiaSharp;
using System.Text;

namespace _3DSTTool
{
    // The code used in this encoding is based from the Ohana3DS-Rebirth implementation, made by gdkchan.
    // You can look at the original code here: https://github.com/gdkchan/Ohana3DS-Rebirth/blob/master/Ohana3DS%20Rebirth/Ohana/TextureCodec.cs
    internal class RGBA4
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
                        int alpha = (int)Math.Round((decimal)(pixelColor.Alpha / 16));
                        int blue = (int)Math.Round((decimal)(pixelColor.Blue / 16));
                        int green = (int)Math.Round((decimal)(pixelColor.Green / 16));
                        int red = (int)Math.Round((decimal)(pixelColor.Red / 16));
                        
                        // Move blue and red to left side of byte
                        output[tileCount * 2 + pixelCount] = (byte)(alpha + (blue << 4));
                        output[tileCount * 2 + pixelCount + 1] = (byte)(green + (red << 4));
                    }
                    pixelCount += 128;
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

                        // Save the color parameters on integers, selecting left or right side of byte
                        int blue = input[tileCount * 2 + pixelCount] >> 4;
                        byteCount++;
                        int alpha = input[tileCount * 2 + pixelCount] - (blue << 4);
                        byteCount++;
                        int red = input[tileCount * 2 + pixelCount + 1] >> 4;
                        byteCount++;
                        int green = input[tileCount * 2 + pixelCount + 1] - (red << 4);
                        byteCount++;

                        // Save the pixel into the bitmap
                        SKColor pixelColor = new SKColor((byte)(red * 16), (byte)(green * 16), (byte)(blue * 16), (byte)(alpha * 16));
                        bitmap.SetPixel(pixelX, pixelY, pixelColor);
                    }
                    pixelCount += 128;
                }
            }
        }
    }
}
