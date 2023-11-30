using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class FireballCustom : MonoBehaviour
{
    private VisualEffect fireballTrails;
    private static readonly string fireballTrailsActive = "FireballTrailsActive";
    private static readonly string fireballTrailsScale = "FireballTrailsScale";

    private void Awake()
    {
        fireballTrails = gameObject.GetComponent<VisualEffect>();
        fireballTrails.enabled = true;
        fireballTrails.SetBool(fireballTrailsActive, true);
    }
    private void Update()
    {
        fireballTrails.SetVector3(fireballTrailsScale, transform.localScale);
    }
}
