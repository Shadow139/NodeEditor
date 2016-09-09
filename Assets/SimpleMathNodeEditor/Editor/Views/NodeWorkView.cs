using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;

public class NodeWorkView : ViewBaseClass
{
    #region Variables
    private Vector2 mousePos;
    private Rect workSpaceRect;
    private NodeBase NodeToDelete;
    public bool isInsidePropertyView;

    private NodeTimelineView currentTimelineView;
    private List<NodeDescriptor> typesOfNodes;

    private float panX = 0;
    private float panY = 0;

    private const float kZoomMin = 0.6f; // max zoom out
    private const float kZoomMax = 1.25f; // max zoom in
    public static float _zoom = 1.0f;
    private readonly Rect _zoomArea = new Rect(0.0f, 0.0f, 10000f, 10000f);

    #endregion

    #region Constructors
    public NodeWorkView() : base("Node View") {
        currentTimelineView = new NodeTimelineView();
        typesOfNodes = XMLUtilities.getNodeTypes();
    }
    #endregion

    #region Methods
    public override void UpdateView(Rect editorRect, Rect percentageRect, Event e, NodeGraph nodeGraph)
    {
        base.UpdateView(editorRect, percentageRect, e, nodeGraph);

        if(currentTimelineView == null)
            currentTimelineView = new NodeTimelineView();

        if (currentNodeGraph != null)
        {
            viewTitle = currentNodeGraph.graphName;
            currentNodeGraph.isInsidePropertyView = isInsidePropertyView;
            currentNodeGraph.zoom = _zoom;
            currentNodeGraph.panX = panX;
            currentNodeGraph.panY = panY;
        }
        else{   viewTitle = "No Graph Loaded.";     }

        workSpaceRect = new Rect(panX, panY, 10000, 10000);

        EditorZoomArea.Begin(_zoom, _zoomArea);

        GUI.BeginGroup(workSpaceRect);
        GUI.Box(new Rect(0, 0, 10000, 10000), viewTitle, viewSkin.GetStyle("bg_view"));

        //Draw a Grid
        DrawUtilities.DrawGrid(new Rect(0, 0, 10000, 10000), EditorPreferences.gridSpacingDark, EditorPreferences.gridColorOuter);
        DrawUtilities.DrawGrid(new Rect(0, 0, 10000, 10000), EditorPreferences.gridSpacingLight, EditorPreferences.gridColorInner);

        GUILayout.BeginArea(new Rect(0, 0, 10000, 10000));

        if (currentNodeGraph != null)
        {
            currentNodeGraph.UpdateGraphGUI(e, viewRect, new Rect(0, 0, 10000, 10000), viewSkin);
        }

        DrawStepOutOfNode(viewRect);

        GUILayout.EndArea();

        currentTimelineView.UpdateView(new Rect(panX, -panY + (viewRect.height / _zoom) - (40f / _zoom), 10000, 40f / _zoom),
                new Rect(0f, 1f, 1f, 1f), e, currentNodeGraph);
        currentTimelineView.ProcessEvents(e);

        EditorZoomArea.End();
        GUI.EndGroup();
    }

    public override void ProcessEvents(Event e)
    {
        base.ProcessEvents(e);

        if (workSpaceRect.Contains(e.mousePosition))
        {
            if(e.button == 0) // Left Mouseclick
            {
                if(e.type == EventType.MouseDown && e.clickCount == 2)
                {
                    if(currentNodeGraph != null)
                    {
                        NodeBase n = currentNodeGraph.nodes.FirstOrDefault(x => x.nodeRect.Contains(currentNodeGraph.mousePos));
                        if(n != null)
                        {
                            if(n.nodeType == NodeType.Graph)
                            {
                                NodeUtilities.DisplayGraph(n.nodeGraph);
                            }
                        }
                    }
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
                        NodeToDelete = currentNodeGraph.nodes.FirstOrDefault(x => x.nodeRect.Contains(currentNodeGraph.mousePos));
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
                    panX += e.delta.x;
                    panY += e.delta.y;
                    currentTimelineView.scrollViewRect(panX);
                }
            }

            if (Event.current.type == EventType.ScrollWheel)
            {
                Vector2 screenCoordsMousePos = Event.current.mousePosition;
                Vector2 delta = Event.current.delta;
                Vector2 zoomCoordsMousePos = ConvertScreenCoordsToZoomCoords(screenCoordsMousePos);
                float zoomDelta = -delta.y / 150.0f;
                float oldZoom = _zoom;
                _zoom += zoomDelta;
                _zoom = Mathf.Clamp(_zoom, kZoomMin, kZoomMax);
                mousePos += (zoomCoordsMousePos - mousePos) - (oldZoom / _zoom) * (zoomCoordsMousePos - mousePos);

                Event.current.Use();
            }
        }

        if(e.keyCode == KeyCode.Delete)
        {
            //TODO Delete Selected Nodes
        }

        //Limit Panning Outside of Bounds
        if (panX > 0)
            panX = 0;

        if (panY > 0)
            panY = 0;
    }

    //Draws The StepOut Button to return to the previous Graph
    public void DrawStepOutOfNode(Rect viewRect)
    {
        if (currentNodeGraph != null)
        {
            if (currentNodeGraph.graphNode != null)
            {
                if (GUI.Button(new Rect(viewRect.x + (viewRect.width * 0.5f) - panX, viewRect.y, 250f, 50f), "Step Out"))
                {
                    if (currentNodeGraph.graphNode.nodeGraph != null)
                        NodeUtilities.DisplayGraph(currentNodeGraph.graphNode.parentGraph);
                }
            } 
        }
    }    

    private Vector2 ConvertScreenCoordsToZoomCoords(Vector2 screenCoords)
    {
        return (screenCoords - _zoomArea.TopLeft()) / _zoom + mousePos;
    }
    #endregion

    #region Utilities
    private void ProcessContextMenu(Event e, int contextMenuID)
    {
        GenericMenu menu = new GenericMenu();

        if (contextMenuID == 0)
        {
            menu.AddItem(new GUIContent("Create Graph"), false, ContextCallback, "createGraph");
            menu.AddItem(new GUIContent("Load Graph"), false, ContextCallback, "loadGraph");

            if (currentNodeGraph != null)
            {
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Unload Graph"), false, ContextCallback, "unloadGraph");

                menu.AddSeparator("");

                for(int i = 0; i < typesOfNodes.Count; i++)
                {
                    if(typesOfNodes[i].nodeName != "Group Node")
                        menu.AddItem(new GUIContent("Add " + typesOfNodes[i].nodeName), false, ContextCallback, i);
                }

                menu.AddItem(new GUIContent("Add Graph Node"), false, ContextCallback, "createGroupNode");
                menu.AddItem(new GUIContent("Load Existing Graph Node"), false, ContextCallback, "loadGroupNode");
            }
        }

        if (contextMenuID == 1)
        {
            if (currentNodeGraph != null)
            {
                menu.AddItem(new GUIContent("Delete Node"), false, ContextCallback, "deleteNode");
                if(NodeToDelete.nodeType == NodeType.Graph)
                {
                    menu.AddItem(new GUIContent("Step into NodeGraph"), false, ContextCallback, "stepIntoNode");
                }
            }

        }
        menu.ShowAsContext();
        e.Use();
    }

    private void ContextCallback(object obj)
    {
        switch (obj.ToString())
        {
            case "createGraph":
                NodePopupWindow.InitNodePopup();
                Debug.Log("Creating New Graph");
                break;
            case "loadGraph":
                NodeUtilities.LoadGraph();
                Debug.Log("Loading Graph");
                break;
            case "unloadGraph":
                NodeUtilities.UnloadGraph();
                Debug.Log("Unloading Graph");
                break;                
            case "createGroupNode":
                if (currentNodeGraph != null)
                {
                    NodeBase currentNode = NodeUtilities.CreateNode(getGroupNodeDescriptor());
                    NodeGraphPopupWindow.InitNodePopup(currentNode, currentNodeGraph);
                    currentNode.InitNodeFromDescriptor(getGroupNodeDescriptor());
                    NodeUtilities.positionNode(currentNode, currentNodeGraph, currentNodeGraph.mousePos);
                    NodeUtilities.saveNode(currentNode, currentNodeGraph);
                }
                Debug.Log("New Graph Node added");
                break;
            case "loadGroupNode":
                if (currentNodeGraph != null)
                {
                    NodeBase currentNode = NodeUtilities.CreateNode(getGroupNodeDescriptor());
                    NodeGraph graph = NodeUtilities.getSavedNodegraph();
                    currentNode.nodeGraph = graph;
                    graph.graphNode = currentNode;
                    currentNode.nodeName = graph.graphName;
                    currentNode.InitNodeFromDescriptor(getGroupNodeDescriptor());
                    NodeUtilities.positionNode(currentNode, currentNodeGraph, currentNodeGraph.mousePos);
                    NodeUtilities.saveNode(currentNode, currentNodeGraph);
                }
                Debug.Log("Existing Graph Node added");
                break;
            case "deleteNode":
                Debug.Log("Deleting Node");
                NodeUtilities.DeleteNode(NodeToDelete, currentNodeGraph);
                break;
            case "stepIntoNode":
                NodeUtilities.DisplayGraph(NodeToDelete.nodeGraph);
                Debug.Log("Stepping into NodeGraph");
                break;
            default:
                //Normal Node Creation from the XML Data Get the index of the XML NodeDescriptor and create a node
                int index = Convert.ToInt16(obj.ToString());
                NodeUtilities.CreateNode(currentNodeGraph, typesOfNodes[index], currentNodeGraph.mousePos);
                break;
        }
    }

    private NodeDescriptor getGroupNodeDescriptor()
    {
        foreach (NodeDescriptor n in typesOfNodes)
        {
            if (n.nodeName == "Group Node")
                return n;
        }
        return null;
    }

    #endregion

}
