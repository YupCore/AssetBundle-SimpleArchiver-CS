using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

//Yup Core 2021
// Part of YupEngine
namespace AssetBundleUtils
{
    public class AssetBundleUtil
    {
        public static byte[] Compress(byte[] data)
        {
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal))
            {
                dstream.Write(data, 0, data.Length);
                dstream.Close();
            }
            byte[] arr = output.ToArray();
            output.Close();
            return arr;
        }

        public static byte[] Decompress(byte[] data)
        {
            MemoryStream input = new MemoryStream(data);
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
            {
                dstream.CopyTo(output);
                dstream.Close();
            }
            byte[] arr = output.ToArray();
            output.Close();
            input.Close();
            return arr;
        }
    }
    public class AssetBundle
    {
        public string bundleInfoStr;
        public string bundlePathRelative;
        public FileStream bundleFile = null;
        public static AssetBundle CreateBundle(string bundleName, string bundleInfoName, string[] args)
        {
            File.WriteAllText(bundleInfoName, "");
            FileStream ms = new FileStream(bundleName,FileMode.Create);
            int fileEndIndex = 0;
            for (int i = 0; i < args.Length; i++)
            {
                byte[] bytes = AssetBundleUtil.Compress(File.ReadAllBytes(args[i]));
                fileEndIndex += bytes.Length;
                if (i == args.Length - 1)
                {
                    File.AppendAllText(bundleInfoName, Path.GetFileName(args[i]) + ":" + fileEndIndex);
                }
                else
                {
                    File.AppendAllText(bundleInfoName, Path.GetFileName(args[i]) + ":" + fileEndIndex + ";");
                }
                ms.Write(bytes, 0, bytes.Length);
            }
            AssetBundle bundle = new AssetBundle();
            bundle.bundleInfoStr = File.ReadAllText(bundleInfoName);
            bundle.bundlePathRelative = bundleInfoName;
            ms.Close();
            bundle.bundleFile = new FileStream(bundleName, FileMode.Open);
            return bundle;
        }
        public static AssetBundle CacheBundleInfo(string bundlePath,string bundleInfoPath)
        {
            AssetBundle bundle = new AssetBundle();
            bundle.bundlePathRelative = bundlePath;
            bundle.bundleInfoStr = File.ReadAllText(bundleInfoPath);
            bundle.bundleFile = new FileStream(bundlePath, FileMode.Open);
            return bundle;
        }
        public byte[] ReadData(string FileName)
        {
            string bundleInfo = bundleInfoStr;

            string[] split1 = bundleInfo.Split(';');
            List<string> names = new List<string>();
            List<int> startIndexes = new List<int>();
            

            List<string> spt = new List<string>();
            foreach (string str in split1)
            {
                names.Add(str.Split(':')[0]);
                spt.Add(str);
            }
            foreach (string str in spt)
            {
                try
                {
                    startIndexes.Add(int.Parse(str.Split(':')[1]));
                }
                catch
                {
                    continue;
                }
            }
            for(int i = 0; i < names.Count; i++)
            {
                string str = names[i];
                if (str == FileName)
                {
                    if (i == 0)
                    {
                        byte[] segment = new byte[startIndexes[names.IndexOf(str)]];
                        bundleFile.Read(segment, 0, segment.Length);
                        bundleFile.Close();
                        byte[] uncompressed = AssetBundleUtil.Decompress(segment);
                        return uncompressed;
                    }
                    else
                    {
                        byte[] segment = new byte[startIndexes[names.IndexOf(str)] - startIndexes[names.IndexOf(str) - 1]];
                        bundleFile.Position = startIndexes[names.IndexOf(str) - 1];
                        bundleFile.Read(segment, 0, segment.Length);
                        bundleFile.Close();
                        byte[] uncompressed = AssetBundleUtil.Decompress(segment);
                        return uncompressed;
                    }
                }
            }
            throw new Exception("FILE DOESN'T EXISTS IN ASSET BUNDLE:" + FileName);
        }

        #region NotFinished
        //public void OverrideFile(string fileName, byte[] newData) //
        //{
        //    MemoryStream mem = new MemoryStream(File.ReadAllBytes(bundlePathRelative));

        //    string bundleInfo = bundleInfoStr;

        //    string[] split1 = bundleInfo.Split(';');
        //    List<string> names = new List<string>();
        //    List<int> startIndexes = new List<int>();


        //    List<string> spt = new List<string>();
        //    foreach (string str in split1)
        //    {
        //        names.Add(str.Split(':')[0]);
        //        spt.Add(str);
        //    }
        //    foreach (string str in spt)
        //    {
        //        try
        //        {
        //            startIndexes.Add(int.Parse(str.Split(':')[1]));
        //        }
        //        catch
        //        {
        //            continue;
        //        }
        //    }

        //    for (int i = 0; i < names.Count; i++)
        //    {
        //        string str = names[i];
        //        if (str == fileName)
        //        {
        //            if (i == 0)
        //            {
        //                split1[i] = "";
        //                byte[] processed = startIndexes[names.IndexOf(str)];
        //            }
        //            else
        //            {
        //                split1[i] = "";
        //            }
        //        }
        //    }
        //}
        #endregion
    }

    // Sample Usage

    //class Program
    //{
    //    public static void Main(string[] args)
    //    {
    //        AssetBundle.CreateBundle("bundle.bin", "binfo.txt", args);
    //        //var abs = AssetBundle.CacheBundleInfo("bundle.bin","binfo.txt");

    //        //File.WriteAllBytes("tt.txt", abs.ReadData("tt.txt"));
    //        //File.WriteAllBytes("conifer_macedonian_pine_Normal.png", abs.ReadData("conifer_macedonian_pine_Normal.png"));
    //        //File.WriteAllBytes("11229-normal.jpg", abs.ReadData("11229-normal.jpg"));
    //    }
    //}
}
