using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class LoadSceneManager : MonoBehaviour
{
    [SerializeField] private Button startGameButton;
    private LoadSceneSystem _loadSceneSystem;

    private void Start()
    {
        _loadSceneSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<LoadSceneSystem>();
    }

    private void OnEnable()
    {
        startGameButton.onClick.AddListener(LoadGameScene);
    }

    private void OnDisable()
    {
        startGameButton.onClick.RemoveListener(LoadGameScene);
    }

    public void LoadGameScene()
    {
        _loadSceneSystem.LoadGameScene();
    }
}