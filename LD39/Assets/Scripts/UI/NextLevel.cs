using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NextLevel : MonoBehaviour {

    private Slider hpGuage;
    private Slider powerGuage;
    private Text cellCount;

    private Slider corePower;

    private void Awake()
    {
        hpGuage = GameObject.FindGameObjectWithTag("HPGuage")?.GetComponent<Slider>();
        powerGuage = GameObject.FindGameObjectWithTag("PPGuage")?.GetComponent<Slider>();
        cellCount = GameObject.FindGameObjectWithTag("CellCount")?.GetComponent<Text>();
        corePower = GameObject.FindGameObjectWithTag("CoreGuage")?.GetComponent<Slider>();
    }

    private void Update()
    {
        if (hpGuage)
            hpGuage.value = GameManager.instance.playerHealth;

        if (powerGuage)
            powerGuage.value = GameManager.instance.playerPower;

        if (cellCount)
            cellCount.text = "" + GameManager.instance.playerCells;

        if (corePower)
            corePower.value = GameManager.instance.corePower;
    }

    public void AddHP()
    {
        if (GameManager.instance.playerHealth < 10)
        {
            GameManager.instance.playerHealth += 1;
        }

        if (GameManager.instance.playerCells >= 5)
        {
            GameManager.instance.playerCells -= 5;
        }
        
        
    }

    public void AddPP()
    {
        if (GameManager.instance.playerPower <= 90)
        {
            GameManager.instance.playerPower += 10;
        }

        if (GameManager.instance.playerCells >= 5)
        {
            GameManager.instance.playerCells -= 5;
        }
    }

    public void AddCP()
    {
        if (GameManager.instance.corePower <= 900)
        {
            GameManager.instance.corePower += 100;
        }

        if (GameManager.instance.playerCells >= 5)
        {
            GameManager.instance.playerCells -= 5;
        }
    }

    public void LevelTwo()
    {
        SceneManager.LoadScene("Level2");
    }

    public void NewGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
