using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

public static class DebugUtilities
{
    public static void drawWindowOutline(Rect viewRect, Color col)
    {
        Handles.color = col;

        Vector3[] lineSegments = new Vector3[4];
        lineSegments[0] = new Vector3(viewRect.x, viewRect.y, 0);
        lineSegments[1] = new Vector3(viewRect.x + viewRect.width, viewRect.y, 0);
        lineSegments[2] = new Vector3(viewRect.x + viewRect.width, viewRect.y + viewRect.height, 0);
        lineSegments[3] = new Vector3(viewRect.x, viewRect.y + viewRect.height, 0);

        Handles.DrawBezier(lineSegments[0], lineSegments[1], lineSegments[0], lineSegments[1], col, null, 10);
        Handles.DrawBezier(lineSegments[1], lineSegments[2], lineSegments[1], lineSegments[2], col, null, 10);
        Handles.DrawBezier(lineSegments[2], lineSegments[3], lineSegments[2], lineSegments[3], col, null, 10);
        Handles.DrawBezier(lineSegments[3], lineSegments[0], lineSegments[3], lineSegments[0], col, null, 10);
    }
}
