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
    public bool multiInput;

    public GraphNode()
    {
    }

    public override void InitNode()
    {
        base.InitNode();
        nodeType = NodeType.Graph;
        nodeRect = new Rect(10f, 10f, 150f, 100f);
    }

    public override void UpdateNode(Event e, Rect viewRect)
    {
        base.UpdateNode(e, viewRect);
    }

    public override void evaluateNode()
    {

    }

    public void drawInputHandles(GUISkin guiSkin)
    {
        if (multiInput)
        {
            if (GUI.Button(new Rect(nodeRect.x - 20f, nodeRect.y + (nodeRect.height * 0.5f) - 25f, 20f, 50f), "", guiSkin.GetStyle("node_multiInput")))
            {
                for (int i = 0; i < nodeInputs.Count; i++)
                {
                    if (parentGraph != null)
                    {
                        nodeInputs[i].inputNode = parentGraph.connectionNode;
                        nodeInputs[i].isOccupied = nodeInputs[i].inputNode != null;

                        parentGraph.wantsConnection = false;
                        parentGraph.connectionNode = null;
                        Debug.Log("Connected");
                    }
                }
            }
        }
        else
        {
            //Input
            for (int i = 0; i < nodeInputs.Count; i++)
            {
                if (GUI.Button(new Rect(nodeRect.x - 12f, nodeRect.y + (nodeRect.height * 0.33f) * (i + 1) - 12f, 24f, 24f), "", guiSkin.GetStyle("node_input")))
                {
                    if (parentGraph != null)
                    {
                        nodeInputs[i].inputNode = parentGraph.connectionNode;
                        nodeInputs[i].isOccupied = nodeInputs[i].inputNode != null;

                        parentGraph.wantsConnection = false;
                        parentGraph.connectionNode = null;
                        Debug.Log("Connected");
                    }
                }
            }
        }
    }

#if UNITY_EDITOR
    public override void UpdateNodeGUI(Event e, Rect viewRect, GUISkin guiSkin)
    {
        base.UpdateNodeGUI(e, viewRect, guiSkin);
    }

    public override void DrawNodeProperties(Rect viewRect, GUISkin guiSkin)
    {
        base.DrawNodeProperties(viewRect, guiSkin);

    }
#endif

}

