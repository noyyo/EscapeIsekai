using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [SerializeField] private Transform _target; //플레이어
    [SerializeField] private float _offsetRatio; //미니맵에 위치할 y축 퍼센티지

    Camera cam;
    Vector2 size;
    public Transform indicator;
    private void Start()
    {
        cam = GetComponent<Camera>();
        size = new Vector2(cam.orthographicSize, cam.orthographicSize * cam.aspect);//카메라 크기
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
    public void ShowBorderIndicator(Vector3 position)
    {
        float reciprocal;
        float rotation;
        Vector2 distance = new Vector3(transform.position.x - position.x, transform.position.z - position.z); //카메라와 오브젝트 사이 거리

        distance = Quaternion.Euler(0, 0, _target.eulerAngles.y) * distance; //카메라의 회전을 고려해서 거리 벡터를 회전

        // X axis
        if (Mathf.Abs(distance.x) > Mathf.Abs(distance.y))
        {
            reciprocal = Mathf.Abs(size.x / distance.x);
            rotation = (distance.x > 0) ? 90 : -90;
        }
        // Y axis
        else
        {
            reciprocal = Mathf.Abs(size.y / distance.y);  //화살표 위치?
            rotation = (distance.y > 0) ? 180 : 0; //화살표 회전값
        }

        indicator.localPosition = new Vector3(distance.x * -reciprocal, distance.y * -reciprocal, 1);
        indicator.localEulerAngles = new Vector3(0, 0, rotation);
    }

    public void HideBorderIncitator()
    {
        indicator.gameObject.SetActive(false);
    }
}
