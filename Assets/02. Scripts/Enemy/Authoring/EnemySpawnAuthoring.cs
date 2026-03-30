using Unity.Entities;
using UnityEngine;

class EnemySpawnAuthoring : MonoBehaviour
{
    public GameObject EnemyPrefab;
    public int EnemyCount = 30000;
    public float MinSpeed = 0.2f;
    public float MaxSpeed = 1.0f;
    public float SpawnRadius = 50f;
    public uint RandomSeed = 12345;
}

class EnemySpawnAuthoringBaker : Baker<EnemySpawnAuthoring>
{
    public override void Bake(EnemySpawnAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        
        AddComponent(entity, new EnemySpawnComponent
        {
            Prefab = GetEntity(authoring.EnemyPrefab, TransformUsageFlags.Dynamic),
            NumEnemies = authoring.EnemyCount,
            MinSpeed = authoring.MinSpeed,
            MaxSpeed = authoring.MaxSpeed,
            SpawnRadius = authoring.SpawnRadius,
            RandomSeed = authoring.RandomSeed,
        });
        
        AddComponent<EnemySpawnTag>(entity);
    }
}
