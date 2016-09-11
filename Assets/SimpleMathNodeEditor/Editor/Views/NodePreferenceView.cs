using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.AnimatedValues;

public class NodePreferenceView : ViewBaseClass
{
    bool showGridControls = false;
    bool showNodeControls = false;
    bool showTimelineControls = false;

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
            GUILayout.Space(10f);

            WorkPreferences.gridColorOuter = EditorGUILayout.ColorField("Grid Outer Color", WorkPreferences.gridColorOuter);
            GUILayout.Space(6f);
            WorkPreferences.gridColorInner = EditorGUILayout.ColorField("Grid Inner Color", WorkPreferences.gridColorInner);
            GUILayout.Space(6f);
            WorkPreferences.gridSpacingDark = EditorGUILayout.FloatField("Grid Spacing Outer", WorkPreferences.gridSpacingDark);
            GUILayout.Space(6f);
            WorkPreferences.gridSpacingLight = EditorGUILayout.FloatField("Grid Spacing Inner", WorkPreferences.gridSpacingLight);
        }

        GUILayout.Space(10f);

        showNodeControls = EditorGUILayout.Foldout(showNodeControls, "Show Node Controls");
        if (showNodeControls)
        {
            GUILayout.Space(10f);

            WorkPreferences.nodeCurveColor = EditorGUILayout.ColorField("Node Curve Color", WorkPreferences.nodeCurveColor);
            GUILayout.Space(6f);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Node Curve Thickness: ");
            WorkPreferences.nodeCurveThickness = EditorGUILayout.Slider(WorkPreferences.nodeCurveThickness, 0.5f, 10f);
            GUILayout.EndHorizontal();

            GUILayout.Space(6f);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Default Curve Opacity: ");
            WorkPreferences.timelineCurveOpacityMin = EditorGUILayout.Slider(WorkPreferences.timelineCurveOpacityMin, 0.0f, 1f);
            GUILayout.EndHorizontal();

            GUILayout.Space(6f);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Highlighted Curve Opacity: ");
            WorkPreferences.timelineCurveOpacityMax = EditorGUILayout.Slider(WorkPreferences.timelineCurveOpacityMax, 0.0f, 1f);
            GUILayout.EndHorizontal();

        }
        GUILayout.Space(10f);

        showTimelineControls = EditorGUILayout.Foldout(showTimelineControls, "Show Timeline Controls");
        if (showTimelineControls)
        {
            GUILayout.Space(10f);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Timeline Opacity: ");
            WorkPreferences.timelineRectOpacity = EditorGUILayout.Slider(WorkPreferences.timelineRectOpacity, 0.0f, 1f);
            GUILayout.EndHorizontal();
            GUILayout.Space(6f);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Show Group Node sub Nodes ");
            WorkPreferences.showSubGraph = EditorGUILayout.Toggle("", WorkPreferences.showSubGraph, GUILayout.Width(500));
            GUILayout.EndHorizontal();

        }
        GUILayout.Space(10f);

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    public override void ProcessEvents(Event e)
    {
        base.ProcessEvents(e);        
    }
}
