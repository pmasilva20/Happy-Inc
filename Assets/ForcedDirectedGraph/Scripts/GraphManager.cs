using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ForceDirectedGraph.DataStructure;
using UnityEngine;

namespace ForceDirectedGraph
{
    public class GraphManager : MonoBehaviour
    {

        /// The repulsion force between any two nodes.
        [SerializeField]
        [Tooltip("The repulsion force between any two nodes.")]
        public float REPULSION_FORCE = 50000f;

        /// The maximum distance for applying repulsion forces.
        [SerializeField]
        [Tooltip("The maximum distance for applying repulsion forces.")]
        public float REPULSION_DISTANCE = 3f;

        /// The attraction force between any two nodes.
        [SerializeField]
        [Tooltip("The attraction force between any two nodes.")]
        public float ATTRACTION_FORCE = 40000f;

        /// Zoom level
        [SerializeField]
        [Tooltip("Zoom level, bigger means zooms out")]
        public float zoomLevel = 0.1f;


        public void Initialize(DataStructure.Network network)
        {
            audioManager = GameObject.Find("AudioManagerSoundEffects").GetComponent<AudioManager>();
            _Network = network;
            Display();
            HandleZoom();
        }


        #region Fields/Properties


        private AudioManager audioManager;


        [Header("Bubbles")]

        /// <summary>
        /// References the parent holding all bubbles.
        /// </summary>
        [SerializeField]
        [Tooltip("References the parent holding all bubbles.")]
        private GameObject BubblesParent;

        /// <summary>
        /// Template used for initiating bubbles.
        /// </summary>
        [SerializeField]
        [Tooltip("Template used for initiating nodes.")]
        private GameObject BubbleTemplate;

        /// <summary>
        /// List of all bubbles displayed on the graph.
        /// </summary>
        private List<GraphBubble> GraphBubbles;

        public List<GraphBubble> getGraphBubbles { get { return GraphBubbles; } }


        [Header("Nodes")]

        /// <summary>
        /// References the parent holding all nodes.
        /// </summary>
        [SerializeField]
        [Tooltip("References the parent holding all nodes.")]
        private GameObject NodesParent;

        /// <summary>
        /// Template used for initiating nodes.
        /// </summary>
        [SerializeField]
        [Tooltip("Template used for initiating nodes.")]
        private GameObject NoteTemplate;

        /// <summary>
        /// List of all nodes displayed on the graph.
        /// </summary>
        private Dictionary<Guid, GraphNode> GraphNodes;

        public Dictionary<Guid, GraphNode> getGraphNodes { get { return GraphNodes; } }



        [Header("Links")]

        /// <summary>
        /// References the parent holding all links.
        /// </summary>
        [SerializeField]
        [Tooltip("References the parent holding all links.")]
        private GameObject LinksParent;

        /// <summary>
        /// Template used for initiating links.
        /// </summary>
        [SerializeField]
        [Tooltip("Template used for initiating links.")]
        private GameObject LinkTemplate;

        /// <summary>
        /// List of all links displayed on the graph.
        /// </summary>
        private List<GraphLink> GraphLinks;

        public List<GraphLink> getGraphLinks { get { return GraphLinks; } }


        [Header("Data")]

        /// <summary>
        /// The netwok being displayed.
        /// </summary>
        [SerializeField]
        [Tooltip("The netwok being displayed.")]
        private DataStructure.Network _Network;

        /// <summary>
        /// The netwok being displayed.
        /// </summary>
        public DataStructure.Network Network { get { return _Network; } }

        #endregion

        #region Display Methods

        /// <summary>
        /// Displays the network.
        /// </summary>
        private void Display()
        {
            // Clear everything
            Clear();

            // Display nodes
            DisplayNodes();

            // Display links
            DisplayLinks();

            // Shuffle the nodes
            ShuffleNodes();

        }

        /// <summary>
        /// Deletes all nodes and links in the graph.
        /// </summary>
        private void Clear()
        {
            // Clear nodes
            GraphNodes = new Dictionary<Guid, GraphNode>();
            foreach (Transform entity in NodesParent.transform)
                GameObject.Destroy(entity.gameObject);

            // Clear paths
            GraphLinks = new List<GraphLink>();
            foreach (Transform path in LinksParent.transform)
                GameObject.Destroy(path.gameObject);

            //Clear any bubbles
            GraphBubbles = new List<GraphBubble>();
            foreach (Transform path in BubblesParent.transform)
                GameObject.Destroy(path.gameObject);
        }

        /// <summary>
        /// Displays nodes on the graph.
        /// </summary>
        private void DisplayNodes()
        {
            // For each position, create an entity
            foreach (var node in Network?.Nodes)
            {
                // Create a new entity instance
                GameObject graphNode = Instantiate(NoteTemplate, NodesParent.transform);
                graphNode.transform.position = Vector3.zero;
                graphNode.transform.localRotation = Quaternion.Euler(Vector3.zero);

                // Extract the script
                GraphNode script = graphNode.GetComponent<GraphNode>();
                NodeMenu guiScript = graphNode.GetComponent<NodeMenu>();

                // Initialize data
                script.Initialize(node);
                guiScript.Initialize(node.Person);

                // Add to list
                GraphNodes?.Add(node.Id, script);
            }
        }

        /// <summary>
        /// Displays links on the graph.
        /// </summary>
        private void DisplayLinks()
        {
            // For each position, create an entity
            foreach (var link in Network?.Links)
            {
                // Find graph nodes
                if (!GraphNodes.ContainsKey(link.FirstNodeId)
                    || !GraphNodes.ContainsKey(link.SecondNodeId))
                    continue;
                AddLink(link);
            }
        }

        public void RemoveBubble(GraphBubble graphBubble)
        {
            GraphBubbles.Remove(graphBubble);
            GameObject.Destroy(graphBubble.gameObject);
        }


        /// <summary>
        /// Add a new bubble to be displayed
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="bubble"></param>
        public void AddBubble(List<GraphNode> nodes, Bubble bubble)
        {
            //Create new entity instance
            GameObject graphBubble = Instantiate(BubbleTemplate, BubblesParent.transform);
            graphBubble.transform.localRotation = Quaternion.Euler(Vector3.zero);


            // Extract the script
            GraphBubble script = graphBubble.GetComponent<GraphBubble>();

            // Initialize data
            script.Initialize(nodes,bubble);

            // Add to list
            GraphBubbles.Add(script);
        }

        /// <summary>
        /// Take from displaying a removed node from graph
        /// </summary>
        public void RemoveNode(GraphNode node)
        {
            audioManager.playSoundEffect(audioManager.removeNode);
            //Take out any links he may have been
            foreach (Link link in _Network.Links)
            {
                if(link.FirstNodeId == node.Node.Id || link.SecondNodeId == node.Node.Id)
                {
                    RemoveLink(link);
                }
            }

            _Network.Nodes.Remove(node.Node);

            GraphNodes.Remove(node.Node.Id);
            GameObject.Destroy(node.gameObject);
        }


        /// <summary>
        /// Display a new node added to graph
        /// </summary>
        public void AddNode(Node node)
        {
            audioManager.playSoundEffect(audioManager.newNode);
            _Network.Nodes.Add(node);

            // Create a new entity instance
            GameObject graphNode = Instantiate(NoteTemplate, NodesParent.transform);
            graphNode.transform.position = Vector3.zero;
            graphNode.transform.localRotation = Quaternion.Euler(Vector3.zero);

            // Extract the script
            GraphNode script = graphNode.GetComponent<GraphNode>();
            NodeMenu guiScript = graphNode.GetComponent<NodeMenu>();

            // Initialize data
            script.Initialize(node);
            guiScript.Initialize(node.Person);

            // Add to list
            GraphNodes?.Add(node.Id, script);
        }

        /// <summary>
        /// Take from displaying a removed link from graph
        /// </summary>
        public void RemoveLink(Link link)
        {
            audioManager.playSoundEffect(audioManager.removeLink);
            _Network.Links.Remove(link);
            GraphLink toDelete = null;
            foreach(GraphLink graphLink in GraphLinks)
            {
                if (graphLink.Link.Equals(link))
                {
                    toDelete = graphLink;

                }
            }

            if(toDelete != null)
            {
                GraphLinks.Remove(toDelete);
                GameObject.Destroy(toDelete.gameObject);
            }
        }


        /// <summary>
        /// Display a new link added to graph
        /// Takes the nodes as input
        /// </summary>
        public void AddLink(GraphNode firstNode, GraphNode secondNode)
        {
            audioManager.playSoundEffect(audioManager.makelink);
            var initialSimilarity = Pop.calculateSimilarity(firstNode.Node.Person, secondNode.Node.Person);
            InfoLink info = new InfoLink(initialSimilarity);
            var edge = new Link(firstNode.Node.Id, secondNode.Node.Id, 0.5f, Color.white, info);
            
            if (!_Network.Links.Contains(edge))
            {
                _Network.Links.Add(edge);

                // Create a new entity instance
                GameObject graphLink = Instantiate(LinkTemplate, LinksParent.transform);
                graphLink.transform.position = Vector3.zero;
                graphLink.transform.localRotation = Quaternion.Euler(Vector3.zero);

                // Extract the script
                GraphLink script = graphLink.GetComponent<GraphLink>();

                // Initialize data
                script.Initialize(edge, firstNode, secondNode);

                // Add to list
                GraphLinks.Add(script);
            }
        }

        /// <summary>
        /// Display a new link added to graph
        /// </summary>
        public void AddLink(Link link)
        {
            GraphNode firstNode = GraphNodes?[link.FirstNodeId];
            GraphNode secondNode = GraphNodes?[link.SecondNodeId];

            // Create a new entity instance
            GameObject graphLink = Instantiate(LinkTemplate, LinksParent.transform);
            graphLink.transform.position = Vector3.zero;
            graphLink.transform.localRotation = Quaternion.Euler(Vector3.zero);

            // Extract the script
            GraphLink script = graphLink.GetComponent<GraphLink>();

            // Initialize data
            script.Initialize(link, firstNode, secondNode);

            // Add to list
            GraphLinks.Add(script);
        }

        public void HandleZoom()
        {
            var totalNodes = GraphNodes.Values.Count;
            var size = 1f;
            if (totalNodes > 10)
            {
                var remainingNodes = totalNodes - 10;
                size = 1f - zoomLevel * remainingNodes / 10;
            }
            if (size < 0.4) size = 0.4f;
            foreach (GraphNode graphNode in GraphNodes.Values)
            {
                graphNode.gameObject.transform.localScale = new Vector3(size, size, 1);
            }
        }

        /// <summary>
        /// Shuffles the nodes randomly.
        /// </summary>
        private void ShuffleNodes()
        {
            System.Random random = new System.Random();
            foreach (var node in GraphNodes.Values)
                node.ApplyForces(new List<Vector2>() { new Vector2(random.Next(-10, 10) / 10f, random.Next(-10, 10) / 10f) }, true);
        }

        #endregion

        #region Force Methods

        /// <summary>
        /// Continuously apply forces to nodes.
        /// </summary>
        private void Update()
        {
            ApplyForces();
        }

        /// <summary>
        /// Computes and applies forces to nodes.
        /// </summary>
        private void ApplyForces()
        {
            if (GraphNodes == null)
                return;

            // Stores all the forces to be applied to each node
            Dictionary<GraphNode, List<Vector2>> nodeForces = new Dictionary<GraphNode, List<Vector2>>();
            foreach (var node1 in GraphNodes.Values)
                nodeForces.Add(node1, new List<Vector2>());

            // Compute repulsion forces
            foreach (var node1 in GraphNodes.Values)
                foreach (var node2 in GraphNodes.Values)
                    if (node1 != node2)
                        nodeForces[node1].Add(ComputeRepulsiveForce(node1, node2));

            // Compute attraction forces
            foreach (var link in GraphLinks)
            {
                var force = ComputeAttractionForce(link);
                nodeForces[link.FirstNode].Add(-force);
                nodeForces[link.SecondNode].Add(force);
            }

            // Apply forces
            foreach (var node in nodeForces.Keys)
                node.ApplyForces(nodeForces[node]);
        }

        /// <summary>
        /// Computes the distance between two nodes.
        /// </summary>
        private float ComputeDistance(GraphNode node1, GraphNode node2)
        {
            return (float)
                Math.Sqrt
                (
                    Math.Pow(node1.transform.position.x - node2.transform.position.x, 2)
                    +
                    Math.Pow(node1.transform.position.y - node2.transform.position.y, 2)
                );
        }

        /// <summary>
        /// Computes the repulsive force against a node.
        /// </summary>
        private Vector2 ComputeRepulsiveForce(GraphNode node, GraphNode repulsiveNode)
        {
            // Compute distance
            float distance = ComputeDistance(node, repulsiveNode);
            if (distance > REPULSION_DISTANCE)
                return Vector3.zero;

            // Compute force direction
            Vector2 forceDirection = (node.transform.position - repulsiveNode.transform.position).normalized;

            // Compute distance force
            float distanceForce = (REPULSION_DISTANCE - distance) / REPULSION_DISTANCE;

            // Compute repulsive force
            return forceDirection * distanceForce * REPULSION_FORCE * Time.deltaTime;
        }

        /// <summary>
        /// Computes the attraction force between two nodes.
        /// </summary>
        private Vector2 ComputeAttractionForce(GraphLink link)
        {
            // Compute force direction
            Vector2 forceDirection = (link.FirstNode.transform.position - link.SecondNode.transform.position).normalized;

            // Compute repulsive force
            return forceDirection * link.Link.Width * ATTRACTION_FORCE * Time.deltaTime;
        }

        #endregion

    }
}
