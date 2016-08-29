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
        nodeRect = new Rect(10f, 10f, 150f, 100f);
    }

    public override void UpdateNodeGUI(Event e, Rect viewRect, Rect workViewRect, GUISkin guiSkin)
    {        
        base.UpdateNodeGUI(e, viewRect, workViewRect, guiSkin);

        drawOutputHandles(guiSkin);
        drawInputHandles(guiSkin);

        evaluateNode();
        DrawInputLines();

        GUI.Label(new Rect(nodeRect.x + nodeRect.width * 0.5f - 10f, nodeRect.y + nodeRect.height * 0.5f - 10f, nodeRect.width * 0.5f - 10f, 20f), nodeSum.ToString(), labelGuiStyle);   
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
                else if (nodeInputs[i].inputNode.GetType() == typeof(GraphNode))
                {
                    tempSum += ((AdditionNode)((GraphNode)nodeInputs[i].inputNode).nodeOutputs[0].outputNode).nodeSum;
                }
            }
        }
        nodeSum = tempSum;
    }

    public override void DrawNodeProperties(Rect viewRect, GUISkin guiSkin)
    {
        EditorGUILayout.LabelField("Value :", nodeSum.ToString("F"));

        base.DrawNodeProperties(viewRect, guiSkin);
    }

}
