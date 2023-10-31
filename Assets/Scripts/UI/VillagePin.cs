using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagePin : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    public MinimapCamera minimapCamera;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        transform.rotation = Quaternion.Euler(90, transform.parent.eulerAngles.y, 0);
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        if (spriteRenderer.isVisible == false)
        {
            minimapCamera.ShowBorderIndicator(transform.position);
        }
        else
        {
            minimapCamera.HideBorderIncitator();
        }
    }
    
}
