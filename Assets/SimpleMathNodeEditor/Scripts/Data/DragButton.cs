using UnityEngine;
using System;

[Serializable]
public class DragButton
{
    public Rect leftButtonRect;
    public Rect middleButtonRect;
    public Rect rightButtonRect;

    public void InitDragButton()
    {
        leftButtonRect = new Rect(10f, 10f, 36f, 48f);
        middleButtonRect = new Rect(10f, 10f, 36f, 48f);
        rightButtonRect = new Rect(10f, 10f, 36f, 48f);
    }

    public void DrawDragButton(Event e, Rect nodeRect, GUISkin guiSkin)
    {
        leftButtonRect = new Rect(nodeRect.x, nodeRect.y + nodeRect.height, 30f, 30f);
        middleButtonRect = new Rect(nodeRect.x + nodeRect.width * 0.5f - 15f, nodeRect.y + nodeRect.height, 30f, 30f);
        rightButtonRect = new Rect(nodeRect.x + nodeRect.width - 30f, nodeRect.y + nodeRect.height, 30f, 30f);

        GUI.Box(leftButtonRect, "", guiSkin.GetStyle("node_default"));
        GUI.Box(rightButtonRect, "", guiSkin.GetStyle("node_default"));
        GUI.Box(middleButtonRect, "", guiSkin.GetStyle("node_default"));
    }
}
