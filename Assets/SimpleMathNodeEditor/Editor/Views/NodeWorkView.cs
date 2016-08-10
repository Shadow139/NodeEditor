using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class NodeWorkView : ViewBaseClass
{
    #region Variables
    private Vector2 mousePos;
    protected NodeBase NodeToDelete;

    float panX = 0;
    float panY = 0;
    #endregion


    #region Constructors
    public NodeWorkView() : base("Node View") { }
    #endregion

    #region Methods
    public override void UpdateView(Rect editorRect, Rect percentageRect, Event e, NodeGraph nodeGraph)
    {
        base.UpdateView(editorRect, percentageRect, e, nodeGraph);

        //Update view Title
        if (currentNodeGraph != null)
        {
            viewTitle = currentNodeGraph.graphName;
        }
        else
        {
            viewTitle = "No Graph Loaded.";
        }

        // Testing Group with some test values 
        //TODO Update and improve Performance 
        GUI.BeginGroup(new Rect(panX, panY, 10000, 10000));
        GUI.Box(new Rect(0, 0, 10000, 10000), viewTitle, viewSkin.GetStyle("bg_view"));

        //Draw a Grid
        NodeUtilities.DrawGrid(new Rect(0, 0, 10000, 10000), 60f, 0.15f, Color.black);
        NodeUtilities.DrawGrid(new Rect(0, 0, 10000, 10000), 20f, 0.05f, Color.black);

        GUILayout.BeginArea(new Rect(0, 0, 10000, 10000));

        if (currentNodeGraph != null)
        {
            currentNodeGraph.UpdateGraphGUI(e, new Rect(0, 0, 10000, 10000), viewSkin);
        }

        GUILayout.EndArea();

        GUI.EndGroup();
    }

    public override void ProcessEvents(Event e)
    {
        base.ProcessEvents(e);

        if (viewRect.Contains(e.mousePosition))
        {
            if(e.button == 0) // Left Mouseclick
            {
                if(e.type == EventType.MouseDown)
                {
                    Debug.Log("Left Click in: " + viewTitle);
                }
                if (e.type == EventType.MouseDrag)
                {
                    Debug.Log("LeftMouseDrag in: " + viewTitle);
                }
                if (e.type == EventType.MouseUp)
                {
                    Debug.Log("LeftMouse Released in: ");
                }
            }

            if (e.button == 1) // Right Mouseclick
            {
                if (e.type == EventType.MouseDown)
                {
                    mousePos = e.mousePosition;
                    NodeToDelete = null;
                    if (currentNodeGraph != null)
                    {
                        NodeToDelete = currentNodeGraph.nodes.FirstOrDefault(x => x.nodeRect.Contains(e.mousePosition));
                    }

                    if (NodeToDelete == null)
                    {
                        ProcessContextMenu(e, 0);
                    }
                    else
                    {
                        ProcessContextMenu(e, 1);
                    }
                }
            }

            if(e.button == 2)// Middle Mouseclick
            {
                if (e.type == EventType.MouseDrag)
                {                    
                    Debug.Log("MiddleMouseDrag in: " + viewTitle);
                    //panX += e.mousePosition.x - mousePos.x;
                    //panY += e.mousePosition.y - mousePos.y;

                    //mousePos = e.mousePosition;
                }
            }
        }

        if (e.keyCode == KeyCode.UpArrow)
        {
            Debug.Log("UpArrow: " + panY);
            panY += 10;
        }
        if (e.keyCode == KeyCode.DownArrow)
        {
            Debug.Log("DownArrow: " + panY);
            panY -= 10;
        }
        if (e.keyCode == KeyCode.LeftArrow)
        {
            Debug.Log("LeftArrow: " + panX);
            panX += 10;
        }
        if (e.keyCode == KeyCode.RightArrow)
        {
            Debug.Log("RightArrow: " + panX);
            panX -= 10;
        }

        if (panX > 0)
            panX = 0;

        if (panY > 0)
            panY = 0;
    }

    #endregion

    #region Utilities
    private void ProcessContextMenu(Event e, int contextMenuID)
    {
        GenericMenu menu = new GenericMenu();

        if (contextMenuID == 0)
        {
            menu.AddItem(new GUIContent("Create Graph"), false, ContextCallback, "0");
            menu.AddItem(new GUIContent("Load Graph"), false, ContextCallback, "1");

            if (currentNodeGraph != null)
            {
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Unload Graph"), false, ContextCallback, "2");

                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Add Float Node"), false, ContextCallback, "3");
                menu.AddItem(new GUIContent("Add Addition Node"), false, ContextCallback, "4");

            }
        }

        if (contextMenuID == 1)
        {
            if (currentNodeGraph != null)
            {
                menu.AddItem(new GUIContent("Delete Node"), false, ContextCallback, "5");
            }

        }

        menu.ShowAsContext();
        e.Use();
    }

    private void ContextCallback(object obj)
    {
        switch (obj.ToString())
        {
            case "0":
                Debug.Log("Creating New Graph");
                NodePopupWindow.InitNodePopup();
                break;
            case "1":
                Debug.Log("Loading Graph");
                NodeUtilities.LoadGraph();
                break;
            case "2":
                Debug.Log("Unloading Graph");
                NodeUtilities.UnloadGraph();
                break;
            case "3":
                Debug.Log("Float Node added");
                NodeUtilities.CreateNode(currentNodeGraph, NodeType.Float, mousePos);
                break;
            case "4":
                Debug.Log("Addition Node added");
                NodeUtilities.CreateNode(currentNodeGraph, NodeType.Addition, mousePos);
                break;
            case "5":
                Debug.Log("Deleting Node");
                NodeUtilities.DeleteNode(NodeToDelete, currentNodeGraph);
                break;
            default:
                break;
        }
    }

    #endregion

}
