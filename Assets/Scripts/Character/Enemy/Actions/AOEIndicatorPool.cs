using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;


public class AOEIndicatorPool : CustomSingleton<AOEIndicatorPool>
{
    [SerializeField][ReadOnly] private AOEIndicator[] IndicatorPrefabs;
    private const string PrefabPath = "Prefabs/Utils/AOEIndicator";

    private Dictionary<AOETypes, ObjectPool<AOEIndicator>> indicators;
    private void Awake()
    {
        LoadIndicatorPrefabs();
        InitializePool();
    }
    public ObjectPool<AOEIndicator> GetIndicatorPool(AOETypes type)
    {
        if (type == AOETypes.None)
        {
            Debug.LogError("AOE 타입이 None입니다.");
            return null;
        }
        return indicators[type];
    }
    private void LoadIndicatorPrefabs()
    {
        IndicatorPrefabs = Resources.LoadAll<AOEIndicator>(PrefabPath);
    }
    private void InitializePool()
    {
        indicators = new Dictionary<AOETypes, ObjectPool<AOEIndicator>>(IndicatorPrefabs.Length);
        for (int i = 0; i < IndicatorPrefabs.Length; i++)
        {
            if (IndicatorPrefabs[i].AOEType == AOETypes.None)
            {
                Debug.LogError("제대로 설정되지 않은 AOEIndicator입니다.");
                continue;
            }
            AOEIndicator indicator = IndicatorPrefabs[i];
            indicators.Add(indicator.AOEType, new ObjectPool<AOEIndicator>(() => Instantiate(indicator), indi => indi.gameObject.SetActive(true), projector => projector.gameObject.SetActive(false), maxSize: 50));
        }
    }
}
