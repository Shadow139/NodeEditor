using UnityEngine;
using UnityEditor;
using System;

[Serializable]
public class TimePointer
{
    public Rect arrowRect;
    public Vector2 centerPoint;
    public float x;
    public float startAnimOffset, endAnimOffset;
    public TimeSpan startTime, endTime;

    public NodeBase parentNode;
    public bool isSelected, isHighlighted;
    public bool isMoveable, resizeStartOffset, resizeEndOffset;

    private float opacity = 0.2f;
    private float curveHeight = 150f;

    public void InitTimePointer()
    {
        arrowRect = new Rect(10f, 10f, 36f, 48f);
        centerPoint = new Vector2(0f, 0f);

        startAnimOffset = -100f;
        endAnimOffset = 100f;
        if(parentNode.nodeType == NodeType.Graph)
        {
            startAnimOffset = 0;
            endAnimOffset = 0;
        }
    }

    public void drawArrow(Event e,Rect viewRect, Rect workViewRect, GUISkin guiSkin)
    {
        ProcessEvents(e, viewRect, workViewRect);

        x = GetLowerRectCenter().x;
        float y = viewRect.height / parentNode.parentGraph.zoom - (40f / parentNode.parentGraph.zoom);
        arrowRect.y = (y - arrowRect.height) - parentNode.parentGraph.panY;

        curveHeight = 150 - (75 * parentNode.parentGraph.zoom);

        adjustGraphNode();

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
                                 new Vector3(arrowRect.x + arrowRect.width * 0.5f, arrowRect.y - curveHeight, 0f),
                                 parentNode.getColorByNodeType(), opacity, 3);
        }

        DrawUtilities.DrawRangeCurve(GetStartAnimPos(), GetEndAnimPos(),
                             parentNode.getLowerCenter(),
                             new Vector3(arrowRect.x + arrowRect.width * 0.5f, arrowRect.y - curveHeight, 0f),
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
                    float delta = snapPoint(e.delta.x, NodeBase.snapSize);

                    if (parentNode.nodeType == NodeType.Graph)
                    {
                        translateGraphNodeInsides(delta);
                    }
                    else
                    {
                        arrowRect.x += delta;
                    }
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
                    if (parentNode.nodeType == NodeType.Graph)
                    {
                        NodeBase tmp = parentNode.nodeGraph.getFirstAnimatedNode();
                        Debug.Log(tmp.nodeName + " - " + tmp.parameters["value"].floatParam);
                    }
                    else
                    {
                        startAnimOffset += e.delta.x;
                        startAnimOffset = snapPoint(startAnimOffset, NodeBase.snapSize);
                    }

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
                    if (parentNode.nodeType == NodeType.Graph)
                    {
                        NodeBase tmp = parentNode.nodeGraph.getLastAnimatedNode();
                        Debug.Log(tmp.nodeName + " - " + tmp.parameters["value"].floatParam);
                    }
                    else
                    {
                        endAnimOffset += e.delta.x;
                        endAnimOffset = snapPoint(endAnimOffset, NodeBase.snapSize);
                    }

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

    private void adjustGraphNode()
    {
        if (parentNode.nodeType == NodeType.Graph)
        {
            NodeBase first = parentNode.nodeGraph.getFirstAnimatedNode();
            NodeBase last = parentNode.nodeGraph.getLastAnimatedNode();

            if (first != null)
                startAnimOffset = getDifferenceFromStart(first.timePointer.x) + first.timePointer.startAnimOffset;

            if (last != null)
                endAnimOffset = getDifferenceFromEnd(last.timePointer.x) + first.timePointer.endAnimOffset;
        }
    }

    private void translateGraphNodeInsides(float delta)
    {
        foreach (NodeBase n in parentNode.nodeGraph.nodes)
        {
            Debug.Log("moving  " + n.nodeName);
            if(n.nodeType == NodeType.Graph)
            {
                n.timePointer.translateGraphNodeInsides(delta);
            }
            else
            {
                n.timePointer.arrowRect.x += delta;
            }

            NodeBase first = parentNode.nodeGraph.getFirstAnimatedNode();
            NodeBase last = parentNode.nodeGraph.getLastAnimatedNode();

            if (first != null)
                startAnimOffset = getDifferenceFromStart(first.timePointer.x) + first.timePointer.startAnimOffset;

            if (last != null)
                endAnimOffset = getDifferenceFromEnd(last.timePointer.x) + first.timePointer.endAnimOffset;
        }
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

    private float getDifferenceFromStart(float start)
    {
        return start - GetLowerRectCenter().x;
    }

    private float getDifferenceFromEnd(float end)
    {
        return end - GetLowerRectCenter().x;
    }
}
