using UnityEngine;

public class MinimapIndicator : MonoBehaviour
{
    private Transform _player;
    [SerializeField] private Transform _target;
    [SerializeField] private GameObject _indicator; //타겟을 바라볼 오브젝트
    [SerializeField] private GameObject _arrow; //눈에 보일 스프라이트
    [SerializeField] private float indicatorDistance;  //플레이어와 인디케이터 거리
    [SerializeField] private float appearanceDistance; //인디케이터를 표시시작할 거리

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
