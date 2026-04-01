using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button retryButton;

    private EntityQuery _gameOverQuery;
    private LoadSceneSystem _loadSceneSystem;
    
    
    void Start()
    {
        gameOverUI.SetActive(false);
        
        _gameOverQuery = World.DefaultGameObjectInjectionWorld.EntityManager
            .CreateEntityQuery(typeof(GameOverTag));

        _loadSceneSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<LoadSceneSystem>();
    }

    private void OnEnable()
    {
        menuButton.onClick.AddListener(LoadMenuScene);
        retryButton.onClick.AddListener(ReloadScene);
    }

    private void OnDisable()
    {
        menuButton.onClick.RemoveListener(LoadMenuScene);
        retryButton.onClick.RemoveListener(ReloadScene);
    }

    private void LoadMenuScene()
    {
        _loadSceneSystem.LoadMenuScene();
    }

    private void ReloadScene()
    {
        gameOverUI.SetActive(false);
        _loadSceneSystem.ReloadScene();
    }

    void Update()
    {
        if (gameOverUI.activeSelf == false && !_gameOverQuery.IsEmpty)
        {
            gameOverUI.SetActive(true);
        }
    }
}
