using System.Collections;
using System.Collections.Generic;
using ForceDirectedGraph;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    UIManager uiManager;
    LevelManager levelManager;


    // Last 2 Nodes clicked by user
    GraphNode clickedNode1 = null;
    GraphNode clickedNode2 = null;

    void Awake()
    {
        uiManager = GetComponent<UIManager>();
        levelManager = GetComponent<LevelManager>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Cast ray");
            DetectObjectClick();
        }
    }

    void DetectObjectClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
        if (hit.collider != null)
        {
            Debug.Log(hit.collider.gameObject.name);
            var clickedObject = hit.collider.gameObject;
            if (clickedObject.tag.Equals("Node"))
            {
                OnNodeClick(clickedObject.GetComponent<GraphNode>());
            }
        }
        else
        {
            OnBackgroundClick();
        }
    }


    /// <summary>
    /// One of the nodes was just selected
    /// If first change UI, if second make link and deselect nodes
    /// </summary>
    /// <param name="node"></param>
    public void OnNodeClick(GraphNode node)
    {
        if (clickedNode1 == null)
        {
            clickedNode1 = node;
            uiManager.setSideMenuNodeUI(node.Node.Person);
            uiManager.setNodeSelected(node);

        }
        else if (clickedNode1.Equals(node))
        {
            //Clicked same node twice
            OnBackgroundClick();
        }
        else
        {
            clickedNode2 = node;
            uiManager.setNodeSprite(clickedNode1);
            uiManager.disableSideMenuNodeUI();

            //Dont make a link between same node
            if (clickedNode1.Equals(clickedNode2)) return;

            //Do link or take out link
            onClickBothNodes();
            clickedNode1 = null;
            clickedNode2 = null;
        }

    }

    /// <summary>
    /// When clicking two nodes it either makes a link if none exists
    /// or removes existing link between them
    /// </summary>
    private void onClickBothNodes()
    {
        levelManager.changeLinks(clickedNode1.Node, clickedNode2.Node);
        Debug.Log("I made new link");
    }

    /// <summary>
    /// No nodes are being selected, disable any UI and clickedNode1 sprite
    /// </summary>
    public void OnBackgroundClick()
    {
        if(clickedNode1 != null)uiManager.setNodeSprite(clickedNode1);
        clickedNode1 = null;
        clickedNode2 = null;
        uiManager.disableSideMenuNodeUI();

    }


}
