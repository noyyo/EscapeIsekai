using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class StartStory : MonoBehaviour
{
    public TMP_Text storyTxt;
    public GameObject nextButton;
    public GameObject endButton;

    public string[] dialogues;
    public int talkNum;
    
    public StringBuilder st =new StringBuilder();
    public WaitForSeconds next = new WaitForSeconds(0.05f);
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
        storyTxt.text = null;
        talkNum++;

        if(talkNum == dialogues.Length)
        {
            nextButton.SetActive(false);
            endButton.SetActive(true);
            EndTalk();
            return;
        }
        StartCoroutine(Typing(dialogues[talkNum]));
    }

    public void EndTalk()
    {
        talkNum = 0;
        Debug.Log("end");
    }

    IEnumerator Typing(string talk)
    {
        storyTxt.text = null;
        st.Clear();

        for (int i = 0; i < talk.Length; i++)
        {
            st.Append(talk[i]);
            storyTxt.text = st.ToString();
            yield return next;
        }
    }
}
