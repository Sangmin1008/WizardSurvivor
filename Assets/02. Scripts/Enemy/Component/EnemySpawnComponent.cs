using Unity.Entities;

public struct EnemySpawnComponent : IComponentData
{
    public Entity Prefab;
    public int NumEnemies;
    public float MinSpeed;
    public float MaxSpeed;
    public float SpawnRadius;
    public uint RandomSeed;
}

public struct EnemySpawnTag : IComponentData {}