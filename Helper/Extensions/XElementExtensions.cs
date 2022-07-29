using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Helper
{
    public static class XElementExtensions
    {
        public static XmlElement ToXmlElement(this XElement el)
        {
            var doc = new XmlDocument();
            doc.Load(el.CreateReader());
            return doc.DocumentElement!;
        }

        public static string InnerXML(this XElement el)
        {
            var reader = el.CreateReader();
            reader.MoveToContent();
            return reader.ReadInnerXml();
        }
    }
}
