using UnityEngine;
using System;

[Serializable]
public class NodeInput
{
    public bool isOccupied;
    public Rect rect;
    public Rect connectionNodeRect;
    public NodeOutput connectionOutput;
    public NodeBase inputNode;
}
