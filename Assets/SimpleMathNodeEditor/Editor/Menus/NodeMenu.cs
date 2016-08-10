using UnityEngine;
using UnityEditor;
using System.Collections;

public class NodeMenu
{
    [MenuItem("Node Editor/Launch Editor")]
    public static void InitNodeEditor()
    {
        Debug.Log("Node Editor Started!");
        NodeEditorWindow.InitEditorWindow();
    }

}
