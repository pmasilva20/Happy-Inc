using System.Collections;
using System.Collections.Generic;
using ForceDirectedGraph.DataStructure;
using UnityEngine;

public class Bubble
{

    /// <summary>
    /// List of nodes in buble
    /// </summary>
    private List<Node> _Nodes;

    public Bubble(List<Node> nodes)
    {
        _Nodes = nodes;
    }

    public List<Node> Nodes { get { return _Nodes; } }
}
