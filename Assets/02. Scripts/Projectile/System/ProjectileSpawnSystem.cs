using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.SocialPlatforms;

partial struct ProjectileSpawnSystem : ISystem
{
    // Player 엔티티 캐싱
    private Entity _playerEntity;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();
        state.RequireForUpdate<ProjectileSpawnComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (_playerEntity == Entity.Null || !SystemAPI.Exists(_playerEntity))
        {
            _playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
        }
        
        // ProjectileSpawnComponent 추출
        ref var spawnComponent = ref SystemAPI.GetSingletonRW<ProjectileSpawnComponent>().ValueRW;
        
        // 현재 시간 가져오기
        var currentTime = SystemAPI.Time.ElapsedTime;
        
        // 발사 주기 체크
        if (currentTime >= spawnComponent.NextFireTime)
        {
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
            
            // 다음 발사시간 기록
            spawnComponent.NextFireTime = (float)currentTime + spawnComponent.FireRate;
            
            // 발사 각도 계산 (라디안)
            var angleStep = math.PI * 2f / spawnComponent.FireCount;
            
            // Player의 위치 가쟈오기
            var playerTransform = SystemAPI.GetComponent<LocalTransform>(_playerEntity);
            
            // Projectile Prefab 추출
            var prefabData = SystemAPI.GetComponent<ProjectileComponent>(spawnComponent.ProjectilePrefab);
            
            // 발사 횟수 만큼 반복해서 Projectile 생성
            for (int i = 0; i < spawnComponent.FireCount; i++)
            {
                // i번째 발사 각도 계산
                var angle = angleStep * i;
                
                // 발사 좌표 계산 (x, y) = (cos, sin)
                float3 direction = new float3(math.cos(angle), math.sin(angle), 0f);
                
                // 주인공 위치를 기준으로 발사 위치 계산 (주인공 위치 + 발사 좌표 * 최소발사반경)
                float3 spawnPosition = playerTransform.Position + direction * spawnComponent.Radius;
                
                // 발사 엔티티 생성
                Entity projectileEntity = ecb.Instantiate(spawnComponent.ProjectilePrefab);
                
                // 발사체 각도 계산
                quaternion projectileRotation = quaternion.RotateZ(angle - math.PIHALF);
                
                // 발사체 엔티티의 위치, 각도, 스케일 설정
                ecb.SetComponent(projectileEntity, new LocalTransform
                {
                    Position = spawnPosition,
                    Rotation = projectileRotation,
                    Scale = 1f,
                });
                
                // 발사체 데이터 설정
                prefabData.Direction = direction;
                prefabData.PassedTime = 0f;
                ecb.SetComponent(projectileEntity, prefabData);
                // ecb.SetComponent(projectileEntity, new ProjectileComponent
                // {
                //     MoveSpeed = 10f,
                //     Damage = 10f,
                //     LifeTime = 2f,
                //     PassedTime = 0f,
                //     Direction = direction,
                // });
            }
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
