using ExitGames.Client.Photon;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public const string Highscore = "Highscore";
    public const string NumberOfVictories = "NumberOfVictories";
    public const string NumberOfDefeats = "NumberOfDefeats";
    public const string AllTimeClick = "AllTimeClick";

    private int highscore;
    private int numberOfVictories;
    private int numberOfDefeats;
    private int allTimeClick;

    private int clickedInCurrentGame = 0;
    private bool iWon = false;
    private bool iLost = false;


    [SerializeField]
    private PhotonLauncher photonLauncher;

    [SerializeField]
    private Button scoreButton;

    private void OnEnable()
    {
        CountdownTimer.OnCountdownTimerHasExpired += OnCountdownTimerHasExpired;
    }

    private void OnDisable()
    {
        CountdownTimer.OnCountdownTimerHasExpired -= OnCountdownTimerHasExpired;
    }

    private void OnCountdownTimerHasExpired(CountdownType countdownType)
    {
        switch (countdownType)
        {
            case CountdownType.BeforeStart:
                scoreButton.interactable = true;

                CountdownTimer.SetStartTime(CountdownType.InGame);

                UIManager.Instance.ToggleGamePanel();

                break;

            case CountdownType.InGame:

                scoreButton.interactable = false;

                GetPlayfabStatistics();

                UIManager.Instance.ToggleGamePanel();

                UIManager.Instance.ToggleAfterGamePanel();


                break;
        }
    }


    private void SetWinLoseStates()
    {
        int myScore = photonLauncher.GetMyScore();
        int playerTwoScore = photonLauncher.GetOpponentScore();

        if (myScore > playerTwoScore)
            iWon = true;
        else if (myScore < playerTwoScore)
            iLost = true;

    }

    public void OnScoreButtonClicked()
    {
        clickedInCurrentGame++;
        UIManager.Instance.UpdateInGameScoreText(clickedInCurrentGame);
        photonLauncher.AddMyScore();
    }

    private void UpdateValues()
    {
        allTimeClick = allTimeClick + clickedInCurrentGame;

        highscore = clickedInCurrentGame > highscore ? clickedInCurrentGame : highscore;

        if (iWon)
            numberOfVictories++;
        else if (iLost)
            numberOfDefeats++;
    }

    private void UpdatePlayfabPlayerStatistics()
    {
        UpdateValues();

        photonLauncher.SetMyResults(clickedInCurrentGame, highscore, numberOfVictories, numberOfDefeats);

        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate { StatisticName = AllTimeClick, Value = allTimeClick},
                new StatisticUpdate { StatisticName = Highscore, Value = highscore},
                new StatisticUpdate { StatisticName = NumberOfVictories, Value = numberOfVictories},
                new StatisticUpdate { StatisticName = NumberOfDefeats, Value = numberOfDefeats},
            }

        },
        result => { UpdateResultsField(); },
        errorCallback => { Debug.LogError(errorCallback.GenerateErrorReport()); });

        
    }

    private void UpdateResultsField()
    {
        UIManager.Instance.UpdateResultsPlayerNames(photonLauncher.GetMyName(), photonLauncher.GetOpponentName());

        Hashtable hash = photonLauncher.GetOpponentResults();

        object opponentScore;
        if (hash.TryGetValue(PunPlayerResults.PlayerScoreProp, out opponentScore))
        {
            UIManager.Instance.UpdateResultsScore(clickedInCurrentGame, (int)opponentScore);
        }

        object opponentHighscore;
        if (hash.TryGetValue(PunPlayerResults.PlayerHighscoreProp, out opponentHighscore))
        {
            UIManager.Instance.UpdateResultsHighscore(highscore, (int) opponentHighscore);
        }

        object opponentTotalVictories;
        if (hash.TryGetValue(PunPlayerResults.PlayerTotalVictoriesProp, out opponentTotalVictories))
        {
            UIManager.Instance.UpdateResultsTotalVictories(numberOfVictories, (int)opponentTotalVictories);
        }

        object opponentTotalDefeats;
        if (hash.TryGetValue(PunPlayerResults.PlayerTotalDefeatsProp, out opponentTotalDefeats))
        {
            UIManager.Instance.UpdateResultsTotalDefeats(numberOfDefeats, (int)opponentTotalDefeats);
        }

        if((int) opponentScore < clickedInCurrentGame)
            UIManager.Instance.UpdateWinnerText(false, photonLauncher.GetMyName());
        else if((int) opponentScore > clickedInCurrentGame)
            UIManager.Instance.UpdateWinnerText(false, photonLauncher.GetOpponentName());
        else
            UIManager.Instance.UpdateWinnerText(true, photonLauncher.GetOpponentName());


    }

    private void GetPlayfabStatistics()
    {
        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest(),
            result => { OnGetPlayfabStatistics(result); },
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }

    private void OnGetPlayfabStatistics(GetPlayerStatisticsResult result)
    {
        foreach (var eachStat in result.Statistics)
        {
            Debug.Log("Statistics (" + eachStat.StatisticName + ")" + eachStat.Value);

            switch (eachStat.StatisticName)
            {
                case Highscore:
                    highscore = eachStat.Value;
                    break;

                case NumberOfVictories:
                    numberOfVictories = eachStat.Value;
                    break;

                case NumberOfDefeats: 
                    numberOfDefeats = eachStat.Value;
                    break;

                case AllTimeClick: 
                    allTimeClick = eachStat.Value;
                    break;
            }
        }


        SetWinLoseStates();

        UpdatePlayfabPlayerStatistics();
    }

    public void OnResultsPanelBackButtonClicked()
    {
        PhotonNetwork.LeaveRoom(false);

        UIManager.Instance.ResetMenuDisplay();

        UIManager.Instance.ToggleAfterGamePanel();

        UIManager.Instance.ToggleMenuPanel();

        UIManager.Instance.ResetGamePanel();
        ResetVariables();
    }

    private void ResetVariables()
    {
        highscore = 0;
        numberOfVictories = 0;
        numberOfDefeats = 0;
        allTimeClick = 0;

        clickedInCurrentGame = 0;
        iWon = false;
        iLost = false;
    }
}
