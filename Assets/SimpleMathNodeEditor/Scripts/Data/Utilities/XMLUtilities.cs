using UnityEngine;
using System;
using System.Xml;
using System.Collections.Generic;
using System.IO;

public class XMLUtilities
{
    public static List<NodeDescriptor> getNodeTypes()
    {
        string xmlString = File.ReadAllText(Application.dataPath + "/SimpleMathNodeEditor/nodeTypes.xml");

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlString);             

        return ProcessNodes(xmlDoc.SelectNodes("NodeTypes/Node")); ;
    }

    private static List<NodeDescriptor> ProcessNodes(XmlNodeList nodes)
    {
        List<NodeDescriptor> list = new List<NodeDescriptor>();
        NodeDescriptor nodeDescriptor = null;

        foreach (XmlNode node in nodes)
        {
            nodeDescriptor = new NodeDescriptor();
            nodeDescriptor.nodeType = (NodeType)Convert.ToInt16(node.Attributes.GetNamedItem("type").Value);
            nodeDescriptor.nodeName = node.Attributes.GetNamedItem("name").Value;
            nodeDescriptor.numberOfInputs = Convert.ToInt16(node.SelectSingleNode("numberOfInputs").InnerText);
            nodeDescriptor.minInputs = Convert.ToInt16(node.SelectSingleNode("numberOfInputs").Attributes.GetNamedItem("min").Value);
            nodeDescriptor.maxInputs = Convert.ToInt16(node.SelectSingleNode("numberOfInputs").Attributes.GetNamedItem("max").Value);
            
            nodeDescriptor.numberOfOutputs = Convert.ToInt16(node.SelectSingleNode("numberOfOutputs").InnerText);
            nodeDescriptor.minOutputs= Convert.ToInt16(node.SelectSingleNode("numberOfOutputs").Attributes.GetNamedItem("min").Value);
            nodeDescriptor.maxOutputs = Convert.ToInt16(node.SelectSingleNode("numberOfOutputs").Attributes.GetNamedItem("max").Value);
            nodeDescriptor.titleBarColor = node.SelectSingleNode("color").InnerText;

            nodeDescriptor.parameters = new ParameterDictionary();

            //Loop the parameters
            foreach (XmlNode parameter in node.SelectNodes("parameters/parameter"))
            {
                string key = parameter.Attributes.GetNamedItem("key").Value;

                Type type = Type.GetType(parameter.Attributes.GetNamedItem("type").Value);
                NodeParameter value = new NodeParameter();
                if(type == typeof(float))
                {
                    value.floatParam = (float)Activator.CreateInstance(type);
                }
                if(type == typeof(string))
                {
                    value.stringParam = (string)Activator.CreateInstance(type);
                }
                if(type == typeof(Vector3))
                {
                    value.vectorParam = (Vector3)Activator.CreateInstance(type);
                }


                nodeDescriptor.parameters.Add(key,value);
            }

            list.Add(nodeDescriptor);
        }

        return list;
    }

}
