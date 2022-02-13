# AssetBundle, SimpleArchiver tool CS
A small archivator-like tool to compress files into one and then reading them. 

Mainly targeted for game development, for example when you need to create an archive of your game assets which your game will then read.
It creates binary file wich contains compressed file data and header with fixed size defined by you.

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
        static void ReadToFile(AssetBundle abs,string filename) // Function to read to file, what else to say
        {
            byte[] dat = abs.ReadData(filename);
            File.WriteAllBytes(filename,dat);
            GC.Collect();
        }
        static void Main(string[] args)
        {
            int headerSize = 1024 * 100; // Define size of header to be ~100kb
            AssetBundle abs = null;
            if (!File.Exists("data.pak"))
            {
                abs = AssetBundle.CreateBundle("data.pak",headerSize, args); // Creating asset bundle from console args
            }
            else
            {
                abs = AssetBundle.CacheBundleInfo("data.pak",headerSize); // Cahing bundle from drive
            }
            Console.WriteLine("Enter number of files to read:");
            int numFilesRead = int.Parse(Console.ReadLine());
            for(int i = 0;i< numFilesRead;i++) // Reading files
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
