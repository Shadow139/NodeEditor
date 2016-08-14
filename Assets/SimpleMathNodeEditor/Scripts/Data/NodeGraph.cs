using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;
using System.Collections.Generic;


[Serializable]
public class NodeGraph : ScriptableObject
{
    public string graphName = "New Graph";
    public List<NodeBase> nodes;
    public List<NodeBase> selectedNodes;
    public NodeBase selectedNode;
    public bool showProperties;
    public bool wantsConnection;
    public NodeBase connectionNode;

    void OnEnable()
    {
        if(nodes == null)
        {
            nodes = new List<NodeBase>();
        }
    }

    public void InitGraph()
    {
        if (nodes.Count > 0)
        {
            foreach (NodeBase node in nodes)
            {
                node.InitNode();
            }
        }
    }

    private void ProcessEvents(Event e, Rect viewRect)
    {
        if (viewRect.Contains(e.mousePosition))
        {
            if (e.button == 0 && e.type == EventType.MouseDown)
            {
                if (!wantsConnection && !e.shift)
                {
                    DeselectAllNodes();
                }

                foreach (NodeBase node in nodes)
                {
                    if (node.nodeRect.Contains(e.mousePosition))
                    {
                        if (e.shift)
                        {
                            selectedNode = node;
                            selectedNode.isSelected = true;
                            break;
                        }
                        else
                        {
                            selectedNode = node;
                            selectedNode.isSelected = true;
                            break;
                        }
                    }
                }
            }

        }

    }

    private void DeselectAllNodes()
    {
        nodes.ForEach(node => node.isSelected = false);
        selectedNodes.Clear();
        showProperties = false;
        selectedNode = null;
        wantsConnection = false;
        connectionNode = null;
    }

#if UNITY_EDITOR
    public void UpdateGraphGUI(Event e, Rect viewRect, GUISkin guiSkin)
    {
        //DebugUtilities.drawWindowOutline(viewRect, Color.green);
        //Debug.Log("GraphView Position: " + viewRect.ToString());

        if (nodes.Count > 0)
        {
            ProcessEvents(e, viewRect);

            foreach (NodeBase node in nodes)
            {
                node.UpdateNodeGUI(e, viewRect, guiSkin);
            }
        }

        if (wantsConnection && connectionNode != null)
        {
            DrawConnectionToMouse(e.mousePosition);

            if (e.button == 0) // Right Mouseclick
            {
                if (e.type == EventType.MouseDown)
                {
                    Debug.Log("Connecting was interrupted");
                    wantsConnection = false;
                }
            }
       }

        if (e.type == EventType.Layout && selectedNode != null)
        {
            showProperties = true;
        }

        EditorUtility.SetDirty(this);
    }

    private void DrawConnectionToMouse(Vector2 mousePosition)
    {
        DrawUtilities.DrawMouseCurve(connectionNode.nodeRect, mousePosition);
    }
#endif
}
