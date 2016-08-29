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
    public Dictionary<string, object> parameters;
    public string titleBarColor;

    public Rect nodeRect;
    public Rect viewPortRect;
    public NodeGraph parentGraph;
    public bool isSelected { get; set; }
    public TimePointer timePointer;
    public TimeSpan currentTime;

    protected bool multiInput = false;
    protected int numberOfInputs;
    private int numberOfOutputs;
    public static float snapSize = 10;

    public NodeGraph nodeGraph;

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

        parameters = new Dictionary<string, object>(descriptor.parameters);

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
        }
        numberOfOutputs = descriptor.numberOfOutputs;
        nodeOutputsMin = descriptor.minOutputs;
        nodeOutputsMax = descriptor.maxOutputs;

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
            }
        }
    }

    public virtual void UpdateNodeGUI(Event e, Rect viewRect, Rect workViewRect, GUISkin guiSkin)
    {
        ProcessEvents(e, viewRect, workViewRect);
        evaluateNode();
        viewPortRect = viewRect;

        string currentStyle = isSelected ? "node_selected" : "node_default";
        string lowerBarStyle = currentStyle;
        if (timePointer.isSelected)
            currentStyle = "node_selected";
        GUI.Box(nodeRect, nodeName, guiSkin.GetStyle(currentStyle));
        GUI.Box(new Rect(nodeRect.x, nodeRect.y, nodeRect.width, 27f), nodeName, guiSkin.GetStyle((currentStyle + "_titlebar_" + titleBarColor)));
        GUI.Box(new Rect(nodeRect.x, nodeRect.y + nodeRect.height - 27f, nodeRect.width, 27f), "", guiSkin.GetStyle(currentStyle));

        //if (isSelected || timePointer.isSelected || WorkPreferences.showTimeInfo)
        DrawCurrentTimePosition();
      

        if (timePointer != null)
            timePointer.drawArrow(e, viewRect, workViewRect);

        resizeNodeBox();

        drawOutputHandles(guiSkin);
        drawInputHandles(guiSkin);

        DrawInputLines();

        DrawNodeBoxInsideByType(viewRect);

        EditorUtility.SetDirty(this);
    }

    public virtual void DrawNodeProperties(Rect viewRect, GUISkin guiSkin)
    {
        DrawNodePropertiesByType(viewRect, guiSkin);
        GUILayout.Space(6);
        multiInput = EditorGUILayout.Toggle("Multi Input", multiInput);
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

        if (!multiInput)
        {
            resizeInputHandles(numberOfInputs);
            resizeOutputHandles(numberOfOutputs);
        }
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
            parameters["value"] = EditorGUILayout.FloatField("Float value", Convert.ToSingle(parameters["value"]), guiSkin.GetStyle("property_view"));
        }
    }

    private void DrawAdditionNodeProperties(GUISkin guiSkin)
    {
        if (parameters != null)
        {
            EditorGUILayout.LabelField("Value :", Convert.ToSingle(parameters["value"]).ToString());
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
    }

    private void DrawFloatNodeInsides()
    {
        if (parameters != null)
        {
            GUI.Label(new Rect(nodeRect.x + nodeRect.width * 0.5f - 10f, nodeRect.y + ((nodeRect.height) * 0.5f) - 10f, nodeRect.width * 0.5f - 10f, 20f), Convert.ToSingle(parameters["value"]).ToString(), GuiStyles._instance.whiteNodeLabel);
        }
    }

    private void DrawAdditionNodeInsides()
    {
        if (parameters != null)
        {
            GUI.Label(new Rect(nodeRect.x + nodeRect.width * 0.5f - 10f, nodeRect.y + ((nodeRect.height) * 0.5f) - 10f, nodeRect.width * 0.5f - 10f, 20f), Convert.ToSingle(parameters["value"]).ToString(), GuiStyles._instance.whiteNodeLabel);
        }
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
                        tempSum += Convert.ToSingle(nodeInputs[i].inputNode.parameters["value"]);
                    }
                    else if (nodeInputs[i].inputNode.nodeType == NodeType.Addition)
                    {
                        tempSum += Convert.ToSingle(nodeInputs[i].inputNode.parameters["value"]);
                    }
                    else if (nodeInputs[i].inputNode.nodeType == NodeType.Graph)
                    {
                    
                    }
                }
            }
            parameters["value"] = tempSum;
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
        if (!multiInput && currentInput.inputNode.parentGraph == parentGraph)
        {
            DrawUtilities.DrawNodeCurve(currentInput.inputNode.nodeRect, nodeRect, inputId, nodeInputs.Count);
        }else if(currentInput.inputNode.parentGraph != parentGraph)
        {
            int i = parentGraph.graphNode.nodeInputs.FindIndex(a => a.inputNode == currentInput.inputNode);
            if(i >= 0)
            {
               Rect r = parentGraph.graphInputRects[i];
               DrawUtilities.DrawNodeCurve(new Vector3(r.x + r.width, r.center.y, 0f), nodeRect, inputId, nodeInputs.Count);
            }
        }
        else
        {
            DrawUtilities.DrawMultiInputNodeCurve(currentInput.inputNode.nodeRect, nodeRect, 1);
        }
    }

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
                            numberOfInputs = nodeInputs.Count + 1;
                            nodeInputs.Add(new NodeInput());
                            nodeInputs[i].inputNode = parentGraph.connectionNode;
                            nodeInputs[i].isOccupied = nodeInputs[i].inputNode != null;
                            Debug.Log("Increased Input and Connected");
                        }

                        parentGraph.wantsConnection = false;
                        parentGraph.connectionNode = null;
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
            //Single Input Circles
            for (int i = 0; i < nodeInputs.Count; i++)
            {
                if (GUI.Button(new Rect(nodeRect.x - 10f, nodeRect.y + (nodeRect.height * (1f / (nodeInputs.Count + 1))) * (i + 1) - 10f, 20f, 20f), "", guiSkin.GetStyle("node_input")))
                {
                    if (parentGraph != null)
                    {
                        if (parentGraph.wantsConnection)
                        {
                            nodeInputs[i].inputNode = parentGraph.connectionNode;
                            nodeInputs[i].isOccupied = nodeInputs[i].inputNode != null;

                            parentGraph.wantsConnection = false;
                            parentGraph.connectionNode = null;
                            Debug.Log("Connected");
                        }
                        else
                        {
                            Debug.Log("Removing InputHandle #" + i);
                            nodeInputs[i].inputNode = null;
                            nodeInputs[i].isOccupied = false;
                        }
                    }
                }
            }
        }
    }

    public void drawOutputHandles(GUISkin guiSkin)
    {
        if (multiInput)
        {
            if (GUI.Button(new Rect(nodeRect.x + nodeRect.width, nodeRect.y + ((nodeRect.height + 25f) * 0.5f) - 25f, 24f, 50f), "", guiSkin.GetStyle("node_multiOutput")))
            {
                if (parentGraph != null)
                {
                    parentGraph.wantsConnection = true;
                    parentGraph.connectionNode = this;
                }
            }

            GUI.Label(new Rect(nodeRect.x + nodeRect.width , nodeRect.y + ((nodeRect.height + 25f) * 0.5f) - 10f, nodeRect.width * 0.2f - 10f, 20f), nodeOutputs.Count + "", guiSkin.GetStyle("std_whiteText"));
        }
        else
        {
            for (int i = 0; i < nodeOutputs.Count; i++)
            {
                if (GUI.Button(new Rect(nodeRect.x + nodeRect.width - 10f, nodeRect.y + (nodeRect.height * (1f / (nodeOutputs.Count + 1))) * (i + 1) - 10f, 20f, 20f), "", guiSkin.GetStyle("node_output")))
                {
                    if (parentGraph != null)
                    {
                        parentGraph.wantsConnection = true;
                        parentGraph.connectionNode = this;
                    }
                }
            }
        }
    }
    
    protected void DrawCurrentTimePosition()
    {
        TimeSpan t = TimeSpan.FromMilliseconds(timePointer.arrowRect.center.x * 100);
        currentTime = t;
        string str = string.Format("{0:D2}m:{1:D2}s:{2:D2}ms",
                t.Minutes,
                t.Seconds,
                t.Milliseconds / 10);

        GUI.Label(new Rect(nodeRect.x + nodeRect.width * 0.5f - 65f, nodeRect.y + nodeRect.height - 22f, nodeRect.width, 25f), str, GuiStyles._instance.whiteNodeLabel);
    }

    #endregion

    #region Node Updating Methods

    private void resizeNodeBox()
    {
        if (!multiInput)
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
    #endregion

}
