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

    [SerializeField] private AudioMixer mixer;
    [SerializeField] private int poolMaxCount = 20;
    [SerializeField] private string defaultBGMName = "미지의 섬";

    private Dictionary<string, AudioClip>[] ClipDics;

    private GameObject sfxPrefab;
    private AudioSource bgm;
    private IObjectPool<SFX> objectPool_AudioSources;
    private List<SFX> playLoopSFXList;

    public event Action OnSoundAllStopEvent;
    public event Action OnSFXAllStopEvent;

    private void Awake()
    {
        bgm = this.GetComponent<AudioSource>();
        CreateSoundList();
        sfxPrefab = Resources.Load<GameObject>("Prefabs/Sound/SFX");
        objectPool_AudioSources = new ObjectPool<SFX>(CreateSFX, OnGetSFX, OnReleasSFX, OnDestroySFX, maxSize: poolMaxCount);
        playLoopSFXList = new List<SFX>();
    }
    private void Start()
    {
        BGMPlay(ClipDics[0][defaultBGMName]);
        OnSoundAllStopEvent += BGMStop;
        OnSoundAllStopEvent += SFXAllStop;
    }
    private void CreateSoundList()
    {
        int i = 0;
        ClipDics = new Dictionary<string, AudioClip>[6];
        foreach (ClipType type in Enum.GetValues(typeof(ClipType)))
        {
            ClipDics[i] = new Dictionary<string, AudioClip>();
            AudioClip[] clips = Resources.LoadAll<AudioClip>("Sound/" + type);

            foreach (AudioClip clip in clips)
                ClipDics[i].Add(clip.name, clip);

            i++;
        }
    }

    //Objectpool
    private SFX CreateSFX()
    {
        SFX sfx = Instantiate(sfxPrefab,this.transform).GetComponent<SFX>();
        sfx.SetManagedPool(objectPool_AudioSources);
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
        if (ClipDics[(int)clipType].TryGetValue(sfxName, out AudioClip value))
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
        if (ClipDics[(int)clipType].TryGetValue(sfxName, out AudioClip value))
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
        SFX sfx = objectPool_AudioSources.Get();
        sfx.PlaySFX(clip, transform, playTime, isLoop);
        if (isLoop)
            playLoopSFXList.Add(sfx);
        OnSFXAllStopEvent += sfx.DestroyAudioSource;
    }
    public bool CallStopLoopSFX(ClipType clipType, string sfxName)
    {
        int _playLoopSFXListCount = playLoopSFXList.Count;
        for (int i = _playLoopSFXListCount - 1;  i >= 0; i--)
        {
            if(playLoopSFXList[i].SFXName == sfxName)
            {
                playLoopSFXList[i].DestroyAudioSource();
                playLoopSFXList.RemoveAt(i);
                return true;
            }
        }
        return false;
    }
    //-------

    //사운드 조절
    public void MasterVolume(float val)
    {
        mixer.SetFloat("MasterVolume", Mathf.Log10(val) * 20);
    }

    public void BGMVolume(float val)
    {
        mixer.SetFloat("BGMVolume", Mathf.Log10(val) * 20);
    }

    public void SFXVolume(float val)
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(val) * 20);
    }
    //------

    //배경음 변경
    public void ChangeBGM(string bgmName)
    {
        if (ClipDics[0].TryGetValue(bgmName, out AudioClip value))
            BGMPlay(value);
        else
            BGMPlay(ClipDics[0][defaultBGMName]);
    }

    private void BGMPlay(AudioClip clip)
    {
        bgm.clip = clip;
        bgm.loop = true;
        bgm.volume = 0.1f;
        bgm.Play();
    }

    public void BGMStop()
    {
        bgm.Stop();
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
