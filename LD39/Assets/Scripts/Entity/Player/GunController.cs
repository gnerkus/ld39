using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform projectileSpawn;
    public Projectile bulletType;
    public float msBetweenShots;
    public float bulletSpeed;

    float nextShotTime;

    private void Start()
    {

    }

    public void Shoot()
    {
        if (Time.time > nextShotTime)
        {
            nextShotTime = Time.time + msBetweenShots / 1000;
            Projectile newProjectile = Instantiate(bulletType, projectileSpawn.position, projectileSpawn.rotation);
            newProjectile.SetSpeed(bulletSpeed);
            newProjectile.SetDamage(5);
        }
    }
}