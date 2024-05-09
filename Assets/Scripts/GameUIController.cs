using System;
using System.Collections;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;
public enum Language
{
    ru,
    en
}

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

    //localisation
    private string music_ru = "������: ";
    private string sound_ru = "�����: ";
    private string on_ru = "���";
    private string off_ru = "����";
    private string music_en = "MUSIC: ";
    private string sound_en = "SOUNDS: ";
    private string on_en = "ON";
    private string off_en = "OFF";
    [SerializeField] private Language language;

    private const int GetSecondChanceAdID = 1;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        Debug.Log("lang = " + YandexGame.EnvironmentData.language);
        if (YandexGame.EnvironmentData.language == "ru")
            language = Language.ru;
        else language = Language.en;

    }
    private void OnEnable()
    {
        YandexGame.RewardVideoEvent += GiveSecondLife;
    }
    private void OnDisable()
    {
        YandexGame.RewardVideoEvent -= GiveSecondLife;
    }
    private void GiveSecondLife(int id)
    {
        if (id != GetSecondChanceAdID)
            return;
        ContinueGameAfterLoss();
    }

    private void Start()
    {
        SetStartCanvas(true);
        Time.timeScale = 1f;
    }

    public void OnGetRewardClicked()
    {
        YGAdsProvider.ShowRewardedAd(GetSecondChanceAdID);
    }

    public void UpdateNextPyuos(int puyo1,int puyo2)
    {
        nextPuyos[0].color = nextPyosColors[puyo1];
        nextPuyos[1].color = nextPyosColors[puyo2];
    }
    public void ChangeBGSprites()
    {

        currentBGSprite++;
        //Debug.Log("������ ������ ����� " + currentBGSprite);
        if (currentBGSprite >= bgSprites.Length)
            currentBGSprite = 0;
        bgSprite.sprite = bgSprites[currentBGSprite];
    }

    public void UpdateScore(int whatToAdd)
    {
        int scoreToAdd = 10 + ((whatToAdd - 4) * 5);
        //Debug.Log("����� = " + combo + "\n score = " + score + "\n whatTOADD = " + whatToAdd);
        if (combo > 0)
            score += scoreToAdd * combo;
        // Debug.Log("���� ����� " + score);
        UpdateScoreText();
        StatSaver.instance.SaveGameField();
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
        StatSaver.instance.SaveGameField();
        endCanvas.SetActive(what);
        if(what)
        {
            endScoreText.text = score.ToString();
            StatSaver.instance.SaveBestScore(score);
            bestScoreText.text = StatSaver.instance.GetBestScore().ToString();
        }     
    }
    public void ContinueGameAfterLoss()
    {
        StatSaver.instance.ClearTopLines(10); //�������� �������� �������(��� �������)
        FindObjectOfType<GameStart>().StartGame();
        Time.timeScale = 1f;
    }

    public void AcceptDefeat()
    {
        StatSaver.instance.CleanGameFieldSave();
    }

    public void PauseGame()
    {
        PauseGame(!pauseState);
    }
    public void PauseGame(bool what)
    {
        if (!PuyoSpawner.Ongoing)
            return;
        pauseState = what;
        pauseCanvas.SetActive(pauseState);
        if (pauseState)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;
    }

    public void SetMusicCross(bool active)
    {
        if(language == Language.ru)
        {
            if (active)
                musicText.text = music_ru + on_ru;
            else
                musicText.text = music_ru + off_ru;
        }
        else
        {
            if (active)
                musicText.text = music_en + on_en;
            else
                musicText.text = music_en + off_en;
        }

    }

    public void SetSoundCross(bool active)
    {
        if(language == Language.ru)
        {
            if (active)
                soundText.text = sound_ru + on_ru;
            else
                soundText.text = sound_ru + off_ru;
        }
        else
        {
            if (active)
                soundText.text = sound_en + on_en;
            else
                soundText.text = sound_en + off_en;
        }
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
