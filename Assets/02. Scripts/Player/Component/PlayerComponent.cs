using Unity.Entities;

public struct PlayerComponent : IComponentData
{
    public float Speed;
    public float CurrentHp;
    public float MaxHp;
}

// 태그 컴포넌트: 데이터가 없는 컴포넌트
public struct PlayerTag : IComponentData {}