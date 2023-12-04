using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

public class Story : MonoBehaviour
{
    public TMP_Text storyTxt;

    public string[] dialogues;
    [HideInInspector] public int talkNum;
    [HideInInspector] public bool isEnd = false;

    private StringBuilder sb = new StringBuilder();

    public void StartTalk(string[] talks)
    {
        dialogues = talks;
        StartCoroutine(Typing(dialogues[talkNum]));
    }

    public virtual void NextTalk()
    {

    }

    public void EndTalk()
    {
        talkNum = 0;
    }

    public IEnumerator Typing(string talk)
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
