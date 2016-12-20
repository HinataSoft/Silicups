using System;
using System.Collections.Generic;
using System.Text;

namespace System.Xml
{
    /// <summary>
    /// Support class for System.Xml library (XmlNode etc.)
    /// </summary>
    public static class XmlHelper
    {
        /// <summary>
        /// Appends a XML attribute to a node
        /// </summary>
        /// <returns>XmlNode given as argument for chaining</returns>
        public static XmlNode AppendXmlAttribute(this XmlNode node, string name, string value)
        {
            XmlAttribute att = node.OwnerDocument.CreateAttribute(name);
            att.Value = value;
            ((XmlElement)node).Attributes.Append(att);
            return node;
        }

        /// <summary>
        /// Appends a XML attribute to a node (only when given value is not null nor empty)
        /// </summary>
        /// <returns>XmlNode given as argument for chaining</returns>
        public static XmlNode AppendXmlAttributeWhenNotEmpty(this XmlNode node, string name, string value)
        {
            if (String.IsNullOrEmpty(value))
            { return node; }
            return AppendXmlAttribute(node, name, value);
        }

        /// <summary>
        /// Appends a boolean XML attribute to a node
        /// </summary>
        /// <returns>XmlNode given as argument for chaining</returns>
        public static XmlNode AppendXmlAttribute(this XmlNode node, string name, bool value)
        {
            return AppendXmlAttribute(node, name, value.ToString());
        }

        /// <summary>
        /// Appends an integer XML attribute to a node
        /// </summary>
        /// <returns>XmlNode given as argument for chaining</returns>
        public static XmlNode AppendXmlAttribute(this XmlNode node, string name, int value)
        {
            return AppendXmlAttribute(node, name, value.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Appends a double XML attribute to a node (InvariantCulture formatting)
        /// </summary>
        /// <returns>XmlNode given as argument for chaining</returns>
        public static XmlNode AppendXmlAttribute(this XmlNode node, string name, double value)
        {
            return AppendXmlAttribute(node, name, value.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Appends a double XML attribute to a node (InvariantCulture formatting)
        /// </summary>
        /// <returns>XmlNode given as argument for chaining</returns>
        public static void AppendXmlAttribute(this XmlNode node, string name, double? value)
        {
            if (value.HasValue)
            { AppendXmlAttribute(node, name, value.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)); }
        }

        /// <summary>
        /// For every pair in <paramref name="pairs"/> a subnode is appended to the node with two attributes with key and value of the pair
        /// </summary>
        /// <param name="node">Node operated on</param>
        /// <param name="subnodeName">Subnode name</param>
        /// <param name="keyAttrName">Attribute holding the key of the pair</param>
        /// <param name="valueAttrName">Attribute holding the value of the pair</param>
        /// <param name="pairs">Enumerable with string/string values</param>
        /// <returns>XmlNode given as argument for chaining</returns>
        public static XmlNode AppendXmlAttributePairsAsSubnodes(this XmlNode node, string subnodeName, string keyAttrName, string valueAttrName,
            IEnumerable<KeyValuePair<string, string>> pairs)
        {
            foreach (var pair in pairs)
            {
                XmlNode subNode = node.AppendXmlElement(subnodeName);
                subNode.AppendXmlAttributeWhenNotEmpty(keyAttrName, pair.Key);
                subNode.AppendXmlAttributeWhenNotEmpty(valueAttrName, pair.Value);
            }
            return node;
        }

        /// <summary>
        /// Appends a XML element to given node
        /// </summary>
        /// <returns>Created XmlElement</returns>
        public static XmlElement AppendXmlElement(this XmlNode node, string name)
        {
            return AppendXmlElement(node, name, null);
        }

        /// <summary>
        /// Appends a XML element to given node
        /// </summary>
        /// <returns>Created XmlElement</returns>
        public static XmlElement AppendXmlElement(this XmlNode node, string name, string value)
        {
            XmlDocument xmlDoc = (node is XmlDocument) ? (XmlDocument)node : node.OwnerDocument;
            XmlElement elem = xmlDoc.CreateElement(name);
            node.AppendChild(elem);
            if (value != null)
            { elem.InnerText = value; }
            return elem;
        }

        /// <summary>
        /// Gets a XML attribute from a node
        /// </summary>
        /// <returns>Requested XmlAttribute; throws an exception if not found</returns>
        public static XmlAttribute GetAttribute(this XmlNode node, string name)
        {
            return GetAttribute(node, name, true);
        }

        /// <summary>
        /// Gets a XML attribute from a node
        /// </summary>
        /// <returns>Requested XmlAttribute; returns null if not found</returns>
        public static XmlAttribute FindAttribute(this XmlNode node, string name)
        {
            return GetAttribute(node, name, false);
        }

        private static XmlAttribute GetAttribute(XmlNode node, string name, bool mustExist)
        {
            XmlAttribute att = node.Attributes[name];
            if (mustExist && (att == null))
            { throw new Exception("Attribute '" + name + "' not found"); }
            return att;
        }

        /// <summary>
        /// Returns XML node (element, attribute) value as string
        /// </summary>
        public static string AsString(this XmlNode node)
        {
            if (node == null)
            { return null; }
            var attr = node as XmlAttribute;
            if (attr != null)
            { return attr.Value; }
            else
            { return node.InnerText; }
        }

        /// <summary>
        /// Returns XML node (element, attribute) value as string
        /// </summary>
        public static string AsString(this XmlNode node, string defaultValue)
        {
            return AsString(node) ?? defaultValue;
        }

        /// <summary>
        /// Returns XML node (element, attribute) value as byte
        /// </summary>
        public static byte AsByte(this XmlNode node)
        {
            return Byte.Parse(AsString(node));
        }

        /// <summary>
        /// Returns XML node (element, attribute) value as byte
        /// </summary>
        public static byte AsByte(this XmlNode node, byte defaultValue)
        {
            if (node == null)
            { return defaultValue; }
            return AsByte(node);
        }

        /// <summary>
        /// Returns XML node (element, attribute) value as ushort
        /// </summary>
        public static ushort AsUInt16(this XmlNode node)
        {
            return UInt16.Parse(AsString(node));
        }

        /// <summary>
        /// Returns XML node (element, attribute) value as ushort
        /// </summary>
        public static ushort AsUInt16(this XmlNode node, ushort defaultValue)
        {
            if (node == null)
            { return defaultValue; }
            return AsUInt16(node);
        }

        /// <summary>
        /// Returns XML node (element, attribute) value as int
        /// </summary>
        public static int AsInt32(this XmlNode node)
        {
            return Int32.Parse(AsString(node));
        }

        /// <summary>
        /// Returns XML node (element, attribute) value as int
        /// </summary>
        public static int AsInt32(this XmlNode node, int defaultValue)
        {
            if (node == null)
            { return defaultValue; }
            return AsInt32(node);
        }

        /// <summary>
        /// Returns XML node (element, attribute) value as double
        /// </summary>
        public static double AsDouble(this XmlNode node)
        {
            return Double.Parse(AsString(node), System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns XML node (element, attribute) value as double
        /// </summary>
        public static double AsDouble(this XmlNode node, double defaultValue)
        {
            if (node == null)
            { return defaultValue; }
            return AsDouble(node);
        }

        /// <summary>
        /// Returns XML node (element, attribute) value as double
        /// </summary>
        public static double? AsNullableDouble(this XmlNode node)
        {
            if (node == null)
            { return null; }
            return AsDouble(node);
        }

        /// <summary>
        /// Returns XML node (element, attribute) value as bool
        /// </summary>
        public static bool AsBoolean(this XmlNode node)
        {
            return Boolean.Parse(AsString(node));
        }

        /// <summary>
        /// Returns XML node (element, attribute) value as bool
        /// </summary>
        public static bool AsBoolean(this XmlNode node, bool defaultValue)
        {
            if (node == null)
            { return defaultValue; }
            return AsBoolean(node);
        }

        public static T AsEnum<T>(this XmlNode node)
        {
            return (T)Enum.Parse(typeof(T), AsString(node), true);
        }

        public static T AsEnum<T>(this XmlNode node, T defaultValue)
        {
            if (node == null)
            { return defaultValue; }
            return (T)Enum.Parse(typeof(T), AsString(node), true);
        }

        public static int AsEnumInt<T>(this XmlNode node)
        {
            return (int)Enum.Parse(typeof(T), AsString(node), true);
        }

        public static int AsEnumInt<T>(this XmlNode node, int defaultValue)
        {
            if (node == null)
            { return defaultValue; }
            return (int)Enum.Parse(typeof(T), AsString(node), true);
        }

        /// <summary>
        /// Finds all children XML nodes with given name
        /// </summary>
        public static IEnumerable<XmlNode> FindNodes(this XmlNode node, string name)
        {
            foreach (XmlNode subNode in node.ChildNodes)
            {
                if (subNode.Name == name)
                { yield return subNode; }
            }
        }

        /// <summary>
        /// Finds the one child XML node with given name
        /// </summary>
        /// <returns>Returns the child node, null if not found and an exception if too many found</returns>
        public static XmlNode FindOneNode(this XmlNode node, string name)
        {
            XmlNode result = null;
            foreach (XmlNode subNode in node.ChildNodes)
            {
                if (subNode.Name != name)
                { continue; }
                if (result != null)
                { throw new Exception("Duplicite XML node " + name); }
                result = subNode;
            }
            return result;
        }

        /// <summary>
        /// Get the one child XML node with given name
        /// </summary>
        /// <returns>Returns the child node or an exception if none or too many found</returns>
        public static XmlNode GetOneNode(this XmlNode node, string name)
        {
            XmlNode result = FindOneNode(node, name);
            if(result == null)
            { throw new Exception("Missing XML node " + name); }
            return result;
        }

        /// <summary>
        /// Finds all children XML nodes with given name
        /// </summary>
        /// <param name="nodes">enumerable providing parent nodes</param>
        public static IEnumerable<XmlNode> FindNodes(this IEnumerable<XmlNode> nodes, string name)
        {
            foreach (XmlNode node in nodes)
            {
                foreach (XmlNode subNode in node.FindNodes(name))
                {
                    yield return subNode;
                }
            }
        }

        /// <summary>
        /// Iterates subnodes of name <paramref name="subnodeName"/> and returns its two attributes as a pair enumareble
        /// </summary>
        /// <param name="node">Root of the subnodes</param>
        /// <param name="subnodeName">Subnodes name</param>
        /// <param name="keyAttrName">Attribute holding the key of the pair</param>
        /// <param name="valueAttrName">Attribute holding the value of the pair</param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, string>> GetAttrPairsFromSubnodes(this XmlNode node, string subnodeName, string keyAttrName, string valueAttrName)
        {
            foreach (XmlNode subnode in node.FindNodes(subnodeName))
            {
                yield return new KeyValuePair<string, string>(
                    subnode.GetAttribute(keyAttrName).AsString(""),
                    subnode.GetAttribute(valueAttrName).AsString("")
                );
            }
        }

        /// <summary>
        /// Loads a XmlDocument and returns it
        /// </summary>
        /// <param name="fileName">XML file to load</param>
        /// <returns>the XmlDocument</returns>
        public static XmlDocument LoadXml(string fileName)
        {
            var doc = new XmlDocument();
            doc.Load(fileName);
            return doc;
        }
    }
}
