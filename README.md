# AssetBundleCS
A small archivator-like tool to compress files into one and the read them.

It creates binary compressed file wich contains raw data and txt file that represents header from wich the location of file is read and its name.

Example Usage:

```cs
var test = AssetBundle.CreateBundle("bundle.bin", "binfo.txt", args); // Create an asset bundle with name bundle.bin and info path, and with string[] args as file paths
var abs = AssetBundle.CacheBundleInfo("bundle.bin","binfo.txt"); // Read AB from disk (just for testing)

File.WriteAllBytes("tt.txt", abs.ReadData("tt.txt")); // Read some asset and write it to file
File.WriteAllBytes("conifer_macedonian_pine_Normal.png", test.ReadData("conifer_macedonian_pine_Normal.png")); // Read some asset and write it to file
```
