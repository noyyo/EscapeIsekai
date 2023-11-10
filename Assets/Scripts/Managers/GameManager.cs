using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : CustomSingleton<GameManager>
{
    [SerializeField] private GameObject _soundManagerObject;
    [SerializeField] private GameObject _player;
    [Range(0.0f, 1.0f)]
    public float time; //�Ϸ� ����Ŭ �ð�  0.2~0.8 �ض��ִ� �ð�
    public bool IsDay;

    
    private SoundManager _soundManager;
    

    public GameObject Player { get { return _player; } }

    private void Awake()
    {
        if (_soundManagerObject == null)
            _soundManagerObject = Instantiate(Resources.Load<GameObject>("Prefabs/Manager/SoundManager"));
        _soundManager = _soundManagerObject.GetComponent<SoundManager>();

        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(Tags.PlayerTag);
        foreach (GameObject gameObject in gameObjects)
        {
            if (gameObject.name == "Player")
            {
                _player = gameObject;
            }
        }
        if(_player == null)
        {
            _player = Instantiate(Resources.Load<GameObject>("Prefabs/Player/Player"));
        }
    }

}
