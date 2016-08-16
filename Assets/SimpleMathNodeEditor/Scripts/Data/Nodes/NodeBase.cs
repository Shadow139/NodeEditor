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
    public static float snapSize;

    public List<NodeInput> nodeInputs = new List<NodeInput>();
    public List<NodeOutput> nodeOutputs = new List<NodeOutput>();

    public GUISkin nodeSkin { get; set; }

    [Serializable]
    public class NodeInput
    {
        public bool isOccupied;
        public NodeBase inputNode;
    }

    [Serializable]
    public class NodeOutput
    {
        public bool isOccupied;
        public NodeBase outputNode;
    }

    public virtual void InitNode() { }

    public virtual void UpdateNode(Event e, Rect viewRect)
    {
        //ProcessEvents(e, viewRect);
    }

    public virtual void evaluateNode()
    {

    }

    private void ProcessEvents(Event e, Rect viewRect)
    {
        if (isSelected)
        {
            if (viewRect.Contains(e.mousePosition))
            {
                if (e.type == EventType.MouseDrag)
                {
                    var rect = nodeRect;

                    if (e.delta.x > 0)
                    {

                    }
                    else
                    {

                    }

                    rect.x += e.delta.x;
                    rect.y += e.delta.y;

                    rect.position = snap(rect.position, snapSize);

                    //Debug.Log("Node Position: ( " + rect.x + " , " + rect.y + " ) - " + e.delta.x);

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

    public virtual void UpdateNodeGUI(Event e, Rect viewRect, GUISkin guiSkin)
    {
        ProcessEvents(e, viewRect);

        workViewRect = viewRect;
        string currentStyle = isSelected ? "node_selected" : "node_default";
        GUI.Box(nodeRect, nodeName, guiSkin.GetStyle(currentStyle));

        if (isSelected || WorkPreferences.showTimeInfo)
        {
            drawTimelineConnetion();
            GUI.Label(new Rect(nodeRect.x + nodeRect.width * 0.5f - 10f, nodeRect.y - 16f, nodeRect.width * 0.5f, 20f), nodeRect.center.x + "");
        }

        EditorUtility.SetDirty(this);
    }

    public void drawTimelineConnetion()
    {
        Handles.color = Color.black;
        
        Handles.DrawLine(new Vector3(nodeRect.x + nodeRect.width * 0.5f,
            nodeRect.y + nodeRect.height , 0f),
            new Vector3(nodeRect.x + nodeRect.width * 0.5f, workViewRect.height, 0f));
    }

    public virtual void DrawNodeProperties(Rect viewRect, GUISkin guiSkin)
    {
        GUILayout.BeginVertical();

        GUILayout.Space(20);

        multiInput = EditorGUILayout.Toggle("Multi Input", multiInput);

        GUILayout.Space(10);

        if (!multiInput)
        {
            numberOfInputs = EditorGUILayout.IntField("Number of InputHandles", nodeInputs.Count, guiSkin.GetStyle("property_view"));
            GUILayout.Space(10);

            resizeInputHandles(numberOfInputs);
            //TODO Resize the InputHandlesList
        }

        snapSize = EditorGUILayout.IntSlider((int)snapSize, 1, 100);

        GUILayout.EndVertical();
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
                        int i = nodeInputs.Count;
                        nodeInputs.Add(new NodeInput());
                        nodeInputs[i].inputNode = parentGraph.connectionNode;
                        nodeInputs[i].isOccupied = nodeInputs[i].inputNode != null;

                        Debug.Log("Added NodeInput and Connected " + parentGraph.connectionNode.ToString() + " at Pos: " + (i));

                        parentGraph.wantsConnection = false;
                        parentGraph.connectionNode = null;
                    }
                    else
                    {
                        Debug.Log("Removing MultiInput");
                        nodeInputs = new List<NodeInput>();
                        numberOfInputs = nodeInputs.Count;
                    }
                }
            }
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

}
