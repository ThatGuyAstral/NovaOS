using System;
using System.Drawing;
using System.Threading;
using Cosmos.Core.Memory;
using Cosmos.System.Graphics;
using CosmosTTF;
using NovaOS.NovaShell;
using NovaOS.Apps;
using IL2CPU.API.Attribs;
using Sys = Cosmos.System;

namespace NovaOS
{
    public class Kernel : Sys.Kernel
    {
        Timer T = new((_) => { fps = frames; frames = 0; }, null, 1000, 0);
        
        public static int fps = 0;
        public static int frames;

        [ManifestResourceStream(ResourceName = "NovaOS.Images.Cursor.bmp")] public static byte[] cursorBytes;
        public static Bitmap Cursor = new(cursorBytes);

        [ManifestResourceStream(ResourceName = "NovaOS.Images.Wallpapers.Wallpaper.bmp")] public static byte[] wallpaperBytes;
        public static Bitmap Wallpaper = new(wallpaperBytes);

        [ManifestResourceStream(ResourceName = "NovaOS.Fonts.Inter.ttf")] public static byte[] interBytes;

        [ManifestResourceStream(ResourceName = "NovaOS.Fonts.Ubuntu.ttf")] public static byte[] ubuntuBytes;

        public static Canvas cv;

        protected override void BeforeRun()
        {
            Shell.Main();
        }

        protected override void Run()
        {
            try
            {
                cv.DrawImage(Wallpaper, 0, 0);
                
                cv.DrawFilledRectangle(Color.FromArgb(unchecked((int)0xFF0E0E0E)), 0, 0, 1920, 30);

                cv.DrawString(DateTime.Now.ToString("ddd dd  hh:mm tt") + $"  {fps} FPS", Shell.shellFont1, Color.White, 920, 8);

                DrawRoundedRectangle(4, 2, 70, 25, 12, Color.FromArgb(unchecked((int)0xFF040404)));
                
                cv.DrawString("Start", Shell.shellFont1, Color.White, 25, 8);

                AppManager.Manage(cv);

                cv.DrawImageAlpha(Cursor, (int)Sys.MouseManager.X, (int)Sys.MouseManager.Y);

                Heap.Collect();
                
                cv.Display();
                
                frames++;
            }
            catch (Exception e)
            {
                Crash(e);
            }
        }

        public static void DrawRoundedRectangle(int x, int y, int width, int height, int radius, Color col)
        {
            cv.DrawFilledRectangle(col, x + radius, y, width - 2 * radius, height);
            cv.DrawFilledRectangle(col, x, y + radius, radius, height - 2 * radius);
            cv.DrawFilledRectangle(col, x + width - radius, y + radius, radius, height - 2 * radius);
            cv.DrawFilledCircle(col, x + radius, y + radius, radius);
            cv.DrawFilledCircle(col, x + width - radius - 1, y + radius, radius);
            cv.DrawFilledCircle(col, x + radius, y + height - radius - 1, radius);
            cv.DrawFilledCircle(col, x + width - radius - 1, y + height - radius - 1, radius);
        }

        /// <summary>
        /// Crashes the system and displays the error / exception.
        /// </summary>
        /// <param name="e">The exception to be displayed</param>
        public static void Crash(Exception e)
        {
            cv.Disable();
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.SetWindowSize(90, 30);
            Console.Clear();
            Console.WriteLine($"A problem has been detected and nova has been shut down to prevent damage to your PC.\n\n{e.GetType()}: {e.Message}\n\nIf this is the first time you've seen this error screen, restart your computer.\nIf this screen appears again, follow these steps:\n\nCheck to make sure any new hardware had been properly installed.\n\nIf this appears on your first time on NovaOS, please report this as a bug on the GitHub.\n(https://github.com/ThatGuyAstral/NovaOS)");
            while (true) { Thread.Sleep(1); }
        }
    }
}
