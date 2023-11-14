using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [SerializeField] private Transform _target; //플레이어
    private float _offsetRatio = 0.5f; //미니맵에 위치할 y축 퍼센티지

    Camera cam;
    Vector2 size;
    private void Start()
    {
        cam = GetComponent<Camera>();
        size = new Vector2(cam.orthographicSize, cam.orthographicSize * cam.aspect);//카메라 크기
    }

    private void LateUpdate()
    {
        Vector3 targetForward = _target.forward;
        targetForward.y = 0;
        targetForward.Normalize();

        Vector3 position = new Vector3(_target.transform.position.x, 70, _target.transform.position.z); 
                            //+ Vector3.one * _offsetRatio * cam.orthographicSize;
        transform.position = position;
        transform.eulerAngles = new Vector3( 90, 0, -_target.eulerAngles.y );

    }
}
