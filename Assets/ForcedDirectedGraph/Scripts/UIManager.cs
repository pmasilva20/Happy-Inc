using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ForceDirectedGraph;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{


    //IdeologyColors

    //rgba(155,0,3)
    public Color communistColor = new Color(155f / 255f, 0f, 3f / 255f);

    //rgba(254,0,0)
    public Color socialistColor = new Color(254f / 255f, 0f, 0f);

    //rgba(119,119,119)
    public Color centristColor = new Color(119f / 255f, 119f / 255f, 119f / 255f);

    //rgba(254,215,0)
    public Color liberalColor = new Color(254f / 255f, 215f / 255f, 0f);

    //rgba(0,0,254,255)
    public Color populistColor = new Color(0f, 0f, 254f / 255f);


    //NodeSprites
    [SerializeField]
    private Sprite nodeHobbyMovies;

    [SerializeField]
    private Sprite nodeHobbyGames;

    [SerializeField]
    private Sprite nodeHobbyBooks;

    [SerializeField]
    private Sprite nodeHobbyFashion;

    [SerializeField]
    private Sprite nodeHobbyFootball;

    [SerializeField]
    private Sprite nodeHobbyMoviesSelected;

    [SerializeField]
    private Sprite nodeHobbyGamesSelected;

    [SerializeField]
    private Sprite nodeHobbyBooksSelected;

    [SerializeField]
    private Sprite nodeHobbyFashionSelected;

    [SerializeField]
    private Sprite nodeHobbyFootballSelected;

    public float getNodeSpriteExtent { get { return nodeHobbyGames.bounds.extents.magnitude * 4; } }


    //NodeSideMenuUI
    private GameObject personInsidePanel;
    private GameObject personPanel;

    private Text personEmotionLabel;

    private Text personHobbyLabel;

    private Text personPoliticsLabel;

    private Image personEmotionIcon;

    private Image personHobbyIcon;

    private Image personPoliticsIcon;

    public Pop currentPersonSelected = null;

    //LevelUI
    [SerializeField]
    public Text disinformationValueLabel;

    [SerializeField]
    public Text netGrowthValueLabel;

    [SerializeField]
    public Text adRevenueValueLabel;

    [SerializeField]
    public Text timeLeftLabel;

    //GameOverUI
    [SerializeField]
    public GameObject gameOverPanel;

    [SerializeField]
    public TextMeshProUGUI gameOverMessageLabel;

    [SerializeField]
    public TextMeshProUGUI totalScoreLabel;

    [SerializeField]
    public TextMeshProUGUI lastGameScoreLabel;

    [SerializeField]
    public TextMeshProUGUI gameOverTitleLabel;

    [SerializeField]
    public Button levelSelectGoBack;

    [SerializeField]
    public TextMeshProUGUI levelSelectGoBackLabel;

    //HelpPanel
    [SerializeField]
    public GameObject helpPanel;

    //StartPanel
    [SerializeField]
    public GameObject startPanel;


    private void Awake()
    {
        personPanel = GameObject.Find("PersonInfoMenu");
        personInsidePanel = GameObject.Find("PersonInsidePanel");
        personEmotionLabel = personPanel.transform.Find("PersonEmotion").GetComponent<Text>();
        personHobbyLabel = personPanel.transform.Find("PersonHobby").GetComponent<Text>();
        personPoliticsLabel = personPanel.transform.Find("PersonPolitics").GetComponent<Text>();
        personEmotionIcon = personPanel.transform.Find("PersonEmotionIcon").GetComponent<Image>();
        personHobbyIcon = personPanel.transform.Find("PersonHobbyIcon").GetComponent<Image>();
        personPoliticsIcon = personPanel.transform.Find("PersonPoliticsIcon").GetComponent<Image>();

        changeVisibilityGameOverUI(false);
        changeVisibilityPersonUI(false);
        changeVisibilityHelpPanel(false);
        changeVisibilityStartPanel(true);
    }

    public void changeVisibilityStartPanel(bool state)
    {
        startPanel.SetActive(state);
    }

    public void changeVisibilityHelpPanel(bool state)
    {
        helpPanel.SetActive(state);
    }

    /// <summary>
    /// Set/Initialize node sprite and color
    /// </summary>
    /// <param name="node"></param>
    public void setNodeSprite(GraphNode node)
    {
        var spriteRenderer = node.GetComponent<SpriteRenderer>();
        var person = node.Node.Person;

        //Change sprite based on hobby
        switch (person.hobbyDenomination)
        {
            case Hobby.HobbyDenomination.Film:
                spriteRenderer.sprite = nodeHobbyMovies;
                break;
            case Hobby.HobbyDenomination.Games:
                spriteRenderer.sprite = nodeHobbyGames;
                break;
            case Hobby.HobbyDenomination.Football:
                spriteRenderer.sprite = nodeHobbyFootball;
                break;
            case Hobby.HobbyDenomination.Books:
                spriteRenderer.sprite = nodeHobbyBooks;
                break;
            case Hobby.HobbyDenomination.Fashion:
                spriteRenderer.sprite = nodeHobbyFashion;
                break;
        }
        //Change sprite color based on ideology
        if (person.emotionalValue > 0) spriteRenderer.color = Color.green;
        else spriteRenderer.color = Color.red;

        switch (person.ideologyDenominaion)
        {
            case Ideology.IdeologyDenomination.Communist:
                spriteRenderer.color = communistColor;
                break;
            case Ideology.IdeologyDenomination.Socialist:
                spriteRenderer.color = socialistColor;
                break;
            case Ideology.IdeologyDenomination.Centrist:
                spriteRenderer.color = centristColor;
                break;
            case Ideology.IdeologyDenomination.Liberal:
                spriteRenderer.color = liberalColor;
                break;
            case Ideology.IdeologyDenomination.Populist:
                spriteRenderer.color = populistColor;
                break;
        }
    }
    

    /// <summary>
    /// Set UI for when a node is first selected
    /// </summary>
    /// <param name="node"></param>
    public void setNodeSelected(GraphNode node)
    {
        var spriteRenderer = node.GetComponent<SpriteRenderer>();
        var person = node.Node.Person;
        //Change sprite to selected versions
        switch (person.hobbyDenomination)
        {
            case Hobby.HobbyDenomination.Film:
                spriteRenderer.sprite = nodeHobbyMoviesSelected;
                break;
            case Hobby.HobbyDenomination.Games:
                spriteRenderer.sprite = nodeHobbyGamesSelected;
                break;
            case Hobby.HobbyDenomination.Football:
                spriteRenderer.sprite = nodeHobbyFootballSelected;
                break;
            case Hobby.HobbyDenomination.Books:
                spriteRenderer.sprite = nodeHobbyBooksSelected;
                break;
            case Hobby.HobbyDenomination.Fashion:
                spriteRenderer.sprite = nodeHobbyFashionSelected;
                break;
        } 
    }

    private void changeVisibilityGameOverUI(bool state)
    {
        gameOverPanel.SetActive(state);
    }

    /// <summary>
    /// Set visibility of upper right panel with Person information
    /// </summary>
    /// <param name="state"></param>
    private void changeVisibilityPersonUI(bool state)
    {
        //Hide all elements in this panel
        personInsidePanel.GetComponent<Image>().enabled = state;
        personPanel.GetComponent<Image>().enabled = state;
        personEmotionLabel.enabled = state;
        personHobbyLabel.enabled = state;
        personPoliticsLabel.enabled = state;

        personEmotionIcon.enabled = state;
        personHobbyIcon.enabled = state;
        personPoliticsIcon.enabled = state;
    }

    /// <summary>
    /// Disable side menu visibility of person information
    /// </summary>
    public void disableSideMenuNodeUI()
    {
        changeVisibilityPersonUI(false);
        currentPersonSelected = null;
    }

    public void setSideMenuNodeUI(Pop person)
    {
        changeVisibilityPersonUI(true);
        updateSideMenuNodeUI(person);
        currentPersonSelected = person;

    }
    public void updateSideMenuNodeUI(Pop person)
    {
        Debug.Log("Showing POP UI");
        personEmotionLabel.text = person.emotionalName;
        personHobbyLabel.text = person.hobbyName;
        personPoliticsLabel.text = person.ideologyName;

        if (person.emotionalValue > 0) personEmotionIcon.color = Color.green;
        else personEmotionIcon.color = Color.red;

        switch (person.ideologyDenominaion)
        {
            case Ideology.IdeologyDenomination.Communist:
                personPoliticsIcon.color = communistColor;
                break;
            case Ideology.IdeologyDenomination.Socialist:
                personPoliticsIcon.color = socialistColor;
                break;
            case Ideology.IdeologyDenomination.Centrist:
                personPoliticsIcon.color = centristColor;
                break;
            case Ideology.IdeologyDenomination.Liberal:
                personPoliticsIcon.color = liberalColor;
                break;
            case Ideology.IdeologyDenomination.Populist:
                personPoliticsIcon.color = populistColor;
                break;
        }


        personPanel.SetActive(true);
    }

    /// <summary>
    /// Update level UI with disinformation, network growth and ad revenue
    /// </summary>
    public void setLevelUI(float disinformation, int totalNumberNodes,float networkGrowth,float adRevenueTotal, float adRevenueGained)
    {
        var disinfoFormated = (int)Math.Round(disinformation * 100);
        disinformationValueLabel.text = string.Format("{0}%", disinfoFormated);

        netGrowthValueLabel.text = string.Format("{0} people (+{1})", totalNumberNodes, networkGrowth);
        if (networkGrowth > 0) netGrowthValueLabel.color = Color.green;
        else if (networkGrowth < 0) netGrowthValueLabel.color = Color.red;
        else netGrowthValueLabel.color = Color.black;

        var adRevenueGainedRounded = (int)Math.Round(adRevenueGained);
        var adRevenueTotalRounded = (int)Math.Round(adRevenueTotal);

        adRevenueValueLabel.text = string.Format("{0} (+{1})$", adRevenueTotalRounded, adRevenueGainedRounded);
    }

    public void setTimeLevelUI(float timeLeftSeconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(timeLeftSeconds);
        string timeText = string.Format("{0:D2}:{1:D2}",timeSpan.Minutes, timeSpan.Seconds);
        timeLeftLabel.text = timeText;
    }

    public void showGameOverScreen(int currentLevel, string gameOverMessage, float totalAdRevenue)
    {
        changeVisibilityGameOverUI(true);
        int lastGameScore = PlayerPrefs.GetInt("totalScore" + currentLevel, 0);
        var adRevenueTotalRounded = (int)Math.Round(totalAdRevenue);
        gameOverMessageLabel.text = gameOverMessage;
        totalScoreLabel.text = string.Format("You made us {0}$", adRevenueTotalRounded);
        if (lastGameScore != 0) lastGameScoreLabel.text = string.Format("Record Profit:{0}$", lastGameScore);
        else lastGameScoreLabel.enabled = false;


        if (lastGameScore < adRevenueTotalRounded) PlayerPrefs.SetInt("totalScore"+currentLevel, adRevenueTotalRounded);
        PlayerPrefs.Save();
    }

    public void setNodeBubble(GraphBubble bubble)
    {
        var spriteRenderer = bubble.GetComponent<SpriteRenderer>();
        var color = communistColor;
        Dictionary<Ideology.IdeologyDenomination, int> countColors = new Dictionary<Ideology.IdeologyDenomination, int>(); 
        foreach(GraphNode graphNode in bubble.NodesInBubble)
        {
            var denomination = graphNode.Node.Person.ideologyDenominaion;
            if (countColors.ContainsKey(denomination))
            {
                countColors[graphNode.Node.Person.ideologyDenominaion] += 1;
            }
            else
            {
                countColors.Add(graphNode.Node.Person.ideologyDenominaion, 1);
            }
            
        }
        var mostPrevalentIdeology = countColors.OrderBy(x => x.Value).ToList()[0].Key;

        switch (mostPrevalentIdeology)
        {
            case Ideology.IdeologyDenomination.Communist:
                color = communistColor;
                break;
            case Ideology.IdeologyDenomination.Socialist:
                color = socialistColor;
                break;
            case Ideology.IdeologyDenomination.Centrist:
                color = centristColor;
                break;
            case Ideology.IdeologyDenomination.Liberal:
                color = liberalColor;
                break;
            case Ideology.IdeologyDenomination.Populist:
                color = populistColor;
                break;
        }


        color.a = 0.2f;
        spriteRenderer.color = color;
    }

    /// <summary>
    /// Update Links UI on basis of information flow(color) and similarity(width)
    /// </summary>
    /// <param name="link"></param>
    public void setLinkSprite(GraphLink link)
    {
        float info = link.Link.InfoLink.similairityValue;
        float sim = link.Link.InfoLink.similairityValue;
        var spriteRenderer = link.GetComponent<LineRenderer>();
        Color blueShade = Color.black;
        blueShade.b = 1 - (info / (Pop.maximumSimilarity * 2));
        blueShade.g = 1 - (info / (Pop.maximumSimilarity * 2));
        

        var width = (1 - (sim / Pop.maximumSimilarity)) / 5;
        if (width < 0.05) width = 0.05f;

        spriteRenderer.startWidth = width;
        spriteRenderer.endWidth = width;
        spriteRenderer.startColor = blueShade;
        spriteRenderer.endColor = blueShade;
    }

}
