using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class FireballCustom : MonoBehaviour
{
    private VisualEffect fireballTrails;
    private const string fireballTrailsActiveString = "FireballTrailsActive";

    private void Awake()
    {
        fireballTrails = gameObject.GetComponent<VisualEffect>();
        fireballTrails.enabled = true;
        fireballTrails.SetBool(fireballTrailsActiveString, true);
    }
}
