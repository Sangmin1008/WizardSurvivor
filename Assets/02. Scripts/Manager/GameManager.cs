using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button retryButton;

    private EntityQuery _gameOverQuery;
    
    
    void Start()
    {
        gameOverUI.SetActive(false);
        
        _gameOverQuery = World.DefaultGameObjectInjectionWorld.EntityManager
            .CreateEntityQuery(typeof(GameOverTag));
    }

    void Update()
    {
        if (gameOverUI.activeSelf == false && !_gameOverQuery.IsEmpty)
        {
            gameOverUI.SetActive(true);
        }
    }
}
