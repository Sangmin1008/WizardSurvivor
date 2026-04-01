using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;


[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
partial struct PlayerDamageSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();
        state.RequireForUpdate<EnemyTag>();
        state.RequireForUpdate<SimulationSingleton>();
        state.RequireForUpdate<PlayerHpBarComponent>();
        state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        
        // Player 엔티티 추출
        var playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
        
        // Player HpBar 엔티티 추출
        var playerHpBarEntity = SystemAPI.GetSingletonEntity<PlayerHpBarComponent>();
        
        // PlayerComponent Lookup
        var playerComponentLookup = SystemAPI.GetComponentLookup<PlayerComponent>(false);
        
        // Enemy Lookup
        var enemyLookup = SystemAPI.GetComponentLookup<EnemyTag>(true);
        
        // EnemyComponent Lookup
        var enemyComponentLookup = SystemAPI.GetComponentLookup<EnemyComponent>(true);
        
        state.Dependency = new PlayerDamageJob
        {
            PlayerEntity = playerEntity,
            PlayerHpBarEntity = playerHpBarEntity,
            PlayerComponentLookup = playerComponentLookup,
            EnemyLookup = enemyLookup,
            EnemyComponentLookup = enemyComponentLookup,
            ECB = ecb.AsParallelWriter()
        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }
    [BurstCompile]
    struct PlayerDamageJob : ITriggerEventsJob
    {
        public Entity PlayerEntity;
        public Entity PlayerHpBarEntity;

        [ReadOnly] public ComponentLookup<EnemyTag> EnemyLookup;
        [ReadOnly] public ComponentLookup<EnemyComponent> EnemyComponentLookup;

        public ComponentLookup<PlayerComponent> PlayerComponentLookup;

        public EntityCommandBuffer.ParallelWriter ECB;

        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;
            
            bool isAPlayer = entityA == PlayerEntity;
            bool isBPlayer = entityB == PlayerEntity;
            
            bool isAEnemy = EnemyLookup.HasComponent(entityA);
            bool isBEnemy = EnemyLookup.HasComponent(entityB);
            
            // Player와 Enemy간의 충돌 확인
            if (isAPlayer && isBEnemy)
            {
                // 충돌 처리
                ProcessDamage(PlayerEntity, entityB);
            } else if (isBPlayer && isAEnemy)
            {
                // 충돌 처리
                ProcessDamage(PlayerEntity, entityA);
            }
        }

        void ProcessDamage(Entity player, Entity enemy)
        {
            var playerComponent = PlayerComponentLookup[player];
            var enemyComponent = EnemyComponentLookup[enemy];

            playerComponent.CurrentHp -= enemyComponent.Damage;

            if (playerComponent.CurrentHp <= 0)
            {
                // Player HpBar 삭제
                ECB.DestroyEntity(PlayerHpBarEntity.Index, PlayerHpBarEntity);
                
                //Player Entity 삭제
                ECB.DestroyEntity(PlayerEntity.Index, PlayerEntity);
                
                // GameOverTag Entity 생성
                Entity gameOverEntity = ECB.CreateEntity(0);
                ECB.AddComponent<GameOverTag>(0, gameOverEntity);
            }
            else
            {
                PlayerComponentLookup[player] = playerComponent;
            }
        }
    }
}

public struct GameOverTag : IComponentData {}
