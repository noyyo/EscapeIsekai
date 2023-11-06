using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.InputSystem;

public class CutScene : MonoBehaviour
{
    private PlayableDirector _pd;
    [SerializeField] private TimelineAsset _ta;
    [SerializeField] private GameObject _cylinder; //�����
    private PlayerInput _playerInput;
    private CylinderLighting timeCylinder; //����� Active Ŭ����

    IEnumerator lightCoroutine;
    void Start()
    {
        _pd = GetComponent<PlayableDirector>();
        _playerInput = GetComponent<PlayerInput>();
        lightCoroutine = LightCylinder();
        timeCylinder = _cylinder.GetComponent<CylinderLighting>();
    }

    void OnNavigate()
    {
        if (_cylinder.activeSelf == false)  //����� Ȱ��ȭ, �ð� ����
        {
            _cylinder.SetActive(true);
            timeCylinder.PlusTime(10f);  //Ȱ��ȭ �ð� �߰�
        }
        else
        {
            timeCylinder.PlusTime(10f);
        }
        _pd.Play(_ta);  //Ÿ�Ӷ��� �÷���
        _playerInput.SwitchCurrentActionMap("UI"); //playerInput �׼� �� ����
        StartCoroutine(lightCoroutine);
    }
    private IEnumerator LightCylinder()
    {
        yield return new WaitForSecondsRealtime(5f);  //Ÿ�Ӷ��� ��� �ð� ���� input �׼Ǹ� ���� ����
        _playerInput.SwitchCurrentActionMap("Player");
    }
    void OnSkip()  //Ÿ�Ӷ��� ��ŵ
    {
        if (_pd.time > 0)
        {
            _playerInput.SwitchCurrentActionMap("Player");
            _pd.time = _pd.duration;
        }
    }
}
