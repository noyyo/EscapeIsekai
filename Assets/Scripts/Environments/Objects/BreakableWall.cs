using UnityEngine;

public class BreakableWall : BaseEnvironmentObject
{
    [SerializeField] private int hp = 100;
    [SerializeField] Collider collider;
    [SerializeField] string colliderName = "Weapon";

    private int defaultHP;
    private int damage;
    private bool isTakeDamage;

    private void Start()
    {
        defaultHP = hp;
        if (collider == null)
            collider = GetComponent<Collider>();
        if( collider == null )
            collider = GetComponentInChildren<Collider>();
        Init();
    }

    public override void TakeDamage(int damage)
    {
        this.damage = damage;
        isTakeDamage = true;
    }

    public override void TakeEffect(AttackEffectTypes attackEffectTypes, float value, GameObject attacker)
    { }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == colliderName && isTakeDamage == true)
        {
            if (hp > 0)
                HPControl();
            isTakeDamage = false;
        }
    }

    private void HPControl()
    {
        hp -= damage;
        if( hp <= 0 )
        {
            hp = 0;
            OnBreak();
            return;
        }
        PlayTakeDamageAnimation();
    }

    private void OnBreak()
    {
        collider.enabled = false;
        PlayerBreakAnimation();

        //애니메이션이 끝나면 비활성화
        gameObject.SetActive(false);
    }

    private void PlayTakeDamageAnimation()
    {

    }

    private void PlayerBreakAnimation()
    {

    }

    public void Init()
    {
        hp = defaultHP;
        gameObject.SetActive(true);
        collider.enabled = true;
    }
}
