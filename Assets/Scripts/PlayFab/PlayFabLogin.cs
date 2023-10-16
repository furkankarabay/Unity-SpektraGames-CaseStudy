using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using TMPro;
using UnityEngine;

public class PlayFabLogin : MonoBehaviour
{
    [Header("Login / Register")]
    [SerializeField] 
    private TMP_InputField usernameLoginInput;

    [SerializeField] 
    private TMP_InputField passwordLoginInput;

    [SerializeField]
    private TMP_Text errorText;

    [Header("Display Name")]

    [SerializeField]
    private TMP_InputField displayNameInput;


    public void GuestLogin()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,

            CreateAccount = true,

            TitleId = PlayFabSettings.TitleId,

            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
            {
                GetPlayerProfile = true
            }

        };

        PlayFabClientAPI.LoginWithCustomID(request, OnGuestLoginResult, OnGuestLoginError);
    }

    private void OnGuestLoginError(PlayFabError error)
    {
        StopAllCoroutines();
        StartCoroutine(ErrorReport(error.GenerateErrorReport()));
    }

    private void OnGuestLoginResult(LoginResult result)
    {
        UIManager.Instance.ToggleLoginPanel();

        UIManager.Instance.ToggleMenuPanel();

        if(string.IsNullOrEmpty(result.InfoResultPayload.PlayerProfile.DisplayName))
            result.InfoResultPayload.PlayerProfile.DisplayName = Utils.GetRandomName();

        UIManager.Instance.UpdatePlayerNameText(result.InfoResultPayload.PlayerProfile.DisplayName);

        Debug.Log("Successfuly guest logged in.");
    }

    public void Login()
    {
        var request = new LoginWithPlayFabRequest
        {
            Username = usernameLoginInput.text,
            Password = passwordLoginInput.text,

            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
            {
                GetPlayerProfile = true
            }
        };

        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginError);

    }
    private void OnLoginError(PlayFabError error)
    {
        var request = new RegisterPlayFabUserRequest
        {
            Username = usernameLoginInput.text,
            Password = passwordLoginInput.text,
            DisplayName = Utils.GetRandomName(),

            RequireBothUsernameAndEmail = false,
            
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams() 
            {
                GetPlayerProfile = true
            }
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterError);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        UIManager.Instance.ToggleLoginPanel();

        UIManager.Instance.ToggleMenuPanel();

        UIManager.Instance.UpdatePlayerNameText(result.InfoResultPayload.PlayerProfile.DisplayName);
        Debug.Log("Successfuly logged in.");
    }


    private void OnRegisterError(PlayFabError error)
    {
        StopAllCoroutines();
        StartCoroutine(ErrorReport(error.GenerateErrorReport()));
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("Successfuly created account. You are logging in.");
        Login();
    }

    public void LogOut()
    {
        PlayFabClientAPI.ForgetAllCredentials();

        UIManager.Instance.ToggleMenuPanel();

        UIManager.Instance.ToggleLoginPanel();

        UIManager.Instance.ResetMenuDisplay();

        if(PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();
    }

    public void ChangePlayerDisplayName()
    {
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = displayNameInput.text
        },
        result =>
        {
            UIManager.Instance.UpdatePlayerNameText(result.DisplayName);
            Debug.Log("The player's display name is now: " + result.DisplayName);
        },
        error =>
        {
            StopAllCoroutines();
            StartCoroutine(ErrorReport(error.GenerateErrorReport()));
        });
    }

    private IEnumerator ErrorReport(string errorContext)
    {

        errorText.text = errorContext;

        yield return new WaitForSeconds(5);

        errorText.text = "";
    }
}
