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
        if(selectedNodes == null)
        {
            selectedNodes = new List<NodeBase>();
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
                            if (!node.isSelected)
                            {
                                selectedNodes.Add(node);
                                selectedNode = node;
                                selectedNode.isSelected = true;
                                break;
                            }
                            else
                            {
                                selectedNodes.Remove(node);
                                selectedNode = null;
                                node.isSelected = false;
                                break;
                            }
                        }
                        else
                        {
                            selectedNodes.Add(node);
                            selectedNode = node;
                            selectedNode.isSelected = true;
                            break;
                        }
                    }
                }
            }

            if(e.keyCode == KeyCode.Tab && e.type == EventType.KeyUp)
            {
                Debug.Log("align Nodes " + selectedNodes.Count);
                alignNodesHorizontally();
            }

            if (e.keyCode == KeyCode.LeftAlt && e.type == EventType.KeyUp)
            {
                Debug.Log("align Nodes " + selectedNodes.Count);
                alignNodesVertically();
            }
        }

    }

    private void alignNodesHorizontally()
    {
        float x = selectedNodes[selectedNodes.Count - 1].nodeRect.center.x;

        for(int i = 0; i < selectedNodes.Count; i++)
        {
            selectedNodes[i].nodeRect.center = new Vector2(x, selectedNodes[i].nodeRect.center.y);
        }
    }

    private void alignNodesVertically()
    {
        float y = selectedNodes[selectedNodes.Count - 1].nodeRect.y;

        float height = 20f;

        for (int i = 0; i < selectedNodes.Count - 1; i++)
        {
            height += selectedNodes[i].nodeRect.height;
            selectedNodes[i].nodeRect.position = new Vector2(selectedNodes[i].nodeRect.position.x, y + (height + (20f * i)));
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
