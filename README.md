# AssetBundle, SimpleArchiver tool CS
A small archivator-like tool to compress files into one and the read them. 

Mainly targeted for game development, for example when you need to create an archive of your game assets which your game will then read.
It creates binary file wich contains raw compressed data and txt file that represents header from wich the location of file is read and its name.

Example Usage:

```cs
string[] args = { "tt.txt", "Combined_Ferns_01.psd" };
var abs = AssetBundle.CreateBundle("bundle.bin", "binfo.txt", args); // Create an asset bundle with name bundle.bin and info path, and with string[] args as file paths

File.WriteAllBytes("text.txt", abs.ReadData("tt.txt")); // Read some asset and write it to file
abs.Close(); // Close assetBundle because 2 calls on one file is restricted

            
var abs2 = AssetBundle.CacheBundleInfo("bundle.bin", "binfo.txt"); // Read AB from disk (just for testing)
File.WriteAllBytes("ferns_01.psd", abs2.ReadData("Combined_Ferns_01.psd")); // Read some asset and write it to file
foreach (string str in abs2.ListFiles())
{
      Console.Write(str + " "); // tt.txt, conifer_macedonian_pine_Normal.png
}
```
