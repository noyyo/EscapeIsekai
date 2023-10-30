using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_DayNightCycle : MonoBehaviour
{
    private Image _cycle;
    //private DayNightCycle _dayNightCycle;

    private void Start()
    {
        _cycle = GetComponent<Image>();
    }
    private void Update()
    {
        _cycle.fillAmount = GameManager.Instance.time;
    }
}
