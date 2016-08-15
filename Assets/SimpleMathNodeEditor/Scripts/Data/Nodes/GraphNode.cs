using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;
using System.Collections.Generic;

public class GraphNode : NodeBase
{
    public NodeGraph nodeGraph;

    public GraphNode()
    {
    }

    public override void InitNode()
    {
        base.InitNode();
        nodeType = NodeType.Graph;
        nodeRect = new Rect(10f, 10f, 150f, 100f);
        multiInput = true;
    }

    public override void UpdateNode(Event e, Rect viewRect)
    {
        base.UpdateNode(e, viewRect);
    }

    public override void evaluateNode()
    {

    }     

#if UNITY_EDITOR
    public override void UpdateNodeGUI(Event e, Rect viewRect, GUISkin guiSkin)
    {                
        base.UpdateNodeGUI(e, viewRect, guiSkin);
        drawInputHandles(guiSkin);

        DrawInputLines();

        if (multiInput)
            GUI.Label(new Rect(nodeRect.x - 12f, nodeRect.y + nodeRect.height * 0.5f - 10f, nodeRect.width * 0.2f - 10f, 20f), nodeInputs.Count + "", guiSkin.GetStyle("std_whiteText"));
        
    }

    public override void DrawNodeProperties(Rect viewRect, GUISkin guiSkin)
    {
        base.DrawNodeProperties(viewRect, guiSkin);
        
    }
#endif

}

