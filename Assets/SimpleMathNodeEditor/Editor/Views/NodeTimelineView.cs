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

        DrawTicks(viewRect, 12, 0.6f, Color.grey, false);
        DrawTicks(viewRect, 60, 0.4f, Color.black, true);


        if (currentNodeGraph.nodes.Count > 0)
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

    private void drawTimelineConnetion(Vector2 pos)
    {
        Handles.color = Color.black;

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
            if(drawLabel)
                Handles.Label(new Vector3((gridSpacing * x) - (gridSpacing * 0.2f), 0f, 0f), (gridSpacing * x) + "");
        }
        
        Handles.EndGUI();
    }
}
