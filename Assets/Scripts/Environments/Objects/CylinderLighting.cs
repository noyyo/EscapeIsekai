using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CylinderLighting : MonoBehaviour
{
    private float _time;

    void Update()
    {
        _time -= Time.deltaTime;
        if(_time <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void PlusTime(float time)
    {
        _time += time; 
    }
}
