using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(90, transform.parent.eulerAngles.y, 0);
        transform.position = new Vector3(transform.parent.position.x, 0, transform.parent.position.z);
    }
}
