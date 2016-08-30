using UnityEngine;
using System.Collections;

public class GuiStyles
{
    public static GuiStyles _instance;

    public GUISkin guiSkin;
    public GUIStyle whiteNodeLabel;

    public GuiStyles()
    {
        _instance = this;

        GetEditorSkin();

        whiteNodeLabel = new GUIStyle();
        whiteNodeLabel.fontSize = 16;
        whiteNodeLabel.normal.textColor = Color.white;
        whiteNodeLabel.fontStyle = FontStyle.Bold;
    }

    public void GetEditorSkin()
    {
        guiSkin = (GUISkin)Resources.Load("GUISkins/EditorSkins/NodeEditorSkin");
    }
}
