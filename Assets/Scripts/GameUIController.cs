using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUIController : MonoBehaviour
{
    public static GameUIController instance;
    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject endCanvas;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private SpriteRenderer bgSprite;
    [SerializeField] private Sprite[] bgSprites;
    private int combo;
    private int score;


    private void Awake()
    {
        if(instance==null)
            instance = this;
        bgSprite.sprite = bgSprites[Random.Range(0, bgSprites.Length)];
    }

    public void UpdateScore(int whatToAdd)
    {
        if (whatToAdd < 4)
            Debug.LogError("whatToAdd<4 CRITICAL ERROR");
        int scoreToAdd = 10 + ((whatToAdd - 4) * 5);
        //Debug.Log("����� = " + combo + "\n score = " + score + "\n whatTOADD = " + whatToAdd);
        score += scoreToAdd * combo;
       // Debug.Log("���� ����� " + score);
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
    
    public void SetStartCanvas(bool what)
    {
        startCanvas.SetActive(what);
    }

    public void SetEndCanvas(bool what)
    {
        endCanvas.SetActive(what);
        StatSaver.instance.SaveBestScore(score);
        bestScoreText.text = StatSaver.instance.GetBestScore().ToString();
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
