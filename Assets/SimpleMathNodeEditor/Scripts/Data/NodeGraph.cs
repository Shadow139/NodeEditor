using UnityEngine;
using UnityEditor;
using System.Linq;
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
    public static bool evaluateTrigger = false;

    public Rect graphNodeRect;
    public List<Rect> graphInputRects = new List<Rect>();
    int curveIndex = 0;

    public bool isInsidePropertyView;
    public float zoom;
    public float panX;
    public float panY;

    //Box select
    private Vector2 boxSelectPos;
    private Rect selectionRect;
    float width = 0;
    float height = 0;
    public bool selectionFlag = false;

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
                boxSelectPos = e.mousePosition;
                width = 0;
                height = 0;

                if (!wantsConnection && !e.shift)
                {
                    DeselectAllNodes();
                }

                foreach (NodeBase node in nodes)
                {
                    if (node.nodeRect.Contains(e.mousePosition))
                    {
                        selectionFlag = true;
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
                                if(e.type == EventType.MouseUp)
                                {
                                    selectedNodes.Remove(node);
                                    node.isSelected = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            selectedNodes.Add(node);
                            node.isSelected = true;
                            break;
                        }
                    }
                    else
                    {
                        selectionFlag = false;
                    }

                    if (node.timePointer.arrowRect.Contains(e.mousePosition))
                    {
                        node.timePointer.isSelected = true;
                        node.timePointer.isMoveable = true;
                    }
                }
            }

            if (!selectionFlag && !e.shift && e.button == 0 && e.type == EventType.MouseDrag)
            {
                ConstructSelectionBox(e);
            }

            if(e.button == 0 && e.type == EventType.MouseUp)
            {
                foreach (NodeBase node in nodes)
                {
                    if (selectionRect.Overlaps(node.nodeRect))
                    {
                        selectedNodes.Add(node);
                        node.isSelected = true;
                    }
                }

                selectionRect = new Rect();
                boxSelectPos = new Vector2(0, 0);
                width = 0;
                height = 0;

            }

            if (e.control && e.keyCode == KeyCode.A)
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
                alignNodesHorizontally();
            }

            if (e.control && e.keyCode == KeyCode.Tab && e.type == EventType.KeyUp)
            {
                adjustAnimationLength();
            }

            if (e.keyCode == KeyCode.Space && e.type == EventType.KeyUp)
            {
                if(selectedNodes != null)
                {
                    if (selectedNodes.Count > 0)
                        evaluateNodes(selectedNodes[0], null, 0);
                    //printGraph(selectedNodes[0]);
                }
            }

            if (e.keyCode == KeyCode.K && e.type == EventType.KeyUp)
            {
                if (selectedNodes != null)
                {
                    foreach(NodeBase n in selectedNodes)
                    {
                        n.printKeys();
                    }
                }
            }
        }
    }

    private void alignNodesHorizontally()
    {
        float x = selectedNodes[selectedNodes.Count - 1].timePointer.arrowRect.x;

        for(int i = 0; i < selectedNodes.Count; i++)
        {
            selectedNodes[i].timePointer.arrowRect.x = x;
        }
    }
    
    private void adjustAnimationLength()
    {
        float startOffset = selectedNodes[selectedNodes.Count - 1].timePointer.startAnimOffset;
        float endOffset = selectedNodes[selectedNodes.Count - 1].timePointer.endAnimOffset;

        for (int i = 0; i < selectedNodes.Count; i++)
        {
            selectedNodes[i].timePointer.startAnimOffset = startOffset;
            selectedNodes[i].timePointer.endAnimOffset = endOffset;
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

        if(evaluateTrigger)
        {
            evaluateTrigger = false;
        }

        if (e.type == EventType.Layout && selectedNodes.Count > 0)
        {
            showProperties = true;
        }

        DrawNodeToOutputCurve(viewRect);
        drawSelectionBox();

        EditorUtility.SetDirty(this);
    }

    private void DrawConnectionToMouse(Vector2 mousePosition)
    {
        NodeOutput temp = null;

        if(connectionNode == null && connectionOutputList !=null )
        {
            if(connectionOutputList.Count  > 0)
                temp = connectionOutputList[0];
        }
        else if(connectionOutputList == null && connectionNode != null)
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
                    Vector3 inputCurvePoint = new Vector3(graphNodeRect.x, graphNodeRect .y + 60f);
                    DrawUtilities.DrawCurve(outputCurvePoint, inputCurvePoint , WorkPreferences.nodeCurveColor, WorkPreferences.nodeCurveThickness);
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
                    if (graphNode.parentGraph != null && input != null)
                    {
                        if(input.inputNode != null)
                        {

                            wantsConnection = true;
                            connectionNode = input.inputNode;
                            connectionOutput = input.inputNode.nodeOutputs[input.outputPos];
                        }
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
            NodeBase mostOuterNode = getMostOuterNode();
            graphNodeRect = new Rect(viewRect.x + viewRect.width - 32f, viewRect.y + (viewRect.height * 0.5f) - panY, 32f, 120f);
            graphNodeRect.x = graphNodeRect.x / zoom;
            graphNodeRect.x = graphNodeRect.x - panX;
            graphNodeRect.width = graphNodeRect.width / zoom;

            if(mostOuterNode != null)
            {
                if (graphNodeRect.x < (mostOuterNode.nodeRect.x + mostOuterNode.nodeRect.width + 100))
                    graphNodeRect.x = mostOuterNode.nodeRect.x + mostOuterNode.nodeRect.width + 100;
            }

            if (GUI.Button(graphNodeRect, "", guiSkin.GetStyle("node_multiInput")))
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
            GUI.Label(graphNodeRect, graphNode.nodeOutputs.Count + "", guiSkin.GetStyle("std_whiteText"));

        }
    }

    public NodeBase getFirstAnimatedNode()
    {
        NodeBase tmp = null;
        float min = float.MaxValue;
        foreach(NodeBase n in nodes)
        {
            if ((n.timePointer.x + n.timePointer.startAnimOffset) < min)
            {
                min = n.timePointer.x + n.timePointer.startAnimOffset;
                tmp = n;
            }
        }
        return tmp;
    }

    public NodeBase getLastAnimatedNode()
    {
        NodeBase tmp = null;
        float max = 0f;
        foreach (NodeBase n in nodes)
        {
            if ((n.timePointer.x + n.timePointer.endAnimOffset) > max)
            {
                max = n.timePointer.x + n.timePointer.endAnimOffset;
                tmp = n;
            }
        }
        return tmp;
    }

    private NodeBase getMostOuterNode()
    {
        NodeBase tmp = null;
        float max = 0;
        foreach (NodeBase n in nodes)
        {
            if (n.nodeRect.x > max)
            {
                max = n.nodeRect.x;
                tmp = n;
            }
        }
        return tmp;
    }

    private void ConstructSelectionBox(Event e)
    {
        width += e.delta.x;
        height += e.delta.y;

        selectionRect = new Rect(boxSelectPos, new Vector2(width, height));
    }

    private void drawSelectionBox()
    {
        Texture2D backgroundTexture = Texture2D.whiteTexture;
        GUIStyle textureStyle = new GUIStyle { normal = new GUIStyleState { background = backgroundTexture } };
        Color c = new Color(Color.white.r, Color.white.g, Color.white.b, 0.2f);

        var backgroundColor = GUI.backgroundColor;
        GUI.backgroundColor = c;
        GUI.Box(selectionRect, GUIContent.none, textureStyle);
        GUI.backgroundColor = backgroundColor;
    }

    private void evaluateNodes(NodeBase startNode, NodeBase lastRecursionNode, int level)
    {
        if(level < 8)
        {
            foreach (NodeInput parent in startNode.nodeInputs)
            {
                NodeBase previousNode = parent.inputNode;
                if (previousNode != null && previousNode != lastRecursionNode)
                    evaluateNodes(previousNode, lastRecursionNode, level + 1);
            }

            Debug.Log(startNode.nodeName + startNode.parameters["value"].floatParam);

            foreach (NodeOutput child in startNode.nodeOutputs)
            {
                NodeBase nextNode = child.connectedToNode;
                if (nextNode != null && nextNode != lastRecursionNode)
                    evaluateNodes(nextNode, startNode, level + 1);
            }
        }
    }

    public void printGraph(NodeBase startNode)
    {      
        Debug.Log(startNode.ToString());
        foreach(NodeOutput child in startNode.nodeOutputs)
        {
            NodeBase nextNode = child.connectedToNode;
            if(nextNode != null)
                printGraph(nextNode); //<-- recursive
        }
    }
}

