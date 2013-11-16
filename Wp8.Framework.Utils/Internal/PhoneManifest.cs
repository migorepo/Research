using System;
using System.Xml;

namespace Wp8.Framework.Utils.Internal
{
    public class PhoneManifest
    {
        public static string GetAttribute(string attributeName)
        {
            try
            {
                const string appManifestName = "WMAppManifest.xml";
                const string appNodeName = "App";

                var settings = new XmlReaderSettings { XmlResolver = new XmlXapResolver() };
                using (var reader = XmlReader.Create(appManifestName, settings))
                {
                    reader.ReadToDescendant(appNodeName);
                    return !reader.IsStartElement() ? "" : reader.GetAttribute(attributeName);
                }
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
