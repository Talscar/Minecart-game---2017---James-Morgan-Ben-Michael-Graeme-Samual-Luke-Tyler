﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {

    public int currentScore;

    public GameObject scoretextDisplay;
    private string scoretext;
    private Text text;

    private Rigidbody rb;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();

        text = scoretextDisplay.GetComponent<Text>();
        scoretext = "Score: " + currentScore;
        text.text = scoretext;

    }
	
	// Update is called once per frame
	void Update () {
        text = scoretextDisplay.GetComponent<Text>();
        scoretext = "Score: " + currentScore;
        text.text = scoretext;

    }

    public void AddScore(int addscore,float massToAdd)
    {
        currentScore = currentScore + addscore;
        rb.drag = rb.drag + massToAdd;
        AudioSource audio = GetComponent<AudioSource>();
        audio.Play();
        //audio.PlayDelayed(44100);
    }
}
