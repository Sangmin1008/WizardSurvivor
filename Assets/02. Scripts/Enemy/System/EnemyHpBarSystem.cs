using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

partial struct EnemyHpBarSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnemyHpBarComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var enemyLookup = SystemAPI.GetComponentLookup<EnemyComponent>(true);
        enemyLookup.Update(ref state);
        
        // Job 스케줄링
        state.Dependency = new HpBarUpdateJob
        {
            EnemyLookup = enemyLookup
        }.ScheduleParallel(state.Dependency);
    }
}

[BurstCompile]
partial struct HpBarUpdateJob : IJobEntity
{
    [ReadOnly] public ComponentLookup<EnemyComponent> EnemyLookup;
    public void Execute(ref EnemyHpBarComponent hpBar, in Parent parent)
    {
        if (EnemyLookup.HasComponent(parent.Value))
        {
            var enemy = EnemyLookup[parent.Value];
            hpBar.FillAmount = enemy.MaxHp > 0f ? enemy.CurrentHp / enemy.MaxHp : 0f;
        }
    }
}