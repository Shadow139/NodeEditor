using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.AnimatedValues;

public class NodePreferenceView : ViewBaseClass
{
    bool showGridControls = false;

    public NodePreferenceView() : base("Preferences") { }
    public override void UpdateView(Rect editorRect, Rect percentageRect, Event e, NodeGraph nodeGraph)
    {
        base.UpdateView(editorRect, percentageRect, e, nodeGraph);
        GUI.Box(viewRect, viewTitle, viewSkin.GetStyle("bg_view"));

        GUILayout.BeginArea(viewRect);

        GUILayout.BeginHorizontal();
        GUILayout.Space(15f);
        GUILayout.BeginVertical();
        GUILayout.Space(40f);

        showGridControls = EditorGUILayout.Foldout(showGridControls, "Show Grid Controls");
        if (showGridControls)
        {
            //Draw Properties when nothing is selected            
            EditorPreferences.gridColorOuter = EditorGUILayout.ColorField("Grid Outer Color", EditorPreferences.gridColorOuter);
            GUILayout.Space(6f);
            EditorPreferences.gridColorInner = EditorGUILayout.ColorField("Grid Inner Color", EditorPreferences.gridColorInner);
            GUILayout.Space(6f);
            EditorPreferences.gridSpacingDark = EditorGUILayout.FloatField("Grid Spacing Outer", EditorPreferences.gridSpacingDark);
            GUILayout.Space(6f);
            EditorPreferences.gridSpacingLight = EditorGUILayout.FloatField("Grid Spacing Inner", EditorPreferences.gridSpacingLight);
            GUILayout.Space(6f);
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
