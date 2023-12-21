using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUIController : MonoBehaviour
{
    public static GameUIController instance;
    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject endCanvas;
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private TextMeshProUGUI bestScoreStartText;
    [SerializeField] private SpriteRenderer bgSprite;
    [SerializeField] private Sprite[] bgSprites;
    [SerializeField] private float comboFadeTime;
    [SerializeField] private GameObject soundCrossImage;
    [SerializeField] private GameObject musicCrossImage;

    private bool pauseState = false;
    private int combo;
    private int score;
    private int currentBGSprite;

    private void Awake()
    {
        if(instance==null)
            instance = this;   
    }
    private void Start()
    {
        SetStartCanvas(true);
    }

    public void ChangeBGSprites()
    {
        currentBGSprite++;
        if(currentBGSprite>= bgSprites.Length)
            currentBGSprite = 0;
        bgSprite.sprite = bgSprites[currentBGSprite];
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

    public IEnumerator ClearCombo()
    {
        yield return new WaitForSeconds(comboFadeTime);
        combo = 0;
        UpdateComboText();
    } 
    
    public void SetStartCanvas(bool what)
    {
        startCanvas.SetActive(what);
        bestScoreStartText.text = StatSaver.instance.GetBestScore().ToString();
    }

    public void SetEndCanvas(bool what)
    {
        endCanvas.SetActive(what);
        StatSaver.instance.SaveBestScore(score);
        bestScoreText.text = StatSaver.instance.GetBestScore().ToString();
    }

    public void PauseGame()
    {
        pauseState = !pauseState;
        pauseCanvas.SetActive(pauseState);
        if(pauseState)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;
    }

    public void SetMusicCross()
    {
        musicCrossImage.SetActive(!musicCrossImage.activeInHierarchy);
    }

    public void SetSoundCross()
    {
        soundCrossImage.SetActive(!soundCrossImage.activeInHierarchy);
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
