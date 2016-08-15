using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
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
        
        GUILayout.Space(30f);
        GUILayout.BeginVertical();

        if (currentNodeGraph == null || !currentNodeGraph.showProperties)
        {
            //Draw Properties when nothing is selected
            /*
            EditorPreferences.gridColorOuter = EditorGUILayout.ColorField("Outer Color", EditorPreferences.gridColorOuter);
            GUILayout.Space(6f);
            EditorPreferences.gridColorInner = EditorGUILayout.ColorField("Inner Color", EditorPreferences.gridColorInner);
            GUILayout.Space(6f);
            EditorPreferences.gridSpacingDark = EditorGUILayout.FloatField("Grid Spacing Outer", EditorPreferences.gridSpacingDark);
            GUILayout.Space(6f);
            EditorPreferences.gridSpacingLight = EditorGUILayout.FloatField("Grid Spacing Inner", EditorPreferences.gridSpacingLight);
            GUILayout.Space(6f);
            */
        }
        else
        {
            if(currentNodeGraph.selectedNode != null)
            {
                currentNodeGraph.selectedNode.DrawNodeProperties(viewRect, viewSkin);
            }
            curveX = EditorGUI.CurveField(new Rect(10, 250, viewRect.width - 20, 250), curveX);
        }

        //DebugUtilities.drawWindowOutline(editorRect, Color.blue);
        //Debug.Log(viewTitle + " Position: " + viewRect.ToString());

        GUILayout.EndVertical();

        GUILayout.EndArea();
    }

    public override void ProcessEvents(Event e)
    {
        base.ProcessEvents(e);
        
    }
}
