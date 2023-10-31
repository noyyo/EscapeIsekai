using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [SerializeField] private Transform _target; //�÷��̾�
    [SerializeField] private float _offsetRatio; //�̴ϸʿ� ��ġ�� y�� �ۼ�Ƽ��

    Camera cam;
    Vector2 size;
    public Transform indicator;
    private void Start()
    {
        cam = GetComponent<Camera>();
        size = new Vector2(cam.orthographicSize, cam.orthographicSize * cam.aspect);//ī�޶� ũ��
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
        Vector2 distance = new Vector3(transform.position.x - position.x, transform.position.z - position.z); //ī�޶�� ������Ʈ ���� �Ÿ�

        distance = Quaternion.Euler(0, 0, _target.eulerAngles.y) * distance; //ī�޶��� ȸ���� ����ؼ� �Ÿ� ���͸� ȸ��

        // X axis
        if (Mathf.Abs(distance.x) > Mathf.Abs(distance.y))
        {
            reciprocal = Mathf.Abs(size.x / distance.x);
            rotation = (distance.x > 0) ? 90 : -90;
        }
        // Y axis
        else
        {
            reciprocal = Mathf.Abs(size.y / distance.y);  //ȭ��ǥ ��ġ?
            rotation = (distance.y > 0) ? 180 : 0; //ȭ��ǥ ȸ����
        }

        indicator.localPosition = new Vector3(distance.x * -reciprocal, distance.y * -reciprocal, 1);
        indicator.localEulerAngles = new Vector3(0, 0, rotation);
    }

    public void HideBorderIncitator()
    {
        indicator.gameObject.SetActive(false);
    }
}
