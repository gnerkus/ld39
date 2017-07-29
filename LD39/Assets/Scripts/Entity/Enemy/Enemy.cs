using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{

    public float rangeOfSight; // how far the Enemy can see
    public float rangeOfAttack; // how far the Enemy can attack
    public LayerMask viewMask; // layer on which obstacles to sight exist
    public LayerMask lightScanMask; // layer to scan for lightsticks
    public Transform attackSpawn;
    public Projectile attackType;

    public float msBetweenAttacks;
    float nextAttackTime;
    float attackSpeed = 2f;
    float attackLifeTime = .2f;

    float currentIntensity = 0f;

    enum State { Idle, Chasing, Attacking };
    State currentState;

    private NavMeshAgent navMeshAgent;

    Transform target;
    Player player;
    LivingEntity targetEntity;
    bool hasTarget;

    float collisionRadius;
    float targetCollisionRadius;

    Color flashColor = Color.white;
    float flashDuration = .15f;
    Material skinMaterial;
    Color originalColor;

    int itemCount = 1;
    int exp = 2;

    System.Random prng = new System.Random();

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        skinMaterial = GetComponent<Renderer>().material;
    }

    protected override void Start()
    {
        base.Start();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        // Each enemy sets the Player as its initial target
        if (player != null)
        {
            hasTarget = true;

            // Set the Player as a target
            // Get a reference to the Player's lamp
            target = player.lamp.transform;
            currentIntensity = player.lamp.Intensity;
            targetEntity = player.GetComponent<LivingEntity>();

            // The Player and Enemy should collide on their surfaces and not their centres.
            collisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = player.GetComponent<CapsuleCollider>().radius;

            currentState = State.Idle;

            // Listen for the Player's death event
            targetEntity.OnDeath += OnTargetDeath;

            // Cast a ray from the Enemy to the Player
            StartCoroutine(UpdatePath());
        }

        EventManager.instance.StartListening("NewGlowingObject", GetGlowingObject);
    }

    void OnEnable()
    {
        
    }

    void OnDisable()
    {
        EventManager.instance.StopListening("NewGlowingObject", GetGlowingObject);
    }

    void GetGlowingObject(GlowingObject glowingObject)
    {
        // Get the glowingObject's intensity
        float goIntensity = glowingObject.Intensity;

        if (CanSeeTarget(glowingObject.transform))
        {
            if (goIntensity >= currentIntensity)
            {
                target = glowingObject.transform;
                hasTarget = true;
                currentIntensity = goIntensity;
                glowingObject.OnDeath += OnLightstickDeath;
            }
        }
    }

    private void Update()
    {
        if (hasTarget)
        {
            if (CanSeeTarget(target))
            {
                transform.LookAt(new Vector3(target.position.x, 1, target.position.z));
                if (CanAttackTarget())
                {
                    Attack();
                }
            }
        }
    }

    // check if the Enemy can see the Player
    bool CanSeeTarget(Transform sightTarget)
    {
        if (sightTarget != null)
        {
            // 1. Check the distance between the Enemy and the Player
            if (Vector3.Distance(transform.position, sightTarget.position) < rangeOfSight)
            {
                // 2. Cast a ray from the Enemy to the Player. If it doesn't hit anything, the Enemy can see the Player
                if (!Physics.Linecast(transform.position, sightTarget.position, viewMask))
                {
                    // 3. Check if the Player's light source is enabled
                    Light targetLight = sightTarget.Find("Point light").GetComponent<Light>();
                    if (targetLight.enabled)
                    {
                        return true;
                    }

                }
            }
        }

        return false;
    }

    // Enemies can attack you in the dark if you're close enough
    // check if the Enemy can attack the Player
    bool CanAttackTarget()
    {
        if (target != null)
        {
            // 1. Check the distance between the Enemy and the Player
            if (Vector3.Distance(transform.position, target.position) < rangeOfAttack)
            {
                return true;
            }
        }
        
        return false;
    }

    // Calculate a new path to the target four times a second.
    // This is more performant than calculating the path every frame
    // (if this was in the update method).
    IEnumerator UpdatePath()
    {
        float refreshRate = .25f;

        while (hasTarget)
        {
            if (CanSeeTarget(target))
            {
                // When a LivingEntity dies, its transform remains but its
                // navMeshAgent does not.
                if (!dead)
                {
                    navMeshAgent.enabled = true;
                    currentState = State.Chasing;

                    Vector3 dirToTarget = (target.position - transform.position).normalized;
                    Vector3 targetPos = target.position - dirToTarget * (collisionRadius + targetCollisionRadius);

                    navMeshAgent.SetDestination(targetPos);
                }
            }
            else
            {
                if (!dead)
                {
                    navMeshAgent.enabled = false; // this stops the Enemy from chasing the Player
                }
            }

            yield return new WaitForSeconds(refreshRate);
        }
    }

    void Attack()
    {
        if (Time.time > nextAttackTime)
        {
            // The Enemy might not have been chasing the Player before attacking
            // so we store the previous state in order to return to it when
            // the Enemy is done attacking.
            State prevState = currentState;
            currentState = State.Attacking;
            // The Enemy should not be chasing the Player while attacking.
            navMeshAgent.enabled = false;

            nextAttackTime = Time.time + msBetweenAttacks / 1000;
            Projectile newProjectile = Instantiate(attackType, attackSpawn.position, attackSpawn.rotation);
            newProjectile.SetSpeed(attackSpeed);

            currentState = prevState;
            navMeshAgent.enabled = true;
        }
    }

    IEnumerator DamageFlash()
    {
        float duration = 0f;
        skinMaterial.color = flashColor;

        while (duration < flashDuration)
        {
            duration += Time.deltaTime;
            yield return null;
        }

        skinMaterial.color = originalColor;
    }

    public override void TakeDamage(float damage)
    {
        // Enemy should flash when it takes damage
        StartCoroutine(DamageFlash());

        base.TakeDamage(damage);
    }

    void OnTargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }

    void OnLightstickDeath()
    {
        hasTarget = false;
        target = null;
        currentState = State.Idle;
        // scan the area for lightsticks. if none, set target to player
        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, rangeOfSight, lightScanMask);
        if (initialCollisions.Length > 0)
        {
            Collider stick = initialCollisions[0];
            GetGlowingObject(stick.GetComponent<GlowingObject>());
        } else
        {
            // reset target to Player
            target = player.lamp.transform;
            currentIntensity = player.lamp.Intensity;
            hasTarget = true;
        }
    }

    public void Explode(ParticleSystem deathEffect, Vector3 hitPoint)
    {
        Destroy(Instantiate(deathEffect.gameObject, hitPoint, transform.rotation));
    }

    public override void Die()
    {
        base.Die();
    }
}