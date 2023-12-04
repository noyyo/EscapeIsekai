using UnityEngine;

public enum PointReferenceTypes
{
    Breath,
    LaunchProjectile,
}

public class PointReference : MonoBehaviour
{
    public PointReferenceTypes PointType;
    [Tooltip("이 포인트를 들고있는 최상위 게임오브젝트.")]
    public GameObject Character;
    private Vector3 direction;
    private void Awake()
    {
        if (Character == null)
        {
            Debug.LogError("PointReference에 Character를 설정해줘야 합니다.");
            return;
        }
    }
    //private void Update()
    //{
    //    if (Character == null)
    //        return;
    //    direction = transform.position - Character.transform.position;
    //    direction.y = 0;
    //    direction.Normalize();
    //    Debug.Log(direction);
    //    transform.forward = direction;
    //}
}
