using Unity.Entities;

public struct ProjectileSpawnComponent : IComponentData
{
    public Entity ProjectilePrefab;
    public float FireRate;
    public float NextFireTime;
    public int FireCount;
    public int Radius;
}
