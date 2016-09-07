using UnityEngine;
using System;

[Serializable]
public class NodeInput
{
    public bool isOccupied;
    public Rect rect = new Rect(0f, 0f, 20f, 20f);
    public int position;
    public int outputPos;
    public NodeBase inputNode;

    public void updatePosition()
    {
        rect.x = inputNode.nodeRect.x - 10f;
        rect.y = inputNode.nodeRect.y + (inputNode.nodeRect.height * (1f / (inputNode.nodeInputs.Count + 1))) * (position + 1) - 10f;
    }

    public Rect getOutputPos()
    {
        return inputNode.nodeOutputs[outputPos].rect;
    }
}
