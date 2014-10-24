using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Helpers
{
    public class XMLHelper
    {
        private static readonly string BasePath = "Resources/Data/";
        private static readonly string DotXMLExtension = ".xml";

        /// <summary>
        /// Checks if the file name exists.
        /// </summary>
        /// <returns><c>true</c>, if file exits, <c>false</c> otherwise.</returns>
        /// <param name="fileName">File name.</param>
        public static bool FileExits(string fileName)
        {
            return File.Exists(BasePath + fileName + XMLHelper.DotXMLExtension);
        }

        /// <summary>
        /// Creates the XML file
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="obj">Object.</param>
        public static void CreateXMLFile(string fileName, System.Object obj)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());

            using (FileStream stream = new FileStream(XMLHelper.BasePath + fileName + XMLHelper.DotXMLExtension, FileMode.Create))
            {
                serializer.Serialize(stream, obj);
            }
        }

        /// <summary>
        /// Loads the XML file
        /// </summary>
        /// <returns>The XML file.</returns>
        /// <param name="fileName">File name.</param>
        /// <param name="type">Type.</param>
        public static System.Object LoadXMLFile(string fileName, System.Type type)
        {
            XmlSerializer serializer = new XmlSerializer(type);

            using (FileStream stream = new FileStream(XMLHelper.BasePath + fileName + XMLHelper.DotXMLExtension, FileMode.Open))
            {
                return serializer.Deserialize(stream) as System.Object;
            }
        }
    }
}