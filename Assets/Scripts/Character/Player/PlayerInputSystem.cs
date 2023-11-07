using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputSystem : MonoBehaviour
{
    public PlayerInputAction InputActions { get; private set; }
    public PlayerInputAction.PlayerActions PlayerActions { get; private set; }

    private void Awake()
    {
        InputActions = new PlayerInputAction();

        PlayerActions = InputActions.Player;
    }

    // playerinput을 활성화, 비활성화 시키는 부분
    private void OnEnable()
    {
        InputActions.Enable();
    }

    private void OnDisable()
    {
        InputActions.Disable();
    }
}
