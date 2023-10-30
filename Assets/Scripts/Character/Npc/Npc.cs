using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class Npc : MonoBehaviour
{
    public int id;
    public bool isNPC;
    public TimelineAsset[] Motion;
    private bool isHit;
    private GameObject player;
    private void Update()
    {
        if(isHit && player !=null)
        {
            Vector3 lookDirection = player.transform.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 2.0f * Time.deltaTime);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        player = collision.gameObject;
        isHit = true;
        Dialog.instance.Action(gameObject);
    }

    public void ResetTarget()
    {
        isHit = false;
        player = null;
    }
}
