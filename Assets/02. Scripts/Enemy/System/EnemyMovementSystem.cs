using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct EnemyMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnemyTag>();
        state.RequireForUpdate<PlayerTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Player 엔티티 추출
        var playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
        
        // Player 엔티티의 Transform
        var playerTransform = SystemAPI.GetComponentRO<LocalTransform>(playerEntity);
        
        var deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (enemyTransform, enemyComponent) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<EnemyComponent>>())
        {
            // 이동 방향 계산
            float3 direction = playerTransform.ValueRO.Position - enemyTransform.ValueRO.Position;
            
            // 위치 변경
            if (math.lengthsq(direction) > 0.01f)
                enemyTransform.ValueRW.Position += math.normalize(direction) * deltaTime * enemyComponent.ValueRO.Speed;

            // Facing 처리
            float yRotation = direction.x < 0 ? math.PI : 0f;
            if (math.abs(enemyTransform.ValueRO.Rotation.value.y - yRotation) > 0.01f)
            {
                enemyTransform.ValueRW.Rotation = quaternion.RotateY(yRotation);
            }
        }
    }
}
