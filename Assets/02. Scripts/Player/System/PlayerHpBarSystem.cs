using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
partial struct PlayerHpBarSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerHpBarComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // HpBar 엔티티 추출
        var hpBarEntity = SystemAPI.GetSingletonEntity<PlayerHpBarComponent>();
        
        // HpBar의 Parent 추출
        var parent = SystemAPI.GetComponent<Parent>(hpBarEntity);
        
        // Parent의 PlayerComponent 추출
        var playerComponent = SystemAPI.GetComponent<PlayerComponent>(parent.Value);

        // HpBar의 FillAmount 값을 수정하기 위해서 RW로 추출
        var playerHpBarComponent = SystemAPI.GetSingletonRW<PlayerHpBarComponent>();
        
        playerHpBarComponent.ValueRW.FillAmount = playerComponent.MaxHp > 0f ? playerComponent.CurrentHp / playerComponent.MaxHp : 0f;
    }
}
