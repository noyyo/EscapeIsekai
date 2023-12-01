using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    private GameObject player;
    private CinemachineVirtualCamera virtualCamera;
    private Transform lookPoint;
    void Start()
    {
        player = GameManager.Instance.Player;
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        lookPoint = player.transform.GetChild(0);
        virtualCamera.LookAt = lookPoint;
        virtualCamera.Follow = lookPoint;
    }
}
