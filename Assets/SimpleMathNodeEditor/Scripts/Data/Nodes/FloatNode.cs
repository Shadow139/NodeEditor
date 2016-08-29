using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Globalization;

[Serializable]
public class FloatNode : NodeBase
{
    public float nodeValue;
    public string stringToEdit = "";
    public NodeOutput output;

    public FloatNode()
    {
        output = new NodeOutput();
        nodeOutputs.Add(output);
    }

    public override void InitNode()
    {
        base.InitNode();
        nodeType = NodeType.Float;
        nodeRect = new Rect(10f, 10f, 150f, 100f);
    }

    public override void UpdateNodeGUI(Event e, Rect viewRect, Rect workViewRect, GUISkin guiSkin)
    {
        base.UpdateNodeGUI(e, viewRect, workViewRect, guiSkin);

        stringToEdit = GUI.TextField(new Rect(nodeRect.x + nodeRect.width * 0.5f + 8f, nodeRect.y + nodeRect.height * 0.5f - 10f, nodeRect.width * 0.4f, 20f), nodeValue.ToString(), 25);
        try
        {
            nodeValue = float.Parse(stringToEdit); // TODO handle Exception from Bad Input
        }catch(FormatException ex){ }

        if (GUI.Button(new Rect(nodeRect.x + nodeRect.width - 10f, nodeRect.y + nodeRect.height * 0.5f - 10f, 20f, 20f), "", guiSkin.GetStyle("node_output")))
        {
            if (parentGraph != null)
            {
                parentGraph.wantsConnection = true;
                parentGraph.connectionNode = this;
            }
        }
    }

    public override void DrawNodeProperties(Rect viewRect, GUISkin guiSkin)
    {
        nodeValue = EditorGUILayout.FloatField("Float value", nodeValue, guiSkin.GetStyle("property_view"));
        base.DrawNodeProperties(viewRect, guiSkin);
    }
}
