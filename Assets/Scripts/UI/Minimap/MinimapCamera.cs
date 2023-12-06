using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    private Transform _target; //플레이어
    private int height = 70;

    Camera cam;
    private void Start()
    {
        _target = GameManager.Instance.Player.GetComponent<Transform>();
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        Vector3 targetForward = _target.forward;
        targetForward.y = 0;
        targetForward.Normalize();

        Vector3 position = new Vector3(_target.transform.position.x, _target.transform.position.y + height, _target.transform.position.z);
        transform.position = position;
        transform.eulerAngles = new Vector3(90, 0, -_target.eulerAngles.y);

    }
}
