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

    public void updatePosition()
    {
        rect.x = outputNode.nodeRect.x - 10f;
        rect.y = outputNode.nodeRect.y + (outputNode.nodeRect.height * (1f / (outputNode.nodeInputs.Count + 1))) * (position + 1) - 10f;
    }
}
