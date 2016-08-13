using UnityEngine;
using UnityEditor;
using System.Collections;

public class NodeGraphPopupWindow : EditorWindow
{
    #region Variables
    private static NodeGraphPopupWindow currentNodePopupWindow;
    private NodeBase node;
    private string wantedName = "Enter a name...";
    #endregion

    public static void InitNodePopup(NodeBase node)
    {
        currentNodePopupWindow = (NodeGraphPopupWindow)EditorWindow.GetWindow<NodeGraphPopupWindow>();
        currentNodePopupWindow.node = node;
        currentNodePopupWindow.title = "Create a new GraphNode";
    }

    void OnGUI()
    {
        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.Space(20);

        GUILayout.BeginVertical();

        EditorGUILayout.LabelField("Create New GraphNode", EditorStyles.boldLabel);

        wantedName = EditorGUILayout.TextField("Enter GraphNode Name: ", wantedName);

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Create GraphNode", GUILayout.Height(40)))
        {
            if (!string.IsNullOrEmpty(wantedName) && wantedName != "Enter a name...")
            {
                NodeGraph graph = NodeUtilities.CreateAndSaveGraph(wantedName);
                ((GraphNode)node).nodeGraph = graph;
                node.nodeName = wantedName;

                currentNodePopupWindow.Close();
            }
            else
            {
                EditorUtility.DisplayDialog("Node Message:", "Please enter a valid GraphNode name!", "OK");
            }
        }
        if (GUILayout.Button("Cancel", GUILayout.Height(40)))
        {
            currentNodePopupWindow.Close();
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        GUILayout.Space(20);
        GUILayout.EndHorizontal();
        GUILayout.Space(20);
    }
}
