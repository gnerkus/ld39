using UnityEngine;

[RequireComponent(typeof(Light))]
public class GlowingObject : LivingEntity {

    Light lightSource;

	// Use this for initialization
	void Start () {
        base.Start();
        lightSource = transform.Find("Point light").GetComponent<Light>();
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
	
	// Update is called once per frame
	void Update () {

	}
}
