using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FadeEffect : MonoBehaviour
{
    public TMP_Text[] credits;
    private int txtNum = 0;

    void Start()
    {
        StartCoroutine(FadeTextToFullAlpha(credits[txtNum]));
    }

    public IEnumerator FadeTextToFullAlpha(TMP_Text text) // ���İ� 0���� 1�� ��ȯ
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        while (text.color.a < 1.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + (Time.deltaTime / 2.0f));
            yield return null;
        }
        StartCoroutine(FadeTextToZero(text));
    }

    public IEnumerator FadeTextToZero(TMP_Text text)  // ���İ� 1���� 0���� ��ȯ
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        while (text.color.a > 0.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime / 2.0f));
            yield return null;
        }
        ++txtNum;
        if(txtNum < credits.Length)
        {
            StartCoroutine(FadeTextToFullAlpha(credits[txtNum]));
        }
    }

    private void Update()
    {
        if (Input.anyKey) QuitGame();
    }

    private void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}