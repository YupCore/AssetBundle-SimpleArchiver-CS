using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
            byte[] res = output.ToArray();
            output.Close();
            return res;
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
            input.Close();
            byte[] result = output.ToArray();
            output.Close();
            return result;
        }
    }
    public class AssetBundle
    {
        public string bundleInfoStr;
        public string bundlePathRelative;
        public FileStream bundleFile = null;
        public static AssetBundle CreateBundle(string bundleName, int headerSize, string[] args)
        {
            FileStream ms = new FileStream(bundleName, FileMode.Create);
            long fileEndIndex = 0;
            string bInfo = "";
            for (int i = 0; i < args.Length; i++)
            {
                byte[] uncompressedFile = File.ReadAllBytes(args[i]);
                byte[] localFile = AssetBundleUtil.Compress(uncompressedFile);
                fileEndIndex += localFile.Length;
                if (i == args.Length - 1)
                {
                    bInfo += Path.GetFileName(args[i]) + ":" + fileEndIndex;
                }
                else
                {
                    bInfo += Path.GetFileName(args[i]) + ":" + fileEndIndex + ";";
                }
                ms.Write(localFile, 0, localFile.Length);
                GC.Collect();
            }
            AssetBundle bundle = new AssetBundle();
            bundle.bundleInfoStr = bInfo;
            bundle.bundlePathRelative = bundleName;

            byte[] headerB = Encoding.UTF8.GetBytes(bInfo);
            List<byte> header = new List<byte>(headerSize); // Create a header for files
            for(int i = 0;i < headerB.Length;i++)
            {
                header.Add(headerB[i]);
            }
            var whitespace = Encoding.UTF8.GetBytes("!");
            while (header.Count < headerSize)
            {
                for (int i = 0; i < whitespace.Length; i++)
                {
                    header.Add(whitespace[i]);
                }
            }
            ms.Write(header.ToArray(), 0, header.Count);
            ms.Close();
            bundle.bundleFile = new FileStream(bundleName, FileMode.Open);
            return bundle;
        }
        public static AssetBundle CacheBundleInfo(string bundlePath, int headerSize)
        {
            AssetBundle bundle = new AssetBundle();
            bundle.bundlePathRelative = bundlePath;
            bundle.bundleFile = new FileStream(bundlePath, FileMode.Open);

            //Header read part
            bundle.bundleFile.Position = bundle.bundleFile.Length - headerSize;
            byte[] headerBytes = new byte[headerSize];
            bundle.bundleFile.Read(headerBytes, 0, headerBytes.Length);
            string headerStr = Encoding.UTF8.GetString(headerBytes);
            string bundleInfo = new string(headerStr.Where(c => !char.IsLetter('!')).ToArray());
            bundle.bundleFile.Position = 0;
            bundle.bundleInfoStr = bundleInfo;

            return bundle;
        }

        public void Close()
        {
            bundleFile.Close();
        }

        public string[] ListFiles()
        {
            string[] files = bundleInfoStr.Split(';');
            List<string> names = new List<string>();
            foreach (string str in files)
            {
                names.Add(str.Split(':')[0]);
            }
            return names.ToArray();
        }
        public async Task<byte[]> ReadDataAsync(string fileName)
        {
            var t = await Task.Run(() => ReadData(fileName));
            return t;
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
            for (int i = 0; i < names.Count; i++)
            {
                string str = names[i];
                if (str == FileName)
                {
                    if (i == 0)
                    {
                        byte[] segment = new byte[startIndexes[names.IndexOf(str)]];
                        bundleFile.Read(segment, 0, segment.Length);
                        byte[] uncompressed = AssetBundleUtil.Decompress(segment);
                        GC.Collect();
                        return uncompressed;
                    }
                    else
                    {
                        byte[] segment = new byte[startIndexes[names.IndexOf(str)] - startIndexes[names.IndexOf(str) - 1]];
                        bundleFile.Position = startIndexes[names.IndexOf(str) - 1];
                        bundleFile.Read(segment, 0, segment.Length);
                        byte[] uncompressed = AssetBundleUtil.Decompress(segment);
                        GC.Collect();
                        return uncompressed;
                    }
                }
            }
            return null;
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
