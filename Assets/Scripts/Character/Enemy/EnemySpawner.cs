using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

[RequireComponent(typeof(BoxCollider))]
public class EnemySpawner : MonoBehaviour
{
    public Enemy[] EnemyPrefabs;
    public int MaxEnemyCount;
    private int currentEnemyCount;
    public float SpawnDelay;
    private float lastSpawnTime;
    [SerializeField][ReadOnly]
    public List<Enemy> Enemies = new List<Enemy>();
    BoxCollider boxCollider;
    private void Awake()
    {
        EnemySpawnManager.Instance.AddSpawner(this);
        boxCollider = GetComponent<BoxCollider>();
    }
    private void Update()
    {
        if (currentEnemyCount < MaxEnemyCount && Time.time - lastSpawnTime > SpawnDelay)
        {
            SpawnEnemy();
        }
    }
    private void SpawnEnemy()
    {
        int index = Random.Range(0, EnemyPrefabs.Length);
        float yOffset = EnemyPrefabs[index].transform.position.y;
        ObjectPool<Enemy> enemyPool = EnemySpawnManager.Instance.GetPool(EnemyPrefabs[index]);
        Enemy enemy = enemyPool.Get();
        Vector3 max = boxCollider.bounds.max;
        Vector3 min = boxCollider.bounds.min;
        float x = Random.Range(min.x, max.x);
        float z = Random.Range(min.z, max.z);
        Ray ray = new Ray(new Vector3(x, max.y, z), Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, max.y, 1 << LayerMask.NameToLayer("Ground")))
        {
            NavMeshAgent agent;
            enemy.TryGetComponent(out agent);
            if (agent == null)
            {
                Debug.LogError("생성하려는 적에게 NavMeshAgent가 없습니다.");
            }
            enemy.transform.position = new Vector3(x, hit.point.y + agent.height / 2 - agent.baseOffset + yOffset, z);
            enemy.gameObject.SetActive(true);
            agent.enabled = false;
            agent.enabled = true;
            Enemies.Add(enemy);
            enemy.StateMachine.OnDieAction += ReleaseEnemy;
            currentEnemyCount++;
            lastSpawnTime = Time.time;
        }
        else
        {
            Debug.LogError("생성할 지면을 찾을 수 없습니다.");
        }
    }
    private void ReleaseEnemy(Enemy enemy)
    {
        enemy.StateMachine.OnDieAction -= ReleaseEnemy;
        if (!Enemies.Contains(enemy))
        {
            Debug.LogError("이미 Release가 되었거나 이 Spawner가 관리하지 않는 적입니다.");
            return;
        }
        Enemies.Remove(enemy);
        EnemySpawnManager.Instance.GetPool(enemy).Release(enemy);
        currentEnemyCount--;
    }
}
