using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartStory : MonoBehaviour
{
    public TMP_Text storyTxt;
    public GameObject nextButton;
    public GameObject endButton;
    public Image black;
    public GameObject white;
    public AudioSource effect;

    public string[] dialogues;
    private int talkNum;
    private bool isEnd = false;
    
    private StringBuilder sb =new StringBuilder();
    void Start()
    {
        StartTalk(dialogues);
    }
    public void StartTalk(string[] talks)
    {
        dialogues = talks;
        StartCoroutine(Typing(dialogues[talkNum]));
    }

    public void NextTalk()
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
            if(talkNum == dialogues.Length)
            {
                nextButton.SetActive(false);
                endButton.SetActive(true);
                EndTalk();
                return;
            }
            StartCoroutine(Typing(dialogues[talkNum]));
        }
    }

    public void EndTalk()
    {
        talkNum = 0;
    }

    IEnumerator Typing(string talk)
    {
        storyTxt.text = null;
        sb.Clear();

        for (int i = 0; i < talk.Length; i++)
        {
            sb.Append(talk[i]);
            storyTxt.text = sb.ToString();
            yield return null;
        }
        isEnd = true;
    }
}
