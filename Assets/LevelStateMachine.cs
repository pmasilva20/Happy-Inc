using System;
using System.Collections;
using System.Collections.Generic;
using ForceDirectedGraph;
using ForceDirectedGraph.DataStructure;
using UnityEngine;

public class LevelStateMachine : MonoBehaviour
{

    private UIManager UIManager;

    private AudioManager audioManager;

    private GraphManager graphManager;

    [SerializeField]
    [Tooltip("Chance to remove links(highter means less chance)")]
    public int limitRemoveRandomLinks = 10;

    [SerializeField]
    [Tooltip("Chance to make good links(highter means less chance)")]
    public int limitAddRandomLinks = 5;

    [SerializeField]
    [Tooltip("Chance max range for make bad links(highter means less chance)")]
    public int limitAddBadRandomLinks = 6;

    [SerializeField]
    [Tooltip("Every X time units it checks happiness values")]
    public int timeToCheckHappiness = 5;

    [SerializeField]
    [Tooltip("Every X time units it checks if it should alter links")]
    public int timeToCheckLinks = 5;

    [SerializeField]
    [Tooltip("Every X time units it checks network growth values and makes new nodes or removes nodes")]
    public int timeToCheckNetworkGrowth = 10;

    [SerializeField]
    [Tooltip("Every X time units it updates total Ad Revenue")]
    public int timeToUpdateAdRevenue = 10;

    [SerializeField]
    [Tooltip("Threeshold to make new links(in percentage of maximum similarity) 0 is max similarity")]
    public float addLinkThreeshold = 0.3f;

    [SerializeField]
    [Tooltip("Threeshold to break links(in percentage of maximum similarity) 0 is max similarity")]
    public float breakLinkThreeshold = 0.8f;

    [SerializeField]
    [Tooltip("Threeshold for person to be happier(in percentage) 0 is max similarity(highter means more happy)")]
    public float happierThreeshold = 0.3f;

    [SerializeField]
    [Tooltip("Threeshold for person to be sadder(in percentage) 0 is max similarity(highter means less angry)")]
    public float sadderThreeshold = 0.8f;

    [SerializeField]
    [Tooltip("Decreasse in happiness rate(0-1)(highter means angrier faster)")]
    public float emotionRateAngry = 0.6f;

    [SerializeField]
    [Tooltip("Increasse in happiness rate(0-1)(highter means happier faster)")]
    public float emotionRateHappy = 1f;

    [SerializeField]
    [Tooltip("Base revenue gain for when information flow is maximum(in dollars)")]
    public float adRevenueGainMaximum = 100f;

    [SerializeField]
    [Tooltip("Update level state and variables every X seconds")]
    public float secondsUpdate = 10f;

    [SerializeField]
    [Tooltip("Total level time in seconds, after it game over")]
    public float totalLevelTime = 30f;

    [SerializeField]
    [Tooltip("Current Level index")]
    public int currentLevel = 0;

    private float maxInformationFlow = Pop.maximumSimilarity * 2;

    private float disinformation = 0f;

    private float netGrowth = 0f;

    private float totalAdRevenue = 0f;

    private float currentAdRevenue = 0f;

    private int currentGameTime = 0;

    private bool pauseFlag = false;

    private bool startOutFlag = true;

    private bool gameOverFlag = false;

    /// <summary>
    /// Corrotine that checks for Game Over conditions and updates time left
    /// Done separatelly because these conditions should be immediate
    /// </summary>
    /// <returns></returns>
    IEnumerator UpdateStateTimeUrgent()
    {
        while (startOutFlag)
        {
            yield return new WaitForSeconds(0.1f);
        }

        while (!gameOverFlag)
        {
            if (pauseFlag)
            {
                while (pauseFlag)
                {
                    yield return new WaitForSeconds(0.2f);
                }
            }

            currentGameTime++;
            try
            {
                updateTimeUI();
                checkForGameOver();
            }
            catch (Exception ex)
            {
                Debug.Log(string.Format("Error during time update {0}", ex));
            }

            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator UpdateState()
    {
        while (startOutFlag)
        {
            yield return new WaitForSeconds(0.1f);
        }

        while (!gameOverFlag)
        {
            if (pauseFlag)
            {
                while (pauseFlag)
                {
                    yield return new WaitForSeconds(0.2f);
                }
            }

            //Debug.Log("SM Update time:" + currentGameTime);
            try
            {
                calculateSimilarity();
                if((currentGameTime % timeToCheckLinks) == 0) checkLinks();
                checkBubbles();
                calculateDisinformation();
                if((currentGameTime % timeToCheckHappiness) == 0) calculateHappiness();
                calculateNetGrowth();
                if (currentGameTime % timeToCheckNetworkGrowth == 0) checkAddRemoveNodes();
                calculateInformationFlow();
                calculateAdRevenue();
                if(currentGameTime % timeToUpdateAdRevenue == 0) totalAdRevenue += currentAdRevenue;

                updateUI();
            }
            catch (Exception ex)
            {
                Debug.Log(string.Format("Error during cycle {0}",ex));
            }

            yield return new WaitForSeconds(secondsUpdate);
        }
    }

    public void closeStartPanel()
    {
        startOutFlag = false;
        UIManager.changeVisibilityStartPanel(false);
    }

    public void openHelp()
    {
        pauseFlag = true;
        UIManager.changeVisibilityHelpPanel(true);
    }

    public void closeHelp()
    {
        pauseFlag = false;
        UIManager.changeVisibilityHelpPanel(false);
    }

    /// <summary>
    /// Check for game over conditions
    /// Condition 1: Time left for level is 0
    /// Condition 2: There are no nodes left in level
    /// </summary>
    private void checkForGameOver()
    {
        if(currentGameTime >= totalLevelTime)
        {
            gameOverFlag = true;
            UIManager.showGameOverScreen(currentLevel,"Time's Up",totalAdRevenue);
            audioManager.playSoundEffect(audioManager.levelComplete);
        }
        else if(graphManager.getGraphNodes.Count == 0)
        {
            gameOverFlag = true;
            UIManager.showGameOverScreen(currentLevel, "Everyone has left the social network", totalAdRevenue);
        }
    }

    /// <summary>
    /// Level time must be updated every second
    /// </summary>
    private void updateTimeUI()
    {
        UIManager.setTimeLevelUI(totalLevelTime - currentGameTime);
    }

    private void updateUI()
    {

        // Update sprite color for links
        foreach (GraphLink link in graphManager.getGraphLinks)
        {
            UIManager.setLinkSprite(link);
        }
        //Update Level UI
        var timeLeftSeconds = totalLevelTime - (currentGameTime * secondsUpdate);
        UIManager.setLevelUI(disinformation, graphManager.getGraphNodes.Count, netGrowth, totalAdRevenue, currentAdRevenue);

    }

    /// <summary>
    /// By using information flow average across all links
    /// Extract current adRevenue growth
    /// AdRevenue = adRevenueMaximum * avgInformationFlow / maxInformationFlow
    /// max == similairty * 2
    /// </summary>
    private void calculateAdRevenue()
    {
        var totalInformationFlow = 0f;
        foreach (GraphLink link in graphManager.getGraphLinks)
        {
            var infoLink = link.Link.InfoLink;
            totalInformationFlow += infoLink.informationFlowValue;
        }
        var avgInformationFlow = totalInformationFlow / graphManager.getGraphLinks.Count;
        currentAdRevenue = adRevenueGainMaximum * avgInformationFlow / maxInformationFlow;
    }

    /// <summary>
    /// Information Flow represents information passed between two persons in the network
    /// InformationFlow = similarity + similarity * bubbleEffect(if in a bubble) * (1-disinformation)
    /// Max information flow == 2 * max similairty
    /// </summary>
    private void calculateInformationFlow()
    {
        foreach(GraphLink link in graphManager.getGraphLinks)
        {
            var infoLink = link.Link.InfoLink;
            var similarity = infoLink.similairityValue;

            //Check if both in same bubble
            var node1Bubble = checkIfInBubble(link.FirstNode);
            var node2Bubble = checkIfInBubble(link.SecondNode);
            if(node1Bubble == null || node2Bubble == null || !node1Bubble.Equals(node2Bubble))
            {
                infoLink.setInformationFlow(similarity);
            }
            else
            {
                var bubbleEffect = node1Bubble.NodesInBubble.Count / graphManager.getGraphNodes.Count;
                var informationFlow = similarity + (similarity * bubbleEffect * (1 - disinformation));
                infoLink.setInformationFlow(informationFlow);
            }
        }

    }

    /// <summary>
    /// If network growth is positive add new nodes, if negative take out unhappy ones
    /// </summary>
    private void checkAddRemoveNodes()
    {
        if (netGrowth > 0)
        {
            Debug.Log("Sim made a new node");
            var node = new Node(Guid.NewGuid(), "Node", Color.black, LevelManager.GenerateRandomPop());
            graphManager.AddNode(node);

        }
        else if(netGrowth < 0)
        {
            Debug.Log("Sim removed a node");
            int neededNodes = (int)Math.Abs(netGrowth);
            List<GraphNode> angryNodes = new List<GraphNode>();
            foreach(GraphNode graphNode in graphManager.getGraphNodes.Values)
            {
                if (angryNodes.Count >= neededNodes) break;
                if (graphNode.Node.Person.emotionalValue < 0)
                {
                    angryNodes.Add(graphNode);
                }
            }
            foreach(GraphNode node in angryNodes)
            {
                graphManager.RemoveNode(node);
            }
    
        }

        graphManager.HandleZoom();
    }

    /// <summary>
    /// Calculate net growth by taking into account average happiness of nodes
    /// </summary>
    private void calculateNetGrowth()
    {
        float totalHappiness = 0f;
        foreach (GraphNode graphNode in graphManager.getGraphNodes.Values)
        {
            var currentHappiness = graphNode.Node.Person.emotionalValue;
            totalHappiness += currentHappiness;
        }
        float avgHappiness = totalHappiness / graphManager.getGraphNodes.Values.Count;

        var diff = Math.Abs(Pop.maxEmotionValue - Pop.minimumEmotionValue) / 5;
        Debug.Log(string.Format("Happy avg {0}", avgHappiness));
        if (avgHappiness > (Pop.maxEmotionValue - diff)) netGrowth = 2;
        else if (avgHappiness > (Pop.maxEmotionValue - diff * 2)) netGrowth = 1;
        else if (avgHappiness > (Pop.maxEmotionValue - diff * 3)) netGrowth = 0;
        else if (avgHappiness > (Pop.maxEmotionValue - diff * 4)) netGrowth = -1;
        else if (avgHappiness > (Pop.maxEmotionValue - diff * 5)) netGrowth = -2;
    }

    /// <summary>
    /// Calculate happiness by taking into account similarity
    /// </summary>
    private void calculateHappiness()
    {
        foreach (GraphNode graphNode in graphManager.getGraphNodes.Values)
        {
            //For each link, get similarity
            //If avg_similarity is hight get happier
            var total_similarity = 0f;
            var avg_similarity = 0f;
            var numLinks = 0;
            foreach(GraphLink graphLink in graphManager.getGraphLinks)
            {
                if(graphLink.FirstNode.Equals(graphNode) || graphLink.SecondNode.Equals(graphNode))
                {
                    numLinks += 1;
                    total_similarity += graphLink.Link.InfoLink.similairityValue;
                }
            }
            //Node is isolated
            if(numLinks == 0)
            {
                //Get Angrier
                graphNode.Node.Person.emotionChangeAngry(emotionRateAngry);
                Debug.Log(string.Format("Got sadder cause am alone"));
                continue;
            }


            if (total_similarity != 0 && numLinks != 0)
            {
                avg_similarity = total_similarity / numLinks;
            }

            if(avg_similarity < (happierThreeshold * Pop.maximumSimilarity))
            {
                //Get Happier
                graphNode.Node.Person.emotionChangeHappy(emotionRateHappy);
                Debug.Log(string.Format("Got happier {0}",avg_similarity));
            }
            else if(avg_similarity > (sadderThreeshold * Pop.maximumSimilarity))
            {
                //Get Angrier
                graphNode.Node.Person.emotionChangeAngry(emotionRateAngry);
                Debug.Log(string.Format("Got sadder {0}", avg_similarity));
            }


            
        }
        //Update UI
        if (UIManager.currentPersonSelected != null)
        {
            UIManager.updateSideMenuNodeUI(UIManager.currentPersonSelected);
        }
    }

    /// <summary>
    /// From bubbles calcualte desinformation
    /// Each bubble leads to a blank 5% to 20% disinformation effect on information flow
    /// </summary>
    private void calculateDisinformation()
    {
        disinformation = 0f;
        foreach (GraphBubble graphBubble in graphManager.getGraphBubbles)
        {
            var numNodes = graphBubble.NodesInBubble.Count;
            var numTotal = graphManager.getGraphNodes.Count;
            var perc = numNodes / numTotal;
            float disinfoBubble = 0f;
            if (perc < 0.2) disinfoBubble = 0.05f;
            else if(perc < 0.3) disinfoBubble = 0.10f;
            else if (perc < 0.4) disinfoBubble = 0.15f;
            disinformation += disinfoBubble;
        }
    }

    /// <summary>
    /// Bubbles are the result of very high similairty between two nodes with a link
    /// Bubbles appear from connections of nodes of same ideology
    /// Bubbles lead to high information flow but many bubbles will lead to disinformation
    /// Disinformation is a blank modfiier from 5-20% from bubble existance depending on size
    /// </summary>
    private void checkBubbles()
    {

        var graphNodes = graphManager.getGraphNodes;
        foreach (GraphNode node1 in graphNodes.Values)
        {
            foreach (GraphNode node2 in graphNodes.Values)
            {
                if (node1.Node.Id == node2.Node.Id) continue;
                if (node1.Node.Person.ideologyDenominaion != node2.Node.Person.ideologyDenominaion) continue;
                var possibleLink = checkIfLinkExists(node1, node2);
                //if (!possibleLink) continue;
                //Check if neither are in a bubble already
                var node1Bubble = checkIfInBubble(node1);
                var node2Bubble = checkIfInBubble(node2);

                //One of them is in a bubble, add other is not in a bubble
                if((node1Bubble != null) && (node2Bubble == null))
                {
                    node1Bubble.addNodeToBubble(node2);
                }
                else if ((node2Bubble != null) && (node1Bubble == null))
                {
                    node2Bubble.addNodeToBubble(node1);
                }
                //None in bubbles, join in a new bubble
                else if((node1Bubble == null) && (node2Bubble == null))
                {
                    Debug.Log("Sim making a new bubble");
                    List<GraphNode> graphNodesInBubble = new List<GraphNode>();
                    graphNodesInBubble.Add(node1);
                    graphNodesInBubble.Add(node2);

                    List<Node> nodesInBubble = new List<Node>();
                    nodesInBubble.Add(node1.Node);
                    nodesInBubble.Add(node2.Node);
                    Bubble bubble = new Bubble(nodesInBubble);

                    graphManager.AddBubble(graphNodesInBubble, bubble);
                }
            }
        }

        //Check for bubbles with nodes with no links to other nodes in bubble
        List<GraphBubble> emptyBubbles = new List<GraphBubble>();
        foreach (GraphBubble graphBubble in graphManager.getGraphBubbles)
        {
            List<GraphNode> allNodesToTakeOut = new List<GraphNode>();
            foreach(GraphNode node1 in graphBubble.NodesInBubble)
            {
                var isolated = true;
                foreach (GraphNode node2 in graphBubble.NodesInBubble)
                {
                    if (node1.Node.Id == node2.Node.Id) continue;
                    var possibleLink = checkIfLinkExists(node1, node2);
                    //If there's a link then node1 is not isolated inside bubble
                    if(possibleLink != null)
                    {
                        isolated = false;
                        break;
                    }
                }
                if (isolated)
                {
                    allNodesToTakeOut.Add(node1);
                }
            }

            if(allNodesToTakeOut.Count > 0)
            {
                foreach(GraphNode graphNode in allNodesToTakeOut)
                {
                    graphBubble.removeNodeFromBubble(graphNode);
                }
            }
            if(graphBubble.NodesInBubble.Count == 0)
            {
                emptyBubbles.Add(graphBubble);
            }
        }

        foreach(GraphBubble bubble in emptyBubbles)
        {
            graphManager.RemoveBubble(bubble);
        }


    }

    /// <summary>
    /// Check if node in any bubble, return bubble if yes
    /// </summary>
    /// <returns></returns>
    private GraphBubble checkIfInBubble(GraphNode graphNode)
    {
        foreach(GraphBubble graphBubble in graphManager.getGraphBubbles)
        {
            if (graphBubble.NodesInBubble.Contains(graphNode))
            {
                return graphBubble;
            }
        }
        return null;
    }

    /// <summary>
    /// Check if similarity between Nodes is big enought to make new links
    /// Or low enought that links might be broken
    /// 0 < sim < 35
    /// It's a random chance function, it doesnt always happens for each possible link to be made/broken
    /// </summary>
    private void checkLinks()
    {
        if (graphManager.getGraphNodes == null) return;
        var graphNodes = graphManager.getGraphNodes;
        foreach(GraphNode node1 in graphNodes.Values)
        {
            foreach (GraphNode node2 in graphNodes.Values)
            {
                if (node1.Node.Id == node2.Node.Id) continue;
                var sim = Pop.calculateSimilarity(node1.Node.Person, node2.Node.Person);
                var possibleLink = checkIfLinkExists(node1, node2);
                if ((sim < (addLinkThreeshold * Pop.maximumSimilarity)) && possibleLink == null)
                {
                    // High similarity, make a link
                    var rd = UnityEngine.Random.Range(0, limitAddRandomLinks + 1);

                    if (rd > 0) continue;

                    Debug.Log(string.Format("New Link made by sim:{0}",sim));

                    graphManager.AddLink(node1, node2);
                }
                else if((sim > (breakLinkThreeshold * Pop.maximumSimilarity)) && possibleLink != null)
                {
                    // Low similarity, delete a link
                    var rd = UnityEngine.Random.Range(0, limitRemoveRandomLinks + 1);

                    if (rd > 0) continue;

                    Debug.Log(string.Format("Link destroyed sim:{0}", sim));
                    graphManager.RemoveLink(possibleLink.Link);
                }
                else
                {
                    // Medium similairty, may make a link but the chance is lower
                    var rd = UnityEngine.Random.Range(0, limitAddBadRandomLinks + 1);

                    if (rd > 0) continue;

                    Debug.Log(string.Format("New Link made randomly by sim:{0}", sim));

                    graphManager.AddLink(node1, node2);
                }
            }
        }
    }

    /// <summary>
    /// Check if a link exists, returns a GraphLink representation if it does else null
    /// </summary>
    /// <param name="node1"></param>
    /// <param name="node2"></param>
    /// <returns></returns>
    private GraphLink checkIfLinkExists(GraphNode node1, GraphNode node2)
    {
        foreach(GraphLink graphLink in graphManager.getGraphLinks)
        {
            if((graphLink.FirstNode.Equals(node1) && graphLink.SecondNode.Equals(node2)) ||
                (graphLink.FirstNode.Equals(node2) && graphLink.SecondNode.Equals(node1)))
            {
                return graphLink;
            }
        }
        return null;
        
    }

    /// <summary>
    /// Take all links, calculate similariry and update these
    /// </summary>
    private void calculateSimilarity()
    {
        var links = graphManager.getGraphLinks;
        if (links == null) return;

        foreach(GraphLink link in links)
        {
            var person1 = link.FirstNode.Node.Person;
            var person2 = link.SecondNode.Node.Person;

            float sim = Pop.calculateSimilarity(person1, person2);
            link.Link.InfoLink.setSimilarity(sim);
        }
    }


    void Awake()
    {
        graphManager = GameObject.Find("Graph").GetComponent<GraphManager>();
        UIManager = GetComponent<UIManager>();
        audioManager = GameObject.Find("AudioManagerSoundEffects").GetComponent<AudioManager>();
        StartCoroutine(UpdateState());
        StartCoroutine(UpdateStateTimeUrgent());
    }

}
