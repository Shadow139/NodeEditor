using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

public static class DrawUtilities
{
    public static void DrawNodeCurve(Rect start, Rect end, float inputId, int numberOfInputs)
    {
        Vector3 startPos = new Vector3(start.x + start.width + 10f, start.y + start.height * 0.5f, 0);
        Vector3 endPos = new Vector3(end.x - 10f, end.y + (end.height * (1f / (numberOfInputs + 1))) * inputId, 0);
        Vector3 startTan = startPos + Vector3.right * 50;
        Vector3 endTan = endPos + Vector3.left * 50;
        Color shadowCol = new Color(0, 0, 0, 0.06f);

        for (int i = 0; i < 3; i++) // Draw a shadow
            Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);

        Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 2);
    }

    public static void DrawMultiInputNodeCurve(Rect start, Rect end, float inputId)
    {
        Vector3 startPos = new Vector3(start.x + start.width + 10f, start.y + start.height * 0.5f, 0);
        Vector3 endPos = new Vector3(end.x - 10f, end.y + (end.height * 0.5f) * inputId, 0);
        Vector3 startTan = startPos + Vector3.right * 50;
        Vector3 endTan = endPos + Vector3.left * 50;
        Color shadowCol = new Color(0, 0, 0, 0.06f);

        for (int i = 0; i < 3; i++) // Draw a shadow
            Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);

        Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 2);
    }

    public static void DrawMouseCurve(Rect start, Vector3 mousePosition)
    {
        Vector3 startPos = new Vector3(start.x + start.width + 10f, start.y + start.height * 0.5f, 0);
        Vector3 endPos = mousePosition;
        Vector3 startTan = startPos + Vector3.right * 50;
        Vector3 endTan = endPos + Vector3.left * 50;
        Color shadowCol = new Color(0, 0, 0, 0.06f);

        for (int i = 0; i < 3; i++) // Draw a shadow
            Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);

        Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 2);
    }
}
