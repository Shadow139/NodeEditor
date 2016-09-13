using UnityEngine;
using System.Collections;

public class GuiStyles
{
    public static GuiStyles _instance;

    public GUIStyle whiteNodeLabel;

    public GuiStyles()
    {
        _instance = this;

        whiteNodeLabel = new GUIStyle();
        whiteNodeLabel.fontSize = 16;
        whiteNodeLabel.normal.textColor = Color.white;
        whiteNodeLabel.fontStyle = FontStyle.Bold;
    }
}
