using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct PlayerSpawnSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerSpawnComponent>();
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        var playerSpawnData = SystemAPI.GetSingleton<PlayerSpawnComponent>();
        
        var playerEntity = ecb.Instantiate(playerSpawnData.PlayerPrefab);
        
        ecb.SetComponent(playerEntity, LocalTransform.FromPositionRotationScale(
            new float3(0f, 0f, -1f),
            quaternion.identity, 
            1.5f
            ));

        state.Enabled = false;
    }
}
