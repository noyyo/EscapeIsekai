using System.Collections;
using UnityEngine;

public class ThunderCustom : MonoBehaviour
{
    public ParticleSystem ParentParticle;
    private ParticleSystem.MainModule parentMain;
    private ParticleSystem self;
    ParticleSystem.MainModule selfMain;
    private ParticleSystem.ShapeModule shape;
    private bool isStarted;
    private float playTime;
    private void Awake()
    {
        if (ParentParticle == null)
        {
            Debug.LogError("메인 파티클을 설정하십시오.");
            return;
        }
        parentMain = ParentParticle.main;
        self = GetComponent<ParticleSystem>();
        shape = self.shape;
        selfMain = self.main;
        selfMain.startSpeed = parentMain.startSpeed;
    }

    private void Update()
    {
        if (ParentParticle == null)
            return;

        if (ParentParticle.isPlaying && !isStarted)
        {
            isStarted = true;
            StartCoroutine(SetParticleShapeLength());
        }
        else if (!ParentParticle.isPlaying && isStarted)
        {
            isStarted = false;
            StopAllCoroutines();
            playTime = 0f;
            shape.length = 0f;
        }
    }
    private IEnumerator SetParticleShapeLength()
    {
        while (shape.length < parentMain.startLifetime.constant * parentMain.startSpeed.constant)
        {
            playTime += Time.deltaTime;
            shape.length = Mathf.Lerp(0, parentMain.startLifetime.constant * parentMain.startSpeed.constant, playTime / parentMain.startLifetime.constant);
            yield return null;
        }
    }
}
