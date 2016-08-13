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
    public NodeOutput output;

    private bool multiInput = false;
    private GUIStyle labelGuiStyle = new GUIStyle();

    public AdditionNode()
    {
        output = new NodeOutput();
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

        // Output
        if (GUI.Button(new Rect(nodeRect.x + nodeRect.width - 12f, nodeRect.y + nodeRect.height * 0.5f - 12f, 24f, 24f), "", guiSkin.GetStyle("node_output")))
        {
            if (parentGraph != null)
            {
                parentGraph.wantsConnection = true;
                parentGraph.connectionNode = this;
            }
        }

        if (multiInput)
        {
            if (GUI.Button(new Rect(nodeRect.x - 12f, nodeRect.y + (nodeRect.height * 0.33f) - 12f, 24f, 48f), "", guiSkin.GetStyle("node_input")))
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

        evaluateNode();
        DrawInputLines();
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
        if (nodeInputs[0].isOccupied)
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
        
        if (nodeInputs[0].isOccupied)
        {
            EditorGUILayout.LabelField("Value :", nodeSum.ToString("F"));
        }
    }

#endif

}
