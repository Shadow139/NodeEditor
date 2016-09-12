using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class NodeBase : ScriptableObject
{
    #region Variables
    public string nodeName;
    public NodeType nodeType;
    public List<NodeInput> nodeInputs = new List<NodeInput>();
    public int nodeInputsMin;
    public int nodeInputsMax;
    public List<NodeOutput> nodeOutputs = new List<NodeOutput>();
    public int nodeOutputsMin;
    public int nodeOutputsMax;
    public ParameterDictionary parameters;
    public string titleBarColor;

    public Rect nodeRect;
    public Rect viewPortRect;
    public NodeGraph parentGraph;
    public bool isSelected { get; set; }
    public TimePointer timePointer;
    public DragButton dragButton;

    public bool multiInput = false;
    public bool multiOutput = false;
    public int numberOfInputs;
    public int numberOfOutputs;
    public static float snapSize = 10;

    public NodeGraph nodeGraph;
    public AnimationCurve curve = AnimationCurve.Linear(0,0,1,1);

    public GUISkin nodeSkin { get; set; }
    #endregion

    public virtual void InitNode() { }

    public void InitNodeFromDescriptor(NodeDescriptor descriptor)
    {
        nodeType = descriptor.nodeType;
        nodeName = descriptor.nodeName;
        titleBarColor = descriptor.titleBarColor;

        timePointer = new TimePointer();
        timePointer.InitTimePointer();
        timePointer.parentNode = this;

        dragButton = new DragButton();
        dragButton.InitDragButton();
        
        parameters = new ParameterDictionary();
        foreach (KeyValuePair<string, NodeParameter> pair in descriptor.parameters)
        {
            NodeParameter tempParameter = new NodeParameter();
            tempParameter.floatParam = pair.Value.floatParam;
            tempParameter.stringParam = pair.Value.stringParam;
            tempParameter.vectorParam = new Vector3(pair.Value.vectorParam.x, pair.Value.vectorParam.y, pair.Value.vectorParam.z);

            parameters.Add(pair.Key, tempParameter);
        }

        for(int i = 0; i < descriptor.numberOfInputs; i++)
        {
            nodeInputs.Add(new NodeInput());
        }
        numberOfInputs = descriptor.numberOfInputs;
        nodeInputsMin = descriptor.minInputs;
        nodeInputsMax = descriptor.maxInputs;
        for (int i = 0; i < descriptor.numberOfOutputs; i++)
        {
            nodeOutputs.Add(new NodeOutput());
            if(nodeType != NodeType.Graph)
                nodeOutputs[i].outputNode = this;
        }
        numberOfOutputs = descriptor.numberOfOutputs;
        nodeOutputsMin = descriptor.minOutputs;
        nodeOutputsMax = descriptor.maxOutputs;

        multiInput = descriptor.isMultiInput;
        multiOutput = descriptor.isMultiOutput;

        nodeRect = new Rect(10f, 10f, 150f, 100f);
    }

    public virtual void evaluateNode()
    {
        EvaluateNodeByType();
    }

    private void ProcessEvents(Event e, Rect viewRect, Rect workViewRect)
    {
        if (isSelected)
        {
            if (workViewRect.Contains(e.mousePosition) && !parentGraph.isInsidePropertyView)
            {
                if (e.button == 0 && e.type == EventType.MouseDrag)
                {
                    Rect rect = nodeRect;

                    rect.x += e.delta.x;
                    rect.y += e.delta.y;

                    rect.position = snap(rect.position, snapSize);

                    nodeRect = rect;
                }

                if (e.shift && dragButton.middleButtonRect.Contains(e.mousePosition) && !timePointer.resizeEndOffset && !timePointer.resizeStartOffset)
                {
                    if (e.button == 0 && e.type == EventType.MouseDown)
                    {
                        timePointer.isMoveable = true;
                        timePointer.isSelected = true;

                        foreach (NodeBase n in parentGraph.selectedNodes)
                        {
                            n.timePointer.isMoveable = true;
                            n.timePointer.isSelected = true;
                        }
                    }
                }
            }
        }

        if (!isSelected)
        {
            if (dragButton.middleButtonRect.Contains(e.mousePosition) && !timePointer.resizeEndOffset && !timePointer.resizeStartOffset)
            {
                if (e.button == 0 && e.type == EventType.MouseDown)
                {
                    if (e.shift)
                        isSelected = true;

                    timePointer.isMoveable = true;
                    timePointer.isSelected = true;
                    parentGraph.selectedNodes.Add(this);
                }
            }

            if (dragButton.leftButtonRect.Contains(e.mousePosition) && !timePointer.resizeEndOffset && !timePointer.isMoveable)
            {
                if (e.button == 0 && e.type == EventType.MouseDown)
                {
                    timePointer.resizeStartOffset = true;
                    timePointer.isSelected = true;
                    parentGraph.selectedNodes.Add(this);
                }
            }
            if (dragButton.rightButtonRect.Contains(e.mousePosition) && !timePointer.resizeStartOffset && !timePointer.isMoveable)
            {
                if (e.button == 0 && e.type == EventType.MouseDown)
                {
                    timePointer.resizeEndOffset = true;
                    timePointer.isSelected = true;
                    parentGraph.selectedNodes.Add(this);
                }
            }
        }

        if (nodeRect.Contains(e.mousePosition))
        {
            timePointer.isHighlighted = true;
        }
        else
        {
            timePointer.isHighlighted = false;
        }
        
        if (e.button == 0 && e.type == EventType.MouseUp)
        {
            timePointer.isMoveable = false;
            timePointer.resizeStartOffset = false;
            timePointer.resizeEndOffset = false;
        }
    }

    public virtual void UpdateNodeGUI(Event e, Rect viewRect, Rect workViewRect, GUISkin guiSkin)
    {
        ProcessEvents(e, viewRect, workViewRect);
        evaluateNode();
        viewPortRect = viewRect;

        string currentStyle = isSelected ? "node_selected" : "node_default";
        if (timePointer.isSelected)
            currentStyle = "node_selected";
        GUI.Box(nodeRect, nodeName, guiSkin.GetStyle(currentStyle));
        GUI.Box(new Rect(nodeRect.x, nodeRect.y, nodeRect.width, 27f), nodeName, guiSkin.GetStyle((currentStyle + "_titlebar_" + titleBarColor)));
        GUI.Box(new Rect(nodeRect.x, nodeRect.y + nodeRect.height - 27f, nodeRect.width, 27f), "", guiSkin.GetStyle(currentStyle));

        DrawCurrentTimePosition();      

        if (timePointer != null)
            timePointer.drawArrow(e, viewRect, workViewRect, guiSkin);

        resizeNodeBox();

        drawOutputHandles(guiSkin);
        drawInputHandles(guiSkin);

        DrawInputLines();

        DrawNodeBoxInsideByType(viewRect);

        dragButton.DrawDragButton(e, nodeRect, guiSkin);

        EditorUtility.SetDirty(this);
    }

    public virtual void DrawNodeProperties(Rect viewRect, GUISkin guiSkin)
    {
        DrawNodePropertiesByType(viewRect, guiSkin);
        GUILayout.Space(15);

        EditorGUILayout.LabelField("Star of Animation:", getEndAnimTime());
        GUILayout.Space(6);
        EditorGUILayout.LabelField("End of Animation :", getStartAnimTime());


        GUILayout.Space(10);
        multiInput = EditorGUILayout.Toggle("Multi Input", multiInput);
        GUILayout.Space(6);
        multiOutput = EditorGUILayout.Toggle("Multi Output", multiOutput);
        GUILayout.Space(6);

        if (nodeInputsMin != nodeInputsMax)
        {
            GUILayout.BeginVertical();
            GUILayout.Space(6);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Inputs: ", GUILayout.MaxWidth(75));
            numberOfInputs = EditorGUILayout.IntSlider(numberOfInputs, nodeInputsMin, nodeInputsMax, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        if (nodeOutputsMin != nodeOutputsMax)
        {
            GUILayout.BeginVertical();
            GUILayout.Space(6);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Outputs: ", GUILayout.MaxWidth(75));
            numberOfOutputs = EditorGUILayout.IntSlider(numberOfOutputs, nodeOutputsMin, nodeOutputsMax, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        EditorGUI.CurveField(new Rect(10, 350, viewRect.width - 20, 250), curve, Color.green, new Rect(0,0,1,1));

        if (!multiInput)
            resizeInputHandles(numberOfInputs);        

        if(!multiOutput)
            resizeOutputHandles(numberOfOutputs);
    }

    #region Node Drawing Helper Methods

    #region Property Drawing Helpers
    private void DrawNodePropertiesByType(Rect viewRect, GUISkin guiSkin)
    {
        if(nodeType == NodeType.Float)
        {
            DrawFloatNodeProperties(guiSkin);
        }
        else if(nodeType == NodeType.Addition)
        {
            DrawAdditionNodeProperties(guiSkin);
        }
    }

    private void DrawFloatNodeProperties(GUISkin guiSkin)
    {
        if(parameters != null)
        {
            parameters["value"].floatParam = EditorGUILayout.FloatField("Float value", parameters["value"].floatParam, guiSkin.GetStyle("property_view"));
        }
    }

    private void DrawAdditionNodeProperties(GUISkin guiSkin)
    {
        if (parameters != null)
        {
            EditorGUILayout.LabelField("Value :", parameters["value"].floatParam.ToString());
        }
    }
    #endregion

    #region Node BoxInside Drawing Helpers
    private void DrawNodeBoxInsideByType(Rect viewRect)
    {
        if (nodeType == NodeType.Float)
        {
            DrawFloatNodeInsides();
        }
        else if (nodeType == NodeType.Addition)
        {
            DrawAdditionNodeInsides();
        }
        else if (nodeType == NodeType.Graph)
        {
            DrawGraphNodeInsides();
        }
    }

    private void DrawFloatNodeInsides()
    {
        if (parameters != null)
            GUI.Label(new Rect(nodeRect.x + nodeRect.width * 0.5f - 10f, nodeRect.y + ((nodeRect.height) * 0.5f) - 10f, nodeRect.width * 0.5f - 10f, 20f), parameters["value"].floatParam.ToString(), GuiStyles._instance.whiteNodeLabel);
    }

    private void DrawAdditionNodeInsides()
    {
        if (parameters != null)
            GUI.Label(new Rect(nodeRect.x + nodeRect.width * 0.5f - 10f, nodeRect.y + ((nodeRect.height) * 0.5f) - 10f, nodeRect.width * 0.5f - 10f, 20f), parameters["value"].floatParam.ToString(), GuiStyles._instance.whiteNodeLabel);
    }

    private void DrawGraphNodeInsides()
    {
        if(nodeGraph != null)
            GUI.Label(new Rect(nodeRect.x + 10f, nodeRect.y + ((nodeRect.height) * 0.5f) - 10f, nodeRect.width - 10f, 20f), nodeGraph.graphName, GuiStyles._instance.whiteNodeLabel);
        
    }
    #endregion

    #region Node Evaluation Helpers

    private void EvaluateNodeByType()
    {
        if (nodeType == NodeType.Addition)
        {
            EvaluateAdditionNode();
        }
    }

    private void EvaluateAdditionNode()
    {
        if (parameters != null)
        {
            float tempSum = 0;
            for (int i = 0; i < nodeInputs.Count; i++)
            {
                if (nodeInputs[i].isOccupied && nodeInputs[i].inputNode != null)
                {
                    if (nodeInputs[i].inputNode.nodeType == NodeType.Float)
                    {
                        tempSum += nodeInputs[i].inputNode.parameters["value"].floatParam;
                    }
                    else if (nodeInputs[i].inputNode.nodeType == NodeType.Addition)
                    {
                        tempSum += nodeInputs[i].inputNode.parameters["value"].floatParam;
                    }
                    else if (nodeInputs[i].inputNode.nodeType == NodeType.Graph)
                    {
                    
                    }
                }
            }
            parameters["value"].floatParam = tempSum;
        }
    }
    #endregion

    protected void DrawInputLines()
    {
        Handles.BeginGUI();

        Handles.color = Color.white;

        for (int i = 0; i < nodeInputs.Count; i++)
        {
            if (nodeInputs[i].inputNode != null && nodeInputs[i].isOccupied)
            {
                DrawNodeConnection(nodeInputs[i], (float)(i + 1));
            }
            else
            {
                nodeInputs[i] = new NodeInput();
            }
        }
        Handles.EndGUI();
    }

    protected void DrawNodeConnection(NodeInput currentInput, float inputId)
    {
        //Draws NodeCurves between nodes in the highest parentGraph and single Input Handles
        if (!currentInput.inputNode.multiOutput && !multiInput && currentInput.inputNode.parentGraph == parentGraph)
        {
            DrawUtilities.DrawNodeCurve(currentInput.getOutputPos(), currentInput.rect, WorkPreferences.nodeCurveColor, WorkPreferences.nodeCurveThickness);
        }
        else if (currentInput.inputNode.parentGraph == parentGraph && currentInput.inputNode.multiOutput && !multiInput)
        {
            DrawUtilities.DrawNodeCurve(currentInput.inputNode.getMultiOutputRect(), currentInput.rect, WorkPreferences.nodeCurveColor, WorkPreferences.nodeCurveThickness);
        }
        else if (currentInput.inputNode.parentGraph == parentGraph && currentInput.inputNode.multiOutput && multiInput)
        {
            DrawUtilities.DrawNodeCurve(currentInput.inputNode.getMultiOutputRect(), getMultiInputRect(), WorkPreferences.nodeCurveColor, WorkPreferences.nodeCurveThickness + currentInput.inputNode.nodeOutputs.Count);
        }
        else if(currentInput.inputNode.parentGraph != parentGraph)//Draws NodeCurves between the leftside Handles of a child Graph to the Nodes
        {
            if (parentGraph.graphNode != null)
            {
                int i = parentGraph.graphNode.nodeInputs.FindIndex(a => a.inputNode == currentInput.inputNode);

                if (i >= 0)
                {
                    Rect r = parentGraph.graphInputRects[i];
                    DrawUtilities.DrawNodeCurve(new Vector3(r.x + r.width, r.center.y, 0f), nodeRect, inputId, nodeInputs.Count);
                }
                else
                {
                    if(currentInput.inputNode.parentGraph.graphNode != null)
                    {
                        if (currentInput.inputNode.parentGraph.graphNode.multiOutput && !multiInput)
                        {
                            DrawUtilities.DrawNodeCurve(currentInput.inputNode.parentGraph.graphNode.getMultiOutputRect(), currentInput.rect, WorkPreferences.nodeCurveColor, WorkPreferences.nodeCurveThickness);
                        }
                        else if (currentInput.inputNode.parentGraph.graphNode.multiOutput && multiInput)
                        {
                            DrawUtilities.DrawNodeCurve(currentInput.inputNode.parentGraph.graphNode.getMultiOutputRect(), getMultiInputRect(), WorkPreferences.nodeCurveColor, WorkPreferences.nodeCurveThickness + currentInput.inputNode.nodeOutputs.Count);
                        }
                        else
                        {
                            DrawUtilities.DrawNodeCurve(currentInput.inputNode.parentGraph.graphNode.nodeOutputs[currentInput.outputPos].rect, currentInput.rect, WorkPreferences.nodeCurveColor, WorkPreferences.nodeCurveThickness);
                        }
                    }
                }
            }
            else
            {
                if (currentInput.inputNode.parentGraph.graphNode.multiOutput && !multiInput)
                {
                    DrawUtilities.DrawNodeCurve(currentInput.inputNode.parentGraph.graphNode.getMultiOutputRect(), currentInput.rect, WorkPreferences.nodeCurveColor, WorkPreferences.nodeCurveThickness);
                }
                else if (currentInput.inputNode.parentGraph.graphNode.multiOutput && multiInput)
                {
                    DrawUtilities.DrawNodeCurve(currentInput.inputNode.parentGraph.graphNode.getMultiOutputRect(), getMultiInputRect(), WorkPreferences.nodeCurveColor, WorkPreferences.nodeCurveThickness);
                }
                else if (!currentInput.inputNode.parentGraph.graphNode.multiOutput && multiInput)
                {
                    DrawUtilities.DrawNodeCurve(currentInput.inputNode.parentGraph.graphNode.nodeOutputs[currentInput.outputPos].rect, getMultiInputRect(), WorkPreferences.nodeCurveColor, WorkPreferences.nodeCurveThickness);
                }
                else
                {
                    DrawUtilities.DrawNodeCurve(currentInput.inputNode.parentGraph.graphNode.nodeOutputs[currentInput.outputPos].rect, currentInput.rect, WorkPreferences.nodeCurveColor, WorkPreferences.nodeCurveThickness);
                }
            }
        }
        else
        {

            //Draws the MultiInput Curve between single Outputs to MultiInputs
            DrawUtilities.DrawMultiInputNodeCurve(currentInput.inputNode.nodeRect, nodeRect, 1);
        }
    }
    //Draws the Inputhandles of the Node (Leftside Red Handles)
    public void drawInputHandles(GUISkin guiSkin)
    {
        if (multiInput)
        {
            //Multi Input Box
            if (GUI.Button(new Rect(nodeRect.x - 24f, nodeRect.y + ((nodeRect.height + 25f) * 0.5f) - 25f, 24f, 50f), "", guiSkin.GetStyle("node_multiInput")))
            {
                if (parentGraph != null)
                {
                    if (parentGraph.wantsConnection)
                    {
                        if (parentGraph.connectionOutputList != null)
                        {
                            for (int i = 0; i < parentGraph.connectionOutputList.Count; i++)
                            {
                                bool needsExpanding = true;
                                for (int k = 0; k < nodeInputs.Count; k++)
                                {
                                    if (nodeInputs[k].inputNode == null && nodeInputs[k].isOccupied == false)
                                    {
                                        nodeInputs[k].inputNode = parentGraph.connectionOutputList[i].outputNode;
                                        nodeInputs[k].isOccupied = nodeInputs[k].inputNode != null;

                                        Debug.Log("Connected at: " + k);
                                        needsExpanding = false;

                                        break;
                                    }
                                    else
                                    {
                                        needsExpanding = true;
                                    }
                                }

                                if (numberOfInputs < nodeInputsMax && needsExpanding)
                                {
                                    int t = nodeInputs.Count;
                                    numberOfInputs = t + 1;
                                    nodeInputs.Add(new NodeInput());
                                    nodeInputs[t].inputNode = parentGraph.connectionOutputList[i].outputNode;
                                    nodeInputs[t].isOccupied = nodeInputs[t].inputNode != null;
                                    Debug.Log("Increased Input and Connected");
                                }
                            }

                            parentGraph.wantsConnection = false;
                            parentGraph.connectionNode = null;
                            parentGraph.connectionOutputList = null;
                        }
                        else
                        {
                            for (int k = 0; k < nodeInputs.Count; k++)
                            {
                                if(nodeInputs[k].inputNode == null && nodeInputs[k].isOccupied == false)
                                {
                                    nodeInputs[k].inputNode = parentGraph.connectionNode;
                                    nodeInputs[k].isOccupied = nodeInputs[k].inputNode != null;

                                    parentGraph.wantsConnection = false;
                                    parentGraph.connectionNode = null;
                                    Debug.Log("Connected at: " + k);

                                    return;
                                }
                            }

                            if (numberOfInputs < nodeInputsMax)
                            {
                                int i = nodeInputs.Count;
                                numberOfInputs = i + 1;
                                nodeInputs.Add(new NodeInput());
                                nodeInputs[i].inputNode = parentGraph.connectionNode;
                                nodeInputs[i].isOccupied = nodeInputs[i].inputNode != null;
                                Debug.Log("Increased Input and Connected");
                            }

                            parentGraph.wantsConnection = false;
                            parentGraph.connectionNode = null;
                        }
                    }
                    else // remove all Inputs
                    {
                        Debug.Log("Removing MultiInput");
                        nodeInputs = new List<NodeInput>();
                        numberOfInputs = nodeInputs.Count;
                    }
                }
            }
            GUI.Label(new Rect(nodeRect.x - 20f, nodeRect.y + ((nodeRect.height + 25f) * 0.5f) - 10f, nodeRect.width * 0.2f - 10f, 20f), nodeInputs.Count + "", guiSkin.GetStyle("std_whiteText"));
        }
        else
        {
            //Single Input Circles(red)
            for (int i = 0; i < nodeInputs.Count; i++)
            {
                nodeInputs[i].rect.x = nodeRect.x - 10f;
                nodeInputs[i].rect.y = nodeRect.y + (nodeRect.height * (1f / (nodeInputs.Count + 1))) * (i + 1) - 10f;
                nodeInputs[i].position = i;

                if (GUI.Button(nodeInputs[i].rect, "", guiSkin.GetStyle("node_input")))
                {
                    if (parentGraph != null)
                    {
                        if (parentGraph.wantsConnection)
                        {                
                            if(parentGraph.connectionOutputList != null)
                            {
                                nodeInputs[i].inputNode = parentGraph.connectionOutputList[0].outputNode;
                                nodeInputs[i].isOccupied = nodeInputs[i].inputNode != null;
                                nodeInputs[i].outputPos = parentGraph.connectionOutputList[0].position;

                                nodeInputs[i].inputNode.nodeOutputs[nodeInputs[i].outputPos].connectedToNode = this;
                                parentGraph.connectionOutputList[0].connectedToNode = this;

                                parentGraph.wantsConnection = false;
                                parentGraph.connectionNode = null;
                                parentGraph.connectionOutputList = null;

                                Debug.Log("Connected from multiInput");
                            }
                            else
                            {
                                nodeInputs[i].inputNode = parentGraph.connectionOutput.outputNode;
                                nodeInputs[i].isOccupied = nodeInputs[i].inputNode != null;
                                nodeInputs[i].outputPos = parentGraph.connectionOutput.position;

                                nodeInputs[i].inputNode.nodeOutputs[nodeInputs[i].outputPos].connectedToNode = this;
                                parentGraph.connectionOutput.connectedToNode = this;

                                parentGraph.wantsConnection = false;
                                parentGraph.connectionNode = null;
                                Debug.Log("Connected");
                            }       
                        }
                        else
                        {
                            Debug.Log("Removing Connection #" + i);
                            nodeInputs[i].inputNode = null;
                            nodeInputs[i].isOccupied = false;
                        }
                    }
                }
            }
        }
    }

    //Draws Outputhandles of the Node (Rightside Green Handles)
    public void drawOutputHandles(GUISkin guiSkin)
    {
        if (multiOutput)
        {
            if (GUI.Button(new Rect(nodeRect.x + nodeRect.width, nodeRect.y + ((nodeRect.height + 25f) * 0.5f) - 25f, 24f, 50f), "", guiSkin.GetStyle("node_multiOutput")))
            {
                if (parentGraph != null)
                {
                    parentGraph.wantsConnection = true;
                    parentGraph.connectionOutputList = new List<NodeOutput>();
                    foreach(NodeOutput n in nodeOutputs)
                    {
                        parentGraph.connectionOutputList.Add(n);
                    }
                }
            }

            GUI.Label(new Rect(nodeRect.x + nodeRect.width , nodeRect.y + ((nodeRect.height + 25f) * 0.5f) - 10f, nodeRect.width * 0.2f - 10f, 20f), nodeOutputs.Count + "", guiSkin.GetStyle("std_whiteText"));
        }
        else
        {
            //Single Output Circles (green)
            for (int i = 0; i < nodeOutputs.Count; i++)
            {
                nodeOutputs[i].rect.x = nodeRect.x + nodeRect.width - 10f;
                nodeOutputs[i].rect.y = nodeRect.y + (nodeRect.height * (1f / (nodeOutputs.Count + 1))) * (i + 1) - 10f;
                nodeOutputs[i].position = i;

                if (GUI.Button(nodeOutputs[i].rect, "", guiSkin.GetStyle("node_output")))
                {
                    if (parentGraph != null)
                    {
                        if(nodeType == NodeType.Graph)
                        {
                            if (nodeOutputs[i].outputNode != null)
                            {
                                parentGraph.wantsConnection = true;
                                parentGraph.connectionNode = nodeOutputs[i].outputNode;                                
                                parentGraph.connectionOutput = nodeOutputs[i];
                            }
                        }
                        else
                        {
                            parentGraph.wantsConnection = true;
                            parentGraph.connectionNode = this;
                            parentGraph.connectionOutput = nodeOutputs[i];
                        }
                    }
                }
            }
        }
    }
    
    private void DrawCurrentTimePosition()
    {
        TimeSpan t = TimeSpan.FromMilliseconds((timePointer.GetEndAnimPos().x * 100) - (timePointer.GetStartAnimPos().x * 100));
        string str = string.Format("{0:D2}m:{1:D2}s:{2:D2}ms",
                t.Minutes,
                t.Seconds,
                t.Milliseconds / 10);

        GUI.Label(new Rect(nodeRect.x + nodeRect.width * 0.5f - 65f, nodeRect.y + nodeRect.height - 22f, nodeRect.width, 25f), str, GuiStyles._instance.whiteNodeLabel);
    }

    private string getStartAnimTime()
    {
        TimeSpan t = TimeSpan.FromMilliseconds((timePointer.GetEndAnimPos().x * 100));
        string str = string.Format("{0:D2}m:{1:D2}s:{2:D2}ms",
                t.Minutes,
                t.Seconds,
                t.Milliseconds / 10);

        return str;
    }

    private string getEndAnimTime()
    {
        TimeSpan t = TimeSpan.FromMilliseconds((timePointer.GetStartAnimPos().x * 100));
        string str = string.Format("{0:D2}m:{1:D2}s:{2:D2}ms",
                t.Minutes,
                t.Seconds,
                t.Milliseconds / 10);

        return str;
    }
    #endregion

    #region Node Updating Methods

    private void resizeNodeBox()
    {
        if (!multiInput && !multiOutput)
        {
            int size = nodeInputs.Count > nodeOutputs.Count ? nodeInputs.Count : nodeOutputs.Count;
            nodeRect.height = 40f + (30f * size);
        }
        else
        {
            nodeRect.height = 100f;
        }
    }

    protected void resizeInputHandles(int size)
    {
        if (nodeInputs.Count >= size)
        {
            for (int i = 0; i < nodeInputs.Count; i++)
            {
                if (i >= size)
                {
                    nodeInputs.RemoveAt(i);
                }
            }
        }
        else if (nodeInputs.Count <= size)
        {
            int numberOfInputsToAdd = size - nodeInputs.Count;
            for (int i = 0; i < numberOfInputsToAdd; i++)
            {
                nodeInputs.Add(new NodeInput());
            }
        }
    }

    protected void resizeOutputHandles(int size)
    {
        if (nodeOutputs.Count > size)
        {
            for (int i = 0; i < nodeInputs.Count; i++)
            {
                if (i >= size)
                {
                    nodeOutputs.RemoveAt(i);
                }
            }
        }
        else if (nodeOutputs.Count < size)
        {
            int numberOfOutputsToAdd = size - nodeInputs.Count;
            for (int i = 0; i < numberOfOutputsToAdd; i++)
            {
                nodeOutputs.Add(new NodeOutput());
            }
        }
    }

    private Vector2 snap(Vector2 v, float snapValue)
    {
        return new Vector2
        (
            snapValue * Mathf.Round(v.x / snapValue),
            snapValue * Mathf.Round(v.y / snapValue)
        );
    }

    public Vector3 getLowerCenter()
    {
        return new Vector3(nodeRect.x + nodeRect.width * 0.5f, nodeRect.y + nodeRect.height,0f);
    }

    public Rect getMultiOutputRect()
    {
        return new Rect(nodeRect.x + nodeRect.width, nodeRect.y + ((nodeRect.height + 25f) * 0.5f) - 25f, 24f, 50f);
    }

    public Rect getMultiInputRect()
    {
        return new Rect(nodeRect.x - 24f, nodeRect.y + ((nodeRect.height + 25f) * 0.5f) - 25f, 24f, 50f);
    }

    public Color getColorByNodeType()
    {
        Color col = Color.grey;

        switch (titleBarColor)
        {
            case "red":
                col = Color.red;
                break;
            case "blue":
                col = Color.blue;
                break;
            case "green":
                col = Color.green;
                break;
            case "lightblue":
                col = new Color(135f / 255f, 206f / 255f, 235f / 255f, 1f);
                break;
            case "violet":
                col = new Color(186f / 255f, 85f / 255f, 211f / 255f, 1f); ;
                break;
            case "solid_blue":
                col = Color.blue;
                break;
            case "solid_green":
                col = Color.green;
                break;
            case "solid_red":
                col = Color.red;
                break;
            case "solid_orange":
                col = new Color(255f / 255f, 165f / 255f, 0f / 255f, 1f); ;
                break;
            case "solid_lightblue":
                col = new Color(135f / 255f, 206f / 255f, 235f / 255f, 1f);
                break;
            case "solid_violet":
                col = new Color(186f / 255f, 85f / 255f, 211f / 255f, 1f); ;
                break;
            default:
                break;
        }

        return col;
    }

    public override string ToString()
    {
        return nodeName + ": " + nodeType + " - \n" + timePointer.GetStartAnimPos().x * 100+ " - " + timePointer.GetEndAnimPos().x * 100;
    }

    #endregion

}
