using UnityEngine;

public class LightstickController : MonoBehaviour
{

    public Transform spawner;
    public GlowingObject lightstick;

    private void Start()
    {
        
    }

    public void DropLight()
    {
        GlowingObject stick = Instantiate(lightstick, spawner.position, Quaternion.identity);
    }
}