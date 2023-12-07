using UnityEngine;
using UnityEngine.UI;

public class EndStory : Story
{
    public GameObject e_nextButton;
    public Image white;
    public AudioSource beep;
    public GameObject credit;
    private Animator animator;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
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
                beep.Play();
                animator.SetTrigger("Trigger");
            }
            if (talkNum == dialogues.Length)
            {
                EndTalk();
                credit.SetActive(true);
                return;
            }
            StartCoroutine(Typing(dialogues[talkNum]));
        }
    }

}
