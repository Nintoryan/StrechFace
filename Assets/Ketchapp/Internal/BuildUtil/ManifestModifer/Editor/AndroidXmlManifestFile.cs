using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using UnityEngine;

namespace Ketchapp.Internal.BuildUtil.Editor.ManifestModifer
{
    internal class AndroidXmlManifestFile : XmlDocument
    {
        public readonly string AndroidXmlNamespace = "http://schemas.android.com/apk/res/android";
        public readonly string ToolXmlNamespace = "http://schemas.android.com/tools";
        private readonly string _path;
        protected readonly XmlNamespaceManager NamespaceManager;

        protected AndroidXmlManifestFile(string path)
        {
            _path = path;
            NamespaceManager = new XmlNamespaceManager(NameTable);
            NamespaceManager.AddNamespace("android", AndroidXmlNamespace);
            NamespaceManager.AddNamespace("tools", ToolXmlNamespace);
            using (var reader = new XmlTextReader(_path))
            {
                reader.Read();
                Load(reader);
            }
        }

        public void Save()
        {
            using (var writer = new XmlTextWriter(_path, new UTF8Encoding(false)) { Formatting = System.Xml.Formatting.Indented })
            {
                Save(writer);
            }
        }
    }
}
