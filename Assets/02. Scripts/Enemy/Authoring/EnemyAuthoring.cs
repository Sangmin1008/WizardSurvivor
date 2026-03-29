using Unity.Entities;
using UnityEngine;

class EnemyAuthoring : MonoBehaviour
{
    public float Speed = 2.0f;
}

class EnemyAuthoringBaker : Baker<EnemyAuthoring>
{
    public override void Bake(EnemyAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        
        AddComponent(entity, new EnemyComponent
        {
            Speed = authoring.Speed
        });
        AddComponent<EnemyTag>(entity);
    }
}
