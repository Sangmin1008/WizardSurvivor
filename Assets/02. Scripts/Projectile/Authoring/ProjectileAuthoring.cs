using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class ProjectileAuthoring : MonoBehaviour
{
    public float MoveSpeed = 10f;
    public float Damage = 10f;
    public float LifeTime = 2f;
    public float PassedTime = 0f;
    public float3 Direction = new float3(0f, 1f, 0f);
}

class ProjectileAuthoringBaker : Baker<ProjectileAuthoring>
{
    public override void Bake(ProjectileAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        
        AddComponent(entity, new ProjectileComponent
        {
            MoveSpeed = authoring.MoveSpeed,
            Damage = authoring.Damage,
            LifeTime = authoring.LifeTime,
            PassedTime = authoring.PassedTime,
            Direction = authoring.Direction,
        });
    }
}
