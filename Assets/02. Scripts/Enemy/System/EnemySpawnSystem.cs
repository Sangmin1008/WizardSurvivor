using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct EnemySpawnSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();
        state.RequireForUpdate<EnemySpawnTag>();
        state.RequireForUpdate<EnemySpawnComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // 한 번만 실행하도록
        state.Enabled = false;
        
        // EntityCommandBuffer 생성
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var enemySpawnComponent = SystemAPI.GetSingleton<EnemySpawnComponent>();
        
        // 난수 생성기 초기화
        var random = new Random(enemySpawnComponent.RandomSeed);

        using (var enemies = state.EntityManager.Instantiate(
                   enemySpawnComponent.Prefab,
                   enemySpawnComponent.NumEnemies,
                   state.WorldUpdateAllocator))
        {
            // 생성된 Enemy에 대한 초기화
            for (int i = 0; i < enemies.Length; i++)
            {
                var enemyEntity = enemies[i];
                
                // 랜덤 위치 설정
                var angle = random.NextFloat(0f, math.PI * 2f);
                var distance = random.NextFloat(10f, enemySpawnComponent.SpawnRadius);
                var position = new float3(
                    math.cos(angle) * distance,
                    math.sin(angle) * distance,
                    0f);
                
                // Enemt에 컴포넌트 데이터 추가
                var enemyData = new EnemyComponent
                {
                    Speed = random.NextFloat(enemySpawnComponent.MaxSpeed, enemySpawnComponent.MaxSpeed)
                };
                
                ecb.AddComponent(enemyEntity, enemyData);
                
                ecb.SetComponent(enemyEntity, LocalTransform.FromPositionRotationScale(
                    position, quaternion.identity, 2.0f));
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}
