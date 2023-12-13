using UnityEngine;

public class Grenade : MonoBehaviour
{
    private new Rigidbody rigidbody;
    public float ExplosionRadius = 5f;
    public float StunTime = 3f;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        Throw();
    }
    private void Throw()
    {
        float throwForce = 15f;
        float throwHeight = 15f;
        rigidbody.AddForce(Vector3.up * throwHeight + transform.forward * throwForce * rigidbody.mass, ForceMode.Impulse);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == TagsAndLayers.GroundLayerIndex || other.gameObject.layer == TagsAndLayers.CharacterLayerIndex || other.gameObject.layer == TagsAndLayers.PlayerLayerIndex)
        {
            Explosion();
        }
    }

    private void Explosion()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, ExplosionRadius);

        foreach (Collider collider in colliders)
        {
            IDamageable target;
            if (collider.CompareTag(TagsAndLayers.EnemyTag))
            {
                Enemy enemy;
                if (!collider.TryGetComponent<Enemy>(out enemy))
                {
                    Debug.Log("Enemy를 찾지 못했습니다.");
                }
                target = enemy.StateMachine;
                target.TakeEffect(AttackEffectTypes.Stun, StunTime, gameObject);
            }
        }
        Destroy(gameObject);
    }

}
