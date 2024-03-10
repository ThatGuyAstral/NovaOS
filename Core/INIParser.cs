using System;
using System.IO;
using NovaOS.NovaShell;

namespace NovaOS.Core
{
    public class INIParser
    {
        public static void ParseFile(string file)
        {
            try
            {
                string[] lines = File.ReadAllLines(file);
                foreach (string line in lines) 
                {
                    if (line.Contains("<cmd>"))
                        if (!line.EndsWith("</cmd>"))
                        {
                            Console.WriteLine("could not parse file: ");
                        }
                        else
                        {
                            string fcBase = line.Remove(line.IndexOf("<cmd>"));
                            string cmd = fcBase.Remove(fcBase.IndexOf("</cmd>"));

                            Shell.ExecuteCommand(cmd);
                        }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("could not parse file: "+ e.Message);
            }
        }
    }
}