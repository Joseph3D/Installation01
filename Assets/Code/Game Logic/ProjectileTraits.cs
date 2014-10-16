using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Helpers;

namespace GameLogic
{
    [XmlRoot]
    public sealed class ProjectileTraits
    {
        [XmlElement]
        public float Velocity { get; private set; }

        [XmlElement]
        public float Weight { get; private set; }

        [XmlElement]
        public float Lifespan { get; set; }

        [XmlElement]
        public float Damage { get; private set; }

        [XmlElement]
        public bool Homing { get; private set; }

        [XmlElement]
        public float MaxHomingTurn { get; private set; }

        [XmlElement]
        public bool Tracer { get; private set; }

        public static ProjectileTraits LoadFromFile(string TraitsFile)
        {
            if(File.Exists(TraitsFile))
            {
                ProjectileTraits LoadedTraits = XMLBaseFile.LoadXMLFile(TraitsFile, typeof(ProjectileTraits)) as ProjectileTraits;
                return LoadedTraits;
            }
            return null;
        }
    }
}