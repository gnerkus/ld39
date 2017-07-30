using System.Collections;
using UnityEngine;

public class Trap : MonoBehaviour {

    public LayerMask collisionLayer;
    // This allows us to create projectiles that don't move forward.
    float trapSpeed = 10;

    float trapDamage = 25; // TODO: this should be configurable for different projectiles

    float startingPosition = 7f;

    // Use this for initialization
    void Start () {
        StartCoroutine(Drop());
    }
	
	// Update is called once per frame
	void Update () {
    }

    IEnumerator Drop()
    {
        for (; ; )
        {
            if (transform.position.y < startingPosition)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, startingPosition, transform.position.z), 1);
            } else
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, 3f, transform.position.z), 1);
            }
            
            // execute block of code here
            yield return new WaitForSeconds(3f);
        }
    }

    // Handle a successful collision
    void OnCollisionEnter(Collision collider)
    {
        // Get a reference to the hit object's IDamageable
        IDamageable damageableObject = collider.gameObject.GetComponent<IDamageable>();

        if (damageableObject != null)
        {
            damageableObject.TakeHit(trapDamage, collider.transform.position, transform.forward);
        }
    }
}
