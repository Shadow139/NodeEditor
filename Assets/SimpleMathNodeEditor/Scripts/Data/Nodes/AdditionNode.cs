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
    public NodeInput inputOne;
    public NodeInput inputTwo;

    private GUIStyle labelGuiStyle = new GUIStyle();

    public AdditionNode()
    {
        output = new NodeOutput();
        inputOne = new NodeInput();
        inputTwo = new NodeInput();

        labelGuiStyle.fontSize = 16;
        labelGuiStyle.normal.textColor = Color.white;
        labelGuiStyle.fontStyle = FontStyle.Bold;
    }

    public override void InitNode()
    {
        base.InitNode();
        nodeType = NodeType.Addition;
        nodeRect = new Rect(10f, 10f, 200f, 75f);
    }

    public override void UpdateNode(Event e, Rect viewRect)
    {
        base.UpdateNode(e, viewRect);
    }

#if UNITY_EDITOR

    public override void UpdateNodeGUI(Event e, Rect viewRect, GUISkin guiSkin)
    {
        base.UpdateNodeGUI(e, viewRect, guiSkin);

        GUILayout.Space(40f);

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

        // Input 1
        if (GUI.Button(new Rect(nodeRect.x - 12f, nodeRect.y + (nodeRect.height * 0.33f) * 2f - 8f, 24f, 24f), "", guiSkin.GetStyle("node_input")))
        {
            if (parentGraph != null)
            {
                inputOne.inputNode = parentGraph.connectionNode;
                inputOne.isOccupied = inputOne.inputNode != null;

                parentGraph.wantsConnection = false;
                parentGraph.connectionNode = null;

                EditorUtility.SetDirty(this);
            }
        }

        // Input 2
        if (GUI.Button(new Rect(nodeRect.x - 12f, nodeRect.y + (nodeRect.height * 0.33f) - 14f, 24f, 24f), "", guiSkin.GetStyle("node_input")))
        {
            if (parentGraph != null)
            {
                inputTwo.inputNode = parentGraph.connectionNode;
                inputTwo.isOccupied = inputTwo.inputNode != null;

                parentGraph.wantsConnection = false;
                parentGraph.connectionNode = null;
            }
        }
        ProcessNodeFunctionality();
        DrawInputLines();
    }

    private void DrawInputLines()
    {
        Handles.BeginGUI();

        Handles.color = Color.white;

        if (inputOne.inputNode != null && inputOne.isOccupied)
        {
            DrawNodeConnection(inputOne, 2f);
        }
        else
        {
            inputOne = new NodeInput();
        }

        if (inputTwo.inputNode != null && inputTwo.isOccupied)
        {
            DrawNodeConnection(inputTwo, 1f);
        }
        else
        {
            inputTwo = new NodeInput();
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


    private void ProcessNodeFunctionality()
    {
        if (inputOne.isOccupied && inputTwo.isOccupied)
        {
            float one = 0;
            float two = 0;

            if (inputOne.inputNode.GetType() == typeof(FloatNode))
            {
                one = ((FloatNode)inputOne.inputNode).nodeValue;
            }
            else if (inputOne.inputNode.GetType() == typeof(AdditionNode))
            {
                one = ((AdditionNode)inputOne.inputNode).nodeSum;
            }

            if (inputTwo.inputNode.GetType() == typeof(FloatNode))
            {
                two = ((FloatNode)inputTwo.inputNode).nodeValue;
            }
            else if (inputTwo.inputNode.GetType() == typeof(AdditionNode))
            {
                two = ((AdditionNode)inputTwo.inputNode).nodeSum;
            }

            nodeSum = one + two;
        }
    }

    public override void DrawNodeProperties(Rect viewRect, GUISkin guiSkin)
    {
        base.DrawNodeProperties(viewRect, guiSkin);
        
        if (inputOne.isOccupied && inputTwo.isOccupied)
        {
            EditorGUILayout.LabelField("Value :", nodeSum.ToString("F"));
        }
    }

#endif

}
