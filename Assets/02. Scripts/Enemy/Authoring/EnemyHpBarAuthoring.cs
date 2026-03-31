using Unity.Entities;
using UnityEngine;

class EnemyHpBarAuthoring : MonoBehaviour
{
    public float FillAmount = 1f;
}

class EnemyHpBarAuthoringBaker : Baker<EnemyHpBarAuthoring>
{
    public override void Bake(EnemyHpBarAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        
        AddComponent(entity, new EnemyHpBarComponent
        {
            FillAmount = authoring.FillAmount,
        });
    }
}
