using Unity.Entities;
using Unity.Mathematics;

public struct ProjectileComponent : IComponentData
{
    public float MoveSpeed;
    public float Damage;
    public float LifeTime;
    public float PassedTime;
    public float3 Direction;
}
