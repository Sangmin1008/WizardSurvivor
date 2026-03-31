using Unity.Entities;
using UnityEngine;

class ProjectileSpawnAuthoring : MonoBehaviour
{
    public GameObject ProjectilePrefab;
    public float FireRate = 0.5f;
    public int FireCount = 10;
    public int Radius = 3;
}

class ProjectileSpawnAuthoringBaker : Baker<ProjectileSpawnAuthoring>
{
    public override void Bake(ProjectileSpawnAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        
        AddComponent(entity, new ProjectileSpawnComponent 
        {
            ProjectilePrefab = GetEntity(authoring.ProjectilePrefab, TransformUsageFlags.Dynamic),
            FireRate = authoring.FireRate,
            FireCount = authoring.FireCount,
            Radius = authoring.Radius,
            NextFireTime = 0f,
        });
    }
}
