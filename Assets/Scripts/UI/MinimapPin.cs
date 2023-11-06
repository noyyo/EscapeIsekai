using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MinimapPin : MonoBehaviour
{
    protected virtual void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(90, transform.parent.eulerAngles.y, 0);
        transform.position = new Vector3(transform.position.x,0,transform.position.z);
    }
}
