using Unity.Entities;
using UnityEngine;

class EnemyAuthoring : MonoBehaviour
{
    public float Speed = 2.0f;
    public float Damage = 0.1f;
}

class EnemyAuthoringBaker : Baker<EnemyAuthoring>
{
    public override void Bake(EnemyAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        
        AddComponent(entity, new EnemyComponent
        {
            Speed = authoring.Speed,
            Damage = authoring.Damage,
        });
        AddComponent<EnemyTag>(entity);
    }
}
