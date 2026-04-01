using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

partial class CameraFollowSystem : SystemBase
{
    /* SystemBase 생명 주기 메서드
     * OnCreate()
     * OnStartRunning()
     * OnUpdate()
     * OnStopRunning()
     * OnDestroy()
     */

    private Transform _mainCamera;
    private Entity _playerEntity;
    private float _cameraZPos;

    [SerializeField] private float damping = 5f;

    protected override void OnCreate()
    {
        RequireForUpdate<PlayerTag>();

    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        _mainCamera = Camera.main?.transform;
        _cameraZPos = _mainCamera.position.z;
    }

    protected override void OnUpdate()
    {
        if (_mainCamera == null) return;
        
        // Player 엔티티 캐싱
        if (_playerEntity == Entity.Null)
        {
            _playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
        }
        
        // Player 위치를 추출
        var playerTransform = SystemAPI.GetComponent<LocalTransform>(_playerEntity);
        
        // 이동할 위치를 계산
        var targetPosition = new Vector3(playerTransform.Position.x, playerTransform.Position.y, _cameraZPos);
        
        // MainCamera 이동 처리
        _mainCamera.position = Vector3.Lerp(
            _mainCamera.position,
            targetPosition,
            SystemAPI.Time.DeltaTime * damping);
    }
}
