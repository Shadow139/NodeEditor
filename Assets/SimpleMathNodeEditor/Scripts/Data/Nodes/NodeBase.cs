using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class NodeBase : ScriptableObject
{
    public string nodeName;
    public Rect nodeRect;
    public NodeGraph parentGraph;
    public NodeType nodeType;
    public bool isSelected { get; set; }

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

                    //Debug.Log("Node Position: ( " + rect.x + " , " + rect.y + " ) - " + e.delta.x);

                    nodeRect = rect;
                }
            }

            if(e.keyCode == KeyCode.LeftArrow)
            {
                nodeRect.x -= 5;
            }
            if (e.keyCode == KeyCode.RightArrow)
            {
                nodeRect.x += 5;
            }
        }
    }

#if UNITY_EDITOR
    public virtual void UpdateNodeGUI(Event e, Rect viewRect, GUISkin guiSkin)
    {
        ProcessEvents(e, viewRect);

        string currentStyle = isSelected ? "node_selected" : "node_default";
        GUI.Box(nodeRect, nodeName, guiSkin.GetStyle(currentStyle));

        if(isSelected)
            drawTimelineConnetion(viewRect);

        EditorUtility.SetDirty(this);
    }

    private void drawTimelineConnetion(Rect viewRect)
    {
        Handles.color = Color.black;

        Handles.DrawLine(new Vector3(nodeRect.x + nodeRect.width * 0.5f,
            nodeRect.y + nodeRect.height , 0f),
            new Vector3(nodeRect.x + nodeRect.width * 0.5f, viewRect.height , 0f));
    }

    public virtual void DrawNodeProperties(Rect viewRect, GUISkin guiSkin)
    {

    }
#endif
}
