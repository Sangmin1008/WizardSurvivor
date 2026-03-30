using Unity.Entities;

public struct EnemySpawnComponent : IComponentData
{
    public Entity Prefab;
    public int NumEnemies;
    public float MinSpeed;
    public float MaxSpeed;
    public float MinSpawnRadius;
    public float SpawnRadius;
    public float StoppingDistance;
    public uint RandomSeed;
}

public struct EnemySpawnTag : IComponentData {}