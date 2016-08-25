using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class JSONUtilities
{
    public static List<NodeDescriptor> getNodeTypes()
    {
        string jsonString = File.ReadAllText("nodeTypes.json");

        Debug.Log(jsonString);

        List<NodeDescriptor> l = JsonConvert.DeserializeObject<List<NodeDescriptor>>(jsonString);

        return null;
    }

}

