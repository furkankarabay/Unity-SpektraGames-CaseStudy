using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonLauncher : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private int maxPlayers = 2;

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void OnPlayOnlineButtonClicked()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        CreateRoom();
    }

    private void CreateRoom()
    {
        string roomName = "Room " + Random.Range(1000, 10000);

        RoomOptions roomOptions = new RoomOptions();

        roomOptions.MaxPlayers = maxPlayers;

        PhotonNetwork.CreateRoom(roomName, roomOptions, null);

        UIManager.Instance.ChangeMenuDisplayToSearchingGame();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room created...");

        base.OnCreatedRoom();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        UIManager.Instance.ResetMenuDisplay();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Player joined.");
        photonView.RPC("CountdownForStartGame", RpcTarget.All);
    }

    [PunRPC]
    void CountdownForStartGame()
    {
        Debug.Log("Started countdown..");

        PhotonNetwork.CurrentRoom.IsOpen = false;
        UIManager.Instance.ToggleMenuPanel();

        CountdownTimer.SetStartTime(CountdownType.BeforeStart);
    }


    public Hashtable GetOpponentResults()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (!PhotonNetwork.PlayerList[i].IsLocal)
            {
                return PhotonNetwork.PlayerList[i].CustomProperties;
            }
        }

        return null;
    }

    public void SetMyResults(int newScore, int highscore, int totalVictories, int totalDefeats)
    {
        PhotonNetwork.LocalPlayer.SetResults(newScore, highscore, totalVictories, totalDefeats);
    }

    public int GetOpponentScore()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (!PhotonNetwork.PlayerList[i].IsLocal)
            {
                return PhotonNetwork.PlayerList[i].GetScore();
            }
        }

        return 0;
    }

    public void AddMyScore()
    {
        PhotonNetwork.LocalPlayer.AddScore(1);
    }
    public int GetMyScore()
    {
        return PhotonNetwork.LocalPlayer.GetScore();
    }

    public string GetOpponentName()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (!PhotonNetwork.PlayerList[i].IsLocal)
            {
                return PhotonNetwork.PlayerList[i].NickName;
            }
        }

        return null;
    }

    public string GetMyName()
    {
        return PhotonNetwork.LocalPlayer.NickName;
    }
}
