using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EconomyManager : CustomSingleton<EconomyManager>
{
    protected EconomyManager() { }

    [Tooltip("���Ѱ� ����")][Range(1f,5f)][SerializeField] private float supremum = 1.5f;
    [Tooltip("���Ѱ� ����")][Range(0f, 1f)][SerializeField] private float infimum = 0.5f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
