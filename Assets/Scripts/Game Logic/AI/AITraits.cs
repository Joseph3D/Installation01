using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Assets.Scripts.Data;

namespace Assets.Scripts.Game_Logic.AI
{
    /// <summary>
    /// A set of general traits that will modify all aspects of an AIEntitie's behavior
    /// </summary>
    [Serializable]
    public class AITraits
    {
        [XmlElement]
        public float Health { get; private set; }

        [XmlElement]
        public float Aggression { get; private set; }

        [XmlElement]
        public float Intelligence { get; private set; }

        [XmlElement]
        public float Cowardice { get; private set; }

        [XmlElement]
        public float Teamwork { get; private set; }

        /// <summary>
        /// Default constructor for AITraits
        /// </summary>
        public AITraits()
        {
            Health = 0;
            Aggression = 0;
            Intelligence = 0;
            Cowardice = 0;
            Teamwork = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="health"></param>
        /// <param name="aggression"></param>
        /// <param name="intelligence"></param>
        /// <param name="cowardice"></param>
        /// <param name="teamwork"></param>
        public AITraits(float health, float aggression, float intelligence, float cowardice, float teamwork)
        {
            Health = health;
            Aggression = aggression;
            Intelligence = intelligence;
            Cowardice = cowardice;
            Teamwork = teamwork;
        }

        /// <summary>
        /// Deserializes and returns AITraits from an XML file.
        /// </summary>
        /// <param name="FilePath">Path of file containing serialized AITraits</param>
        /// <returns>Deserialized AITraits</returns>
        public static AITraits LoadFromFile(string FilePath)
        {
            return XMLBaseFile.LoadXMLFile(FilePath, typeof(AITraits)) as AITraits;
        }

        /// <summary>
        /// Serializes and writs AITraits to a file
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="Traits">AITraits to serialize</param>
        public static void WriteToFile(string FilePath, AITraits Traits)
        {
            XMLBaseFile.CreateXMLFile(FilePath, Traits);
        }
    }
}