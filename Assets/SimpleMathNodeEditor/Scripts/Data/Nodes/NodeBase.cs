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
        ProcessEvents(e, viewRect);
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

                    rect.x += e.delta.x;
                    rect.y += e.delta.y;

                    nodeRect = rect;
                }
            }
        }
    }

#if UNITY_EDITOR
    public virtual void UpdateNodeGUI(Event e, Rect viewRect, GUISkin guiSkin)
    {
        ProcessEvents(e, nodeRect);

        string currentStyle = isSelected ? "node_selected" : "node_default";
        GUI.Box(nodeRect, nodeName, guiSkin.GetStyle(currentStyle));

        EditorUtility.SetDirty(this);
    }

    public virtual void DrawNodeProperties(Rect viewRect, GUISkin guiSkin)
    {

    }
#endif
}
