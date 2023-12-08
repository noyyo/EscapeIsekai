using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum AttackRangeType
{
    Degrree30,
    Degrree45,
    Degrree90,
    Degrree180,
    Circle
}

public class Glowstone : BaseEnvironmentObject
{
    private Collider thisCollider;
    [SerializeField] private AttackRangeType attackRangeType;
    [Range(0f, 8f)][SerializeField] private float playFadeOutTime = 2f;
    [SerializeField] private AttackEffectTypes attackType;
    [Range(0f, 30f)][SerializeField] private float effectValue = 2f;
    [SerializeField] private float objSpeed = 1f;

    [Header("리스폰")]
    [SerializeField] private bool isRespawn = true;
    [SerializeField] private float respawnTime = 5f;
    private Weapon attackerWeapon;
    private bool isPlayer;
    private bool isBreak;
    private Vector3 defaultPos;
    private Quaternion defaultRot;
    private Vector3 playerPos;
    private Vector2 direction;
    private float deg;
    private float speed;


    private void Awake()
    {
        defaultPos = transform.position;
        defaultRot = transform.rotation;
        if (TryGetComponent<Collider>(out thisCollider))
        {
            thisCollider = GetComponentInChildren<Collider>();
        }
    }
    private void Start()
    {
        Init();
    }

    public override Vector3 GetObjectCenterPosition()
    {
        return thisCollider.bounds.center;
    }

    public override void TakeDamage(int damage, GameObject attacker)
    {
        
    }

    public override void TakeEffect(AttackEffectTypes attackEffectTypes, float value, GameObject attacker)
    {}

    private void OnTriggerEnter(Collider other)
    {
        if (isPlayer)
        {
            if (other.gameObject.layer == TagsAndLayers.CharacterLayerIndex)
            {
                if (other.TryGetComponent<Weapon>(out attackerWeapon))
                {
                    //if (attackerWeapon.character.CompareTag(TagsAndLayers.PlayerTag))
 
                }
            }
            isPlayer = false;
        }
    }

    private void Init()
    {
        transform.position = defaultPos;
        transform.rotation = defaultRot;
        thisCollider.enabled = true;
        speed = objSpeed;
        isBreak = false;
        deg = 0;
    }

    private void BreakTree()
    {
        direction.Set(defaultPos.x - playerPos.x, defaultPos.z - playerPos.z);
        direction = direction.normalized;
        isBreak = true;
        StartCoroutine(Falling());
    }

    private IEnumerator Falling()
    {
        while (deg < 95)
        {
            deg += speed + Time.deltaTime;
            speed += 0.001f;
            transform.rotation = Quaternion.Euler(deg * direction.y, 0, deg * -direction.x);
            yield return null;
        }
        Invoke("PlayAnimationFadeOut", playFadeOutTime);
        yield break;
    }

    private void PlayAnimationTakeDamage()
    {

    }

    private void PlayAnimationFadeOut()
    {
        //임시
        Deactivate();
        thisCollider.enabled = false;
    }

    private void Respawn()
    {
        Init();
        Activate();
    }

    private void Activate()
    {
        this.gameObject.SetActive(true);
    }

    private void Deactivate()
    {
        this.gameObject.SetActive(false);
        if (isRespawn)
            Invoke("Respawn", respawnTime);
    }
}
