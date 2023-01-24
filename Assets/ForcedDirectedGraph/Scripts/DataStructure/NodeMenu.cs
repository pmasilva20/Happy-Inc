using ForceDirectedGraph;
using UnityEngine;

public class NodeMenu : MonoBehaviour
{
    InputManager inputManager;

    /// <summary>
    /// Reference to the person object with info on node person
    /// </summary>
    private Pop _pop;

    private GraphNode graphNode;

    public void Initialize(Pop pop)
    {
        _pop = pop;
        graphNode = GetComponent<GraphNode>();
        var go = GameObject.Find("LevelStateMachine");
        inputManager = go.GetComponent<InputManager>();
    }

    /// <summary>
    /// On click show node information
    /// </summary>
    //private void OnMouseDown()
    //{
     //   inputManager.OnNodeClick(graphNode);
     //   Debug.Log("Showing POP UI");
    //}

}
