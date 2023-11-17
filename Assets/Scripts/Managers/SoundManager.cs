using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;
using System;

public enum ClipType
{
    BGM,
    PlayerSFX,
    EnemySFX,
    NPCSFX,
    EnvironmentSFX,
    UISFX
}


public class SoundManager : CustomSingleton<SoundManager>
{
    protected SoundManager() { }

    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private int _poolMaxCount = 20;
    [SerializeField] private string _defaultBGMName = "미지의 섬";
    [SerializeField] private bool _test;
    [SerializeField] private bool _test1;
    [SerializeField] private bool _test2;
    [SerializeField] private bool _test3;
    [SerializeField] private bool _test4;
    [SerializeField] private bool _test5;

    private Dictionary<string, AudioClip>[] _ClipDics;

    private GameObject _sfxPrefab;
    private AudioSource _bgm;
    private IObjectPool<SFX> _objectPool_AudioSources;
    private List<SFX> _playLoopSFXList;

    public event Action OnSoundAllStopEvent;
    public event Action OnSFXAllStopEvent;

    private void Awake()
    {
        _bgm = this.GetComponent<AudioSource>();
        CreateSoundList();
        _sfxPrefab = Resources.Load<GameObject>("Prefabs/Sound/SFX");
        _objectPool_AudioSources = new ObjectPool<SFX>(CreateSFX, OnGetSFX, OnReleasSFX, OnDestroySFX, maxSize: _poolMaxCount);
        _playLoopSFXList = new List<SFX>();
    }
    private void Start()
    {
        BGMPlay(_ClipDics[0][_defaultBGMName]);
        OnSoundAllStopEvent += BGMStop;
        OnSoundAllStopEvent += SFXAllStop;
    }

    private void Update()
    {
        if(_test)
        {
            CallPlaySFX(ClipType.PlayerSFX, "Run_2", GameManager.Instance.Player.transform, false);
            _test = !_test;
        }
        if (_test1)
        {
            CallPlaySFX(ClipType.PlayerSFX, "Run_2", GameManager.Instance.Player.transform, true);
            _test1 = !_test1;
        }
        if (_test2)
        {
            CallPlaySFX(ClipType.PlayerSFX, "Walk", this.transform, true);
            _test2 = !_test2;
        }
        if (_test3)
        {
            CallStopLoopSFX(ClipType.PlayerSFX, "Run_2");
            _test3 = !_test3;
        }
        if (_test4)
        {
            SFXAllStop();
            _test4 = !_test4;
        }
        if (_test5)
        {
            SoundAllStop();
            _test5 = !_test5;
        }
    }

    private void CreateSoundList()
    {
        int i = 0;
        _ClipDics = new Dictionary<string, AudioClip>[6];
        foreach (ClipType type in Enum.GetValues(typeof(ClipType)))
        {
            _ClipDics[i] = new Dictionary<string, AudioClip>();
            AudioClip[] clips = Resources.LoadAll<AudioClip>("Sound/" + type);

            foreach (AudioClip clip in clips)
                _ClipDics[i].Add(clip.name, clip);

            i++;
        }
    }

    //Objectpool
    private SFX CreateSFX()
    {
        SFX sfx = Instantiate(_sfxPrefab,this.transform).GetComponent<SFX>();
        sfx.SetManagedPool(_objectPool_AudioSources);
        return sfx;
    }

    private void OnGetSFX(SFX sfx)
    {
        sfx.gameObject.SetActive(true);
    }
    private void OnReleasSFX(SFX sfx)
    {
        sfx.gameObject.SetActive(false);
    }

    private void OnDestroySFX(SFX sfx)
    {
        Destroy(sfx.gameObject);
    }
    //---------

    public bool CallPlaySFX(ClipType clipType, string sfxName, Transform transform, bool isLoop)
    {
        if (_ClipDics[(int)clipType].TryGetValue(sfxName, out AudioClip value))
        {
            PlaySFX(value, transform, value.length, isLoop);
            return true;
        }
        else
        {
            Debug.Log("Error 이 효과음과 같은 이름이 없습니다. 다시 확인해 주세요");
            return false;
        }
    }

    public bool CallPlaySFX(ClipType clipType, string sfxName, Transform transform, float playTime)
    {
        if (_ClipDics[(int)clipType].TryGetValue(sfxName, out AudioClip value))
        {
            PlaySFX(value, transform, playTime, false);
            return true;
        }
        else
        {
            Debug.Log("Error 이 효과음과 같은 이름이 없습니다. 다시 확인해 주세요");
            return false;
        }
    }

    private void PlaySFX(AudioClip clip, Transform transform, float playTime, bool isLoop)
    {
        SFX sfx = _objectPool_AudioSources.Get();
        sfx.PlaySFX(clip, transform, playTime, isLoop);
        if (isLoop)
            _playLoopSFXList.Add(sfx);
        OnSFXAllStopEvent += sfx.DestroyAudioSource;
    }
    public bool CallStopLoopSFX(ClipType clipType, string sfxName)
    {
        int _playLoopSFXListCount = _playLoopSFXList.Count;
        for (int i = _playLoopSFXListCount - 1;  i >= 0; i--)
        {
            if(_playLoopSFXList[i].SFXName == sfxName)
            {
                _playLoopSFXList[i].DestroyAudioSource();
                _playLoopSFXList.RemoveAt(i);
                return true;
            }
        }
        return false;
    }
    //-------

    //사운드 조절
    public void MasterVolume(float val)
    {
        _mixer.SetFloat("MasterVolume", Mathf.Log10(val) * 20);
    }

    public void BGMVolume(float val)
    {
        _mixer.SetFloat("BGMVolume", Mathf.Log10(val) * 20);
    }

    public void SFXVolume(float val)
    {
        _mixer.SetFloat("SFXVolume", Mathf.Log10(val) * 20);
    }
    //------

    //배경음 변경
    public void ChangeBGM(string bgmName)
    {
        if (_ClipDics[0].TryGetValue(bgmName, out AudioClip value))
            BGMPlay(value);
        else
            BGMPlay(_ClipDics[0][_defaultBGMName]);
    }

    private void BGMPlay(AudioClip clip)
    {
        _bgm.clip = clip;
        _bgm.loop = true;
        _bgm.volume = 0.1f;
        _bgm.Play();
    }

    public void BGMStop()
    {
        _bgm.Stop();
    }
    //-------

    public void SoundAllStop()
    {
        OnSoundAllStopEvent?.Invoke();
    }

    public void SFXAllStop() 
    {
        OnSFXAllStopEvent?.Invoke();
    }
}
