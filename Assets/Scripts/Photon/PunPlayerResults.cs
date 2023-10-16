using UnityEngine;

using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PunPlayerResults : MonoBehaviour
{
    public const string PlayerScoreProp = "Score";
    public const string PlayerHighscoreProp = "Highscore";
    public const string PlayerTotalVictoriesProp = "TotalVictories";
    public const string PlayerTotalDefeatsProp = "TotalDefeats";
}

public static class ResultsExtensions
{
    public static void SetResults(this Player player, int newScore, int highscore, int totalVictories, int totalDefeats)
    {
        Hashtable score = new Hashtable();  // using PUN's implementation of Hashtable
        score[PunPlayerResults.PlayerScoreProp] = newScore;
        score[PunPlayerResults.PlayerHighscoreProp] = highscore;
        score[PunPlayerResults.PlayerTotalVictoriesProp] = totalVictories;
        score[PunPlayerResults.PlayerTotalDefeatsProp] = totalDefeats;

        player.SetCustomProperties(score);  // this locally sets the score and will sync it in-game asap.
    }

    public static void AddScore(this Player player, int scoreToAddToCurrent)
    {
        int current = player.GetScore();
        current = current + scoreToAddToCurrent;

        Hashtable score = new Hashtable();  // using PUN's implementation of Hashtable
        score[PunPlayerResults.PlayerScoreProp] = current;

        player.SetCustomProperties(score);  // this locally sets the score and will sync it in-game asap.
    }

    public static int GetScore(this Player player)
    {
        object score;

        if (player.CustomProperties.TryGetValue(PunPlayerResults.PlayerScoreProp, out score))
        {
            return (int)score;
        }

        return 0;
    }

}