/*using Cosmos.System;
using Cosmos.System.Graphics;
using IL2CPU.API.Attribs;

namespace NovaOS.Apps
{
    public class About
    {
        public static int winX;
        public static int winY;
        public static int prevX;
        public static int prevY;
        public static bool dragging = false;

        [ManifestResourceStream(ResourceName = "NovaOS.Apps.About.About.bmp")] public static byte[] WindowBytes;
        public static Bitmap Window = new(WindowBytes);

        public static void Main(Canvas cv)
        {
            if (MouseManager.MouseState == MouseState.Left && MouseManager.LastMouseState == MouseState.None)
            {
                if (MouseManager.X >= winX && MouseManager.X <= winX + Window.Width - 52)
                {
                    if (MouseManager.Y >= winY && MouseManager.Y <= winY + 28)
                    {
                        prevX = (int)MouseManager.X - winX;
                        prevY = (int)MouseManager.Y - winY;
                        dragging = true;
                    }
                }
            }
            if (MouseManager.MouseState == MouseState.None)
            {
                if (dragging)
                {
                    dragging = false;
                }
            }
            if (dragging)
            {
                winX = (int)MouseManager.X - prevX;
                winY = (int)MouseManager.Y - prevY;
            }
            /*Kernel.DrawRoundedRectangle(winX, winY, 152, 222, 9, Kernel.hexColor("#E1E1E1"));
            Kernel.DrawRoundedRectangle(winX, winY, 152, 25, 9, Kernel.hexColor("#FFFFFF"));
            cv.DrawFilledRectangle(Kernel.hexColor("#E1E1E1"), winX, winY + 15, 152, 15);
            cv.DrawString("Calculator", PCScreenFont.Default, Kernel.hexColor("#404040"), winX + 5, winY + 8);
            cv.DrawImageAlpha(Window, winX, winY);
        }
    }
}*/
