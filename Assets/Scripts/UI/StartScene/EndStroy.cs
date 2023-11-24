using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndStroy : Story
{
    public GameObject e_nextButton;
    public Image white;
    private Animator animator;

    void Start()
    {
        animator = white.GetComponent<Animator>();
        StartTalk(dialogues);
    }

    public override void NextTalk()
    {
        if (isEnd)
        {
            isEnd = false;
            storyTxt.text = null;
            talkNum++;

            if (talkNum == 2)
            {
                //white.canvasRenderer.SetAlpha(0f);
                //animator.Play();
                animator.SetTrigger("Trigger");
            }
            if (talkNum == dialogues.Length)
            {
                //nextButton.SetActive(false);
                EndTalk();
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
                    Application.Quit();
                #endif
                return;
            }
            StartCoroutine(Typing(dialogues[talkNum]));
        }
    }

}
