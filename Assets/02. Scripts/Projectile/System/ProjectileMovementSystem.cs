using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

partial struct ProjectileMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<ProjectileComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // EntityCommanderBuffer
        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        
        var deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (projectileComponent, velocity, entity) in 
                 SystemAPI.Query<RefRW<ProjectileComponent>, RefRW<PhysicsVelocity>>().WithEntityAccess())
        {
            velocity.ValueRW.Linear = projectileComponent.ValueRO.Direction * projectileComponent.ValueRO.MoveSpeed;
            velocity.ValueRW.Angular = float3.zero;
            
            // 생존 시간 체크
            projectileComponent.ValueRW.PassedTime += deltaTime;
            if (projectileComponent.ValueRO.PassedTime >= projectileComponent.ValueRO.LifeTime)
            {
                // 화살 삭제
                ecb.DestroyEntity(entity);
            }
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
