using System.Collections;
using TMPro;
using UnityEngine;

public class GameUIController : MonoBehaviour
{
    public static GameUIController instance;
    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject endCanvas;
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI endScoreText;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private TextMeshProUGUI bestScoreStartText;
    [SerializeField] private SpriteRenderer bgSprite;
    [SerializeField] private Sprite[] bgSprites;
    [SerializeField] private float comboFadeTime;
    [SerializeField] private SpriteRenderer[] nextPuyos;
    [SerializeField] private Color[] nextPyosColors;
    [SerializeField] private TextMeshProUGUI soundText;
    [SerializeField] private TextMeshProUGUI musicText;
    private bool pauseState = false;
    private int combo = -1;
    private int score;
    private int currentBGSprite;
    private bool comboLastRound;
    private string music = "MUSIC: ";
    private string sound = "SOUND: ";
    private string on = "ON";
    private string off = "OFF";

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    private void Start()
    {
        SetStartCanvas(true);
        Time.timeScale = 1f;
    }

    public void UpdateNextPyuos(int puyo1,int puyo2)
    {
        nextPuyos[0].color = nextPyosColors[puyo1];
        nextPuyos[1].color = nextPyosColors[puyo2];
    }
    public void ChangeBGSprites()
    {

        currentBGSprite++;
        //Debug.Log("теперь спрайт номер " + currentBGSprite);
        if (currentBGSprite >= bgSprites.Length)
            currentBGSprite = 0;
        bgSprite.sprite = bgSprites[currentBGSprite];
    }

    public void UpdateScore(int whatToAdd)
    {
        if (whatToAdd < 4)
            Debug.LogError("whatToAdd<4 CRITICAL ERROR");
        int scoreToAdd = 10 + ((whatToAdd - 4) * 5);
        //Debug.Log("комбо = " + combo + "\n score = " + score + "\n whatTOADD = " + whatToAdd);
        if (combo > 0)
            score += scoreToAdd * combo;
        // Debug.Log("счет после " + score);
        UpdateScoreText();
    }

    public void UpdateCombo(int whatToAdd)
    {
        combo += whatToAdd;
        UpdateComboText();
    }
    public void SetLastRoundComboed(bool comboed)
    {
        comboLastRound = comboed;
    }
    public bool GetLastRoundComboed()
    {
        return comboLastRound;
    }
    public IEnumerator ClearCombo()
    {

        yield return new WaitForSeconds(comboFadeTime);
        combo = 0;
        UpdateComboText();
        yield break;
    }

    public void SetStartCanvas(bool what)
    {
        startCanvas.SetActive(what);
        bestScoreStartText.text = StatSaver.instance.GetBestScore().ToString();
    }

    public void SetEndCanvas(bool what)
    {
        endCanvas.SetActive(what);
        endScoreText.text = score.ToString()    ;
        StatSaver.instance.SaveBestScore(score);
        bestScoreText.text = StatSaver.instance.GetBestScore().ToString();
    }

    public void PauseGame()
    {
        pauseState = !pauseState;
        pauseCanvas.SetActive(pauseState);
        if (pauseState)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;
    }

    public void SetMusicCross(bool active)
    {
        if (active)
            musicText.text = music + on;
        else
            musicText.text = music+off;

    }

    public void SetSoundCross(bool active)
    {
        if (active)
            soundText.text = sound + on;
        else
            soundText.text = sound + off;
    }
    public int GetScore()
    {
        return score;
    }
    public void SetCurrentScore(int score)
    {
        this.score = score;
        UpdateScoreText();
    }
    private void UpdateComboText()
    {
        comboText.text = combo.ToString();
        if (combo < 0)
            comboText.text = "0";
    }
    private void UpdateScoreText()
    {
        scoreText.text = score.ToString();
    }

}
