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

    public static void restoreDefaults()
    {
        gridSpacingLight = 10f;
        gridSpacingDark = 50f;

        gridColorInner = new Color(Color.black.r, Color.black.g, Color.black.b, 0.10f);
        gridColorOuter = new Color(Color.black.r, Color.black.g, Color.black.b, 0.25f);
        nodeCurveColor = Color.black;

        nodeCurveThickness = 2f;
        timelineCurveOpacityMin = 0.2f;
        timelineCurveOpacityMax = 1f;

        timelineRectOpacity = 0.2f;
        showSubGraph = true;

        savePreferences();
    }

    public static void savePreferences()
    {
        PlayerPrefs.SetInt("prefsSaved", 1);

        PlayerPrefs.SetFloat("gridSpacingLight", gridSpacingLight);
        PlayerPrefs.SetFloat("gridSpacingDark", gridSpacingDark);

        PlayerPrefs.SetFloat("gridColorInner.r", gridColorInner.r);
        PlayerPrefs.SetFloat("gridColorInner.g", gridColorInner.g);
        PlayerPrefs.SetFloat("gridColorInner.b", gridColorInner.b);
        PlayerPrefs.SetFloat("gridColorInner.a", gridColorInner.a);

        PlayerPrefs.SetFloat("gridColorOuter.r", gridColorOuter.r);
        PlayerPrefs.SetFloat("gridColorOuter.g", gridColorOuter.g);
        PlayerPrefs.SetFloat("gridColorOuter.b", gridColorOuter.b);
        PlayerPrefs.SetFloat("gridColorOuter.a", gridColorOuter.a);


        PlayerPrefs.SetFloat("nodeCurveColor.r", nodeCurveColor.r);
        PlayerPrefs.SetFloat("nodeCurveColor.g", nodeCurveColor.g);
        PlayerPrefs.SetFloat("nodeCurveColor.b", nodeCurveColor.b);
        PlayerPrefs.SetFloat("nodeCurveColor.a", nodeCurveColor.a);

        PlayerPrefs.SetFloat("nodeCurveThickness", nodeCurveThickness);
        PlayerPrefs.SetFloat("timelineCurveOpacityMin", timelineCurveOpacityMin);
        PlayerPrefs.SetFloat("timelineCurveOpacityMax", timelineCurveOpacityMax);

        PlayerPrefs.SetFloat("timelineRectOpacity", timelineRectOpacity);

        int toggle = 0;
        if (!showSubGraph)
            toggle = 0;
        else toggle = 1;
        PlayerPrefs.SetInt("showSubGraph", toggle);
    }

    public static void loadPreferences()
    {
        if (PlayerPrefs.GetInt("prefsSaved") == 1)//to check if there are any.
        {
            gridSpacingLight = PlayerPrefs.GetFloat("gridSpacingLight");
            gridSpacingDark = PlayerPrefs.GetFloat("gridSpacingDark");

            float r = PlayerPrefs.GetFloat("gridColorInner.r");
            float g = PlayerPrefs.GetFloat("gridColorInner.g");
            float b = PlayerPrefs.GetFloat("gridColorInner.b");
            float a = PlayerPrefs.GetFloat("gridColorInner.a");

            gridColorInner = new Color(r,g,b,a);

            r = PlayerPrefs.GetFloat("gridColorOuter.r");
            g = PlayerPrefs.GetFloat("gridColorOuter.g");
            b = PlayerPrefs.GetFloat("gridColorOuter.b");
            a = PlayerPrefs.GetFloat("gridColorOuter.a");

            gridColorOuter = new Color(r, g, b, a);


            r = PlayerPrefs.GetFloat("nodeCurveColor.r");
            g = PlayerPrefs.GetFloat("nodeCurveColor.g");
            b = PlayerPrefs.GetFloat("nodeCurveColor.b");
            a = PlayerPrefs.GetFloat("nodeCurveColor.a");

            nodeCurveColor = new Color(r, g, b, a);

            nodeCurveThickness = PlayerPrefs.GetFloat("nodeCurveThickness");
            timelineCurveOpacityMin = PlayerPrefs.GetFloat("timelineCurveOpacityMin");
            timelineCurveOpacityMax = PlayerPrefs.GetFloat("timelineCurveOpacityMax");

            timelineRectOpacity = PlayerPrefs.GetFloat("timelineRectOpacity");

            int toggle = PlayerPrefs.GetInt("showSubGraph");
            if (toggle == 1)
            {
                showSubGraph = true;
            }
            else
            {
                showSubGraph = false;
            }
        }
    }
}
