using UnityEngine;
using UnityEditor;
using System.Collections;

public static class NodeUtilities
{
    public static void CreateNewGraph(string wantedName)
    {
        NodeGraph currentNodeGraph = (NodeGraph) ScriptableObject.CreateInstance<NodeGraph>();

        if(currentNodeGraph != null)
        {
            currentNodeGraph.graphName = wantedName;
            currentNodeGraph.InitGraph();

            AssetDatabase.CreateAsset(currentNodeGraph, "Assets/SimpleMathNodeEditor/Database/" + wantedName + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            NodeEditorWindow currentWindow = (NodeEditorWindow) EditorWindow.GetWindow<NodeEditorWindow>();
            if(currentWindow != null)
            {
                currentWindow.currentNodeGraph = currentNodeGraph;
            }
        }
        else
        {
            EditorUtility.DisplayDialog("Node Message", "Unable to create new graph!", "OK");
        }
    }

    public static void LoadGraph()
    {
        NodeGraph currentGraph = null;

        string graphPath = EditorUtility.OpenFilePanel("Load Graph", Application.dataPath + "/SimpleMathNodeEditor/Database/","");

        int pathLength = Application.dataPath.Length;

        string finalPath = graphPath.Substring(pathLength - 6); // remove .asset extension from Path

        currentGraph = (NodeGraph) AssetDatabase.LoadAssetAtPath(finalPath, typeof(NodeGraph));

        if(currentGraph != null)
        {
            NodeEditorWindow currentWindow = (NodeEditorWindow)EditorWindow.GetWindow<NodeEditorWindow>();
            if(currentGraph != null)
            {
                currentWindow.currentNodeGraph = currentGraph;
            }
        }
        else
        {
            EditorUtility.DisplayDialog("Node Message", "Unable to load graph!", "OK");
        }

    }

    public static void UnloadGraph()
    {
        NodeEditorWindow currentWindow = (NodeEditorWindow)EditorWindow.GetWindow<NodeEditorWindow>();
        if (currentWindow != null)
        {
            currentWindow.currentNodeGraph = null;
        }
    }

    public static void CreateNode(NodeGraph currentGraph, NodeType nodeType, Vector2 mousePos)
    {
        if(currentGraph != null)
        {
            NodeBase currentNode = null;
            switch (nodeType)
            {
                case NodeType.Float:
                    currentNode = (FloatNode) ScriptableObject.CreateInstance<FloatNode>();
                    currentNode.nodeName = "Float";
                    break;
                case NodeType.Addition:
                    currentNode = (AdditionNode)ScriptableObject.CreateInstance<AdditionNode>();
                    currentNode.nodeName = "Addition";
                    break;
                default:
                    break;
            }

            if(currentNode != null)
            {
                currentNode.InitNode();
                currentNode.nodeRect.x = mousePos.x;
                currentNode.nodeRect.y = mousePos.y;
                currentNode.parentGraph = currentGraph;
                currentGraph.nodes.Add(currentNode);

                AssetDatabase.AddObjectToAsset(currentNode, currentGraph);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }

    public static void DeleteNode(NodeBase currentNode, NodeGraph currentGraph)
    {
        if (currentGraph != null)
        {
            currentGraph.nodes.Remove(currentNode);
            GameObject.DestroyImmediate(currentNode, true);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }


    public static void DrawGrid(Rect viewRect, float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(viewRect.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(viewRect.height / gridSpacing);

        Handles.BeginGUI();

        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        for (int x = 0; x < widthDivs; x++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * x, 0f, 0f), new Vector3(gridSpacing * x, viewRect.height, 0f));
        }

        for (int y = 0; y < heightDivs; y++)
        {
            Handles.DrawLine(new Vector3(0f, gridSpacing * y, 0f), new Vector3(viewRect.width, gridSpacing * y, 0f));
        }

        Handles.EndGUI();
    }
}
