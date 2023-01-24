using ForceDirectedGraph.DataStructure;
using ForceDirectedGraph;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network = ForceDirectedGraph.DataStructure.Network;

public class LevelManager : MonoBehaviour
{
    /// <summary>
    /// Level Manager
    /// Determines which Level it is and generates a blank network with N initial nodes
    /// </summary>

    private void Start()
    {
        GenerateNetwork();
    }

    [SerializeField]
    [Tooltip("Level Graph")]
    private GraphManager Graph;

    [SerializeField]
    [Tooltip("Number of nodes")]
    private int nodeNumber = 10;

    [SerializeField]
    [Tooltip("Number of total edges distributed randomly")]
    private int edgeNumber = 3;

    private Network network;

    public void changeLinks(Node n1, Node n2)
    {

        Link toDelete = null;
        foreach (Link edge in network.Links)
        {
            if ((edge.FirstNodeId == n1.Id && edge.SecondNodeId == n2.Id) || (edge.SecondNodeId == n1.Id && edge.FirstNodeId == n2.Id))
            {
                toDelete = edge;
            }
        }
        if(toDelete != null)
        {
            Graph.RemoveLink(toDelete);
            network.Links.Remove(toDelete);
        }
        else
        {
            var gf1 = Graph.getGraphNodes[n1.Id];
            var gf2 = Graph.getGraphNodes[n2.Id];
            Graph.AddLink(gf1, gf2);
        }
    }


    private void GenerateNetwork()
    {
        // Start a new network
        network = new Network();

        var randGen = new System.Random();

        //Add random nodes
        for(int i = 0; i < nodeNumber; i++)
        {
            var node = new Node(Guid.NewGuid(), "Node", Color.black, GenerateRandomPop());
            network.Nodes.Add(node);
        }

        while(network.Links.Count < edgeNumber)
        {
            if (randGen.Next(2) == 0)
            {
                List<Node> nodesWithEdges = GetRandomNodes(network.Nodes, 2);
                var initialSimilarity = Pop.calculateSimilarity(nodesWithEdges[0].Person, nodesWithEdges[1].Person);
                InfoLink info = new InfoLink(initialSimilarity);
                var edge = new Link(nodesWithEdges[0].Id, nodesWithEdges[1].Id, 0.5f, Color.white, info);
                if(!network.Links.Contains(edge)) network.Links.Add(edge);
            }
        }

        // Show the Network with the Prefabs rendered
        Graph.Initialize(network);

    }

    /// <summary>
    /// Generate a person with random attributes
    /// </summary>
    public static Pop GenerateRandomPop()
    {
        float emotionalState = UnityEngine.Random.Range(-10.0f, 10.0f);
        var ideology = new Ideology(UnityEngine.Random.Range(0.0f, 25.0f));
        var hobby = new Hobby((Hobby.HobbyDenomination)UnityEngine.Random.Range(0,5));

        return new Pop(emotionalState, ideology, hobby);

    }

    private List<Node> GetRandomNodes(List<Node> nodes, int count)
    {
        var nodesRet = new List<Node>();
        var idxUsed = new List<int>();
        while(nodesRet.Count < count)
        {
            var rand = UnityEngine.Random.Range(0, nodes.Count);
            if(idxUsed.Contains(rand)) continue;

            nodesRet.Add(nodes[rand]);
            idxUsed.Add(rand);
        }
        return nodesRet;

    }
}
