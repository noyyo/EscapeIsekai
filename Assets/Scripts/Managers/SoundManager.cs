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

    private void Start()
    {
        BGMPlay(_ClipDics[0][_defaultBGMName]);
        OnSoundAllStopEvent += BGMStop;
        OnSoundAllStopEvent += OnSFXAllStopEvent;
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

    //효과음
    /// <summary>
    /// 효과음 출력(Button or Trigger) - 재생시간은 효과음의 재생시간으로 설정됩니다.
    /// </summary>
    /// <param name="clipType">효과음 종류를 골라주세요</param>
    /// <param name="sfxName">효과음의 이름</param>
    /// <param name="pos">효과음이 생기는 위치를 정해주세요 ex) 플레이어의 효과음이면 플레이어의 위치를 보내주세요</param>
    public bool CallPlaySFX(ClipType clipType, string sfxName, Vector3 pos)
    {
        if (_ClipDics[(int)clipType].TryGetValue(sfxName, out AudioClip value))
        {
            PlaySFX(value, pos, value.length);
            return true;
        }
        else
        {
            Debug.Log("Error 이 효과음과 같은 이름이 없습니다. 다시 확인해 주세요");
            return false;
        }
    }

    /// <summary>
    /// 효과음 출력 (Button or Trigger)
    /// </summary>
    /// <param name="clipType">효과음 종류를 골라주세요</param>
    /// <param name="sfxName">효과음의 이름</param>
    /// <param name="pos">효과음이 생기는 위치를 정해주세요 ex) 플레이어의 효과음이면 플레이어의 위치를 보내주세요</param>
    /// <param name="playTime">재생시간을 설정해 주세요</param>
    /// <returns></returns>
    public bool CallPlaySFX(ClipType clipType, string sfxName, Vector3 pos, float playTime)
    {
        if (_ClipDics[(int)clipType].TryGetValue(sfxName, out AudioClip value))
        {
            PlaySFX(value, pos, playTime);
            return true;
        }
        else
        {
            Debug.Log("Error 이 효과음과 같은 이름이 없습니다. 다시 확인해 주세요");
            return false;
        }
    }

    private void PlaySFX(AudioClip clip, Vector3 pos, float playTime)
    {
        SFX sfx = _objectPool_AudioSources.Get();
        sfx.PlaySFX(clip, pos, playTime);
        OnSFXAllStopEvent += sfx.DestroyAudioSource;
    }


    public bool CallPlayLoopSFX(ClipType clipType, string sfxName, Vector3 pos)
    {
        if (_ClipDics[(int)clipType].TryGetValue(sfxName, out AudioClip value))
        {
            PlayLoopSFX(value, pos);
            return true;
        }
        else
        {
            Debug.Log("Error 이 효과음과 같은 이름이 없습니다. 다시 확인해 주세요");
            return false;
        }
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


    private void PlayLoopSFX(AudioClip clip, Vector3 pos)
    {
        SFX sfx = _objectPool_AudioSources.Get();
        _playLoopSFXList.Add(sfx.PlayLoopSFX(clip, pos));
        OnSFXAllStopEvent += sfx.DestroyAudioSource;
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
