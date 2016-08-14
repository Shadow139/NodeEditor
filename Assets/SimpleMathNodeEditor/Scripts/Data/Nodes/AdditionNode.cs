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

        GUI.Label(new Rect(nodeRect.x + nodeRect.width * 0.5f - 12f, nodeRect.y + nodeRect.height * 0.5f - 6f, nodeRect.width * 0.5f - 12f, 24f), nodeSum.ToString(), labelGuiStyle);

        if (Handles.Button(new Vector3(nodeRect.x, nodeRect.y,0f), Quaternion.identity, 1f, 2f, DrawFunc))
        {
            Debug.Log("hello");
        }

        drawOutputHandles(guiSkin);
        drawInputHandles(guiSkin);
        
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
            if (GUI.Button(new Rect(nodeRect.x - 20f, nodeRect.y + (nodeRect.height * 0.5f) - 25f, 20f, 50f), "", guiSkin.GetStyle("node_multiInput")))
            {
                for (int i = 0; i < nodeInputs.Count; i++)
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
        else
        {
            //Input
            for (int i = 0; i < nodeInputs.Count; i++)
            {
                if (GUI.Button(new Rect(nodeRect.x - 12f, nodeRect.y + (nodeRect.height * 0.33f) * (i + 1) - 12f, 24f, 24f), "", guiSkin.GetStyle("node_input")))
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
        if (GUI.Button(new Rect(nodeRect.x + nodeRect.width - 12f, nodeRect.y + nodeRect.height * 0.5f - 12f, 24f, 24f), "", guiSkin.GetStyle("node_output")))
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
        DrawUtilities.DrawNodeCurve(currentInput.inputNode.nodeRect, nodeRect, inputId);
        //DrawLine(currentInput, inputId);
    }

    private void DrawLine(NodeInput currentInput, float inputId)
    {
        Handles.DrawLine(new Vector3(
            currentInput.inputNode.nodeRect.x + currentInput.inputNode.nodeRect.width + 12f,
            currentInput.inputNode.nodeRect.y + currentInput.inputNode.nodeRect.height * 0.5f, 0f),
            new Vector3(nodeRect.x - 12f, nodeRect.y + (nodeRect.height * 0.33f) * inputId, 0f));
    }


    public override void evaluateNode()
    {
        if (nodeInputs[0].isOccupied && nodeInputs[1].isOccupied)
        {
            float tempSum = 0;
            for (int i = 0; i < nodeInputs.Count; i++)
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
            nodeSum = tempSum;
        }
    }

    public override void DrawNodeProperties(Rect viewRect, GUISkin guiSkin)
    {
        base.DrawNodeProperties(viewRect, guiSkin);

        GUILayout.Space(20);

        GUILayout.BeginVertical();
        multiInput = EditorGUILayout.Toggle("Multi Input", multiInput);

        if (nodeInputs[0].isOccupied)
        {
            EditorGUILayout.LabelField("Value :", nodeSum.ToString("F"));
        }

        GUILayout.EndHorizontal();
    }

#endif

}
