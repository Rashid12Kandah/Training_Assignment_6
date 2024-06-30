using System;
using System.Drawing;
using System.Drawing.Imaging;

public class Converter{
    public static Bitmap bit_2_eight(string inputPath, string outputPath){
        Bitmap bitmap1bit = new Bitmap(inputPath);
        Bitmap bitmap8bit = new Bitmap(bitmap1bit.Width, bitmap1bit.Height, PixelFormat.Format8bppIndexed);

        ColorPalette palette = bitmap8bit.Palette;

        for(int i = 0; i<255; i++){
            palette.Entries[i] = Color.FromArgb(i,i,i);
        }

        bitmap8bit.Palette = palette;

        BitmapData bitmapData1bit = bitmap1bit.LockBits(new Rectangle(0,0,bitmap1bit.Width, bitmap1bit.Height), ImageLockMode.ReadOnly, PixelFormat.Format1bppIndexed);
        BitmapData bitmapData8bit = bitmap8bit.LockBits(new Rectangle(0,0,bitmap8bit.Width, bitmap8bit.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

        int height = bitmap1bit.Height;
        int width = bitmap1bit.Width;

        unsafe{
            byte* ptr1bit = (byte*)(void*)bitmapData1bit.Scan0;
            byte* ptr8bit = (byte*)(void*)bitmapData8bit.Scan0;

            for(int y=0; y<height; y++){
                for(int x=0; x<width; x++){
                    byte bit = (byte)(ptr1bit[y * bitmapData1bit.Stride + x/8] & (0x80 >> (x%8)));
                    ptr8bit[y * bitmapData8bit.Stride + x] = (byte)(bit > 0 ? 255:0);
                }
            }
        }

        bitmap1bit.UnlockBits(bitmapData1bit);
        bitmap8bit.UnlockBits(bitmapData8bit);

        bitmap8bit.Save(outputPath, ImageFormat.Jpeg);

        return bitmap8bit;
    }

    public static void Verify(Bitmap bitmap){
        
            Console.WriteLine($"Image Width: {bitmap.Width}");
            Console.WriteLine($"Image Height: {bitmap.Height}");
            Console.WriteLine($"Pixel Format: {bitmap.PixelFormat}");

            bool is8Bit = bitmap.PixelFormat == PixelFormat.Format8bppIndexed;
            if (is8Bit)
            {
                Console.WriteLine("The image is 8-bit.");
            }
            else
            {
                Console.WriteLine("The image is not 8-bit.");
            }
        
    }
    static void Main(string[] args){

        if(args.Length < 1){
            Console.WriteLine("Usage: mono Trying.exe <path/to/1-bit/img>");
            return;
        }

        string inputPath = args[0];
        string uuid = Guid.NewGuid().ToString();
        string outputPath = $"8_bit_{uuid}.jpeg";

        Bitmap result = bit_2_eight(inputPath, outputPath);
        Console.WriteLine("Verifying Correct Conversion...");
        Verify(new Bitmap(inputPath));
        Console.WriteLine("-----------------------------------------------------------");
        Verify(result);
    }
}