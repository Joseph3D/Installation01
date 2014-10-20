using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

namespace Helpers
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

        private static Timer FileWatchlistTask;

        private static object OutputListLock;

        static XMLHotloader()
        {
            FileWatchList = new List<XMLFileData>();
            OutputList = new Dictionary<string, object>();

            FileWatchlistTask = new Timer(new TimerCallback(Process));

            OutputListLock = new object();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param">junk</param>
        private static void Process(object param)
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
                            lock (OutputListLock)
                            {
                                XmlSerializer Deserializer = new XmlSerializer(FileWatchList[i].ObjectType);
                                FileStream Stream = File.Open(FileWatchList[i].FilePath, FileMode.Open, FileAccess.Read);

                                System.Object Handle = Deserializer.Deserialize(Stream);

                                OutputList.Add(FileWatchList[i].FilePath, Handle);

                                FileWatchList[i].LastModifiedTime = DateTime.Now;

                                continue;
                            }
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

        /// <summary>
        /// Gets the latest handle to the object in the XML file
        /// if the specified file output does not exist in the output list it will return null
        /// </summary>
        /// <param name="FromFile">Gets hotloaded output from this file (if data is present)</param>
        /// <returns></returns>
        public static object GetOutputObject(string FromFile)
        {
            lock (OutputListLock)
            {
                if (OutputList.ContainsKey(FromFile))
                {
                    object output = OutputList[FromFile];

                    OutputList.Remove(FromFile);

                    return output;
                }
                return null;
            }
        }
    }
}