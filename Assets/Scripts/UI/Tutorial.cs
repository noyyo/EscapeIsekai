using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    RectTransform content;
    private void Awake()
    {
        content = gameObject.GetComponent<ScrollRect>().content;
        ScrollRect scrollRect = gameObject.GetComponent<ScrollRect>();
        scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
    }
    private void OnScrollValueChanged(Vector2 value)
    {
        float offsetY = content.anchoredPosition.y;
        if(offsetY < 0)
        {
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, 0);
        }
        if(offsetY > 500)
        {
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, 500);
        }
    }

    public void OnClik()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        gameObject.SetActive(false);

    }

}
