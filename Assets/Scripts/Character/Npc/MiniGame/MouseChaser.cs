﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MouseChaser : MonoBehaviour
{
    // 카메라로부터의 거리
    public float _distanceFromCamera = 5f;

    [Range(0.01f, 1.0f)]
    public float _ChasingSpeed = 0.1f;

    private Vector3 _mousePos;
    private Vector3 _nextPos;

    private void OnValidate()
    {
        if(_distanceFromCamera < 0f)
            _distanceFromCamera = 0f;
    }

    private void Start()
    {
    }

    void Update()
    {
        _mousePos = Input.mousePosition;
        _mousePos.z = _distanceFromCamera;
        if (Input.GetMouseButtonDown(0))
        {
            MouseSlider.Instance.isClick = true;
            gameObject.GetComponent<ParticleSystem>().Play();
        }
        if (Input.GetMouseButtonUp(0))
        {
            MouseSlider.Instance.isClick = false;
            gameObject.GetComponent<ParticleSystem>().Stop();
        }
        if (MouseSlider.Instance.isClick)
        {
         transform.position = Camera.main.ScreenToWorldPoint(_mousePos);
        }

    }
}
