using ForceDirectedGraph.DataStructure;
using System.Collections.Generic;
using UnityEngine;

namespace ForceDirectedGraph
{
    public class GraphNode : MonoBehaviour
    {

        #region Constants

        /// <summary>
        /// The maximum value the node's velocity can be at any time.
        /// </summary>
        private const float MAX_VELOCITY_MAGNITUDE = 400f;

        #endregion

        #region Initialization

        /// <summary>
        /// Executes once on start.
        /// </summary>
        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            Draggable = GetComponent<Draggable>();

            var go = GameObject.Find("LevelStateMachine");
            Manager = go.GetComponent<UIManager>();

            // Freeze rotation
            Rigidbody.angularVelocity = 0;
            Rigidbody.freezeRotation = true;
        }

        /// <summary>
        /// Initializes the graph entity.
        /// </summary>
        /// <param name="node">The node being presented.</param>
        public void Initialize(Node node)
        {
            _Node = node;
            //Initilize the node GUI
            Manager.setNodeSprite(this);
        }

        #endregion

        #region Fields/Properties

        /// <summary>
        /// The node being presented.
        /// </summary>
        [SerializeField]
        [Tooltip("The node being presented.")]
        private Node _Node;

        /// <summary>
        /// The node being presented.
        /// </summary>
        public Node Node { get { return _Node; } }

        /// <summary>
        /// References the rigid body that handles the movements of the node.
        /// </summary>
        private Rigidbody2D Rigidbody;

        /// <summary>
        /// References the draggable script that will notify us if the node is being dragged.
        /// </summary>
        private Draggable Draggable;

        /// <summary>
        /// Refrences the ui manager script that changes any sprites in level
        /// </summary>
        private UIManager Manager;

        /// <summary>
        /// List of all forces to apply.
        /// </summary>
        private List<Vector2> Forces;

        #endregion

        #region Movement

        /// <summary>
        /// Apply forces to the node.
        /// </summary>
        /// <param name="applyImmediately">States whether we should apply the forces immediately or wait till the next frame.</param>
        public void ApplyForces(List<Vector2> forces, bool applyImmediately = false)
        {
            if (applyImmediately)
                foreach (var force in forces)
                    Rigidbody.AddForce(force);
            else
                Forces = forces;
        }

        /// <summary>
        /// Updates the forces applied to the node.
        /// </summary>
        private void Update()
        {
            // Check if the object is being dragged
            if (Draggable.IsBeingDragged)
            {
                // Do nothing
            }

            // The object is not being dragged
            else
            {
                Rigidbody.velocity = Vector3.zero;

                Vector2 velocity = Vector2.zero;
                if (Forces != null)
                    foreach (var force in Forces)
                        velocity += force;

                velocity = velocity.normalized * Mathf.Clamp(velocity.magnitude, 0f, MAX_VELOCITY_MAGNITUDE);

                Rigidbody.AddForce(velocity);
            }
        }

        public override bool Equals(object obj)
        {
            return obj is GraphNode node &&
                   base.Equals(obj) &&
                   EqualityComparer<GameObject>.Default.Equals(gameObject, node.gameObject) &&
                   EqualityComparer<Node>.Default.Equals(_Node, node._Node);
        }

        public override int GetHashCode()
        {
            int hashCode = -1869675292;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<GameObject>.Default.GetHashCode(gameObject);
            hashCode = hashCode * -1521134295 + EqualityComparer<Node>.Default.GetHashCode(_Node);
            return hashCode;
        }

        #endregion

    }
}
