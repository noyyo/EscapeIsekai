using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class QuestUI : MonoBehaviour
{
    [SerializeField]
    RectTransform content;
    [SerializeField]ScrollRect scrollRect;
    [SerializeField] GameObject panel;


    private void Awake()
    {
        scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
    }
    
    private void OnScrollValueChanged(Vector2 value)
    {
        float offsetY = content.anchoredPosition.y;
        if (offsetY < 0)
        {
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, 0);
        }
        if (offsetY > 500)
        {
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, 500);
        }
    }
}
