using Unity.Entities;
using UnityEngine;

class PlayerHpBarAuthoring : MonoBehaviour
{
    public float FillAmount = 1f;
}

class PlayerHpBarAuthoringBaker : Baker<PlayerHpBarAuthoring>
{
    public override void Bake(PlayerHpBarAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        
        AddComponent(entity, new PlayerHpBarComponent
        {
            FillAmount = authoring.FillAmount
        });
    }
}
