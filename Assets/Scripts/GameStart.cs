﻿ using System.Collections;
using System.Collections.Generic;
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
        //GameObject.Find("PuyoSpawner").GetComponent<PuyoSpawner>().enabled = true;
        puyoSpawner.enabled = true;
        canvas.gameObject.SetActive(false);
    }
}