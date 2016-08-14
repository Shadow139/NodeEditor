using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;

[Serializable]
public class AdditionNode : NodeBase
{
    public float nodeSum;
    private bool multiInput = false;
    private int numberOfInputs = 2;
    private GUIStyle labelGuiStyle = new GUIStyle();

    public AdditionNode()
    {
        nodeOutputs.Add(new NodeOutput());
        nodeInputs.Add(new NodeInput());
        nodeInputs.Add(new NodeInput());

        labelGuiStyle.fontSize = 16;
        labelGuiStyle.normal.textColor = Color.white;
        labelGuiStyle.fontStyle = FontStyle.Bold;
    }

    public override void InitNode()
    {
        base.InitNode();
        nodeType = NodeType.Addition;
        nodeRect = new Rect(10f, 10f, 160f, 100f);
    }

    public override void UpdateNode(Event e, Rect viewRect)
    {
        base.UpdateNode(e, viewRect);
    }

#if UNITY_EDITOR

    public override void UpdateNodeGUI(Event e, Rect viewRect, GUISkin guiSkin)
    {
        base.UpdateNodeGUI(e, viewRect, guiSkin);

        GUI.Label(new Rect(nodeRect.x + nodeRect.width * 0.5f - 10f, nodeRect.y + nodeRect.height * 0.5f - 10f, nodeRect.width * 0.5f - 10f, 20f), nodeSum.ToString(), labelGuiStyle);
        GUI.Label(new Rect(nodeRect.x + 14f, nodeRect.y + nodeRect.height * 0.5f - 10f, nodeRect.width * 0.2f - 10f, 20f), nodeInputs.Count + "", labelGuiStyle);


        if (Handles.Button(new Vector3(nodeRect.x, nodeRect.y,0f), Quaternion.identity, 1f, 2f, DrawFunc))
        {
            Debug.Log("hello");
        }

        drawOutputHandles(guiSkin);
        drawInputHandles(guiSkin);

        if (!multiInput)
        {
            nodeRect.height = 40f + (30f * nodeInputs.Count);
        }
        else
        {
            nodeRect.height = 100f;
        }

        evaluateNode();
        DrawInputLines();
    }

    void DrawFunc(int controlId, Vector3 position, Quaternion rotation,float size)
    //Draw the button
    {
        //You can draw other stuff than cube, but i havent found something better as a "Button" than cube
        Handles.DrawCube(controlId, position, rotation, size);
    }

    public void drawInputHandles(GUISkin guiSkin)
    {
        if (multiInput)
        {   
            //Multi Input Box
            if (GUI.Button(new Rect(nodeRect.x - 16f, nodeRect.y + (nodeRect.height * 0.5f) - 30f, 16f, 60f), "", guiSkin.GetStyle("node_multiInput")))
            {            
                for (int i = 0; i < nodeInputs.Count; i++)
                {
                    if (parentGraph != null)
                    {
                        if(nodeInputs[i].inputNode == null)
                        {
                            nodeInputs[i].inputNode = parentGraph.connectionNode;
                            nodeInputs[i].isOccupied = nodeInputs[i].inputNode != null;

                            parentGraph.wantsConnection = false;
                            parentGraph.connectionNode = null;
                            Debug.Log("Connected");
                        }
                        else
                        {
                            if (parentGraph.wantsConnection)
                            {
                                nodeInputs.Add(new NodeInput());
                                nodeInputs[i++].inputNode = parentGraph.connectionNode;
                                nodeInputs[i++].isOccupied = nodeInputs[i++].inputNode != null;

                                parentGraph.wantsConnection = false;
                                parentGraph.connectionNode = null;
                                Debug.Log("Added NodeInput and Connected");
                            }
                        }
                    }
                }
            }
        }
        else
        {
            //Single Input Circles
            for (int i = 0; i < nodeInputs.Count; i++)
            {
                if (GUI.Button(new Rect(nodeRect.x - 10f, nodeRect.y + (nodeRect.height * (1f/(nodeInputs.Count + 1))) * (i + 1) - 10f, 20f, 20f), "", guiSkin.GetStyle("node_input")))
                {
                    if (parentGraph != null)
                    {
                        nodeInputs[i].inputNode = parentGraph.connectionNode;
                        nodeInputs[i].isOccupied = nodeInputs[i].inputNode != null;

                        parentGraph.wantsConnection = false;
                        parentGraph.connectionNode = null;
                        Debug.Log("Connected");
                    }
                }
            }
        }
    }

    public void drawOutputHandles(GUISkin guiSkin)
    {
        // Output
        if (GUI.Button(new Rect(nodeRect.x + nodeRect.width - 10f, nodeRect.y + nodeRect.height * 0.5f - 10f, 20f, 20f), "", guiSkin.GetStyle("node_output")))
        {
            if (parentGraph != null)
            {
                parentGraph.wantsConnection = true;
                parentGraph.connectionNode = this;
            }
        }
    }

    private void DrawInputLines()
    {
        Handles.BeginGUI();

        Handles.color = Color.white;

        for (int i = 0; i < nodeInputs.Count; i++)
        {
            if (nodeInputs[i].inputNode != null && nodeInputs[i].isOccupied)
            {
                DrawNodeConnection(nodeInputs[i], (float) (i + 1));
            }
            else
            {
                nodeInputs[i] = new NodeInput();
            }
        }
        
            Handles.EndGUI();
    }

    private void DrawNodeConnection(NodeInput currentInput, float inputId)
    {
        if (!multiInput)
        {
            DrawUtilities.DrawNodeCurve(currentInput.inputNode.nodeRect, nodeRect, inputId, nodeInputs.Count);
        }
        else
        {
            DrawUtilities.DrawMultiInputNodeCurve(currentInput.inputNode.nodeRect, nodeRect, 1);
        }

        //DrawLine(currentInput, inputId);
    }

    private void DrawLine(NodeInput currentInput, float inputId)
    {
        Handles.DrawLine(new Vector3(
            currentInput.inputNode.nodeRect.x + currentInput.inputNode.nodeRect.width + 10f,
            currentInput.inputNode.nodeRect.y + currentInput.inputNode.nodeRect.height * 0.5f, 0f),
            new Vector3(nodeRect.x - 10f, nodeRect.y + (nodeRect.height * 0.33f) * inputId, 0f));
    }


    public override void evaluateNode()
    {
        float tempSum = 0;
        for (int i = 0; i < nodeInputs.Count; i++)
        {
            if (nodeInputs[i].isOccupied && nodeInputs[i].inputNode != null)
            {
                if (nodeInputs[i].inputNode.GetType() == typeof(FloatNode))
                {
                    tempSum += ((FloatNode)nodeInputs[i].inputNode).nodeValue;
                }
                else if (nodeInputs[i].inputNode.GetType() == typeof(AdditionNode))
                {
                    tempSum += ((AdditionNode)nodeInputs[i].inputNode).nodeSum;
                }
            }  
        }
        nodeSum = tempSum;
    }

    public override void DrawNodeProperties(Rect viewRect, GUISkin guiSkin)
    {
        base.DrawNodeProperties(viewRect, guiSkin);

        GUILayout.Space(20);

        GUILayout.BeginVertical();

        if (nodeInputs[0].isOccupied)
        {
            EditorGUILayout.LabelField("Value :", nodeSum.ToString("F"));
        }

        GUILayout.Space(10);

        multiInput = EditorGUILayout.Toggle("Multi Input", multiInput);

        GUILayout.Space(10);

        if (!multiInput)
        {
            numberOfInputs = EditorGUILayout.IntField("Number of InputHandles", nodeInputs.Count, guiSkin.GetStyle("property_view"));

            resizeInputHandles(numberOfInputs);
            //TODO Resize the InputHandlesList
        }

        GUILayout.EndHorizontal();
    }

#endif

    private void resizeInputHandles(int size)
    {
        if(nodeInputs.Count > size)
        {
            for (int i = 0; i < nodeInputs.Count; i++)
            {
                if(i >= size)
                {
                    nodeInputs.RemoveAt(i);
                }
            }
        }else if(nodeInputs.Count < size)
        {
            int numberOfInputsToAdd = size - nodeInputs.Count;
            for (int i = 0; i < numberOfInputsToAdd; i++)
            {
                nodeInputs.Add(new NodeInput());
            }
        }
    }

}
