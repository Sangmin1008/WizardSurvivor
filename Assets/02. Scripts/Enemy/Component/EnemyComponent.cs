using Unity.Entities;
using Unity.Rendering;

public struct EnemyComponent : IComponentData
{
    public float Speed;
    public float StoppingDistance;
    public float CurrentHp;
    public float MaxHp;
    public float Damage;
}

public struct EnemyTag : IComponentData {}

// EnemySpawn 시간 오프셋 값을 변경하는 컴포넌트
// Shader _SpawnTime 자동으로 동기화

[MaterialProperty("_SpawnTime")]
public struct EnemySpawnTime : IComponentData
{
    public float Value;
}