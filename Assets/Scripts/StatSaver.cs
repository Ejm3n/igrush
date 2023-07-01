using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatSaver : MonoBehaviour
{
    public static StatSaver instance;
    
    private void Awake()
    {
        if (instance == null)
            instance = this;      
    }

    public int GetBestScore()
    {
       return PlayerPrefs.GetInt(GlobalVariables.PlayerPrefs.BestScore, 0);
    }

    public void SaveBestScore(int score)
    {
        if(score > PlayerPrefs.GetInt(GlobalVariables.PlayerPrefs.BestScore, 0))
            PlayerPrefs.SetInt(GlobalVariables.PlayerPrefs.BestScore, score);
    }


}
