using UnityEngine;

[RequireComponent(typeof(Light))]
public class GlowingObject : LivingEntity {

    Light lightSource;

    private void Awake()
    {
        lightSource = transform.Find("Point light").GetComponent<Light>();
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();
	}

    public void ToggleLight()
    {
        lightSource.enabled = !lightSource.enabled;
    }

    public float Intensity
    {
        get
        {
            return lightSource.intensity;
        }

        set
        {
            lightSource.intensity = value;
        }
    }
	
    public bool isLightSourceEnabled()
    {
        return lightSource.enabled;
    }

	// Update is called once per frame
	void Update () {

	}
}
