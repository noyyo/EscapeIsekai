using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SFX : MonoBehaviour
{
    private AudioSource _audioSource;
    private Transform _thisTransform;
    private IObjectPool<SFX> _managedPool;
    private SoundManager _soundManager;

    public string SFXName { get; private set; }

    private void Awake()
    {
        _soundManager = SoundManager.Instance;
        _thisTransform = this.transform;
        _audioSource = GetComponent<AudioSource>();
    }

    public void SetManagedPool(IObjectPool<SFX> pool)
    {
        _managedPool = pool;
    }

    public void PlaySFX(AudioClip audioClip, Transform transform, float playTime, bool isLoop)
    {
        _thisTransform.position = transform.position;
        _thisTransform.parent = transform;
        _audioSource.clip = audioClip;
        SFXName = audioClip.name;
        _audioSource.Play();
        if(isLoop)
        {
            _audioSource.loop = true;
        }
        else
        {
            Invoke("DestroyAudioSource", playTime);
            _audioSource.loop = false;
        }
        
    }

    public void DestroyAudioSource()
    {
        _soundManager.OnSFXAllStopEvent -= DestroyAudioSource;
        StopSFX();
        _managedPool.Release(this);
    }

    private void StopSFX()
    {
        _audioSource.Stop();
    }
}
