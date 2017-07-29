using UnityEngine;

public class Loader : MonoBehaviour
{

    public GameObject gameManager;
    public GameObject eventManager;

    private void Awake()
    {
        if (GameManager.instance == null)
            Instantiate(gameManager);

        if (EventManager.instance == null)
            Instantiate(eventManager);
    }
}