using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeDescriptor
{
    public NodeType nodeType;
    public string nodeName;
    public string titleBarColor;

    public bool isMultiInput;
    public bool isMultiOutput;

    public int numberOfInputs;
    public int numberOfOutputs;

    public int minInputs;
    public int maxInputs;
    public int minOutputs;
    public int maxOutputs;

    public Dictionary<string, object> parameters;

    public override string ToString()
    {
        return nodeType + "";
    }
}
