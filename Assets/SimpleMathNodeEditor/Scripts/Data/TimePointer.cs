using UnityEngine;
using UnityEditor;
using System;

[Serializable]
public class TimePointer
{
    public Rect arrowRect;
    public float startAnimOffset, endAnimOffset;
    public TimeSpan startTime, endTime;

    public NodeBase parentNode;
    public bool isSelected, isHighlighted;
    public bool isMoveable, resizeStartOffset, resizeEndOffset;

    private float opacity = 0.2f;

    public void InitTimePointer()
    {
        arrowRect = new Rect(10f, 10f, 36f, 48f);
        startAnimOffset = -100f;
        endAnimOffset = 100f;
    }

    public void drawArrow(Event e,Rect viewRect, Rect workViewRect, GUISkin guiSkin)
    {
        ProcessEvents(e, viewRect, workViewRect);

        float y = viewRect.height / parentNode.parentGraph.zoom;
        arrowRect.y = (y - 40f - arrowRect.height) - parentNode.parentGraph.panY ;
        
        if (isSelected || parentNode.isSelected || isHighlighted) { opacity = 1f; } else { opacity = 0.2f; }

        if (isSelected || parentNode.isSelected || isHighlighted)
        {
            DrawConnectionToNode(true);
        }else
        {
            DrawConnectionToNode(false);
        }
    }

    private void DrawConnectionToNode(bool highlighted)
    {
        Handles.BeginGUI();

        if (highlighted)// Draw Shadow if highlighted
        {
            DrawUtilities.DrawRangeCurveShadow(GetStartAnimPos(), GetEndAnimPos(),
                                 parentNode.getLowerCenter(),
                                 new Vector3(arrowRect.x + arrowRect.width * 0.5f, arrowRect.y - 150f, 0f),
                                 parentNode.getColorByNodeType(), opacity, 3);
        }

        DrawUtilities.DrawRangeCurve(GetStartAnimPos(), GetEndAnimPos(),
                             parentNode.getLowerCenter(),
                             new Vector3(arrowRect.x + arrowRect.width * 0.5f, arrowRect.y - 150f, 0f),
                             parentNode.getColorByNodeType(), opacity, 3);
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
        }

        if (parentNode.isSelected)
        {
            if (e.keyCode == KeyCode.LeftArrow && e.type == EventType.KeyUp)
            {
                arrowRect.x -= 10f;
            }
            if (e.keyCode == KeyCode.RightArrow && e.type == EventType.KeyUp)
            {
                arrowRect.x += 10f;
            }
            if (e.keyCode == KeyCode.DownArrow && e.type == EventType.KeyUp)
            {
                arrowRect.x -= 1f;
            }
            if (e.keyCode == KeyCode.UpArrow && e.type == EventType.KeyUp)
            {
                arrowRect.x += 1f;
            }
        }

        if (resizeStartOffset)
        {
            if (workViewRect.Contains(e.mousePosition) && !parentNode.parentGraph.isInsidePropertyView)
            {
                if (e.button == 0 && e.type == EventType.MouseDrag)
                {
                    startAnimOffset += e.delta.x;
                    startAnimOffset = snapPoint(startAnimOffset, NodeBase.snapSize);

                    if (startAnimOffset > 0)
                    {
                        startAnimOffset = 0;
                    }

                }
            }
        }

        if (resizeEndOffset)
        {
            if (workViewRect.Contains(e.mousePosition) && !parentNode.parentGraph.isInsidePropertyView)
            {
                if (e.button == 0 && e.type == EventType.MouseDrag)
                {
                    endAnimOffset += e.delta.x;
                    endAnimOffset = snapPoint(endAnimOffset, NodeBase.snapSize);

                    if (endAnimOffset < 0)
                    {
                        endAnimOffset = 0;
                    }
                }
            }
        }

    }
    private Vector2 GetLowerRectCenter()
    {
        return new Vector2(arrowRect.x + arrowRect.width * 0.5f, arrowRect.y + arrowRect.height);
    }

    public Vector2 GetStartAnimPos()
    {
        Vector2 start = GetLowerRectCenter();
        start.x = start.x + startAnimOffset;
        return start;
    }
    
    public Vector2 GetEndAnimPos()
    {
        Vector2 end = GetLowerRectCenter();
        end.x = end.x + endAnimOffset;
        return end;
    }

    private float snapPoint(float xCoord, float snapValue)
    {
        return (snapValue * Mathf.Round(xCoord / snapValue));
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
