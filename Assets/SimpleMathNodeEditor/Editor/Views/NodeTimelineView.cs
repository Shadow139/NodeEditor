using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

public class NodeTimelineView : ViewBaseClass
{
    public NodeTimelineView() : base("") { }
    
    public override void UpdateView(Rect editorRect, Rect percentageRect, Event e, NodeGraph nodeGraph)
    {
        base.UpdateView(editorRect, percentageRect, e, nodeGraph);
        GUI.Box(viewRect, viewTitle, viewSkin.GetStyle("bg_timeline"));

        GUILayout.BeginArea(viewRect);

        GUILayout.BeginVertical();

        DrawTicks(viewRect, 8, 0.6f, Color.grey);
        DrawTicks(viewRect, 40, 0.4f, Color.black);

        GUILayout.EndVertical();

        GUILayout.EndArea();
    }

    public override void ProcessEvents(Event e)
    {
        base.ProcessEvents(e);

    }

    public static void DrawTicks(Rect viewRect, float gridSpacing, float heightOfTicks, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(viewRect.width / gridSpacing);

        Handles.BeginGUI();

        Handles.color = gridColor;

        for (int x = 0; x < widthDivs; x++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * x, viewRect.height * heightOfTicks, 0f), new Vector3(gridSpacing * x, viewRect.height, 0f));
        }
        
        Handles.EndGUI();
    }
}
