using System.IO;
using System.Xml;
using UnityEngine;

public class ReadXML
{
    internal XmlNodeList xmlNodeList;
    XmlDocument xmlDoc;
    public ReadXML(string path, int ChildNode = -1)
    {
        TextAsset xml = Resources.Load<TextAsset>(path); //Read File xml
        xmlDoc = new XmlDocument();
        xmlDoc.Load(new StringReader(xml.text));
        xmlNodeList = xmlDoc.DocumentElement.ChildNodes; // ----> Read all childNode in file
        if (ChildNode != -1)
        {
            xmlNodeList = xmlNodeList.Item(ChildNode).ChildNodes;// ----> read all childNode of one childNode
        }
    }
    public ReadXML(TextAsset xml)
    {
        xmlDoc = new XmlDocument();
        xmlDoc.Load(new StringReader(xml.text));
        xmlNodeList = xmlDoc.DocumentElement.ChildNodes; // ----> Read all childNode in file
    }

    public XmlNode getDataByValue(string key, string val)
    {
        foreach (XmlNode node in xmlNodeList)
        {
            if (node.Attributes[key].Value.Equals(val)) return node;
        }
        return null;
    }
    public XmlNode getDataByValue(string key1, string val1, string key2, string val2)
    {
        foreach (XmlNode node in xmlNodeList)
        {
            if (node.Attributes[key1].Value.Equals(val1) && node.Attributes[key2].Value.Equals(val2)) return node;
        }
        return null;
    }
    public XmlNode getDataByIndex(int index)
    {
        if (index >= xmlNodeList.Count)
            return null;
        else return xmlNodeList[index];
    }
    public XmlNode getDataByName(string name)
    {
        return xmlDoc.SelectSingleNode(name);
    }

    public XmlNodeList getChilds()
    {
        return xmlNodeList;
    }
}