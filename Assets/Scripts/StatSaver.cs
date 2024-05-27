using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using YG;

[System.Serializable]
public class BoardData
{
    public Vector2 position;
    public int status;
   

    public BoardData(Vector2 pos, int stat)
    {
        position = pos;
        status = stat;
    }
}
[System.Serializable]
public class BoardDataList
{
    public List<BoardData> items;
    public BoardDataList(List<BoardData> items)
    {
        this.items = items;
    }
}
public class StatSaver : MonoBehaviour
{
    public static StatSaver instance;
    [SerializeField] private PuyoSpawner spawner;
    private static string YandexLeaderBoardName = "Leaderboard";

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void OnEnable()
    {
        YandexGame.GetDataEvent += LoadScene;
    }

    private void OnDisable()
    {
        YandexGame.GetDataEvent -= LoadScene;
    }

    public int GetBestScore()
    {
       return PlayerPrefs.GetInt(GlobalVariables.PlayerPrefs.BestScore, 0);
    }

    public void SaveBestScore(int score)
    {
        if(score > PlayerPrefs.GetInt(GlobalVariables.PlayerPrefs.BestScore, 0))
        {
            PlayerPrefs.SetInt(GlobalVariables.PlayerPrefs.BestScore, score);
            YandexGame.NewLeaderboardScores(YandexLeaderBoardName, score);
        }
            
    }
    public void SaveGameField()
    {
        string json = "";
        List<BoardData> myObjectData = new List<BoardData> ();
        foreach (PuyoUnit puyoUnit in GameBoard.GetPuyoUnitsWithoutFlyingOne())
        {
            myObjectData.Add(new BoardData(new Vector2(Mathf.Round(puyoUnit.transform.position.x), Mathf.Round(puyoUnit.transform.position.y)), puyoUnit.GetColorIdx()));
            
        }
        json = JsonUtility.ToJson(new BoardDataList(myObjectData));
        PlayerPrefs.SetString("GameBoard", json);
        PlayerPrefs.SetInt("CurrentScore", GameUIController.instance.GetScore());
        PlayerPrefs.SetInt("PuyoSpeed", spawner.Difficulty);
        Debug.Log(json);
    }

    public void CleanGameFieldSave()
    {
        PlayerPrefs.SetString("GameBoard", "");
    }

    public int GetCurrentScore()
    {
        return PlayerPrefs.GetInt("CurrentScore",0);
    }

    public BoardDataList LoadGameField()
    {
        BoardDataList loadedData = new BoardDataList(new List<BoardData>());
        try
        {
            loadedData = JsonUtility.FromJson<BoardDataList>(PlayerPrefs.GetString("GameBoard"));
            return loadedData;
        }
        catch
        {
            return null;
        }      
    }

    public void LoadScene()
    {
        GameBoard.DeleteAllPuyos();
        BoardDataList boardDatas = LoadGameField();
        if (boardDatas != null && boardDatas.items.Count > 0)
        {
            foreach (BoardData boardData in boardDatas.items)
            {
                PuyoUnit newPuyo = Instantiate(Resources.Load("PuyoUnit"), boardData.position, Quaternion.identity).GetComponent<PuyoUnit>();
                newPuyo.SetColorIdx(boardData.status);
                newPuyo.StopFalling();
                GameBoard.Add(boardData.position.x, boardData.position.y, newPuyo.transform);
            }
            GameUIController.instance.SetCurrentScore(GetCurrentScore());
            
            int timesToChangeSpeed = PlayerPrefs.GetInt("PuyoSpeed", 0);
            Debug.Log("ttcs = " + timesToChangeSpeed);
            for (int i = 0; i < timesToChangeSpeed ; i++)
                spawner.NextPuyoSpeed();
           
        }
    }

    /// <summary>
    ///  Clear top lines of the game field
    /// </summary>
    /// <param name="linesToKeep"></param>
    public void ClearTopLines(int linesToKeep)
    {
        BoardDataList boardDatas = LoadGameField();
        BoardDataList newBoardDatas = new BoardDataList(new List<BoardData>());

        for (int i = 0; i < boardDatas.items.Count; i++)
        {
            if (boardDatas.items[i].position.y < linesToKeep)
            {
                newBoardDatas.items.Add(boardDatas.items[i]);
            }
        }
        string json = JsonUtility.ToJson(newBoardDatas);
        PlayerPrefs.SetString("GameBoard", json);
    }
}
