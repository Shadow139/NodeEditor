using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

[Serializable]
public class NodeGraph : ScriptableObject
{
    public string graphName = "New Graph";
    public Vector2 mousePos;
    public List<NodeBase> nodes;
    public List<NodeBase> selectedNodes;
    public NodeBase graphNode = null;

    public bool showProperties;
    public bool wantsConnection;
    public NodeBase connectionNode;
    public List<NodeOutput> connectionOutputList;
    public NodeOutput connectionOutput;

    public List<Rect> graphInputRects = new List<Rect>();
    int curveIndex = 0;

    public bool isInsidePropertyView;
    public float zoom;
    public float panX;
    public float panY;

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

                    if (node.timePointer.arrowRect.Contains(e.mousePosition))
                    {
                        node.timePointer.isSelected = true;
                        node.timePointer.isMoveable = true;
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

            if (e.keyCode == KeyCode.Space && e.type == EventType.KeyUp)
            {
                if(selectedNodes != null)
                {
                    if(selectedNodes.Count > 0)
                        printGraph(selectedNodes[0]);
                }
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
        nodes.ForEach(node => node.timePointer.isSelected = false);
        nodes.ForEach(node => node.timePointer.isMoveable = false);
        selectedNodes.Clear();
        showProperties = false;
        wantsConnection = false;
        connectionNode = null;
        connectionOutputList = null;
        connectionOutput = null;
        curveIndex = -1;
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

        if (wantsConnection && (connectionNode != null || connectionOutputList != null))
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

        DrawNodeToOutputCurve(viewRect);

        EditorUtility.SetDirty(this);
    }

    private void DrawConnectionToMouse(Vector2 mousePosition)
    {
        NodeOutput temp = null;

        if(connectionNode == null)
        {
            temp = connectionOutputList[0];
        }
        else if(connectionOutputList == null)
        {
            temp = connectionOutput;
        }

        if(temp != null)
        {
            if (this != temp.outputNode.parentGraph)
            {
                //Draw Curve from Leftside Inputs to Mouse
                if (graphInputRects.Count > curveIndex && curveIndex != -1)
                {
                    DrawUtilities.DrawCurve(new Vector3(graphInputRects[curveIndex].x + graphInputRects[curveIndex].width, 
                                            graphInputRects[curveIndex].center.y, 0f), mousePosition, Color.red, 2f);
                }
                else
                {
                    if(connectionNode != null || connectionOutput != null && connectionOutputList == null)
                    {
                        if (connectionOutput.outputNode.parentGraph.graphNode.multiOutput)
                        {
                            DrawUtilities.DrawMouseCurve(connectionOutput.outputNode.parentGraph.graphNode.getMultiOutputRect(), mousePosition, connectionOutputList.Count + 2f);
                        }
                        else
                        {
                            //Draw out of the GroupNode to Mouse
                            DrawUtilities.DrawMouseCurve(connectionOutput.outputNode.parentGraph.graphNode.nodeOutputs[connectionOutput.position].rect, mousePosition, 2f);
                        }
                    }
                    else if (connectionOutputList != null)
                    {
                        if (connectionOutputList[0].outputNode.parentGraph.graphNode.multiOutput)
                        {
                            DrawUtilities.DrawMouseCurve(connectionOutputList[0].outputNode.parentGraph.graphNode.getMultiOutputRect(), mousePosition, connectionOutputList.Count + 2f);
                        }
                        else
                        {
                            DrawUtilities.DrawMouseCurve(connectionOutputList[0].outputNode.nodeRect, mousePosition, connectionOutputList.Count + 2);
                        }
                    }
                }
            }
            else
            {
                if(connectionNode != null || connectionOutput != null && connectionOutputList == null)
                {
                    if (connectionOutput.outputNode.multiOutput)
                    {
                        DrawUtilities.DrawMouseCurve(connectionOutput.outputNode.getMultiOutputRect(), mousePosition, 2f);
                    }
                    else
                    {
                        DrawUtilities.DrawMouseCurve(connectionOutput.rect, mousePosition, 2f);
                    }
                }
                else if (connectionOutputList != null)
                {
                    if (connectionOutputList[0].outputNode.multiOutput)
                    {
                        DrawUtilities.DrawMouseCurve(connectionOutputList[0].outputNode.getMultiOutputRect(), mousePosition, 2f);
                    }
                    else
                    {
                        DrawUtilities.DrawMouseCurve(connectionOutputList[0].outputNode.nodeRect, mousePosition, connectionOutputList.Count + 2);
                    }
                }
            }
        }
    }

    //Draws Curve from a Node to the right Output Handles
    public void DrawNodeToOutputCurve(Rect viewRect)
    {
        if (graphNode != null)
        {
            for (int i = 0; i < graphNode.nodeOutputs.Count; i++)
            {
                if (graphNode.nodeOutputs[i].outputNode!= null)
                {
                    Vector3 outputCurvePoint = new Vector3(graphNode.nodeOutputs[i].outputNode.nodeOutputs[0].rect.x + graphNode.nodeOutputs[i].outputNode.nodeOutputs[0].rect.width * 0.5f,
                                                           graphNode.nodeOutputs[i].outputNode.nodeOutputs[0].rect.y + graphNode.nodeOutputs[i].outputNode.nodeOutputs[0].rect.height * 0.5f);
                    Vector3 inputCurvePoint = new Vector3(viewRect.x + viewRect.width - 32f, viewRect.y + (viewRect.height * 0.5f) + 60f);
                    DrawUtilities.DrawCurve(outputCurvePoint, inputCurvePoint , Color.black, 2f);
                }
            }
        }
    }
    //Leftside Green
    public void DrawNodeGraphInputs(Rect viewRect, GUISkin guiSkin)
    {
        if(graphNode != null)
        {
            graphInputRects.Clear();
            for (int i = 0; i < graphNode.nodeInputs.Count; i++)
            {
                graphInputRects.Add(new Rect(viewRect.x, viewRect.y + (viewRect.height * (1f / (graphNode.nodeInputs.Count + 1))) * (i + 1), 32f, 64f));
                NodeInput input = graphNode.nodeInputs[i];
                if (GUI.Button(new Rect(viewRect.x, viewRect.y + (viewRect.height * (1f / (graphNode.nodeInputs.Count + 1))) * (i + 1), 32f, 64f), "", guiSkin.GetStyle("node_multiOutput")))
                {
                    curveIndex = i;
                    if (graphNode.parentGraph != null)
                    {
                        wantsConnection = true;
                        connectionNode = input.inputNode;
                        connectionOutput = input.inputNode.nodeOutputs[input.outputPos];
                    }
                }
            }
        }
    }
    //Rightside Red
    public void DrawNodeGraphOutputs(Rect viewRect, GUISkin guiSkin)
    {
        if (graphNode != null)
        {
            if (GUI.Button(new Rect(viewRect.x + viewRect.width - 32f, viewRect.y + (viewRect.height * 0.5f), 32f, 120f), "", guiSkin.GetStyle("node_multiInput")))
            {
                if (wantsConnection)
                {
                    for (int k = 0; k < graphNode.nodeOutputs.Count; k++)
                    {
                        if (graphNode.nodeOutputs[k].outputNode == null && graphNode.nodeOutputs[k].isOccupied == false)
                        {
                            graphNode.nodeOutputs[k].outputNode = connectionNode;
                            graphNode.nodeOutputs[k].isOccupied = graphNode.nodeOutputs[k].outputNode != null;
                            //graphNode.nodeOutputs[k].position = k;

                            wantsConnection = false;
                            connectionNode = null;
                            Debug.Log("Connected to GroupNode Output at: " + k);

                            return;
                        }
                    }

                    if (graphNode.numberOfOutputs < graphNode.nodeOutputsMax)
                    {
                        int i = graphNode.nodeOutputs.Count;
                        graphNode.numberOfOutputs = i + 1;
                        graphNode.nodeOutputs.Add(new NodeOutput());
                        graphNode.nodeOutputs[i].outputNode = connectionNode;
                        graphNode.nodeOutputs[i].isOccupied = graphNode.nodeOutputs[i].outputNode != null;
                        Debug.Log("Increased Output of GroupNode and Connected");
                    }
                    
                    wantsConnection = false;
                    connectionNode = null;
                }
                else
                {
                    Debug.Log("Removing Group Node Outputs");
                    graphNode.nodeOutputs = new List<NodeOutput>();
                    graphNode.numberOfOutputs = graphNode.nodeOutputs.Count;
                }
            }
            GUI.Label(new Rect(viewRect.x + viewRect.width - 32f, viewRect.y + (viewRect.height * 0.5f), 32f, 120f), graphNode.nodeOutputs.Count + "", guiSkin.GetStyle("std_whiteText"));

        }
    }

    public void printGraph(NodeBase node)
    {      
        Debug.Log(node.nodeName + " -> " + node.parameters["value"].floatParam);
        foreach(NodeOutput child in node.nodeOutputs)
        {
            NodeBase nextNode = child.connectedToNode;
            if(nextNode != null)
                printGraph(nextNode); //<-- recursive
        }
    }
}

