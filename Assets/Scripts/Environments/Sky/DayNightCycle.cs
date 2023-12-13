using UnityEngine;

public class DayNightCycle : MonoBehaviour
{

    [SerializeField] private float _fulldayLength;
    [SerializeField] private float _startTime = 0.4f;
    private float timeRate;

    [SerializeField] private Vector3 _noon;

    [Header("Sun")]
    public Light sun;
    public Gradient sunColor;
    public AnimationCurve sunIntensity;
    public Material sunBox;
    [Header("Moon")]
    public Light moon;
    public Gradient moonColor;
    public AnimationCurve moonIntensity;
    public Material nightBox;
    [Header("Other Lighting")]
    public AnimationCurve lightingIntensityMultiplier;
    public AnimationCurve reflectionintensityMultiplier;

    private void Start()
    {
        timeRate = 1.0f / _fulldayLength;
        GameManager.Instance.time = _startTime;
    }

    private void Update()
    {
        GameManager.Instance.time = (GameManager.Instance.time + timeRate * Time.deltaTime) % 1.0f;

        UpdateLighting(sun, sunColor, sunIntensity);
        GameManager.Instance.IsDay = sun.gameObject.activeSelf;
        UpdateLighting(moon, moonColor, moonIntensity);

        UpdateSkyBox();
        RenderSettings.ambientIntensity = lightingIntensityMultiplier.Evaluate(GameManager.Instance.time);
        RenderSettings.reflectionIntensity = reflectionintensityMultiplier.Evaluate(GameManager.Instance.time);
    }

    void UpdateLighting(Light lightSource, Gradient colorGradiant, AnimationCurve intensityCurve)
    {
        float intensity = intensityCurve.Evaluate(GameManager.Instance.time);

        lightSource.transform.eulerAngles = (GameManager.Instance.time - (lightSource == sun ? 0.25f : -0.75f)) * _noon * 4.0f;
        lightSource.color = colorGradiant.Evaluate(GameManager.Instance.time);
        lightSource.intensity = intensity;

        GameObject go = lightSource.gameObject;
        if (lightSource.intensity == 0 && go.activeInHierarchy)
            go.SetActive(false);
        else if (lightSource.intensity > 0 && !go.activeInHierarchy)
            go.SetActive(true);
    }

    void UpdateSkyBox()
    {
        if (sun.gameObject.activeSelf)
        {
            RenderSettings.skybox = sunBox;
            RenderSettings.sun = sun;
        }
        else
        {
            RenderSettings.skybox = nightBox;
            RenderSettings.sun = moon;
        }
    }
}
