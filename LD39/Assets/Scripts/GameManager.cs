using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;

    private Player player;

    public float playerHealth;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        playerHealth = 10;

        // Ensure the instance is of the type GameManager
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
        }


        // Persist the GameManager instance across scenes
        DontDestroyOnLoad(gameObject);

        player.OnDeath += GameOver;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }

    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void GameOver()
    {
        enabled = false;
    }
}