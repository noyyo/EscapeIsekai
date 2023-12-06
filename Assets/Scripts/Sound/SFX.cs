using UnityEngine;
using UnityEngine.Pool;

public class SFX : MonoBehaviour
{
    public AudioSource audioSource;
    private Transform thisTransform;
    private IObjectPool<SFX> managedPool;
    private SoundManager soundManager;

    public string SFXName { get; private set; }

    private void Awake()
    {
        soundManager = SoundManager.Instance;
        thisTransform = this.transform;
        audioSource = GetComponent<AudioSource>();
    }

    public void SetManagedPool(IObjectPool<SFX> pool)
    {
        managedPool = pool;
    }

    public void PlaySFX(AudioClip audioClip, Transform transform, float playTime, bool isLoop, Vector3 vector3)
    {
        thisTransform.position = vector3;
        thisTransform.parent = transform;
        audioSource.clip = audioClip;
        SFXName = audioClip.name;
        audioSource.Play();
        if (isLoop)
        {
            audioSource.loop = true;
        }
        else
        {
            Invoke("DestroyAudioSource", playTime);
            audioSource.loop = false;
        }
    }

    public void DestroyAudioSource()
    {
        soundManager.OnSFXAllStopEvent -= DestroyAudioSource;
        StopSFX();
        managedPool.Release(this);
    }

    private void StopSFX()
    {
        audioSource.Stop();
    }
}
