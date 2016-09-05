using UnityEngine;
using System;

[Serializable]
public class NodeInput
{
    public bool isOccupied;
    public Rect rect = new Rect(0f, 0f, 20f, 20f);
    public int position;
    public NodeOutput connectionToOutput;
    public NodeBase inputNode;
}
