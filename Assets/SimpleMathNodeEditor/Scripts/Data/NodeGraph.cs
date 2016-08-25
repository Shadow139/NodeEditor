using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

[Serializable]
public class NodeGraph : ScriptableObject
{
    public string graphName = "New Graph";
    public List<NodeBase> nodes;
    public List<NodeBase> selectedNodes;
    public NodeBase graphNode = null;
    public bool showProperties;
    public bool wantsConnection;
    public NodeBase connectionNode;
    public Vector2 mousePos;
    public bool isInsidePropertyView;


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

    private void ProcessEvents(Event e, Rect viewRect, Rect workViewRect)
    {
        if (workViewRect.Contains(e.mousePosition))
        {
            if (e.button == 0 && e.type == EventType.MouseDown && !isInsidePropertyView)
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
                                node.isSelected = true;
                                break;
                            }
                            else
                            {
                                selectedNodes.Remove(node);
                                node.isSelected = false;
                                break;
                            }
                        }
                        else
                        {
                            selectedNodes.Add(node);
                            node.isSelected = true;
                            break;
                        }
                    }
                }
            }

            if(e.control && e.keyCode == KeyCode.A)
            {
                selectedNodes.Clear();
                foreach (NodeBase node in nodes)
                {
                    selectedNodes.Add(node);
                    node.isSelected = true;
                }
            }

            if(e.keyCode == KeyCode.Tab && e.type == EventType.KeyUp)
            {
                Debug.Log("align Nodes Horizontally " + selectedNodes.Count);
                alignNodesHorizontally();
            }

            if (e.keyCode == KeyCode.LeftAlt && e.type == EventType.KeyUp)
            {
                Debug.Log("align Nodes Vertically " + selectedNodes.Count);
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
        wantsConnection = false;
        connectionNode = null;
    }

    public void UpdateGraphGUI(Event e, Rect viewRect, Rect workViewRect, GUISkin guiSkin)
    {
        mousePos = e.mousePosition;

        DrawNodeGraphInputs(viewRect, guiSkin);
        DrawNodeGraphOutputs(viewRect, guiSkin);

        if (nodes.Count > 0)
        {
            ProcessEvents(e, viewRect, workViewRect);

            foreach (NodeBase node in nodes)
            {
                node.UpdateNodeGUI(e, viewRect, workViewRect, guiSkin);
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

        if (e.type == EventType.Layout && selectedNodes.Count > 0)
        {
            showProperties = true;
        }

        EditorUtility.SetDirty(this);
    }

    private void DrawConnectionToMouse(Vector2 mousePosition)
    {
        DrawUtilities.DrawMouseCurve(connectionNode.nodeRect, mousePosition);
    }
    
    public void DrawNodeGraphInputs(Rect viewRect, GUISkin guiSkin)
    {
        if(graphNode != null)
        {
            for(int i = 0; i < graphNode.nodeInputs.Count; i++)
            {
                NodeInput input = graphNode.nodeInputs[i];
                if (GUI.Button(new Rect(viewRect.x, viewRect.y + (viewRect.height * (1f / (graphNode.nodeInputs.Count + 1))) * (i + 1), 32f, 64f), "", guiSkin.GetStyle("node_multiOutput")))
                {
                    if (graphNode.parentGraph != null)
                    {
                        wantsConnection = true;
                        connectionNode = input.inputNode;
                    }
                }
            }
        }
    }

    public void DrawNodeGraphOutputs(Rect viewRect, GUISkin guiSkin)
    {
        if (graphNode != null)
        {
            if (GUI.Button(new Rect(viewRect.x + viewRect.width - 32f, viewRect.y + (viewRect.height * 0.5f), 32f, 120f), "", guiSkin.GetStyle("node_multiInput")))
            {
                int i = graphNode.nodeOutputs.Count;
                graphNode.nodeOutputs.Add(new NodeOutput());

                graphNode.nodeOutputs[i].outputNode = connectionNode;
                graphNode.nodeOutputs[i].isOccupied = graphNode.nodeOutputs[i].outputNode != null;

                wantsConnection = false;
                connectionNode = null;
            }
        }
    }
}
