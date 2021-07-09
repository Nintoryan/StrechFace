/* 
 Licensed under the Apache License, Version 2.0

 http://www.apache.org/licenses/LICENSE-2.0
 */
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
namespace Ketchapp.Internal.Configuration
{
    [Serializable]
    [XmlRoot(ElementName = "dependencies")]
    public class Dependencies
    {
        [XmlElement(ElementName = "unityversion")]
#pragma warning disable SA1401 // Fields should be private
        public string Unityversion;
        [XmlElement(ElementName = "androidPackages")]
        public List<AndroidPackages> AndroidPackages;
        [XmlElement(ElementName = "iosPods")]
        public List<IosPods> IosPods;
    }

    [Serializable]
    [XmlRoot(ElementName = "repositories")]
    public class Repositories
    {
        [XmlElement(ElementName = "repository")]
        public string Repository;
    }

    [Serializable]
    [XmlRoot(ElementName = "androidPackage")]
    public class AndroidPackage
    {
        [XmlElement(IsNullable = false, ElementName = "repositories")]
        public Repositories Repositories;
        [XmlAttribute(AttributeName = "spec")]
        public string Spec;
    }

    [Serializable]
    [XmlRoot(ElementName = "androidPackages")]
    public class AndroidPackages
    {
        [XmlElement(ElementName = "androidPackage")]
        public AndroidPackage AndroidPackage;
    }

    [Serializable]
    [XmlRoot(ElementName = "sources")]
    public class Sources
    {
        [XmlElement(ElementName = "source")]
        public string Source;
    }

    [Serializable]
    [XmlRoot(ElementName = "iosPod")]
    public class IosPod
    {
        [XmlElement(ElementName = "sources")]
        public Sources Sources;
        [XmlAttribute(AttributeName = "name")]
        public string Name;
        [XmlAttribute(AttributeName = "version")]
        public string Version;
    }

    [Serializable]
    [XmlRoot(ElementName = "iosPods")]
    public class IosPods
    {
        [XmlElement(ElementName = "iosPod")]
        public IosPod IosPod;
    }
#pragma warning restore SA1401 // Fields should be private
}
