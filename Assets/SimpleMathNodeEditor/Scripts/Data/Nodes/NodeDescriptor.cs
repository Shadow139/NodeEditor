using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeDescriptor
{
    private NodeType nodeType { get; set; }
    private string nodeName { get; set; }

    private int numberOfInputs { get; set; }
    private int numberOfOutputs { get; set; }

    private int minInputs { get; set; }
    private int maxInputs { get; set; }
    private int minOutputs { get; set; }
    private int maxOutputs { get; set; }

    private Dictionary<string,object> parameters { get; set; }

}
