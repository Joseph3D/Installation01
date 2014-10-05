using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public class XMLFileData
    {
        public object ObjectHandle { get; set; }
        public string FilePath { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public Type ObjectType
        {
            get
            {
                return ObjectHandle.GetType();
            }
        }
    }

    public class XMLHotloader
    {
        private static List<XMLFileData> FileWatchList;
        private static Dictionary<string, object> OutputList;

        private Timer FileWatchlistTask;

        public static XMLHotloader()
        {
            FileWatchList = new List<XMLFileData>();
            OutputList = new Dictionary<string, object>();
        }

        private static void Process()
        {
            for(int i = 0; i < FileWatchList.Count; ++i)
            {
                if(File.Exists(FileWatchList[i].FilePath))
                {
                    DateTime LastWriteTime = File.GetLastWriteTime(FileWatchList[i].FilePath);

                    if(LastWriteTime.CompareTo(FileWatchList[i].LastModifiedTime) > 0) // File has been modified since last load
                    {
                        try
                        {
                            XmlSerializer Deserializer = new XmlSerializer(FileWatchList[i].ObjectType);
                            FileStream Stream = File.Open(FileWatchList[i].FilePath, FileMode.Open, FileAccess.Read);

                            System.Object Handle = Deserializer.Deserialize(Stream);

                            OutputList.Add(FileWatchList[i].FilePath, Handle);

                            FileWatchList[i].LastModifiedTime = DateTime.Now;

                            continue;
                        }
                        catch(IOException e)
                        {
                            Debug.LogException(e);
                            continue;
                        }
                    }
                }
            }
        }
    }
}