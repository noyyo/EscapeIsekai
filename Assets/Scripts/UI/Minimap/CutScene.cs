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
    private GameObject _cylinder; //빛기둥
    private PlayerInput _playerInput;
    private CylinderLighting timeCylinder; //빛기둥 Active 클래스
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
        if (_cylinder.activeSelf == false)  //빛기둥 활성화, 시간 조절
        {
            _cylinder.SetActive(true);
            timeCylinder.PlusTime(10f);  //활성화 시간 추가
        }
        else
        {
            timeCylinder.PlusTime(10f);
        }
        _pd.Play(_ta);  //타임라인 플레이
        _playerInput.SwitchCurrentActionMap("UI"); //playerInput 액션 맵 변경
        isPlaying = true;
        StartCoroutine(lightCoroutine);
    }
    private IEnumerator LightCylinder()
    {
        yield return new WaitForSecondsRealtime(5f);  //타임라인 재생 시간 동안 input 액션맵 변경 유지
        _playerInput.SwitchCurrentActionMap("Player");
    }
    void OnSkip()  //타임라인 스킵
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
