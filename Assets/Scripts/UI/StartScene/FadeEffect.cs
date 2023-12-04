using System.Collections;
using TMPro;
using UnityEngine;

public class FadeEffect : MonoBehaviour
{
    public TMP_Text[] credits;
    private int txtNum = 0;

    void Start()
    {
        StartCoroutine(FadeTextToFullAlpha(credits[txtNum]));
    }

    public IEnumerator FadeTextToFullAlpha(TMP_Text text) // 알파값 0에서 1로 전환
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        while (text.color.a < 1.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + (Time.deltaTime / 2.0f));
            yield return null;
        }
        StartCoroutine(FadeTextToZero(text));
    }

    public IEnumerator FadeTextToZero(TMP_Text text)  // 알파값 1에서 0으로 전환
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        while (text.color.a > 0.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime / 2.0f));
            yield return null;
        }
        ++txtNum;
        if (txtNum < credits.Length)
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