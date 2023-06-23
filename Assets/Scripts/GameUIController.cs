using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUIController : MonoBehaviour
{
    public static GameUIController instance;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI scoreText;
    private int combo;
    private int score;


    private void Awake()
    {
        if(instance==null)
            instance = this;
    }

    public void UpdateScore(int whatToAdd)
    {
        if (whatToAdd < 4)
            Debug.LogError("whatToAdd<4 CRITICAL ERROR");
        int scoreToAdd = 10 + ((whatToAdd - 4) * 5);
        //Debug.Log("комбо = " + combo + "\n score = " + score + "\n whatTOADD = " + whatToAdd);
        score += scoreToAdd * combo;
       // Debug.Log("счет после " + score);
        UpdateScoreText();
    }

    public void UpdateCombo(int whatToAdd)
    {
        combo += whatToAdd;
        UpdateComboText();
    }

    public void ClearCombo()
    {       
        combo = 0;
        UpdateComboText();
    } 
    private void UpdateComboText()
    {
        comboText.text = combo.ToString();
    }
    private void UpdateScoreText()
    {
        scoreText.text = score.ToString();  
    }
    
}
