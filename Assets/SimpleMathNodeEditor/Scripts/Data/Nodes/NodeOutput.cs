using UnityEngine;
using System;

[Serializable]
public class NodeOutput
{
    public bool isOccupied;
    public Rect rect = new Rect(0f,0f,20f,20f);
    public int position;
    public NodeBase outputNode;
    public NodeBase connectedToNode;
}
