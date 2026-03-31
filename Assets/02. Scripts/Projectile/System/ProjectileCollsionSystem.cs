using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

// ITriggerEventsJob, ICollisionEventsJob을 사용해야 함

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))] // 물리 업데이트 그룹에 포함 (FixedUpdate 주기)
[UpdateAfter(typeof(PhysicsSystemGroup))]
partial struct ProjectileCollsionSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob); // 수동으로 해제 해야함

        var projectileLookup = SystemAPI.GetComponentLookup<ProjectileComponent>(true);
        var enemyLookup = SystemAPI.GetComponentLookup<EnemyTag>(true);
        var enemyComponentLookup = SystemAPI.GetComponentLookup<EnemyComponent>(false);
        
        projectileLookup.Update(ref state);
        enemyLookup.Update(ref state);
        enemyComponentLookup.Update(ref state);
        
        // Job 스케줄링
        state.Dependency = new ProjectileCollisionJob
        {
            ProjectileLookup = projectileLookup,
            EnemyLookup = enemyLookup,
            EnemyComponentLookup = enemyComponentLookup,
            
            ECB = ecb.AsParallelWriter()
        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
        
        // Job 완료 대기
        state.Dependency.Complete();
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
    
    [BurstCompile]
    public struct ProjectileCollisionJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentLookup<ProjectileComponent> ProjectileLookup;
        [ReadOnly] public ComponentLookup<EnemyTag> EnemyLookup;
        public ComponentLookup<EnemyComponent> EnemyComponentLookup;

        public EntityCommandBuffer.ParallelWriter ECB;
        public void Execute(TriggerEvent triggerEvent)
        {
            // TriggerEvent
            // - EntityA, EntityB : 충돌한 엔티티
            // - BodyIndexA, BodyIndexB : PhysicsWorld
            // - ColliderKeyA, ColliderKeyB : 충돌한 Collider 식별자
            
            // 충돌한 엔티티 추출
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;
            
            bool isAProjectile = ProjectileLookup.HasComponent(entityA);
            bool isBEnemy = EnemyLookup.HasComponent(entityB);
            
            bool isBProjectile = ProjectileLookup.HasComponent(entityB);
            bool isAEnemy = EnemyLookup.HasComponent(entityA);

            if (isAProjectile && isBEnemy)
            {
                // 충돌 체크
                ProcessCollision(entityA, entityB);
            } else if (isBProjectile && isAEnemy)
            {
                // 충돌 체크
                ProcessCollision(entityB, entityA);
            }
            
        }
        
        void ProcessCollision(Entity projectile, Entity enemy)
        {
            // Enemy 체력 감소
            var enemyData = EnemyComponentLookup[enemy];
            var projectileData = ProjectileLookup[projectile];
            
            enemyData.CurrentHp -= projectileData.Damage;
            
            // 체력이 0 이하이면 적 제거
            if (enemyData.CurrentHp <= 0)
            {
                ECB.DestroyEntity(enemy.Index, enemy);
            }
            else
            {
                EnemyComponentLookup[enemy] = enemyData;
            }
            
            ECB.DestroyEntity(projectile.Index, projectile);
        }
    }
}
