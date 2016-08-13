using UnityEngine;
using UnityEditor;
using System.Collections;

public class NodePopupWindow : EditorWindow
{
    #region Variables
    private static NodePopupWindow currentNodePopupWindow;
    private string wantedName = "Enter a name...";
    #endregion

    #region Methods
    public static void InitNodePopup()
    {
        currentNodePopupWindow = (NodePopupWindow) EditorWindow.GetWindow<NodePopupWindow>();
        currentNodePopupWindow.title = "Create a new Graph";
    }

    void OnGUI()
    {
        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.Space(20);

        GUILayout.BeginVertical();

        EditorGUILayout.LabelField("Create New Graph", EditorStyles.boldLabel);

        wantedName = EditorGUILayout.TextField("Enter Graph Name: ", wantedName);

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Create Graph", GUILayout.Height(40)))
        {
            if(!string.IsNullOrEmpty(wantedName) && wantedName != "Enter a name...")
            {
                NodeUtilities.CreateNewGraph(wantedName);
                currentNodePopupWindow.Close();
            }
            else
            {
                EditorUtility.DisplayDialog("Node Message:", "Please enter a valid Graph name!", "OK");
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
    #endregion

    #region Utilities
    #endregion

}
