using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class NodeBase : ScriptableObject
{
    public string nodeName;
    public Rect nodeRect;
    public Rect workViewRect;
    public NodeGraph parentGraph;
    public NodeType nodeType;
    public bool isSelected { get; set; }

    protected bool multiInput = false;
    protected int numberOfInputs;
    public static float snapSize = 10;

    public List<NodeInput> nodeInputs = new List<NodeInput>();
    public List<NodeOutput> nodeOutputs = new List<NodeOutput>();

    public GUISkin nodeSkin { get; set; }
    
    public virtual void InitNode() { }

    public virtual void UpdateNode(Event e, Rect viewRect)
    {
        //ProcessEvents(e, viewRect);
    }

    public virtual void evaluateNode()
    {

    }

    private void ProcessEvents(Event e, Rect viewRect, Rect workViewRect)
    {
        if (isSelected)
        {
            if (workViewRect.Contains(e.mousePosition))
            {
            if (e.button == 0 && e.type == EventType.MouseDrag)
                {
                    var rect = nodeRect;

                    rect.x += e.delta.x;
                    rect.y += e.delta.y;

                    rect.position = snap(rect.position, snapSize);

                    nodeRect = rect;
                }
            }

            if(e.keyCode == KeyCode.LeftArrow && e.type == EventType.KeyUp)
            {
                nodeRect.x -= snapSize;
            }
            if (e.keyCode == KeyCode.RightArrow && e.type == EventType.KeyUp)
            {
                nodeRect.x += snapSize;
            }
            if (e.keyCode == KeyCode.UpArrow && e.type == EventType.KeyUp)
            {
                nodeRect.y -= snapSize;
            }
            if (e.keyCode == KeyCode.DownArrow && e.type == EventType.KeyUp)
            {
                nodeRect.y += snapSize;
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

    public virtual void UpdateNodeGUI(Event e, Rect viewRect, Rect workViewRect, GUISkin guiSkin)
    {
        ProcessEvents(e, viewRect, workViewRect);

        workViewRect = viewRect;
        string currentStyle = isSelected ? "node_selected" : "node_default";
        GUI.Box(nodeRect, nodeName, guiSkin.GetStyle(currentStyle));
        GUI.Box(new Rect(nodeRect.x, nodeRect.y, nodeRect.width, 27f), nodeName, guiSkin.GetStyle(currentStyle));

        if (isSelected || WorkPreferences.showTimeInfo)
        {
            DrawTimelineConnetion();
            DrawCurrentTimePosition();

            //GUI.Label(new Rect(nodeRect.x + nodeRect.width * 0.5f - 10f, nodeRect.y - 16f, nodeRect.width * 0.5f, 20f), nodeRect.center.x + "");
        }

        if (!multiInput)
        {
            nodeRect.height = 40f + (30f * nodeInputs.Count);
            if (nodeInputs.Count == 0)
                nodeRect.height = 75f;
        }
        else
        {
            nodeRect.height = 100f;
        }

        EditorUtility.SetDirty(this);
    }

    private void DrawTimelineConnetion()
    {
        Handles.color = Color.black;
        
        Handles.DrawLine(new Vector3(nodeRect.x + nodeRect.width * 0.5f, nodeRect.y + nodeRect.height , 0f),
            new Vector3(nodeRect.x + nodeRect.width * 0.5f, 10000, 0f));
    }

    public virtual void DrawNodeProperties(Rect viewRect, GUISkin guiSkin)
    {
        GUILayout.Space(6);
        GUILayout.BeginHorizontal();
        multiInput = EditorGUILayout.Toggle("Multi Input", multiInput);
        GUILayout.Space(6);

        if (!multiInput)
        {
            numberOfInputs = EditorGUILayout.IntField("", nodeInputs.Count, guiSkin.GetStyle("property_view"));

            resizeInputHandles(numberOfInputs);
            //TODO Resize the InputHandlesList
        }

        GUILayout.EndHorizontal();
    }

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

    protected void DrawCurrentTimePosition()
    {
        TimeSpan t = TimeSpan.FromMilliseconds(nodeRect.center.x * 100);
        string str = string.Format("{0:D2}m:{1:D2}s:{2:D2}ms",
                t.Minutes,
                t.Seconds,
                t.Milliseconds/10);

        GUI.Label(new Rect(nodeRect.x + nodeRect.width * 0.5f - 50f, nodeRect.y - 16f, nodeRect.width, 25f), str);
    }

    protected void DrawNodeConnection(NodeInput currentInput, float inputId)
    {
        if (!multiInput)
        {
            DrawUtilities.DrawNodeCurve(currentInput.inputNode.nodeRect, nodeRect, inputId, nodeInputs.Count);
        }
        else
        {
            DrawUtilities.DrawMultiInputNodeCurve(currentInput.inputNode.nodeRect, nodeRect, 1);
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

    public void drawInputHandles(GUISkin guiSkin)
    {
        if (multiInput)
        {
            //Multi Input Box
            if (GUI.Button(new Rect(nodeRect.x - 16f, nodeRect.y + (nodeRect.height * 0.5f) - 30f, 16f, 60f), "", guiSkin.GetStyle("node_multiInput")))
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
                                return;
                            }
                        }

                        int i = nodeInputs.Count;
                        nodeInputs.Add(new NodeInput());
                        nodeInputs[i].inputNode = parentGraph.connectionNode;
                        nodeInputs[i].isOccupied = nodeInputs[i].inputNode != null;

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

            GUI.Label(new Rect(nodeRect.x - 12f, nodeRect.y + nodeRect.height * 0.5f - 10f, nodeRect.width * 0.2f - 10f, 20f), nodeInputs.Count + "", guiSkin.GetStyle("std_whiteText"));

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
                            nodeInputs.RemoveAt(i);
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
            if (GUI.Button(new Rect(nodeRect.x + nodeRect.width, nodeRect.y + nodeRect.height * 0.5f - 30f, 16f, 60f), "", guiSkin.GetStyle("node_multiOutput")))
            {
                if (parentGraph != null)
                {
                    parentGraph.wantsConnection = true;
                    parentGraph.connectionNode = this;
                }
            }

            GUI.Label(new Rect(nodeRect.x + nodeRect.width , nodeRect.y + nodeRect.height * 0.5f - 10f, nodeRect.width * 0.2f - 10f, 20f), nodeOutputs.Count + "", guiSkin.GetStyle("std_whiteText"));
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
}
