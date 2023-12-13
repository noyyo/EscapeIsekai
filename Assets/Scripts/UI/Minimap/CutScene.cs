using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CutScene : MonoBehaviour
{
    private PlayableDirector _pd;
    private TimelineAsset _ta;
    private GameObject _cylinder;
    private PlayerInput _playerInput;
    private CylinderLighting timeCylinder; //빛기둥 Active 클래스
    private UI_Manager uiManager;

    private static readonly string assetPath = "Timeline/PlayerTimeline";

    IEnumerator lightCoroutine;
    void Start()
    {
        uiManager = UI_Manager.Instance;
        _pd = GetComponent<PlayableDirector>();
        _playerInput = GetComponent<PlayerInput>();
        lightCoroutine = LightCylinder();
        if (_cylinder == null)
        {
            _cylinder = Instantiate(Resources.Load("Prefabs/Entities/Environments/LightCylinder", typeof(GameObject))) as GameObject;
            _cylinder.SetActive(false);
        }
        timeCylinder = _cylinder.GetComponent<CylinderLighting>();
        _ta = (TimelineAsset)Resources.Load(assetPath, typeof(TimelineAsset));
    }

    void OnNavigate()
    {
        if (_cylinder.activeSelf == false)  
        {
            _cylinder.SetActive(true);
            timeCylinder.PlusTime(10f);
        }
        else
        {
            timeCylinder.PlusTime(10f);
        }
        _pd.Play(_ta);
        _playerInput.SwitchCurrentActionMap("UI");
        uiManager.isPlaying = true;
        StartCoroutine(lightCoroutine);
    }
    private IEnumerator LightCylinder()
    {
        yield return new WaitForSecondsRealtime(5f);  //타임라인 재생 시간 동안 input 액션맵 변경 유지
        _playerInput.SwitchCurrentActionMap("Player");
    }
    void OnSkip()
    {
        if (!uiManager.isPlaying)
        {
            return;
        }
        if (_pd.time > 0)
        {
            uiManager.isPlaying = false;
            _playerInput.SwitchCurrentActionMap("Player");
            _pd.time = _pd.duration;
        }
    }
}
