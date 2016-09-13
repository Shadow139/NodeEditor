using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class NodePropertyView : ViewBaseClass
{
    public NodePropertyView() : base("Properties") { }

    public override void UpdateView(Rect editorRect, Rect percentageRect, Event e, NodeGraph nodeGraph)
    {
        base.UpdateView(editorRect, percentageRect, e, nodeGraph);
        GUI.Box(viewRect, viewTitle, viewSkin.GetStyle("bg_view"));

        GUILayout.BeginArea(viewRect);

        GUILayout.BeginHorizontal();
        GUILayout.Space(15f);
        GUILayout.BeginVertical();
        GUILayout.Space(40f);
        
        if (currentNodeGraph == null) { }
        else
        {
            if(currentNodeGraph.selectedNodes != null && currentNodeGraph.showProperties)
            {
                if(currentNodeGraph.selectedNodes.Count > 0)
                {
                    currentNodeGraph.selectedNodes[currentNodeGraph.selectedNodes.Count - 1].DrawNodeProperties(viewRect, viewSkin);
                }
            }
        }

        GUILayout.Space(6f);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Snap Interval: ");
        NodeBase.snapSize = EditorGUILayout.IntSlider((int)NodeBase.snapSize, 1, 50);
        GUILayout.EndHorizontal();

        GUILayout.Space(40f);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Zoom: ");
        NodeWorkView._zoom = EditorGUILayout.Slider(NodeWorkView._zoom, 0.6f, 1.25f);
        GUILayout.EndHorizontal();

        GUILayout.Space(450f);

        if (GUILayout.Button("Evaluate"))
        {
            NodeGraph.evaluateTrigger = true;
            Debug.Log("Evaluate pressed");
        }

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    public override void ProcessEvents(Event e)
    {
        base.ProcessEvents(e);        
    }
}
