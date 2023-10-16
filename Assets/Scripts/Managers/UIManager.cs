using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]

    [SerializeField] 
    private GameObject sectionLogin;

    [SerializeField] 
    private GameObject sectionMenu;

    [SerializeField]
    private GameObject sectionGame;

    [SerializeField]
    private GameObject sectionResults;


    [Header("Menu Panel UI Elements")]

    [SerializeField]
    private Button playOnlineButton;

    [SerializeField]
    private TMP_Text playOnlineButtonText;

    [SerializeField]
    private TMP_Text playerNameText;

    [SerializeField]
    private TMP_InputField playerNameInput;

    [SerializeField]
    private Button playerNameChangeApplyButton;

    [SerializeField]
    private TMP_Text scoreText;

    [Header("Results Panel UI Elements")]

    [SerializeField]
    private TMP_Text localPlayerNameText;
    [SerializeField]
    private TMP_Text opponentPlayerNameText;

    [SerializeField]
    private TMP_Text localPlayerScoreText;
    [SerializeField]
    private TMP_Text opponentPlayerScoreText;

    [SerializeField]
    private TMP_Text localPlayerHighscoreText;
    [SerializeField]
    private TMP_Text opponentPlayerHighscoreText;

    [SerializeField]
    private TMP_Text localPlayerTotalVictoriesText;
    [SerializeField]
    private TMP_Text opponentPlayerTotalVictoriesText;

    [SerializeField]
    private TMP_Text localPlayerTotalDefeatsText;
    [SerializeField]
    private TMP_Text opponentPlayerTotalDefeatText;

    [SerializeField]
    private TMP_Text winnerText;

    public static UIManager Instance;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    public void ToggleLoginPanel()
    {
        if (sectionLogin != null)
        {
            sectionLogin.SetActive(!sectionLogin.activeSelf);
        }
    }

    public void ToggleMenuPanel()
    {
        if (sectionGame != null)
        {
            sectionMenu.SetActive(!sectionMenu.activeSelf);
        }
    }

    public void ToggleGamePanel()
    {
        if (sectionMenu != null)
        {
            sectionGame.SetActive(!sectionGame.activeSelf);
        }
    }

    public void ToggleAfterGamePanel()
    {
        if (sectionResults != null)
        {
            sectionResults.SetActive(!sectionResults.activeSelf);
        }
    }

    public void ChangeMenuDisplayToSearchingGame()
    {
        playOnlineButton.interactable = false;
        playOnlineButtonText.text = "Matching...";
        playerNameInput.gameObject.SetActive(false);
        playerNameChangeApplyButton.gameObject.SetActive(false);
    }

    public void ResetMenuDisplay()
    {
        playOnlineButton.interactable = true;
        playOnlineButtonText.text = "Play Online";
        playerNameInput.gameObject.SetActive(true);
        playerNameChangeApplyButton.gameObject.SetActive(true);

    }

    public void UpdatePlayerNameText(string playerName)
    {
        playerNameText.text = "Welcome " + playerName;

        PhotonNetwork.LocalPlayer.NickName = playerName;
    }

    public void UpdateInGameScoreText(int score)
    {
        scoreText.text = score.ToString();
    }

    public void UpdateResultsPlayerNames(string localPlayerName, string opponentPlayerName)
    {
        localPlayerNameText.text = localPlayerName;
        opponentPlayerNameText.text = opponentPlayerName;
    }
    public void UpdateResultsScore(int localScore, int opponentScore)
    {
        localPlayerScoreText.text = localScore.ToString();
        opponentPlayerScoreText.text = opponentScore.ToString();
    }

    public void UpdateResultsHighscore(int localHighscore, int opponentHighscore)
    {
        localPlayerHighscoreText.text = localHighscore.ToString();
        opponentPlayerHighscoreText.text = opponentHighscore.ToString();
    }

    public void UpdateResultsTotalVictories(int localTotalVictories, int opponentTotalVictories)
    {
        localPlayerTotalVictoriesText.text = localTotalVictories.ToString();
        opponentPlayerTotalVictoriesText.text = opponentTotalVictories.ToString();
    }

    public void UpdateResultsTotalDefeats(int localTotalDefeats, int opponentTotalDefeats)
    {
        localPlayerTotalDefeatsText.text = localTotalDefeats.ToString();
        opponentPlayerTotalDefeatText.text = opponentTotalDefeats.ToString();
    }

    public void UpdateWinnerText(bool isDraw, string winnerName)
    {
        if(isDraw)
            winnerText.text = "Draw";
        else
            winnerText.text = "Winner is " + winnerName;
    }

    public void ResetGamePanel()
    {
        scoreText.text = "0";

    }
}
