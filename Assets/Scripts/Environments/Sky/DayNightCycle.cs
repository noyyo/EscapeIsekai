using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    
    [SerializeField] private float _fulldayLength; //���ӻ� �Ϸ��� ���� �ð�(��)
    [SerializeField] private float _startTime = 0.4f;
    private float timeRate;

    [SerializeField] private Vector3 _noon;

    [Header("Sun")]
    public Light sun;
    public Gradient sunColor;
    public AnimationCurve sunIntensity;

    [Header("Moon")]
    public Light moon;
    public Gradient moonColor;
    public AnimationCurve moonIntensity;

    [Header("Other Lighting")]
    public AnimationCurve lightingIntensityMultiplier;
    public AnimationCurve reflectionintensityMultiplier;

    private void Start()
    {
        timeRate = 1.0f / _fulldayLength; //��ŭ�� ���ؾ� �ϴ���
        GameManager.Instance.time = _startTime;
    }

    private void Update()
    {
        GameManager.Instance.time = (GameManager.Instance.time + timeRate * Time.deltaTime) % 1.0f;  //�Ϸ��� �ۼ�����?

        UpdateLighting(sun, sunColor, sunIntensity);
        UpdateLighting(moon, moonColor, moonIntensity);

        RenderSettings.ambientIntensity = lightingIntensityMultiplier.Evaluate(GameManager.Instance.time); //ȯ�汤
        RenderSettings.reflectionIntensity = reflectionintensityMultiplier.Evaluate(GameManager.Instance.time); //�ݻ籤
    }

    void UpdateLighting(Light lightSource, Gradient colorGradiant, AnimationCurve intensityCurve)
    {
        float intensity = intensityCurve.Evaluate(GameManager.Instance.time);

        lightSource.transform.eulerAngles = (GameManager.Instance.time - (lightSource == sun ? 0.25f : -0.75f)) * _noon * 4.0f;
        lightSource.color = colorGradiant.Evaluate(GameManager.Instance.time);
        lightSource.intensity = intensity;

        GameObject go = lightSource.gameObject;
        if(lightSource.intensity == 0 && go.activeInHierarchy) 
            go.SetActive(false);
        else if( lightSource.intensity > 0 && !go.activeInHierarchy)
            go.SetActive(true);
    }
}