using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;

    Player player;

    public float playerHealth;
    public float playerPower;
    public float playerCells;

    private float corePower;

    private void Awake()
    {
        corePower = 1000;

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        playerHealth = 10;
        playerPower = 100;
        playerCells = 0;

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

        StartCoroutine(DrainPower());
    }

    IEnumerator DrainPower()
    {
        for (; ; )
        {
            // execute block of code here
            corePower -= 1;
            yield return new WaitForSeconds(1f);
        }
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

    // Store Player stats for the next level
    public void NewLevel()
    {
        // compute new health as fraction of xp
        float currentHealth = player.GetHealth();
        float currentPower = player.GetPower();
        float currentCells = player.GetCells();

        // store player stats
        playerHealth = currentHealth;
        playerPower = currentPower;
        playerCells = currentCells;
    }

    void GameOver()
    {
        enabled = false;
    }

    public Player GetPlayer()
    {
        return player;
    }
}