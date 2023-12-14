using UnityEngine;

public class MinimapIndicator : MonoBehaviour
{
    private Transform _player;
    [SerializeField] private Transform _target;
    [SerializeField] private GameObject _indicator; //Ÿ���� �ٶ� ������Ʈ
    [SerializeField] private GameObject _arrow; //���� ���� ��������Ʈ
    [SerializeField] private float indicatorDistance;  //�÷��̾�� �ε������� �Ÿ�
    [SerializeField] private float appearanceDistance; //�ε������͸� ǥ�ý����� �Ÿ�

    Vector3 targetPosition;
    Vector3 playerPosioton;
    private void Start()
    {
        _player = GameManager.Instance.Player.transform;
    }
    private void LateUpdate()
    {
        float distance = Vector3.Distance(_player.position, _target.position);
        if (Mathf.Abs(distance) > appearanceDistance)
        {
            _arrow.SetActive(true);

            targetPosition = _target.position;
            playerPosioton = _player.position;
            targetPosition.y = 0;
            playerPosioton.y = 0;

            Vector3 directionToTarget = (targetPosition - playerPosioton).normalized;
            _indicator.transform.position = _player.position + directionToTarget * indicatorDistance;
            _indicator.transform.rotation = Quaternion.LookRotation(directionToTarget);
        }
        else
        {
            _arrow.SetActive(false);
        }
    }

}
