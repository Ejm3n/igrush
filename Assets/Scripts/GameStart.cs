 using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private PuyoSpawner puyoSpawner;

    //void Awake(){
    //    canvas = GameObject.Find("GameStartCanvas").GetComponent<Canvas>();
    //}

    public void StartGame(){      
        StatSaver.instance.LoadScene();
        //GameObject.Find("PuyoSpawner").GetComponent<PuyoSpawner>().enabled = true;
        puyoSpawner.enabled = true;
        GameUIController.instance.SetStartCanvas(false);
        GameUIController.instance.SetEndCanvas(false);
        SoundManager.Instance.StartGameMusic();       
    }
}
