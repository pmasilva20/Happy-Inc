using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ForceDirectedGraph
{

    public class GraphBubble : MonoBehaviour
    {

        /// <summary>
        /// Used to set bubble color which depends on majority ideology of nodes in buble
        /// </summary>
        private UIManager UIManager;

        private GraphManager graphManager;


        [SerializeField]
        [Tooltip("Nodes in bubble")]
        private List<GraphNode> _NodesInBubble;

        public List<GraphNode> NodesInBubble { get { return _NodesInBubble; } }


        [SerializeField]
        [Tooltip("The bubble being displayed.")]
        private Bubble _Bubble;

        public Bubble Bubble { get { return _Bubble; } }


        public void addNodeToBubble(GraphNode node)
        {
            _Bubble.Nodes.Add(node.Node);
            _NodesInBubble.Add(node);
        }

        public void removeNodeFromBubble(GraphNode node)
        {
            _Bubble.Nodes.Remove(node.Node);
            _NodesInBubble.Remove(node);
        }

        private void Awake()
        {
            //Set sprite display
            var lsm = GameObject.Find("LevelStateMachine");
            var graph = GameObject.Find("Graph");

            UIManager = lsm.GetComponent<UIManager>();
            graphManager = graph.GetComponent<GraphManager>();

        }

        public void Initialize(List<GraphNode> graphNodes, Bubble bubble)
        {
            _NodesInBubble = graphNodes;
            _Bubble = bubble;
            UIManager.setNodeBubble(this);
        }


        //Update current position based on average of nodes in Bubble
        private void Update()
        {

            Vector2 newCenter = Vector2.zero;
            float radius;
            if (NodesInBubble.Count > 0)
            {
                (newCenter, radius) = minimumBoundingCircle(NodesInBubble);
                gameObject.transform.position = newCenter;
                radius += UIManager.getNodeSpriteExtent;
                gameObject.transform.localScale = new Vector3(radius, radius);
            }

        }

        private (Vector2 center, float radius) minimumBoundingCircle(List<GraphNode> graphNodes)
        {
            //Sort nodes by x coordinate
            var nodesSorted = graphNodes.OrderBy(node => node.transform.position.x).ToList();

            //Make a first bounding circle with 2 nodes
            var v1 = nodesSorted[0].transform.position;
            var v2 = nodesSorted[1].transform.position;

            var center = new Vector2((v1.x + v2.x) / 2, (v1.y + v2.y) / 2);
            var radius = Vector2.Distance(v1, v2);

            //For each other node check if inside if not expand circle

            var bubbleSpriteRenderer = GetComponent<SpriteRenderer>();
            var bubbleBounds = bubbleSpriteRenderer.bounds;

            for (int i = 2; i < nodesSorted.Count; i++)
            {
                //Inside circle already
                //Check if inside each other bounds
                var currentNode = nodesSorted[i];
                var currentNodeSpriteRenderer = currentNode.GetComponent<SpriteRenderer>();
                var nodeBounds = currentNodeSpriteRenderer.bounds;

                var minimalPoint = nodeBounds.min;
                var maxPoint = nodeBounds.max;

                if (bubbleBounds.Contains(minimalPoint) && bubbleBounds.Contains(maxPoint)) continue;

                //Needs to encompass MaxPoint
                Vector2 point;
                if (bubbleBounds.Contains(minimalPoint))
                {
                    point = new Vector2(maxPoint.x, maxPoint.y);
                }
                //Needs to encompass MinPoint
                else
                {
                    point = new Vector2(minimalPoint.x, minimalPoint.y);
                }

                var difX = point.x - center.x;
                var difY = point.y - center.y;
                var magnitude = Vector2.Distance(Vector2.zero, new Vector2(difX, difY));

                if(magnitude > radius)
                {
                    //Expand radius and change center to take into account new node
                    radius = (radius + magnitude) / 2;
                    center.x += difX * (magnitude / 2) / magnitude;
                    center.y += difY * (magnitude / 2) / magnitude;
                }

            }

            return (center, radius);


        }

    }

}

