using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct EnemyMovementSystem : ISystem
{
    // Player Entity 캐싱
    private Entity _playerEntity;
    
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnemyTag>();
        state.RequireForUpdate<PlayerTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (_playerEntity == Entity.Null || !SystemAPI.Exists(_playerEntity))
        {
            // Player 엔티티 추출
            _playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
        }

        // Player 엔티티의 Position 추출
        var playerPosition = SystemAPI.GetComponent<LocalTransform>(_playerEntity).Position;
        
        // Job 병렬처리
        state.Dependency = new EnemyMovementJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            PlayerPosition = playerPosition,
        }.ScheduleParallel(state.Dependency); 

        // Player 엔티티의 Transform
        // var playerTransform = SystemAPI.GetComponentRO<LocalTransform>(_playerEntity);
        //
        // var deltaTime = SystemAPI.Time.DeltaTime;
        //
        // foreach (var (enemyTransform, enemyComponent) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<EnemyComponent>>())
        // {
        //     // 이동 방향 계산
        //     float3 direction = playerTransform.ValueRO.Position - enemyTransform.ValueRO.Position;
        //     
        //     // 위치 변경
        //     if (math.lengthsq(direction) > 0.01f)
        //         enemyTransform.ValueRW.Position += math.normalize(direction) * deltaTime * enemyComponent.ValueRO.Speed;
        //
        //     // Facing 처리
        //     float yRotation = direction.x < 0 ? math.PI : 0f;
        //     if (math.abs(enemyTransform.ValueRO.Rotation.value.y - yRotation) > 0.01f)
        //     {
        //         enemyTransform.ValueRW.Rotation = quaternion.RotateY(yRotation);
        //     }
        // }
    }
}

// Enemy 이동 처리 Job
// IJobEntity : 엔티티를 병렬처리하는 고성능 Job

[BurstCompile]
public partial struct EnemyMovementJob : IJobEntity
{
    public float DeltaTime;
    public float3 PlayerPosition;
    void Execute(ref EnemyComponent enemyComponent, ref LocalTransform transform, Entity entity)
    {
        // 먼 Enemy의 업데이트 빈도를 (50%) 감소
        float distSq = math.distancesq(transform.Position, PlayerPosition);
        if (distSq > 100f && entity.Index % 2 == 0) return;
        
        // Player 방향의 벡터를 계산
        var direction = math.normalize(PlayerPosition - transform.Position);
        
        // 이동 처리
        transform.Position += direction * DeltaTime * enemyComponent.Speed;
        
        // Facing 처리
        float yRotation = direction.x < 0 ? math.PI : 0f;
        if (math.abs(transform.Rotation.value.y - yRotation) > 0.01f)
        {
            transform.Rotation = quaternion.RotateY(yRotation);
        }
    }
}