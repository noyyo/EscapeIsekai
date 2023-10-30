using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [SerializeField] private Transform _target; //플레이어
    [SerializeField] private float _offsetRatio; //미니맵에 위치할 y축 퍼센티지

    Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        Vector3 targetForward = _target.forward;
        targetForward.y = 150;
        targetForward.Normalize();

        Vector3 position = new Vector3(_target.transform.position.x, 1, _target.transform.position.z) 
                            + targetForward * _offsetRatio * cam.orthographicSize;
        transform.position = position;
        transform.eulerAngles = new Vector3( 90, 0, -_target.eulerAngles.y );

    }
}
