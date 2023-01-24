using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OpenCloseCredits : MonoBehaviour
{
    [SerializeField]
    public GameObject creditsPanel;

    [SerializeField]
    public TextMeshProUGUI creditsLabel;

    [SerializeField]
    public Button creditsButton;

    [SerializeField]
    public TextMeshProUGUI creditsButtonLabel;


    public void Start()
    {
        closeCreditsPanel();
    }

    public void openCreditsPanel()
    {
        creditsPanel.SetActive(true);
    }

    public void closeCreditsPanel()
    {
        creditsPanel.SetActive(false);
    }

}
