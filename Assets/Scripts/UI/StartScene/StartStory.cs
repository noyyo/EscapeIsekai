using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartStory : Story
{
    public GameObject nextButton;
    public GameObject endButton;
    public Image black;
    public GameObject white;
    public AudioSource effect;

    
    private StringBuilder sb =new StringBuilder();
    void Start()
    {
        StartTalk(dialogues);
    }

    public override void NextTalk()
    {
        if(isEnd)
        {
            isEnd = false;
            storyTxt.text = null;
            talkNum++;

            if(talkNum == 9)
            {
                black.canvasRenderer.SetAlpha(0f);
            }
            if(talkNum == 11)
            {
                white.SetActive(true);
                effect.Play();
            }
            if(talkNum == dialogues.Length-1)
            {
                nextButton.SetActive(false);
                endButton.SetActive(true);
                EndTalk();
                return;
            }
            StartCoroutine(Typing(dialogues[talkNum]));
        }
    }


}
