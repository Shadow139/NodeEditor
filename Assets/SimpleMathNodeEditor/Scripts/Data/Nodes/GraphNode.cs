using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class GraphNode : NodeBase
{
    public GraphNode() { }

    public override void InitNode()
    {
        base.InitNode();
        nodeType = NodeType.Graph;
        nodeRect = new Rect(10f, 10f, 150f, 100f);
        multiInput = true;
    }

    public override void evaluateNode()
    {

    }     

    public override void UpdateNodeGUI(Event e, Rect viewRect, Rect workViewRect, GUISkin guiSkin)
    {                
        base.UpdateNodeGUI(e, viewRect, workViewRect, guiSkin);
    }

    public override void DrawNodeProperties(Rect viewRect, GUISkin guiSkin)
    {
        base.DrawNodeProperties(viewRect, guiSkin);        
    }
}

