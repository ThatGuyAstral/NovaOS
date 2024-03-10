using Cosmos.Core;
using Cosmos.HAL;
using Cosmos.System.FileSystem;
using Sys = Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4.UDP.DHCP;
using CosmosTTF;
using System.Threading;
using IL2CPU.API.Attribs;
using NovaOS.Core;
using System;
using System.IO;
using NovaOS.Apps;
using Cosmos.HAL.BlockDevice.Ports;

namespace NovaOS.NovaShell
{
    public class Shell
    {
        [ManifestResourceStream(ResourceName = "NovaOS.Fonts.zap-light18.psf")] public static byte[] font1;
        [ManifestResourceStream(ResourceName = "NovaOS.Fonts.zap-vga16.psf")] public static byte[] font2;
        private static bool fsinit;
        private static string dir = @"0:\";
        public static PCScreenFont shellFont1;
        public static PCScreenFont shellFont2;

        private static bool echo = true;
        public static Disk disk;

        public static void Main()
        {
            Console.Clear();
            Console.WriteLine("NovaOS [Version 1.1 Beta]\nCopyright (c) cosmic 2023 - 2024\n");
            try
            {
                AHCI_SATA.Init(true);
                disk = new(SATA.Devices[0]);
                fsinit = true;
                Console.WriteLine("Initialized FS.\n");
                try
                {
                    shellFont1 = PCScreenFont.LoadFont(font1);
                    shellFont2 = PCScreenFont.LoadFont(font2);
                    ExecuteCommand("font 2");
                    Console.WriteLine("Loaded fonts.\n");
                }
                catch
                {
                    Console.WriteLine("Couldn't load fonts!\n");
                }
            }
            catch
            {
                fsinit = false;
                Console.WriteLine("Couldn't initialize FS!\n");
            }
            while (true)
            {
                string input;

                if (!fsinit)
                {
                    if (echo)
                    {
                        Console.Write("#");
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.Write("cosmic");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("@");
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.Write("nova");
                        Console.Write($"> ");
                        Console.ResetColor();
                    }
                    else
                        Console.Write("");
                    input = Console.ReadLine();
                    ExecuteCommand(input);
                }
                else
                {
                    if (echo)
                    {
                        Console.Write("#");
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.Write("cosmic");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("@");
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.Write("nova");
                        Console.Write($" : {dir}> ");
                        Console.ResetColor();
                    }
                    else
                        Console.Write("");
                    input = Console.ReadLine();
                    if (input == "nde")
                    {
                        Thread.Sleep(1000);
                        try
                        {
                            TTFManager.RegisterFont("Ubuntu Mono", Kernel.ubuntuBytes);
                            TTFManager.RegisterFont("Inter", Kernel.interBytes);
                            Kernel.cv = FullScreenCanvas.GetFullScreenCanvas(new Mode(1920, 1080, ColorDepth.ColorDepth32));
                            Sys.MouseManager.ScreenWidth = 1920;
                            Sys.MouseManager.ScreenHeight = 1080;
                            AppManager.Applications.Add("Calculator");
                            break;
                        }
                        catch (Exception e)
                        {
                            Kernel.Crash(e);
                            break;
                        }
                    }
                    else
                        ExecuteCommand(input);
                }
            }
        }

        public static void ExecuteCommand(string input)
        {
            /*if (input == "! ")
            {
                string file = input.Substring(2);
                INIParser.ParseFile();
            }*/
            if (input == "satachk")
            {
                AHCI_SATA.Check();
            }
            if (input.StartsWith("echo "))
            {
                string[] tokens = input.Split(' ');
                if (tokens.Length > 1)
                {
                    for (int i = 1; i < tokens.Length; ++i)
                        if (tokens[i] == "%CPU%")
                            Console.Write(CPU.GetCPUBrandString() + ' ');
                        else if (tokens[i] == "%RAM%")
                            Console.Write(CPU.GetAmountOfRAM() + " MB ");
                        else if (tokens[i] == "%NL%")
                            Console.Write('\n');
                        else if (tokens[i] == "%DIR%")
                            Console.Write(dir + ' ');

                        else
                            Console.Write(tokens[i] + ' ');
                    Console.Write('\n');
                }
                else
                    Console.WriteLine(tokens[1]);
            }
            if (input.StartsWith("@echo "))
            {
                string[] tokens = input.Split(' ');
                if (tokens[1] == "off")
                {
                    echo = false;
                }
                else if (tokens[1] == "on")
                {
                    echo = true;
                }
                else
                {
                    Console.WriteLine($"@echo: \"{tokens[1]}\" is an invalid parameter.");
                }
            }
            if (input == "help")
            {
                Console.WriteLine("echo <msg> - Prints a message.\ncd / chdir - Changes the current directory.\nls - Lists files and directories in the current path.\nbeep <freq> <duration> - Beeps on your speaker.\ncat - Read all lines of a file.\nfont <id> - Changes the current font.\nres <rows> <cols> - Changes the current resolution.\ncls / clear - Clears the screen\nrm - Deletes a file or more.\nmkdir - Create a directory\nnet - Opens the Network Configuration Editor.\nwget <url> <output> - Downloads a file from A domain using HTTP\nneofetch - Fetch info about the system.\nsatachk - Check if any SATA devices were initialized\nnde - Launches Nova Desktop Environment");
            }
            if (input.StartsWith("cd ") || input.StartsWith("chdir "))
            {
                if (!fsinit)
                    Console.WriteLine("Filesystem was not initialized.");
                else
                {
                    string[] tokens = input.Split(' ');
                    if (Directory.Exists(tokens[1]))
                        if (!tokens[1].EndsWith('\\'))
                        {
                            tokens[1] += '\\';
                            dir = tokens[1];
                        }
                        else
                        {
                            dir = tokens[1];
                        }
                    else
                        Console.WriteLine("Directory does not exist.");
                }
            }
            if (input == "ls")
            {
                if (!fsinit)
                    Console.WriteLine("Filesystem was not initialized.");
                else
                {
                    foreach (var dirs in Directory.GetDirectories(dir))
                    {
                        Console.WriteLine($"<DIR> {dirs}");
                    }
                    foreach (var files in Directory.GetFiles(dir))
                    {
                        Console.WriteLine(files);
                    }
                }
            }
            if (input == "neofetch")
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("                                                            \r\n                                                            \r\n                                                            \r\n ##      ###       #########      ###     ###         ##    \r\n ####    ###     ####    ####      ###   ####        ####   \r\n ######  ###     ###      ####      ### ####        ######  \r\n ##  #######     ###      ####      *######        ###  ### \r\n ##    #####     ####    ####        #####        ###    ## \r\n ##      ###       #########          ###        ####     ##\r\n                                                            \r\n                                                            \r\n                                                            ");
                Console.Write("root");
                Console.ResetColor();
                Console.Write("@");
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.Write("novashell\n");
                Console.ResetColor();
                Console.Write($"--------------\nOS: NovaOS 1.1 Beta\nKernel: Cosmos\nUptime: {CPU.GetCPUUptime()}\nShell: NovaShell 2.0\nCPU: {CPU.GetCPUBrandString()}\nMemory: {CPU.GetAmountOfRAM()}MiB\n\n");
                
                // color palette
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write("   ");
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.Write("   ");
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.Write("   ");
                Console.BackgroundColor = ConsoleColor.DarkYellow;
                Console.Write("   ");
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.Write("   ");
                Console.BackgroundColor = ConsoleColor.DarkMagenta;
                Console.Write("   ");
                Console.BackgroundColor = ConsoleColor.DarkCyan;
                Console.Write("   ");
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.Write("   \n");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write("   ");
                Console.BackgroundColor = ConsoleColor.Red;
                Console.Write("   ");
                Console.BackgroundColor = ConsoleColor.Green;
                Console.Write("   ");
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.Write("   ");
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.Write("   ");
                Console.BackgroundColor = ConsoleColor.Magenta;
                Console.Write("   ");
                Console.BackgroundColor = ConsoleColor.Cyan;
                Console.Write("   ");
                Console.BackgroundColor = ConsoleColor.White;
                Console.Write("   \n\n");
                Console.ResetColor();
            }
            if (input.StartsWith("beep "))
            {
                string[] tokens = input.Split(' ');
                try
                {
                    PCSpeaker.Beep(uint.Parse(tokens[1]), uint.Parse(tokens[2]));
                }
                catch
                {
                    Console.WriteLine("beep: Error while trying to beep.");
                }
            }
            if (input == "diskmgr")
            {
                bool open = false;
                if (!fsinit)
                    Console.WriteLine("Filesystem was not initialized.");
                else
                {
                    open = true;
                    Console.WriteLine("Disk Manager [Version 1.0]\n");
                }
                while (open)
                {
                    Console.Write($"{dir}> ");
                    string cmd = Console.ReadLine();
                    if (cmd.StartsWith("cp "))
                    {
                        string[] tokens = cmd.Split(" ");
                        try
                        {
                            disk.CreatePartition(int.Parse(tokens[1]));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"cp: Could not create partition: {e.Message}");
                        }
                    }
                    if (cmd.StartsWith("rm "))
                    {
                        string[] tokens = cmd.Split(" ");
                        try
                        {
                            disk.DeletePartition(int.Parse(tokens[1]));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"rm: Could not remove partition: {e.Message}");
                        }
                    }
                    if (cmd.StartsWith("mnt "))
                    {
                        string[] tokens = cmd.Split(" ");
                        try
                        {
                            disk.MountPartition(int.Parse(tokens[1]));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"mnt: Could not mount partition: {e.Message}");
                        }
                    }
                    if (cmd.StartsWith("fm "))
                    {
                        string[] tokens = cmd.Split(" ");
                        try
                        {
                            disk.FormatPartition(int.Parse(tokens[1]), tokens[2]);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"fm: Could not format partition: {e.Message}");
                        }
                    }
                    if (cmd == "size")
                    {
                        try
                        {
                            Console.WriteLine(disk.Size.ToString());
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"size: Could not fetch disk size: {e.Message}");
                        }
                    }
                    if (cmd == "info")
                    {
                        try
                        {
                            disk.DisplayInformation();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"info: Could not fetch disk info: {e.Message}");
                        }
                    }
                    if (cmd == "clear")
                    {
                        try
                        {
                            disk.Clear();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"clear: Could not clear disk: {e.Message}");
                        }
                    }
                    if (cmd == "mnt")
                    {
                        try
                        {
                            disk.Mount();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"clear: Could not clear disk: {e.Message}");
                        }
                    }
                }
            }
            if (input.StartsWith("cat "))
            {
                if (!fsinit)
                    Console.WriteLine("Filesystem was not initialized.");
                else
                {
                    string[] tokens = input.Split(' ');
                    try
                    {
                        if (tokens[1].Contains("\\") || tokens[1].Contains(":"))
                        {
                            Console.WriteLine(File.ReadAllText(tokens[1]));
                        }
                        else
                        {
                            Console.WriteLine(File.ReadAllText(dir + tokens[1]));
                        }
                    }
                    catch
                    {

                        Console.WriteLine("cat: Cannot open file.");
                    }
                }
            }
            if (input.StartsWith("res "))
            {
                string[] tokens = input.Split(' ');
                try
                {
                    Console.SetWindowSize(int.Parse(tokens[1]), int.Parse(tokens[2]));
                }
                catch
                {
                    Console.WriteLine("Could not change resolution.");
                }
            }
            if (input.StartsWith("font "))
            {
                string[] tokens = input.Split(' ');
                if (tokens[1] == "1")
                {
                    VGAScreen.SetFont(shellFont1.CreateVGAFont(), shellFont1.Height);
                }
                else if (tokens[1] == "2")
                {
                    VGAScreen.SetFont(shellFont2.CreateVGAFont(), shellFont2.Height);
                }
                else if (tokens[1] == "3")
                {
                    VGAScreen.SetFont(PCScreenFont.Default.CreateVGAFont(), PCScreenFont.Default.Height);
                }
            }
            if (input == "cls" || input == "clear")
            {
                Console.Clear();
            }
            if (input.StartsWith("mkdir "))
            {
                if (!fsinit)
                    Console.WriteLine("Filesystem was not initialized.");
                else
                {
                    string[] tokens = input.Split(' ');
                    try
                    {
                        Directory.CreateDirectory(tokens[1]);
                    }
                    catch
                    {
                        Console.WriteLine("mkdir: Cannot create directory.");
                    }
                }
            }
            if (input.StartsWith("rm "))
            {
                if (!fsinit)
                    Console.WriteLine("Filesystem was not initialized.");
                else
                {
                    string[] files = input.Split(' ');
                    for (int i = 0; i < files.Length; ++i)
                    {
                        try
                        {
                            File.Delete(dir + files[i]);
                        }
                        catch
                        {
                            Console.WriteLine("rm: One or more files couldn't be removed");
                        }
                    }
                }
            }
            if (input.StartsWith("wget "))
            {
                string[] tokens = input.Split(' ');
                Console.WriteLine(DateTime.Now.ToString("--yyyy-MM-dd HH:MM:ss--  ") + tokens[1]);
                Console.Write($"Downloading file from {tokens[1].Remove(tokens[1].IndexOf("http://"))}...");
                try
                {
                    File.Create(tokens[3]);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"wget: Failed to fetch response: {e}");
                }
            }
            if (input == "net")
            {
                bool open = false;
                if (!fsinit)
                    Console.WriteLine("Filesystem was not initialized.");
                else
                {
                    open = true;
                    Console.WriteLine("Network Configuration Editor\n[Version 1.0]\n\nPlease be sure you know what you're doing.\n");
                }
                while (open)
                {
                    Console.Write("# ");
                    string cmd = Console.ReadLine();
                    if (cmd == "autocfg")
                    {
                        using (var x = new DHCPClient())
                        {
                            try
                            {
                                x.SendDiscoverPacket();
                            }
                            catch
                            {
                                Console.WriteLine("autocfg: Cannot configurate: Discover packet error.");
                            }
                        }
                    }
                    if (cmd == "cip")
                    {
                        try
                        {
                            Console.WriteLine(NetworkConfiguration.CurrentAddress.ToString());
                        }
                        catch
                        {
                            Console.WriteLine("cip: Failed to fetch IP Address.");
                        }
                    }
                    if (cmd == "exit")
                    {
                        open = false;
                    }
                    if (cmd == "cls" ||  cmd == "clear")
                    {
                        Console.Clear();
                    }
                }
            }
        }
    }
}