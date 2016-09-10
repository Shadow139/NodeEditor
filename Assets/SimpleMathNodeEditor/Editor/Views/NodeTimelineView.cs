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

        if (currentNodeGraph != null)
        {
            foreach (NodeBase node in currentNodeGraph.nodes)
            {
                if (node.isSelected || node.timePointer.isSelected || node.timePointer.isHighlighted || WorkPreferences.showTimeInfo)
                {
                    drawTimelineConnetion(node.timePointer.arrowRect.center);
                    Color col = node.getColorByNodeType();                                         
                    DrawTimelineAnimationLength(node, col, WorkPreferences.timelineRectOpacity);

                    if(WorkPreferences.showSubGraph)
                        DrawSubGraphNodes(node);
                }
            }
        }

        DrawTicks(viewRect, smallTickSpacing, 0.55f, Color.grey, false);
        DrawTicks(viewRect, bigTickSpacing, 0.35f, Color.black, true);

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
                GUIStyle g = new GUIStyle();
                g.fontSize = (int)(14f / NodeWorkView._zoom);
                Handles.Label(new Vector3((gridSpacing * x) - (gridSpacing * 0.2f), 0f, 0f), str,g);
            }
        }
        
        Handles.EndGUI();
    }    

    private void setTickSpacing()
    {
        if (NodeWorkView._zoom > 0.5f && NodeWorkView._zoom < 0.75f)
        {
            smallTickSpacing = 10f;
            bigTickSpacing = 100f;
            NodeBase.snapSize = 50f;
        }
        else if (NodeWorkView._zoom > 0.75f && NodeWorkView._zoom < 1f)
        {
            smallTickSpacing = 10f;
            bigTickSpacing = 100f;
            NodeBase.snapSize = 10f;
        }
        else if (NodeWorkView._zoom > 1f && NodeWorkView._zoom < 1.25f)
        {
            smallTickSpacing = 10f;
            bigTickSpacing = 50f;
            NodeBase.snapSize = 1f;
        }

        //smallTickSpacing = 10f / NodeWorkView._zoom;
        //bigTickSpacing = 50f / NodeWorkView._zoom;
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

    private void DrawTimelineAnimationLength(NodeBase node, Color col, float opacity)
    {
        Color newCol = new Color(col.r, col.g, col.b, opacity);        
        Vector2 startVec = node.timePointer.GetStartAnimPos();
        Vector2 endVec = node.timePointer.GetEndAnimPos();
        EditorGUI.DrawRect(new Rect(new Vector2(startVec.x, 0f), new Vector2(endVec.x - startVec.x, viewRect.height)), newCol);
    }

    private void DrawSubGraphNodes(NodeBase n)
    {
        if (n.nodeType == NodeType.Graph)
        {
            float space = 20f / n.parentGraph.zoom;
            for(int i = 0; i < n.nodeGraph.nodes.Count; i++)
            {
                Vector2 start = n.nodeGraph.nodes[i].timePointer.GetStartAnimPos();
                start.y = (i * 2.5f) / n.parentGraph.zoom + space;
                Vector2 end = n.nodeGraph.nodes[i].timePointer.GetEndAnimPos();
                end.y = (i * 2.5f) / n.parentGraph.zoom + space;

                Handles.color = n.nodeGraph.nodes[i].getColorByNodeType();

                Handles.DrawLine(start,end);
                start.y += 0.5f;
                end.y += 0.5f;
                Handles.DrawLine(start, end);
                start.y += 0.5f;
                end.y += 0.5f;
                Handles.DrawLine(start, end);
            }
        }
    }
}
