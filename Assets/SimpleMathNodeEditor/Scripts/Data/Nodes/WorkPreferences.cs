using UnityEngine;
using System.Collections;

public class WorkPreferences {
    //Grid
    public static float gridSpacingLight = 10f;
    public static float gridSpacingDark = 50f;
    public static Color gridColorInner = new Color(Color.black.r, Color.black.g, Color.black.b, 0.10f);
    public static Color gridColorOuter = new Color(Color.black.r, Color.black.g, Color.black.b, 0.25f);

    public static bool showTimeInfo = false;
    //Node
    public static Color nodeCurveColor = Color.black;
    public static float nodeCurveThickness = 2f;
    public static float timelineCurveOpacityMin = 0.2f;
    public static float timelineCurveOpacityMax = 1f;
    //Timeline
    public static float timelineRectOpacity = 0.2f;
    public static bool showSubGraph = true;


}
