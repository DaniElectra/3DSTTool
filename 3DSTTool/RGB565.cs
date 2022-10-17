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
        // The order that pixels have per block in this format
        private static int[] tile_order = { 0, 1, 8, 9, 2, 3, 10, 11, 16, 17, 24, 25, 18, 19, 26, 27, 4, 5, 12, 13, 6, 7, 14, 15, 20, 21, 28, 29, 22, 23, 30, 31, 32, 33, 40, 41, 34, 35, 42, 43, 48, 49, 56, 57, 50, 51, 58, 59, 36, 37, 44, 45, 38, 39, 46, 47, 52, 53, 60, 61, 54, 55, 62, 63 };

        public static void Encode(SKBitmap bitmap, byte[] output)
        {
            int pixel_count = 0;

            // Divide the pixels in blocks
            for (int By = 0; By < bitmap.Height / 8; By++)
            {
                for (int Bx = 0; Bx < bitmap.Width / 8; Bx++)
                {
                    for (int tile_count = 0; tile_count < 64; tile_count++)
                    {
                        // Load pixels in specified order by tileOrder
                        int x = tile_order[tile_count] % 8;
                        int y = (tile_order[tile_count] - x) / 8;
                        int pixel_x = Bx * 8 + x;
                        int pixel_y = By * 8 + y;

                        // Get color parameters for specified pixel
                        SKColor pixel_color = bitmap.GetPixel(pixel_x, pixel_y);
                        byte red = (byte)(pixel_color.Red >> 3); // 5 bits
                        byte green = (byte)(pixel_color.Green >> 2); // 6 bits
                        byte blue = (byte)(pixel_color.Blue >> 3); // 5 bits

                        // Convert bytes into a short with color bytes aligned
                        short color = (short)((red << 11) | (green << 5) | blue);

                        // Move blue and red to left side of byte
                        output[tile_count * 2 + pixel_count] = (byte)color;
                        output[tile_count * 2 + pixel_count + 1] = (byte)(color >> 8);
                    }
                    pixel_count += 128;
                }
            }
        }

        public static void Decode(byte[] input, short width, short height, SKBitmap bitmap)
        {
            int byte_count = 0;
            int pixel_count = 0;

            // Divide the pixels in blocks
            for (int By = 0; By < height / 8; By++)
            {
                for (int Bx = 0; Bx < width / 8; Bx++)
                {
                    for (int tile_count = 0; tile_count < 64; tile_count++)
                    {
                        // Load pixels in specified order by tileOrder
                        int x = tile_order[tile_count] % 8;
                        int y = (tile_order[tile_count] - x) / 8;
                        int pixel_x = Bx * 8 + x;
                        int pixel_y = By * 8 + y;

                        // Extract the global color from the two bytes for easier extraction of colors
                        short color = (short)(input[tile_count * 2 + pixel_count] | (input[tile_count * 2 + pixel_count + 1] << 8));

                        // Save the color parameters on integers, extracting the colors from short
                        byte red = (byte)(((color >> 11) & 0x1f) << 3);
                        byte_count++;
                        byte green = (byte)(((color >> 5) & 0x3f) << 2);
                        byte_count++;
                        byte blue = (byte)((color & 0x1f) << 3);
                        byte_count++;

                        // Save the pixel into the bitmap
                        SKColor pixel_color = new SKColor(red, green, blue);
                        bitmap.SetPixel(pixel_x, pixel_y, pixel_color);
                    }
                    pixel_count += 128;
                }
            }
        }
    }
}
