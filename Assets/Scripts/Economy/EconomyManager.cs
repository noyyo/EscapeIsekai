using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EconomyManager : CustomSingleton<EconomyManager>
{
    protected EconomyManager() { }

    [Tooltip("상한값 설정")][Range(1f,5f)][SerializeField] private float supremum = 1.5f;
    [Tooltip("하한값 설정")][Range(0f, 1f)][SerializeField] private float infimum = 0.5f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
