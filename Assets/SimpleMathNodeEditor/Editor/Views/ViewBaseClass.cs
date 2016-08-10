using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System;

[Serializable]
public class ViewBaseClass
{
    #region public Variables
    public string viewTitle;
    public Rect viewRect;
    #endregion

    #region protected Variables
    protected GUISkin viewSkin;
    protected NodeGraph currentNodeGraph;
    #endregion

    #region Constructors
    public ViewBaseClass(string title)
    {
        this.viewTitle = title;
        GetEditorSkin();
    }
    #endregion

    #region Methods
    public virtual void UpdateView(Rect editorRect, Rect percentageRect, Event e,NodeGraph nodeGraph)
    {
        if (viewSkin == null)
        {
            GetEditorSkin();
            return;
        }

        //set the current view Graph
        this.currentNodeGraph = nodeGraph;

        viewRect = new Rect(editorRect.x * percentageRect.x, 
                            editorRect.y * percentageRect.y, 
                            editorRect.width * percentageRect.width,
                            editorRect.height * percentageRect.height);
    }
    public virtual void ProcessEvents(Event e) { }
    #endregion

    #region Utilities
    protected void GetEditorSkin()
    {
        viewSkin = (GUISkin) Resources.Load("GUISkins/EditorSkins/NodeEditorSkin");
    }
    #endregion
}
