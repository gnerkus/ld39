using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;

    private Slider hpGuage;
    private Slider powerGuage;
    private Text cellCount;

    Player player;

    public float playerHealth;
    public float playerPower;
    public float playerCells;

    public float corePower;

    private void Awake()
    {
        corePower = 1000;

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        hpGuage = GameObject.FindGameObjectWithTag("HPGuage").GetComponent<Slider>();
        powerGuage = GameObject.FindGameObjectWithTag("PPGuage").GetComponent<Slider>();
        cellCount = GameObject.FindGameObjectWithTag("CellCount").GetComponent<Text>();

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

    private void Update()
    {
        hpGuage.value = player.GetHealth();
        powerGuage.value = player.GetPower();
        cellCount.text = "" + player.GetCells();

        if (corePower <= 0)
        {
            instance.GameOver();
        }
    }

    IEnumerator DrainPower()
    {
        for (; ; )
        {
            Scene currentScene = SceneManager.GetActiveScene();

            if (currentScene.name == "Game")
            {
                corePower -= 20;
            }
            
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

        SceneManager.LoadScene("Loading");
    }

    void GameOver()
    {
        enabled = false;
        SceneManager.LoadScene("GameOver");
    }

    public Player GetPlayer()
    {
        return player;
    }
}