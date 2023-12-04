using UnityEngine;

public class MinimapPin : MonoBehaviour
{
    protected virtual void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(90, transform.parent.eulerAngles.y, 0);
        transform.position = new Vector3(transform.position.x, transform.parent.position.y + 5, transform.position.z);
    }
}
