using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System;

public class NodeTimelineView : ViewBaseClass
{
    public float xOffset;
    public static float timelineRectOpacity = 0.2f;
    private float smallTickSpacing = 10f;
    private float bigTickSpacing = 50f;

    public NodeTimelineView() : base("") { }
    
    public override void UpdateView(Rect editorRect, Rect percentageRect, Event e, NodeGraph nodeGraph)
    {
        base.UpdateView(editorRect, percentageRect, e, nodeGraph);
        GUI.Box(viewRect, viewTitle, viewSkin.GetStyle("bg_timeline"));

        GUILayout.BeginArea(viewRect);
        GUILayout.BeginVertical();

        setTickSpacing();

        DrawTicks(viewRect, smallTickSpacing, 0.6f, Color.grey, false);
        DrawTicks(viewRect, bigTickSpacing, 0.4f, Color.black, true);

        if (currentNodeGraph != null)
        {
            foreach (NodeBase node in currentNodeGraph.nodes)
            {
                if (node.isSelected || node.timePointer.isSelected || node.timePointer.isHighlighted || WorkPreferences.showTimeInfo)
                {
                    drawTimelineConnetion(node.timePointer.arrowRect.center);
                    Color col = node.getColorByNodeType();
                    DrawTimelineAnimationLength(node.timePointer.GetStartAnimPos(), node.timePointer.GetEndAnimPos(), col, timelineRectOpacity);
                }
            }
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    public override void ProcessEvents(Event e)
    {
        base.ProcessEvents(e);
    }

    public void DrawTicks(Rect viewRect, float gridSpacing, float heightOfTicks, Color gridColor, bool drawLabel)
    {
        int widthDivs = Mathf.CeilToInt(viewRect.width / gridSpacing);

        Handles.BeginGUI();
        Handles.color = gridColor;

        for (int x = 0; x < widthDivs; x++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * x, viewRect.height * heightOfTicks, 0f), new Vector3(gridSpacing * x, viewRect.height, 0f));
            if (drawLabel)
            {
                TimeSpan t = TimeSpan.FromMilliseconds(gridSpacing * x * 100);
                string str = string.Format("{0:D2}:{1:D2}", 
                        t.Minutes,
                        t.Seconds);
                Handles.Label(new Vector3((gridSpacing * x) - (gridSpacing * 0.35f), 0f, 0f), str);
            }
        }
        
        Handles.EndGUI();
    }    

    private void setTickSpacing()
    {
        if(NodeWorkView._zoom > 0.5f && NodeWorkView._zoom < 0.75f)
        {
            smallTickSpacing = 10f;
            bigTickSpacing = 50f;
            NodeBase.snapSize = 25f;
        }
        else if (NodeWorkView._zoom > 0.75f && NodeWorkView._zoom < 1f)
        {
            smallTickSpacing = 10f;
            bigTickSpacing = 50f;
            NodeBase.snapSize = 10f;
        }
        else if (NodeWorkView._zoom > 1f && NodeWorkView._zoom < 1.25f)
        {
            smallTickSpacing = 10f;
            bigTickSpacing = 50f;
            NodeBase.snapSize = 1f;
        }
    }

    public void scrollViewRect(float x)
    {
        xOffset = -x;
    }

    private void drawTimelineConnetion(Vector2 pos)
    {
        Handles.color = Color.red;

        Handles.DrawLine(new Vector3(pos.x, 0f , 0f),
            new Vector3(pos.x, viewRect.height, 0f));
    }

    private void DrawTimelineAnimationLength(Vector2 start,Vector2 end, Color col, float opacity)
    {      
        Color newCol = new Color(col.r, col.g, col.b, opacity);
        EditorGUI.DrawRect(new Rect(new Vector2(start.x, 0f), new Vector2(end.x - start.x, viewRect.height)), newCol);
    }
}
