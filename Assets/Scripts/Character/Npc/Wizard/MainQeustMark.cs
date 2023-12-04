using UnityEngine;

public class MainQeustMark : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(Vector3.up * 50 * Time.deltaTime);
    }
}
