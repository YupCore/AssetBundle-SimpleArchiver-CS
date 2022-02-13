# AssetBundle, SimpleArchiver tool CS
A small archivator-like tool to compress files into one and the read them. 

Mainly targeted for game development, for example when you need to create an archive of your game assets which your game will then read.
It creates binary file wich contains raw compressed data and txt file that represents header from wich the location of file is read and its name.

Example Usage:

```cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AssetBundleUtils;

namespace AssetTools
{
    internal class Program
    {
        static void ReadToFile(AssetBundle abs,string filename)
        {
            byte[] dat = abs.ReadData(filename);
            File.WriteAllBytes(filename,dat);
            GC.Collect();
        }
        static void Main(string[] args)
        {
            int headerSize = 1024 * 100;
            AssetBundle abs = null;
            if (!File.Exists("data.pak"))
            {
                abs = AssetBundle.CreateBundle("data.pak",headerSize, args); // Create an asset bundle with name bundle.bin and info path, and with string[] args as file paths
            }
            else
            {
                abs = AssetBundle.CacheBundleInfo("data.pak",headerSize);
            }
            Console.WriteLine("Enter number of files to read:");
            int numFilesRead = int.Parse(Console.ReadLine());
            for(int i = 0;i< numFilesRead;i++)
            {
                Console.WriteLine("Please enter file to read:");
                ReadToFile(abs, Console.ReadLine());
            }

            Console.WriteLine("Press any key to close app:");
            Console.ReadKey();
        }
    }
}

```
