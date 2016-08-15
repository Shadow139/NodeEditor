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

    public override void UpdateNodeGUI(Event e, Rect viewRect, GUISkin guiSkin)
    {        
        base.UpdateNodeGUI(e, viewRect, guiSkin);

        drawOutputHandles(guiSkin);
        drawInputHandles(guiSkin);

        if (!multiInput)
        {
            nodeRect.height = 40f + (30f * nodeInputs.Count);
            if (nodeInputs.Count == 0)
                nodeRect.height = 60f;
        }
        else
        {
            nodeRect.height = 100f;
        }

        evaluateNode();
        DrawInputLines();

        GUI.Label(new Rect(nodeRect.x + nodeRect.width * 0.5f - 10f, nodeRect.y + nodeRect.height * 0.5f - 10f, nodeRect.width * 0.5f - 10f, 20f), nodeSum.ToString(), labelGuiStyle);
        if (multiInput)
            GUI.Label(new Rect(nodeRect.x - 12f, nodeRect.y + nodeRect.height * 0.5f - 10f, nodeRect.width * 0.2f - 10f, 20f), nodeInputs.Count + "", guiSkin.GetStyle("std_whiteText"));

        if (Handles.Button(new Vector3(nodeRect.x, nodeRect.y, 0f), Quaternion.identity, 1f, 2f, DrawFunc))
        {
            Debug.Log("hello");
        }
    }

    void DrawFunc(int controlId, Vector3 position, Quaternion rotation,float size)
    //Draw the button
    {
        //You can draw other stuff than cube, but i havent found something better as a "Button" than cube
        Handles.DrawCube(controlId, position, rotation, size);
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

    //deprecated 
    public void DrawLine(NodeInput currentInput, float inputId)
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

        GUILayout.BeginVertical();

        GUILayout.Space(20);

        EditorGUILayout.LabelField("Value :", nodeSum.ToString("F"));

        base.DrawNodeProperties(viewRect, guiSkin);

        GUILayout.EndVertical();
    }

}
