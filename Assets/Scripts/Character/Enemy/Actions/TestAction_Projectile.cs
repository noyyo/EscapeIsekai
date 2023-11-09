using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "TestAction_ProjectileSO", menuName = "Characters/Enemy/AttackAction/TestAction_Projectile")]
public class TestAction_Projectile : AttackAction
{
    /*
  * 액션의 기본적인 흐름은 다음과 같습니다.
  * Awake가 실행되어 기본적인 초기화를 실행합니다.
  * 캐릭터가 액션을 실행하면 Start가 실행되고 이후 Update문이 매 프레임 호출됩니다. 액션을 시작할 때 캐릭터는 대상을 바라보고 있는 상태입니다.
  * 액션의 로직은 Update를 활용해서 작성해야 합니다. 로직에 활용해야하는 참조나 변수등은 다른 분들이 인스펙터로 참조할 수 있게끔 SerializeField 속성을 사용하는 것을 권장합니다. 내부적으로만 사용되는 변수는 HideInInspector속성을 사용해서 숨깁니다.
  * Update에서 로직을 적용할 때에는 bool값을 가지고 적용 여부를 판단한 뒤 적용하거나 Event를 만들어서 알리는 등 자유롭게 구현하면 됩니다.
  * Update에서 timeStarted 또는 timeRunning등의 필드 값을 통해서 시간을 기준으로 로직을 적용할 수 있습니다.
  * 액션의 실행이 완료되었다면 isCompleted 변수를 true로 설정해줘야 합니다. 그러면 다음 프레임에서 액션이 끝나고 캐릭터는 다른 상태로 전환됩니다.
  * 이펙트를 시작할 때는 OnEffectStart() 메소드를 실행해줘야 합니다. 실행하지 않으면 이펙트가 영영 사라지지 않습니다.
  * 애니메이션을 시작하기 위해선 StartAnimation 메소드를 사용하고 animState 딕셔너리로 해당 애니메이션이 실행되지 않았는지 실행중인지 끝났는지 확인할 수 있습니다.
  * 현재 실행중인 애니메이션은 currentAnimHash 및 currentAnimNormalizedTime 필드로 해쉬값과 실행중이라면 얼만큼 실행됐는지를 확인할 수 있습니다.
  * 만약 애니메이션이 Trigger가 아닌 Bool값의 파라미터로 관리된다면 적절한 시점에 StopAnimation 메소드를 호출해주어야 합니다.
  * 이외에도 base클래스에서 사용되는 메소드 및 매개변수에 마우스를 올려놓으면 설명을 볼 수 있습니다. 
  */
    [Header("Projectile_Setting")]
    [Tooltip("총알의 갯수")]
    [Range(1, 100)]
    public int howManyBullet;

    [Tooltip("총알의 타입 / Nomal은 각도와 갯수에만 영향받음 , ShotGun은 각도 갯수 밀집도 모두 영향받음")]
    public BulletType bulletType;
    [Tooltip("총알의 방사 각도")]
    [Range(0, 360)]
    public int bulletAngle;
    [Tooltip("총알의 밀집도")]
    [Range(0, 1)]
    public float spacing;
    [Tooltip("총알 생성 위치, Enemy의 포지션을 기준으로 y축으로 +-됩니다.")]
    [Range(-10, 10)]
    public float YOffset;
    [Space(10f)]
    [Tooltip("사용할 총알의 프리팹 / *Bullet.cs 달아줘야함")]
    public GameObject howBullet;

    private List<GameObject> bulletPool = new List<GameObject>(); //오브젝트 풀링 리스트
    public override void OnAwake()
    {
        base.OnAwake();
        for (int i = 0; i < howManyBullet; i++)
        {
            GameObject bullet = Instantiate(howBullet, Vector3.zero, Quaternion.identity);
            bullet.SetActive(false);
            bulletPool.Add(bullet);
            bullet.GetComponent<Bullet>().ProjetileColliderEnter += OnProjectileTriggerEnter;
        }
    }
    // 변수들을 다시 세팅해주거나 필요한 작업을 해주면 됩니다.
    public override void OnStart()
    {
        base.OnStart();
        StartAnimation(Config.AnimTriggerHash1); //활 드는 모션
    }
    // 액션이 끝날 때 필요한 작업을 해주면 됩니다.
    public override void OnEnd()
    {
        base.OnEnd();
    }

    // 액션의 로직을 실행하면 됩니다.
    public override void OnUpdate()
    {
        base.OnUpdate();
        // Anim1이 완료되었고, 이펙트를 아직 시작하지 않았다면 이펙트를 실행합니다.
        if (animState[Config.AnimTriggerHash1] == AnimState.Completed && !isEffectStarted)
        {
            MakeBullet();
            OnEffectStart();
        }
        // Anim2가 완료되었다면 액션을 완료합니다.
        if (animState[Config.AnimTriggerHash2] == AnimState.Completed)
        {
            isCompleted = true;
        }
    }
    // 이펙트를 시작할 때 호출해야합니다.
    protected override void OnEffectStart()
    {
        base.OnEffectStart();
        // 애니메이션을 시작합니다.
        StartAnimation(Config.AnimTriggerHash2);
    }
    // 이펙트가 끝날 때 자동으로 호출됩니다. 필요한 작업을 수행합니다.
    protected override void OnEffectFinish()
    {
        base.OnEffectFinish();
    }

    private Vector3 CalTargetVector()
    {
        Vector3 TargetDistance = StateMachine.Player.transform.position - StateMachine.Enemy.transform.position;
        return TargetDistance;
    }
    private void OnProjectileTriggerEnter(Collision other,GameObject attacker)
    {
        IDamageable target = null;
        if (other.transform.tag == Tags.PlayerTag)
        {
            Player player;
            other.transform.TryGetComponent(out player);
            if (player == null)
            {
                Debug.LogError("Player 스크립트를 찾을 수 없습니다.");
                return;
            }
            target = player.StateMachine;
        }
        else if (other.transform.tag == Tags.EnemyTag)
        {
            Enemy enemy;
            other.transform.TryGetComponent(out enemy);
            if (enemy == null)
            {
                Debug.LogError("Enemy 스크립트를 찾을 수 없습니다.");
                return;
            }
            target = enemy.StateMachine;
        }
        else if (other.transform.tag == Tags.EnvironmentTag)
        {
            BaseEnvironmentObject environmentObj;
            other.transform.TryGetComponent(out environmentObj);
            if (environmentObj == null)
            {
                Debug.LogError("대상에게 BaseEnvironmentObject 컴포넌트가 없습니다.");
                return;
            }
            target = environmentObj;
        }
        if (target != null)
        {
            ApplyAttack(target, false, other.gameObject);
           target.TakeEffect(Config.AttackEffectType, Config.AttackEffectValue, attacker);
        }
    }

    private GameObject GetBulletFromPool()
    {
        foreach (var bullet in bulletPool)
        {
            if (!bullet.activeInHierarchy)
            {
                bullet.SetActive(true);
                return bullet;
            }
        }
        return null;
    }

    private void MakeBullet()
    {
        if (howManyBullet != 1)
        {
            if (bulletType == BulletType.ShotGun)
            {
                int rows = Mathf.CeilToInt(Mathf.Sqrt(howManyBullet));
                int cols = Mathf.CeilToInt(howManyBullet / (float)rows);
                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < cols; col++)
                    {
                        if (row * cols + col >= howManyBullet)
                        {
                            return;
                        }

                        float xOffset = (col - (cols - 1) / 2.0f) * spacing;
                        float zOffset = (row - (rows - 1) / 2.0f) * spacing;

                        GameObject bullet = GetBulletFromPool();
                        if (bullet != null)
                        {
                            bullet.transform.position = StateMachine.Enemy.transform.position + StateMachine.Enemy.transform.forward + new Vector3(YOffset, xOffset, zOffset);

                            float angleX = -bulletAngle / 2f + (bulletAngle / (howManyBullet - 1)) * (row * cols + col);
                            float angleZ = -bulletAngle / 2f + (bulletAngle / (howManyBullet - 1)) * (row * cols + col);
                            Vector3 forward = Quaternion.Euler(angleX, 0, angleZ) * StateMachine.Enemy.transform.forward;

                            bullet.GetComponent<Bullet>().targetDir = forward * 10;
                        }
                    }
                }
            }
            else
            {
                for (int i = 1; i <= howManyBullet; i++)
                {
                    GameObject bullet = GetBulletFromPool();
                    if (bullet != null)
                    {
                        bullet.transform.position = StateMachine.Enemy.transform.position + StateMachine.Enemy.transform.forward;
                        float angle = -bulletAngle / 2f + (bulletAngle / (howManyBullet - 1)) * (i - 1);
                        Vector3 forward = Quaternion.Euler(0, angle, 0) * StateMachine.Enemy.transform.forward;
                        bullet.GetComponent<Bullet>().targetDir = forward * 10;
                    }
                }
            }
        }
        else
        {
            GameObject bullet = GetBulletFromPool();
            if (bullet != null)
            {
                bullet.transform.position = StateMachine.Enemy.transform.position + StateMachine.Enemy.transform.forward;
                bullet.GetComponent<Bullet>().targetDir = CalTargetVector();
            }
        }
    }

}
public enum BulletType
{
    Nomal,
    ShotGun
}