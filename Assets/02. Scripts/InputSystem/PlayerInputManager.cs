using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public struct PlayerInputComponent : IComponentData
{
    public float2 Movement;
    public bool IsMoving;
}

// GameObject World <=> ECS World 브릿지 역할 (Hybrid ECS)
public class PlayerInputManager : MonoBehaviour
{
    private EntityManager _entityManager;
    private Entity _playerInputEntity;
    
    // InputSystem Action 참조
    private InputSystem_Actions _inputSystem;
    private InputAction _moveAction;

    private void Awake()
    {
        _inputSystem = new InputSystem_Actions();
        _moveAction = _inputSystem.Player.Move;
        _moveAction.Enable();
    }

    private void Start()
    {
        // ECS World의 EntityManager
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        
        // 싱글톤 Entity 생성
        // _playerInputEntity = _entityManager.CreateEntity();
        
        // 컴포넌트 추가
        // _entityManager.AddComponentData(_playerInputEntity, new PlayerInputComponent());
        
        var query = _entityManager.CreateEntityQuery(typeof(PlayerInputComponent));
        if (!query.IsEmpty)
        {
            _playerInputEntity = query.GetSingletonEntity();
        }
        else
        {
            _playerInputEntity = _entityManager.CreateEntity(typeof(PlayerInputComponent));
        }
        query.Dispose();
    }

    private void OnEnable()
    {
        _moveAction.performed += OnMove;
        _moveAction.canceled += OnMove;
    }

    private void OnDisable()
    {
        _moveAction.performed -= OnMove;
        _moveAction.canceled -= OnMove;
        _moveAction.Dispose();
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        float h = ctx.ReadValue<Vector2>().x;
        float v = ctx.ReadValue<Vector2>().y;
        
        _entityManager.SetComponentData(_playerInputEntity, new PlayerInputComponent
        {
            Movement = new float2(h, v),
            IsMoving = ctx.phase == InputActionPhase.Performed
        });
    }
}
