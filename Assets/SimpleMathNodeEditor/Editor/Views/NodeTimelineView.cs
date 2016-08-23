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

    public NodeTimelineView() : base("") { }
    
    public override void UpdateView(Rect editorRect, Rect percentageRect, Event e, NodeGraph nodeGraph)
    {
        base.UpdateView(editorRect, percentageRect, e, nodeGraph);
        GUI.Box(viewRect, viewTitle, viewSkin.GetStyle("bg_timeline"));

        GUILayout.BeginArea(viewRect);

        GUILayout.BeginVertical();

        DrawTicks(viewRect, 10, 0.6f, Color.grey, false);
        DrawTicks(viewRect, 50, 0.4f, Color.black, true);

        if (currentNodeGraph != null)
        {
            foreach (NodeBase node in currentNodeGraph.nodes)
            {
                if (node.isSelected || WorkPreferences.showTimeInfo)
                {
                    drawTimelineConnetion(node.nodeRect.center);
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

    public void DrawTicks(Rect viewRect, float gridSpacing, float heightOfTicks, Color gridColor,bool drawLabel)
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
}
