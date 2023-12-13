using UnityEngine;
using UnityEngine.UI;

public class UI_DayNightCycle : MonoBehaviour
{
    private Image _cycle;

    private void Start()
    {
        _cycle = GetComponent<Image>();
    }
    private void Update()
    {
        _cycle.fillAmount = GameManager.Instance.time;
    }
}
