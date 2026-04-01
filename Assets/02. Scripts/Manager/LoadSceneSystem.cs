using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

partial class LoadSceneSystem : SystemBase
{
    protected override void OnUpdate()
    {

    }

    public void LoadMenuScene()
    {
        CleanupRuntimeEntities();
        SceneManager.LoadScene("01. Scenes/MainMenu");
    }

    public void LoadGameScene()
    {
        ResetSystem();
        SceneManager.LoadScene("Game");
    }

    public void ReloadScene()
    {
        CleanupRuntimeEntities();
        ResetSystem();
        
    }

    private void CleanupRuntimeEntities()
    {
        // 엔티티 매니저 추출
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        
        // 1. Player 엔티티 (LinkedEntityGroup 포함)
        var playerQuery = entityManager.CreateEntityQuery(typeof(PlayerTag));
        if (!playerQuery.IsEmpty)
        {
            var playerEntity = playerQuery.GetSingletonEntity();
            entityManager.DestroyEntity(playerEntity);
        }
        playerQuery.Dispose();
        
        // 2. Enemy 엔티티 삭제 (LinkedEntityGroup 포함)
        var enemyQuery = entityManager.CreateEntityQuery(typeof(EnemyTag));
        if (!enemyQuery.IsEmpty)
        {
            var enemyEntities = enemyQuery.ToEntityArray(Allocator.Temp);
            foreach (var entity in enemyEntities)
            {
                entityManager.DestroyEntity(entity);
            }

            enemyEntities.Dispose();
        }
        enemyQuery.Dispose();
        
        // 3. Projectile 엔티티 삭제
        var projectileQuery = entityManager.CreateEntityQuery(typeof(ProjectileComponent));
        if (!projectileQuery.IsEmpty)
        {
            entityManager.DestroyEntity(projectileQuery);
        }
        projectileQuery.Dispose();
        
        // 4. GameOver 엔티티 삭제
        var gameOverQuery = entityManager.CreateEntityQuery(typeof(GameOverTag));
        if (!gameOverQuery.IsEmpty)
        {
            entityManager.DestroyEntity(gameOverQuery);
        }
        gameOverQuery.Dispose();
    }
    

    private void ResetSystem()
    {
        var world = World.DefaultGameObjectInjectionWorld;
        if (world == null) return;
        
        // EnemySpawnSystem 재활성화
        var enemySpawnSystemHandle = world.GetExistingSystem<EnemySpawnSystem>();
        if (enemySpawnSystemHandle != default)
        {
            world.Unmanaged.ResolveSystemStateRef(enemySpawnSystemHandle).Enabled = true;
        }
        
        // PlayerSpawnSystem 재활성화
        var playerSpawnSystemHandle = world.GetExistingSystem<PlayerSpawnSystem>();
        if (playerSpawnSystemHandle != default)
        {
            world.Unmanaged.ResolveSystemStateRef(playerSpawnSystemHandle).Enabled = true;
        }
    }

}
