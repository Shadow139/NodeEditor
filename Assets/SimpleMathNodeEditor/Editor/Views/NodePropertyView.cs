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
        GUILayout.BeginHorizontal();

        if (currentNodeGraph == null || !currentNodeGraph.showProperties)
        {
            //Draw Properties when nothing is selected
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

        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    public override void ProcessEvents(Event e)
    {
        base.ProcessEvents(e);
        
    }
}
