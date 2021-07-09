using System.Xml;
using UnityEngine;

namespace Ketchapp.Internal.BuildUtil.Editor.ManifestModifer
{
    internal class AndroidManifest : AndroidXmlManifestFile
    {
        private XmlElement _manifestElement;
        private XmlElement _applicationElement;
        private string AndroidNamespace => NamespaceManager.LookupNamespace("android");
        private string ToolsNamespace => NamespaceManager.LookupNamespace("tools");

        public AndroidManifest(string path)
            : base(path)
            {
            _manifestElement = SelectSingleNode("/manifest") as XmlElement;
            _applicationElement = SelectSingleNode("/manifest/application") as XmlElement;
            }

        public void SetAttributeInManifest(AndroidManifestType manifestType, string name, string value)
        {
            switch (manifestType)
            {
                case AndroidManifestType.ManifestElement:
                    _manifestElement.SetAttribute(name, AndroidNamespace, value);
                    break;
                case AndroidManifestType.ApplicationElement:
                    _applicationElement.SetAttribute(name, AndroidNamespace, value);
                    break;
                case AndroidManifestType.UsesPermission:
                    SetPermission(manifestType, name, value);
                    break;
                case AndroidManifestType.MetaData:
                    SetPermission(manifestType, name, value);
                    break;
                case AndroidManifestType.ToolsElement:
                    _applicationElement.SetAttribute(name, ToolsNamespace, value);
                    break;
            }
        }

        private void SetPermission(AndroidManifestType manifestType, string name, string value)
        {
            XmlElement child;
            XmlAttribute mainAttribute;

            switch (manifestType)
            {
                case AndroidManifestType.UsesPermission:
                    child = CreateElement("uses-permission");
                    _manifestElement.AppendChild(child);
                    mainAttribute = CreateAndroidAttribute(name, value);
                    child.Attributes.Append(mainAttribute);
                    break;
                case AndroidManifestType.MetaData:
                    child = CreateElement("meta-data");
                    _applicationElement.AppendChild(child);
                    mainAttribute = CreateAndroidAttribute("name", name);
                    XmlAttribute valueAttribute = CreateAndroidAttribute("value", value);
                    child.Attributes.Append(mainAttribute);
                    child.Attributes.Append(valueAttribute);
                    break;
            }
        }

        private XmlAttribute CreateAndroidAttribute(string key, string value)
        {
            XmlAttribute attr = CreateAttribute("android", key, AndroidXmlNamespace);
            attr.Value = value;
            return attr;
        }
    }

    public enum AndroidManifestType
    {
        ManifestElement = 1,
        ApplicationElement = 2,
        UsesPermission = 3,
        MetaData = 4,
        ToolsElement = 5
    }
}
