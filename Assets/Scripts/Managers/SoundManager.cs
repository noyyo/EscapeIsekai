using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

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
        if(bgm == null )
        {
            bgm = this.AddComponent<AudioSource>();
            mixer = Resources.Load<AudioMixer>("Sound/AudioVolumeController");
            bgm.outputAudioMixerGroup = mixer.FindMatchingGroups("BGM")[0];
        }
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
        SFX sfx = Instantiate(sfxPrefab, this.transform).GetComponent<SFX>();
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

    public bool CallPlaySFX(ClipType clipType, string sfxName, Transform transform, bool isLoop, float pitchValue = 1, float soundValue = 1)
    {
        if (ClipDics[(int)clipType].TryGetValue(sfxName, out AudioClip value))
        {
            PlaySFX(value, transform, value.length, isLoop, transform.position);
            return true;
        }
        else
        {
            Debug.Log("Error 이 효과음과 같은 이름이 없습니다. 다시 확인해 주세요");
            return false;
        }
    }

    public bool CallPlaySFX(ClipType clipType, string sfxName, Vector3 vector3, bool isLoop, float pitchValue = 1, float soundValue = 1)
    {   
        if (ClipDics[(int)clipType].TryGetValue(sfxName, out AudioClip value))
        {
            PlaySFX(value, transform, value.length, isLoop, vector3);
            return true;
        }
        else
        {
            Debug.Log("Error 이 효과음과 같은 이름이 없습니다. 다시 확인해 주세요");
            return false;
        }
    }

    public bool CallPlaySFX(ClipType clipType, string sfxName, Transform transform, float playTime, float pitchValue = 1, float soundValue = 1)
    {
        if (ClipDics[(int)clipType].TryGetValue(sfxName, out AudioClip value))
        {
            PlaySFX(value, transform, playTime, false, transform.position);
            return true;
        }
        else
        {
            Debug.Log("Error 이 효과음과 같은 이름이 없습니다. 다시 확인해 주세요");
            return false;
        }
    }

    public bool CallPlaySFX(ClipType clipType, string sfxName, Vector3 vector3, float playTime, float pitchValue = 1, float soundValue = 1)
    {
        if (ClipDics[(int)clipType].TryGetValue(sfxName, out AudioClip value))
        {
            PlaySFX(value, transform, playTime, false, vector3);
            return true;
        }
        else
        {
            Debug.Log("Error 이 효과음과 같은 이름이 없습니다. 다시 확인해 주세요");
            return false;
        }
    }

    private void PlaySFX(AudioClip clip, Transform transform, float playTime, bool isLoop, Vector3 vector3, float pitchValue = 1, float soundValue = 1)
    {
        SFX sfx = objectPool_AudioSources.Get();
        sfx.PlaySFX(clip, transform, playTime, isLoop, vector3);
        if (isLoop)
            playLoopSFXList.Add(sfx);
        OnSFXAllStopEvent += sfx.DestroyAudioSource;
    }

    public bool CallStopLoopSFX(ClipType clipType, string sfxName)
    {
        int _playLoopSFXListCount = playLoopSFXList.Count;
        for (int i = _playLoopSFXListCount - 1; i >= 0; i--)
        {
            if (playLoopSFXList[i].SFXName == sfxName)
            {
                playLoopSFXList[i].DestroyAudioSource();
                playLoopSFXList.RemoveAt(i);
                return true;
            }
        }
        return false;
    }
    //------------------------------------------------
    public AudioSource CallPlaySFXReturnSource(ClipType clipType, string sfxName, Transform transform, bool isLoop, float pitchValue = 1, float soundValue = 1)
    {
        if (ClipDics[(int)clipType].TryGetValue(sfxName, out AudioClip value))
        {
            return PlaySFXReturnSource(value, transform, value.length, isLoop, transform.position, pitchValue);
        }
        else
        {
            Debug.Log("Error 이 효과음과 같은 이름이 없습니다. 다시 확인해 주세요");
            return null;
        }
    }

    public AudioSource CallPlaySFXReturnSource(ClipType clipType, string sfxName, Vector3 vector3, bool isLoop, float pitchValue = 1, float soundValue = 1)
    {
        if (ClipDics[(int)clipType].TryGetValue(sfxName, out AudioClip value))
        {
            return PlaySFXReturnSource(value, transform, value.length, isLoop, vector3, pitchValue);
        }
        else
        {
            Debug.Log("Error 이 효과음과 같은 이름이 없습니다. 다시 확인해 주세요");
            return null;
        }
    }

    public AudioSource CallPlaySFXReturnSource(ClipType clipType, string sfxName, Transform transform, float playTime, float pitchValue = 1, float soundValue = 1)
    {
        if (ClipDics[(int)clipType].TryGetValue(sfxName, out AudioClip value))
        {
            return PlaySFXReturnSource(value, transform, playTime, false, transform.position, pitchValue);
        }
        else
        {
            Debug.Log("Error 이 효과음과 같은 이름이 없습니다. 다시 확인해 주세요");
            return null;
        }
    }

    public AudioSource CallPlaySFXReturnSource(ClipType clipType, string sfxName, Vector3 vector3, float playTime, float pitchValue = 1, float soundValue = 1)
    {
        if (ClipDics[(int)clipType].TryGetValue(sfxName, out AudioClip value))
        {
            return PlaySFXReturnSource(value, transform, playTime, false, vector3, pitchValue);
        }
        else
        {
            Debug.Log("Error 이 효과음과 같은 이름이 없습니다. 다시 확인해 주세요");
            return null;
        }
    }

    private AudioSource PlaySFXReturnSource(AudioClip clip, Transform transform, float playTime, bool isLoop, Vector3 vector3, float pitchValue = 1, float soundValue = 1)
    {
        SFX sfx = objectPool_AudioSources.Get();
        sfx.PlaySFX(clip, transform, playTime, isLoop, vector3);
        if (isLoop)
            playLoopSFXList.Add(sfx);
        OnSFXAllStopEvent += sfx.DestroyAudioSource;
        sfx.audioSource.pitch = pitchValue;
        return sfx.audioSource;
    }

    //-------

    //사운드 조절
    public void MasterVolume(float val)
    {
        mixer.SetFloat("MasterVolume", val * 0.4f - 30);
    }

    public void BGMVolume(float val)
    {
        mixer.SetFloat("BGMVolume", val * 0.4f - 40);
    }

    public void SFXVolume(float val)
    {
        mixer.SetFloat("SFXVolume", val * 0.4f - 40);
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
