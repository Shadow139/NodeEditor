using UnityEngine;
using UnityEditor;
using System;

[Serializable]
public class TimePointer
{
    public Rect arrowRect;
    public NodeBase parentNode;
    public bool isSelected;
    public bool isMoveable;

    public void InitTimePointer()
    {
        arrowRect = new Rect(10f, 10f, 36f, 48f);
    }

    public void drawArrow(Event e,Rect viewRect, Rect workViewRect)
    {
        ProcessEvents(e, viewRect, workViewRect);

        float y = viewRect.height / parentNode.parentGraph.zoom;
        arrowRect.y = (y - 40f - arrowRect.height) - parentNode.parentGraph.panY ;

        string currentStyle = (isSelected || parentNode.isSelected) ? "arrow_selected" : "arrow_default";
        GUI.Box(arrowRect, "", GuiStyles._instance.guiSkin.GetStyle(currentStyle));

        if (isSelected || parentNode.isSelected)
        {
            DrawConnectionToNode(Color.red);
        }else
        {
            DrawConnectionToNode(Color.black);
        }
    }

    private void DrawConnectionToNode(Color col)
    {
        //DrawUtilities.DrawCurve(parentNode.getLowerCenter(), new Vector3(arrowRect.x + arrowRect.width * 0.5f, arrowRect.y, 0f),Color.blue, 5);
        Handles.BeginGUI();
        Handles.color = col;
        Handles.DrawLine(parentNode.getLowerCenter(), new Vector3(arrowRect.x + arrowRect.width * 0.5f, arrowRect.y + 5f, 0f));
        Handles.EndGUI();
    }

    private void ProcessEvents(Event e, Rect viewRect, Rect workViewRect)
    {
        if (isMoveable)
        {
            if (workViewRect.Contains(e.mousePosition) && !parentNode.parentGraph.isInsidePropertyView)
            {
                if (e.button == 0 && e.type == EventType.MouseDrag)
                {
                    Rect rect = arrowRect;

                    rect.x += e.delta.x;

                    rect.position = snap(rect.position, NodeBase.snapSize);

                    arrowRect = rect;
                }
            }

            if (e.keyCode == KeyCode.LeftArrow && e.type == EventType.KeyUp)
            {
                arrowRect.x -= NodeBase.snapSize;
            }
            if (e.keyCode == KeyCode.RightArrow && e.type == EventType.KeyUp)
            {
                arrowRect.x += NodeBase.snapSize;
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
}
