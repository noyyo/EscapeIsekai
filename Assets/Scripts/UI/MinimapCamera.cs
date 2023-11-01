using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [SerializeField] private Transform _target; //�÷��̾�
    private float _offsetRatio = 0.5f; //�̴ϸʿ� ��ġ�� y�� �ۼ�Ƽ��

    Camera cam;
    Vector2 size;
    private void Start()
    {
        cam = GetComponent<Camera>();
        size = new Vector2(cam.orthographicSize, cam.orthographicSize * cam.aspect);//ī�޶� ũ��
    }

    private void LateUpdate()
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
