using Cosmos.System;
using Cosmos.System.Graphics;
using CosmosTTF;
using NovaOS.Apps;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace NovaOS.Core
{
    public class Window
    {
        public static int winX;
        public static int winY;
        private static int prevX;
        private static int prevY;
        public static List<Tuple<int, int, int, int, string>> Buttons = new();
        public static bool dragging = false;

        public static void Draw(string title, string content, int width, int height, Canvas cv)
        {
            if (MouseManager.MouseState == MouseState.Left && MouseManager.LastMouseState == MouseState.None)
            {
                if (MouseManager.X >= winX && MouseManager.X <= winX + width - 26)
                {
                    if (MouseManager.Y >= winY && MouseManager.Y <= winY + 29)
                    {
                        prevX = (int)MouseManager.X - winX;
                        prevY = (int)MouseManager.Y - winY;
                        dragging = true;
                    }
                }
            }
            if (MouseManager.MouseState == MouseState.Left && MouseManager.LastMouseState == MouseState.None)
            {
                if (MouseManager.X >= winX + width - 26 && MouseManager.X <= winX + width)
                {
                    if (MouseManager.Y >= winY && MouseManager.Y <= winY + 29)
                    {
                        // close the application
                        AppManager.Applications.Remove(title);
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
            Kernel.DrawRoundedRectangle(winX, winY, width, height, 11, Color.FromArgb(unchecked((int)0xFF040404)));
            Kernel.DrawRoundedRectangle(winX, winY + 29, width, height - 29, 11, Color.FromArgb(unchecked((int)0xFF121212)));
            cv.DrawFilledRectangle(Color.FromArgb(unchecked((int)0xFF121212)), winX, winY + 29, width, 5);
            cv.DrawFilledCircle(Color.FromArgb(unchecked((int)0xFF1B1B1B)), winX + width - 6, winY + 5, 100);
            cv.DrawString(title, NovaShell.Shell.shellFont1, Color.White, winX + 5, winY + 8);
            cv.DrawString(content, NovaShell.Shell.shellFont1, Color.White, winX + 9, winY + 40);
        }
    }
}