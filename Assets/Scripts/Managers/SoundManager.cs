using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : CustomSingleton<SoundManager>
{
    protected SoundManager() { }

    [SerializeField] private AudioSource _bgm;
    [SerializeField] private AudioClip[] _bgmList;
    [SerializeField] private AudioMixer mixer;
    private int _bgmListLength;

    private void Awake()
    {
        if (_bgm == null)
            _bgm = this.GetComponent<AudioSource>();
        //BGMPlay
    }

    private void Start()
    {
        //_bgmListLength = _bgmList.Length;
        //SceneManager.sceneLoaded += OnSceneLoaded;
        BGMPlay(_bgmList[0]);
    }

    //private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    //{
    //    for(int i = 0; i < _bgmListLength; i++)
    //    {
    //        if (arg0.name == _bgmList[i].name)
    //        {
    //            BGMPlay(_bgmList[i]);
    //        }
    //    }
    //}
    
    //사운드 조절
    public void BGMVolume(float val)
    {
        mixer.SetFloat("BGMVolume", Mathf.Log10(val)*20);
    }

    public void SFXPlay(string sfxName, AudioClip clip)
    {
        GameObject go = new GameObject(sfxName+"Sound");
        AudioSource audioSource = go.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
        audioSource.clip = clip;
        audioSource.Play();

        Destroy(go, clip.length);
    }

    public void BGMPlay(AudioClip clip)
    {
        _bgm.outputAudioMixerGroup = mixer.FindMatchingGroups("BGM")[0];
        _bgm.clip = clip;
        _bgm.loop = true;
        _bgm.volume = 0.1f;
        _bgm.Play();
    }
}
