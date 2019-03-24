using UnityEngine;
using Newtonsoft.Json;

using System.IO;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace QTFramework
{
    // List<T>
    [Serializable]
    public class Serialization<T>
    {
        [SerializeField]
        List<T> data;
        public List<T> ToList() { return data; }
        public Serialization(List<T> data){ this.data = data; }
    }


    // Dictionary<TKey, TValue>
    [Serializable]
    public class Serialization<TKey, TValue> : ISerializationCallbackReceiver
    {
        [SerializeField]
        List<TKey> keys;
        [SerializeField]
        List<TValue> values;

        Dictionary<TKey, TValue> data;
        public Dictionary<TKey, TValue> ToDictionary() { return data; }
        public Serialization(Dictionary<TKey, TValue> data)
        {
            this.data = data;
        }

        public void OnBeforeSerialize()
        {
            keys = new List<TKey>(data.Keys);
            values = new List<TValue>(data.Values);
        }

        public void OnAfterDeserialize()
        {
            var count = Math.Min(keys.Count, values.Count);
            data = new Dictionary<TKey, TValue>(count);
            for (var i = 0; i < count; ++i)
            {
                data.Add(keys[i], values[i]);
            }
        }
    }


    //  /Users/mac/Library/Application Support/DefaultCompany/Client/JsonFile/MapData
    public class Json
    {
        public static string pathMapData = Application.persistentDataPath + "/JsonFile/MapData/";
        public static string pathDataInfo = Application.persistentDataPath + "/JsonFile/DataInfo/";

        static public string ToJson<T>(T obj)
        {
            return JsonUtility.ToJson(obj);
        }

        static public string ToJsonList<T>(List<T> obj)
        {
            return JsonUtility.ToJson(new Serialization<T>(obj));
        }

        static public string ToJson<TKey, TValue>(Dictionary<TKey, TValue> obj)
        {
            return JsonUtility.ToJson(new Serialization<TKey, TValue>(obj));
        }


        static public T FromJson<T>(string val)
        {
            return JsonUtility.FromJson<T>(val);
        }

        static public List<T> ListFromJson<T>(string val)
        {
            return JsonUtility.FromJson<Serialization<T>>(val).ToList();
        }

        static public Dictionary<TKey, TValue> FromJson<TKey, TValue>(string val)
        {
            return JsonUtility.FromJson<Serialization<TKey, TValue>>(val).ToDictionary();
        }


        ///// <summary>
        ///// 序列化
        ///// </summary>
        ///// <returns>The json.</returns>
        ///// <param name="obj">Object.</param>
        //static public string ToJson(object obj)
        //{

        //    //return JsonUtility.ToJson(obj);
        //    return JsonConvert.SerializeObject(obj);
        //}


        //static public T FromJson<T>(string val)
        //{
        //    //return JsonUtility.FromJson<T>(val);
        //    return JsonConvert.DeserializeObject<T>(val);
        //}



        public delegate void FixFile(string name);
        static public FixFile OnChanageEnd;


        //    byte[] bytes = Encoding.UTF8.GetBytes(val);
        //    Log.Debug("____Json 服务器数据存储  start____" + fs.Name);
        //    fs.BeginWrite(bytes, 0, val.Length, new AsyncCallback(endWrite), fs);

        static Dictionary<string, DateTime> dic_DT = new Dictionary<string, DateTime>();
        static public void SaveJson(string file_path, string val)
        {
            if (IsFileInUse(file_path))
                return;
            if (!dic_DT.ContainsKey(file_path))
                dic_DT.Add(file_path, DateTime.Now);
            else
                dic_DT[file_path] = DateTime.Now;


            FileStream stream = new FileStream(file_path, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            byte[] bytes = Encoding.UTF8.GetBytes(val);
            //Log.Warning("_____Json 服务器数据存储 start___" + file_path);

            //int len = (int)stream.Length;
            //if (len == 0)
            //len = bytes.Length;
            //Debug.Log("____" + stream.Length);
            stream.BeginWrite(bytes, 0, bytes.Length, new AsyncCallback(endWrite), stream);
        }


        static bool IsFileInUse(string path)
        {
            if (!File.Exists(path))
                return false;
            bool inUse = true;
            FileStream fs = null;
            try
            {
                fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
                inUse = false;
            }
            catch
            {

            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
            return inUse;//true表示正在使用,false没有使用  
        }

        static void endWrite(IAsyncResult asr)
        {
            using (FileStream file = (FileStream)asr.AsyncState)
            {
                string[] sArray = file.Name.Split(new char[2] { '/', '.' });


                TimeSpan ts = DateTime.Now.Subtract(dic_DT[file.Name.Replace("\\","/")]);

                Log.Warning("_____Json 服务器数据存储 end___" + file.Name + "__耗时__" + ts.TotalMilliseconds + "ms");





                file.EndWrite(asr);
                file.Dispose();
                //file.Flush();
                file.Close();
                if (OnChanageEnd != null)
                    OnChanageEnd(sArray[sArray.Length - 2]);
            }
        }

        static public void DeleteFile(string path)
        {
            File.Delete(path);
        }





        ///// 将字符串转成二进制
        //public static string StrToByteToStr(string s)
        //{
        //    byte[] data = Encoding.Unicode.GetBytes(s);
        //    StringBuilder result = new StringBuilder(data.Length * 8);


        //    foreach (byte b in data)
        //    {
        //        result.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
        //    }
        //    return result.ToString();
        //}

        //// 将二进制转成字符串
        //public static string ByteToStrToStr(string s)
        //{
        //    System.Text.RegularExpressions.CaptureCollection cs =
        //        System.Text.RegularExpressions.Regex.Match(s, @"([01]{8})+").Groups[1].Captures;
        //    byte[] data = new byte[cs.Count];
        //    for (int i = 0; i < cs.Count; i++)
        //    {
        //        data[i] = Convert.ToByte(cs[i].Value, 2);
        //    }
        //    return Encoding.Unicode.GetString(data, 0, data.Length);
        //}


        //压缩
        public static string GZipCompressString(string rawString)
        {
            return rawString;
            if (string.IsNullOrEmpty(rawString) || rawString.Length == 0)
            {
                return "";
            }
            else
            {
                byte[] rawData = System.Text.Encoding.UTF8.GetBytes(rawString.ToString());
                byte[] zippedData = Compress(rawData);
                return (string)(Convert.ToBase64String(zippedData));
            }
        }


        //解压
        public static string GZipDecompressString(string zippedString)
        {
            return zippedString;
            if (string.IsNullOrEmpty(zippedString) || zippedString.Length == 0)
            {
                return "";
            }
            else
            {
                byte[] zippedData = Convert.FromBase64String(zippedString);
                return (string)(System.Text.Encoding.UTF8.GetString(Decompress(zippedData)));
            }
        }



        static byte[] Compress(byte[] rawData)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.IO.Compression.GZipStream compressedzipStream = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Compress, true);
            compressedzipStream.Write(rawData, 0, rawData.Length);
            compressedzipStream.Close();
            return ms.ToArray();
        }
        public static byte[] Decompress(byte[] zippedData)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream(zippedData);
            System.IO.Compression.GZipStream compressedzipStream = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Decompress);
            System.IO.MemoryStream outBuffer = new System.IO.MemoryStream();
            byte[] block = new byte[10240];
            while (true)
            {
                int bytesRead = compressedzipStream.Read(block, 0, block.Length);
                if (bytesRead <= 0)
                    break;
                else
                    outBuffer.Write(block, 0, bytesRead);
            }
            compressedzipStream.Close();
            return outBuffer.ToArray();

        }

        public static List<Guid> StringAnalysisGuid(string root)
        {
            List<Guid> listGuid = new List<Guid>();
            if(root != "")
            {
                string[] words = root.Split('|');
                for (int i = 0; i < words.Length; i++)
                {
                    listGuid.Add(Guid.Parse(words[i]));
                }
            }
            return listGuid;
        }

        //typ 1是增加, 2是删除
        public static string DealString(string root, int typ, string child)
        {
            if (root == "")
            {
                root = child;
            }
            else
            {
                if (typ == 1)
                {
                    if (!root.Contains(child))
                        root = root + "|" + child;
                }
                else if (typ == 2)
                {
                    string temp = "";
                    string[] words = root.Split('|');
                    //int num = 0;
                    for (int i = 0; i < words.Length; i++)
                    {
                        if (words[i].Equals(child))
                            continue;
                        //num++;
                        temp = temp + words[i];
                        if (i < words.Length - 1)
                            temp = temp + "|";
                    }
                    root = temp;
                }
            }
            return root;
        }


        public static int GetStringCount(string root)
        {
            string[] words = root.Split('|');
            if (words[0] == "")
                return 0;
            return words.Length;
        }

        public static string GetStringByIdx(string root, int idx)
        {
            string[] words = root.Split('|');
            return words[idx];
        }
    
        public static string Vector3ToStr(Vector3 vector)
        {
            return vector.x + "|" + vector.y + "|" + vector.z;
        }

        public static Vector3 StrToVector3(string str)
        {
            string[] words = str.Split('|');
            Vector3 vector = new Vector3();
            vector.x = int.Parse(words[0]);
            vector.y = int.Parse(words[1]);
            vector.z = int.Parse(words[2]);
            return vector;
        }
    }
}
