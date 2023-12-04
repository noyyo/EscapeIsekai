using UnityEngine;
using UnityEngine.UI;

public class TextSameAsParentName : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Text>().text = transform.parent.gameObject.name;
    }
}
