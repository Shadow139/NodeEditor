using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class NodePropertyView : ViewBaseClass
{
    // Just for showing stuff
    AnimationCurve curveX = AnimationCurve.Linear(0, 0, 10, 10);
    
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
                    curveX = EditorGUI.CurveField(new Rect(10, 250, viewRect.width - 20, 250), curveX);
                }
            }
            else if (!currentNodeGraph.showProperties)
            {
                //Draw Properties when nothing is selected            
                EditorPreferences.gridColorOuter = EditorGUILayout.ColorField("Outer Color", EditorPreferences.gridColorOuter);
                GUILayout.Space(6f);
                EditorPreferences.gridColorInner = EditorGUILayout.ColorField("Inner Color", EditorPreferences.gridColorInner);
                GUILayout.Space(6f);
                EditorPreferences.gridSpacingDark = EditorGUILayout.FloatField("Grid Spacing Outer", EditorPreferences.gridSpacingDark);
                GUILayout.Space(6f);
                EditorPreferences.gridSpacingLight = EditorGUILayout.FloatField("Grid Spacing Inner", EditorPreferences.gridSpacingLight);
                GUILayout.Space(6f);
                WorkPreferences.showTimeInfo = EditorGUILayout.Toggle("showTimeInfo", WorkPreferences.showTimeInfo);
                GUILayout.Space(6f);
                GUILayout.BeginHorizontal();
                GUILayout.Label("Snap Interval: ");
                NodeBase.snapSize = EditorGUILayout.IntSlider((int)NodeBase.snapSize, 1, 50);
                GUILayout.EndHorizontal();
            }
        }
        GUILayout.Space(40f);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Zoom: ");
        NodeWorkView._zoom = EditorGUILayout.Slider(NodeWorkView._zoom, 0.7f, 1f);
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    public override void ProcessEvents(Event e)
    {
        base.ProcessEvents(e);        
    }
}
