using Unity.Entities;
using UnityEngine;

class PlayerAuthoring : MonoBehaviour
{
    public float Speed = 5f;
    public float CurrentHp = 100f;
    public float MaxHp = 100f;

}

class PlayerAuthoringBaker : Baker<PlayerAuthoring>
{
    public override void Bake(PlayerAuthoring authoring)
    {
        // Player 엔티티를 가져오기
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        
        // 엔티티에 PlayerComponent, PlayerTag 추가
        AddComponent(entity, new PlayerComponent()
        {
            Speed = authoring.Speed,
            CurrentHp = authoring.CurrentHp,
            MaxHp = authoring.MaxHp
        });
        
        AddComponent<PlayerTag>();
    }
}
