using System.IO;
using System.Xml.Serialization;

namespace SPR
{
    public class Configuration
    {
        /// <summary>
        /// Définit le niveau max du verbose
        /// </summary>
        [XmlElement("VerbLevel")]
        public ushort VerbLvl { get; set; } = 1;

        /// <summary>
        /// Définit le niveau max du Level des fichiers logs
        /// </summary>
        [XmlElement("LogLevel")]
        public int LogLvl { get; set; } = 1;

        [XmlIgnore]
        public string ConfigLocation { get; private set; }


        #region Serialization XML
        public void SerializeMe(string location = null)
        {
            if (location == null)
                location = ConfigLocation;

            XmlSerializer xs = new XmlSerializer(typeof(Configuration));
            using (StreamWriter wr = new StreamWriter(location))
            {
                xs.Serialize(wr, this);
            }

        }

        /// <summary>
        /// Lit un fichier xml et désérialize
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public static Configuration DeserializeMe(string location)
        {
            Configuration configObj;
            XmlSerializer xs = new XmlSerializer(typeof(Configuration));
            using (StreamReader rd = new StreamReader(location))
            {
                configObj = xs.Deserialize(rd) as Configuration;
            }

            configObj.ConfigLocation = location;

            return configObj;
        }
        #endregion
    }
}
