// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CountdownTimer.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Utilities,
// </copyright>
// <summary>
// This is a basic CountdownTimer. In order to start the timer, the MasterClient can add a certain entry to the Custom Room Properties,
// which contains the property's name 'StartTime' and the actual start time describing the moment, the timer has been started.
// To have a synchronized timer, the best practice is to use PhotonNetwork.Time.
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CountdownTimer : MonoBehaviourPunCallbacks
{
    /// <summary>
    ///     OnCountdownTimerHasExpired delegate.
    /// </summary>
    public delegate void CountdownTimerHasExpired(CountdownType countdownType);

    public const string CountdownStartTime = "StartTime";

    [Header("Countdown time in seconds")]
    public List<float> Countdowns;


    private bool isTimerRunning;

    private static CountdownType currentCountdownType;

    private int startTime;

    [Header("Reference to a Text component for visualizing the countdown")]
    public List<TMP_Text> Texts;


    /// <summary>
    ///     Called when the timer has expired.
    /// </summary>
    public static event CountdownTimerHasExpired OnCountdownTimerHasExpired;

    public void Update()
    {
        if (!this.isTimerRunning) return;

        float countdown = TimeRemaining();
        this.Texts[(int)currentCountdownType].text = countdown.ToString("n0");

        if (countdown > 0.0f) return;

        OnTimerEnds();
    }


    private void OnTimerRuns()
    {
        this.isTimerRunning = true;
        this.enabled = true;
    }

    private void OnTimerEnds()
    {
        this.isTimerRunning = false;
       
        this.Texts[(int)currentCountdownType].text = "";

        if (OnCountdownTimerHasExpired != null) 
            OnCountdownTimerHasExpired(currentCountdownType);
    }


    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        Initialize();
    }


    private void Initialize()
    {
        int propStartTime;
        if (TryGetStartTime(out propStartTime))
        {
            this.startTime = propStartTime;

            this.isTimerRunning = TimeRemaining() > 0;

            if (this.isTimerRunning)
                OnTimerRuns();
            else
                OnTimerEnds();
        }
    }


    private float TimeRemaining()
    {
        int timer = PhotonNetwork.ServerTimestamp - this.startTime;
        return this.Countdowns[(int)currentCountdownType] - timer / 1000f;
    }


    public static bool TryGetStartTime(out int startTimestamp)
    {
        startTimestamp = PhotonNetwork.ServerTimestamp;

        object startTimeFromProps;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(CountdownStartTime, out startTimeFromProps))
        {
            startTimestamp = (int)startTimeFromProps;
            return true;
        }

        return false;
    }


    public static void SetStartTime(CountdownType countdownType)
    {
        
        currentCountdownType = countdownType;
        int startTime = 0;
        bool wasSet = TryGetStartTime(out startTime);

        Hashtable props = new Hashtable
        {
            {CountdownStartTime, (int)PhotonNetwork.ServerTimestamp}
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }
}

public enum CountdownType
{
    BeforeStart,
    InGame,
    AfterGame
}