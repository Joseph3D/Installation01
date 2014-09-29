using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Assets.Scripts.Data
{
    public class XMLBaseFile
    {
        private static readonly string _base_path = Application.persistentDataPath + "/"; 	// Base path for data files.
        private static readonly string _key = "12345678901234567890123456789012";			// Key
        private static readonly string _extension = ".xml";

        /// <summary>
        /// Checks if the file name exists.
        /// </summary>
        /// <returns><c>true</c>, if file exits, <c>false</c> otherwise.</returns>
        /// <param name="fileName">File name.</param>
        public static bool FileExits(string fileName)
        {
            return File.Exists(_base_path + fileName + XMLBaseFile._extension);
        }

        /// <summary>
        /// Creates the XML file (NON-Encrypted)
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="obj">Object.</param>
        public static void CreateXMLFile(string fileName, System.Object obj)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());

            using (FileStream stream = new FileStream(XMLBaseFile._base_path + fileName + XMLBaseFile._extension, FileMode.Create))
            {
                serializer.Serialize(stream, obj);
            }
        }

        /// <summary>
        /// Creates the XML file (Encrypted)
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="obj">Object.</param>
        public static void CreateEXMLFile(string fileName, System.Object obj)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            StringWriter textWriter = new StringWriter();

            serializer.Serialize(textWriter, obj);
            string encryptData = EncryptData(textWriter.ToString());

            using (StreamWriter stream = new StreamWriter(XMLBaseFile._base_path + fileName + XMLBaseFile._extension, false))
            {
                stream.WriteLine(encryptData);
            }
        }

        /// <summary>
        /// Loads the XML file (NON-Encrypted)
        /// </summary>
        /// <returns>The XML file.</returns>
        /// <param name="fileName">File name.</param>
        /// <param name="type">Type.</param>
        public static System.Object LoadXMLFile(string fileName, System.Type type)
        {
            XmlSerializer serializer = new XmlSerializer(type);

            using (FileStream stream = new FileStream(XMLBaseFile._base_path + fileName + XMLBaseFile._extension, FileMode.Open))
            {
                return serializer.Deserialize(stream) as System.Object;
            }
        }

        /// <summary>
        /// Loads the XML file (Encrypted)
        /// </summary>
        /// <returns>The EXML file.</returns>
        /// <param name="fileName">File name.</param>
        /// <param name="type">Type.</param>
        public static System.Object LoadEXMLFile(string fileName, System.Type type)
        {
            XmlSerializer serializer = new XmlSerializer(type);

            using (StreamReader stream = new StreamReader(XMLBaseFile._base_path + fileName + XMLBaseFile._extension, false))
            {
                string decryptData = DecryptData(stream.ReadToEnd());

                using (TextReader reader = new StringReader(decryptData))
                {
                    return serializer.Deserialize(reader) as System.Object;
                }
            }
        }

        /// <summary>
        /// Encrypts the data.
        /// </summary>
        /// <returns>The data.</returns>
        /// <param name="data">Data.</param>
        private static string EncryptData(string data)
        {
            byte[] key = UTF8Encoding.UTF8.GetBytes(XMLBaseFile._key);

            byte[] dataBytes = UTF8Encoding.UTF8.GetBytes(data);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = key;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform iCTransform = rDel.CreateEncryptor();

            byte[] encryptedBytes = iCTransform.TransformFinalBlock(dataBytes, 0, dataBytes.Length);

            return Convert.ToBase64String(encryptedBytes, 0, encryptedBytes.Length);
        }

        /// <summary>
        /// Decrypts the data.
        /// </summary>
        /// <returns>The data.</returns>
        /// <param name="data">Data.</param>
        private static string DecryptData(string data)
        {
            byte[] key = UTF8Encoding.UTF8.GetBytes(XMLBaseFile._key);

            byte[] dataBytes = Convert.FromBase64String(data);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = key;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform iCTransform = rDel.CreateDecryptor();

            byte[] decryptedBytes = iCTransform.TransformFinalBlock(dataBytes, 0, dataBytes.Length);

            return UTF8Encoding.UTF8.GetString(decryptedBytes, 0, decryptedBytes.Length);
        }
    }
}