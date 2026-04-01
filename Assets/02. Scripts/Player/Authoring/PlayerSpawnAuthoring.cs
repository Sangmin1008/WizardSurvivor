using Unity.Entities;
using UnityEngine;

class PlayerSpawnAuthoring : MonoBehaviour
{
    public GameObject PlayerPrefab;
}

class PlayerSpawnAuthoringBaker : Baker<PlayerSpawnAuthoring>
{
    public override void Bake(PlayerSpawnAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        
        AddComponent(entity, new PlayerSpawnComponent
        {
            PlayerPrefab = GetEntity(authoring.PlayerPrefab, TransformUsageFlags.Dynamic)
        });
    }
}
