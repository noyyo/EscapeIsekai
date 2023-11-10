using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemySpawnManager : MonoBehaviour
{
    private static EnemySpawnManager instance;
    // �̱��������� DontDestroy�� �� �ʿ�� ����.
    public static EnemySpawnManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<EnemySpawnManager>();

                if (instance == null)
                {
                    GameObject obj = new GameObject { name = typeof(EnemySpawnManager).Name };
                    instance = obj.AddComponent<EnemySpawnManager>();
                }
            }
            return instance;
        }
    }
    private Dictionary<int, ObjectPool<Enemy>> enemyPools = new Dictionary<int, ObjectPool<Enemy>>();
    [SerializeField][ReadOnly]
    private List<EnemySpawner> spawners = new List<EnemySpawner>();
    public ObjectPool<Enemy> GetPool(Enemy enemy)
    {
        if (enemyPools.ContainsKey(enemy.Data.ID))
        {
            return enemyPools[enemy.Data.ID];
        }
        ObjectPool<Enemy> enemyPool = new ObjectPool<Enemy>(createFunc: () => Instantiate(enemy), actionOnGet: enemy => enemy.ResetEnemy(), actionOnRelease: enemy => enemy.OnRelease(), defaultCapacity: 5, maxSize: 100);
        enemyPools.Add(enemy.Data.ID, enemyPool);
        return enemyPool;
    }
    public void AddSpawner(EnemySpawner spawner)
    {
        spawners.Add(spawner);
    }
    private void OnDestroy()
    {
        foreach (ObjectPool<Enemy> pool in enemyPools.Values)
        {
            pool.Dispose();
        }
        enemyPools.Clear();
        spawners.Clear();
    }
}
