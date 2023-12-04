using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.InputSystem;
using UnityEditor;

public class CutScene : MonoBehaviour
{
    private PlayableDirector _pd;
    private TimelineAsset _ta;
    private GameObject _cylinder; //�����
    private PlayerInput _playerInput;
    private CylinderLighting timeCylinder; //����� Active Ŭ����
    private bool isPlaying = false;

    private static readonly string assetPath = "Timeline/PlayerTimeline";

    IEnumerator lightCoroutine;
    void Start()
    {
        _pd = GetComponent<PlayableDirector>();
        _playerInput = GetComponent<PlayerInput>();
        lightCoroutine = LightCylinder();
        if(_cylinder == null)
        {
            _cylinder = Instantiate(Resources.Load("Prefabs/Entities/Environments/LightCylinder", typeof(GameObject))) as GameObject;
            _cylinder.SetActive(false);
        }
        timeCylinder = _cylinder.GetComponent<CylinderLighting>();
        _ta = (TimelineAsset)Resources.Load(assetPath, typeof(TimelineAsset));
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
        isPlaying = true;
        StartCoroutine(lightCoroutine);
    }
    private IEnumerator LightCylinder()
    {
        yield return new WaitForSecondsRealtime(5f);  //Ÿ�Ӷ��� ��� �ð� ���� input �׼Ǹ� ���� ����
        _playerInput.SwitchCurrentActionMap("Player");
    }
    void OnSkip()  //Ÿ�Ӷ��� ��ŵ
    {
        if(!isPlaying)
        {
            return;
        }
        if (_pd.time > 0)
        {
            isPlaying = false;
            _playerInput.SwitchCurrentActionMap("Player");
            _pd.time = _pd.duration;
        }
    }
}