using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public enum BulletType
{
    Nomal,
    ShotGun
}
[CreateAssetMenu(fileName = "TestAction_ProjectileSO", menuName = "Characters/Enemy/AttackAction/TestAction_Projectile")]
public class TestAction_Projectile : AttackAction
{
    /*
  * �׼��� �⺻���� �帧�� ������ �����ϴ�.
  * Awake�� ����Ǿ� �⺻���� �ʱ�ȭ�� �����մϴ�.
  * ĳ���Ͱ� �׼��� �����ϸ� Start�� ����ǰ� ���� Update���� �� ������ ȣ��˴ϴ�. �׼��� ������ �� ĳ���ʹ� ����� �ٶ󺸰� �ִ� �����Դϴ�.
  * �׼��� ������ Update�� Ȱ���ؼ� �ۼ��ؾ� �մϴ�. ������ Ȱ���ؾ��ϴ� ������ �������� �ٸ� �е��� �ν����ͷ� ������ �� �ְԲ� SerializeField �Ӽ��� ����ϴ� ���� �����մϴ�. ���������θ� ���Ǵ� ������ HideInInspector�Ӽ��� ����ؼ� ����ϴ�.
  * Update���� ������ ������ ������ bool���� ������ ���� ���θ� �Ǵ��� �� �����ϰų� Event�� ���� �˸��� �� �����Ӱ� �����ϸ� �˴ϴ�.
  * Update���� timeStarted �Ǵ� timeRunning���� �ʵ� ���� ���ؼ� �ð��� �������� ������ ������ �� �ֽ��ϴ�.
  * �׼��� ������ �Ϸ�Ǿ��ٸ� isCompleted ������ true�� ��������� �մϴ�. �׷��� ���� �����ӿ��� �׼��� ������ ĳ���ʹ� �ٸ� ���·� ��ȯ�˴ϴ�.
  * ����Ʈ�� ������ ���� OnEffectStart() �޼ҵ带 ��������� �մϴ�. �������� ������ ����Ʈ�� ���� ������� �ʽ��ϴ�.
  * �ִϸ��̼��� �����ϱ� ���ؼ� StartAnimation �޼ҵ带 ����ϰ� animState ��ųʸ��� �ش� �ִϸ��̼��� ������� �ʾҴ��� ���������� �������� Ȯ���� �� �ֽ��ϴ�.
  * ���� �������� �ִϸ��̼��� currentAnimHash �� currentAnimNormalizedTime �ʵ�� �ؽ����� �������̶�� ��ŭ ����ƴ����� Ȯ���� �� �ֽ��ϴ�.
  * ���� �ִϸ��̼��� Trigger�� �ƴ� Bool���� �Ķ���ͷ� �����ȴٸ� ������ ������ StopAnimation �޼ҵ带 ȣ�����־�� �մϴ�.
  * �̿ܿ��� baseŬ�������� ���Ǵ� �޼ҵ� �� �Ű������� ���콺�� �÷������� ������ �� �� �ֽ��ϴ�. 
  */
    [Header("Projectile_Setting")]
    [Tooltip("�Ѿ��� ����")]
    [Range(1, 100)]
    public int howManyBullet;

    [Tooltip("�Ѿ��� Ÿ�� / Nomal�� ������ �������� ������� , ShotGun�� ���� ���� ������ ��� �������")]
    public BulletType bulletType;
    [Tooltip("�Ѿ��� ��� ����")]
    [Range(0, 360)]
    public int bulletAngle;
    [Tooltip("�Ѿ��� ������")]
    [Range(0, 1)]
    public float spacing;
    [Tooltip("�Ѿ� ���� ��ġ, Enemy�� �������� �������� y������ +-�˴ϴ�.")]
    [Range(-10, 10)]
    public float YOffset;
    [Space(10f)]
    [Tooltip("����� �Ѿ��� ������ / *Bullet.cs �޾������")]
    public GameObject howBullet;
    private bool alreadyLaunched;

    private List<GameObject> bulletPool = new List<GameObject>(); //������Ʈ Ǯ�� ����Ʈ
    public override void OnAwake()
    {
        base.OnAwake();
        for (int i = 0; i < howManyBullet; i++)
        {
            GameObject bullet = Instantiate(howBullet, Vector3.zero, Quaternion.identity);
            bullet.SetActive(false);
            bulletPool.Add(bullet);
            bullet.GetComponent<Bullet>().ProjetileColliderEnter += OnProjectileTriggerEnter;
            StateMachine.Enemy.AnimEventReceiver.AnimEventCalled += EventDecision;
        }
    }

    private void EventDecision(AnimationEvent animEvent)
    {
        if (animEvent.stringParameter == "LaunchProjectile")
        {
            MakeBullet();
            alreadyLaunched = true;
        }
    }

    // �������� �ٽ� �������ְų� �ʿ��� �۾��� ���ָ� �˴ϴ�.
    public override void OnStart()
    {
        base.OnStart();
        alreadyLaunched = false;
        StartAnimation(Config.AnimTriggerHash1);
    }
    // �׼��� ���� �� �ʿ��� �۾��� ���ָ� �˴ϴ�.
    public override void OnEnd()
    {
        base.OnEnd();
    }

    // �׼��� ������ �����ϸ� �˴ϴ�.
    public override void OnUpdate()
    {
        base.OnUpdate();
        // �ִϸ��̼� ����� ������ ����ü�� �߻�ƴٸ� �����ϴ�.
        if (animState[Config.AnimTriggerHash1] == AnimState.Completed && alreadyLaunched)
        {
            isCompleted = true;
        }
    }
    // ����Ʈ�� ������ �� ȣ���ؾ��մϴ�.
    protected override void OnEffectStart()
    {
        base.OnEffectStart();
        // �ִϸ��̼��� �����մϴ�.
    }
    // ����Ʈ�� ���� �� �ڵ����� ȣ��˴ϴ�. �ʿ��� �۾��� �����մϴ�.
    protected override void OnEffectFinish()
    {
        base.OnEffectFinish();
    }

    private Vector3 CalTargetVector()
    {
        Vector3 TargetDistance = StateMachine.Player.transform.position - StateMachine.Enemy.transform.position;
        return TargetDistance;
    }
    private void OnProjectileTriggerEnter(GameObject target)
    {
        ApplyAttack(target);
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