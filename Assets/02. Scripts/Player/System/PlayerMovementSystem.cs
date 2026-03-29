using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct PlayerMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // 시승템이 시작될 때 1회 호출
        // OnUpdate가 실행될 조건을 명시
        state.RequireForUpdate<PlayerTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // 입력 데이터 가져오기
        var inputData = SystemAPI.GetSingleton<PlayerInputComponent>();
        var deltaTime = SystemAPI.Time.DeltaTime;
        
        // 이동 중이 아니면 종료
        if (!inputData.IsMoving) return;
        
        // Player 엔티티, 컴포넌트 추출
        var playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
        
        // ECS LocalTransform 추출, 읽기 쓰기 모드
        var transform = SystemAPI.GetComponentRW<LocalTransform>(playerEntity);
        
        // PlayerComponent 추출, 읽기 모드
        var playerComponent = SystemAPI.GetComponentRO<PlayerComponent>(playerEntity);
        
        // 이동 방향벡터 계산
        float3 direction = new float3(inputData.Movement.x, inputData.Movement.y, 0f);
        
        // 이동 처리
        if (math.lengthsq(direction) > 0.01f)
            transform.ValueRW.Position += math.normalize(direction) * deltaTime * playerComponent.ValueRO.Speed;
        
        // Facing 처리
        // direction.x < 0 : 왼쪽 180, mathPI
        // direction.x > 0 : 오른쪽 0
        float yRotation = direction.x < 0 ? math.PI : 0f;
        if (math.abs(transform.ValueRO.Rotation.value.y - yRotation) > 0.01f)
            transform.ValueRW.Rotation = quaternion.RotateY(yRotation);
    }
}
