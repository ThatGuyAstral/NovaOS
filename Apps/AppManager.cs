using Cosmos.System.Graphics;
using System;
using System.Collections.Generic;
using NovaOS.Apps;

namespace NovaOS.Apps
{
    public class AppManager
    {
        public static List<string> Applications = new();
        public static int AppsOpen;

        public static void Manage(Canvas cv)
        {
            foreach (var App in Applications)
            {
                if (App == "Calculator")
                {
                    Calculator.CalculatorApp(cv);
                }
                if (App == "About")
                {

                }
                AppsOpen = Applications.Count;
            }
        }
    }
}
