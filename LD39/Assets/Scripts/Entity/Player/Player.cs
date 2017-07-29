// The Player script receives input for the Player.

using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(LightstickController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{

    public LineRenderer lineOfSight;
    public GlowingObject lamp;

    // The PlayerController script handles the movement of the Player
    PlayerController playerController;
    LightstickController lightstickController;
    GunController gunController;
    Camera viewCamera;

    float moveSpeed = 5f;

    private float lightPower;
    private float cellCount;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        lightstickController = GetComponent<LightstickController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
    }

    protected override void Start()
    {
        base.Start();

        UpdateStats(GameManager.instance.playerHealth, GameManager.instance.playerPower, GameManager.instance.playerCells);
        // Set the line of sight to the Player's position. This will change to the
        // position of the gun's muzzle.
        lineOfSight.SetPosition(0, transform.position);
    }

    void Update()
    {
        // Move Player based on input from the keyboard
        Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        playerController.Move(moveInput.normalized * moveSpeed);
        Vector3 playerPos = transform.position;
        lineOfSight.SetPosition(0, playerPos);
        // Main camera follows player
        viewCamera.transform.position = new Vector3(playerPos.x, playerPos.y + 9, playerPos.z - 5);

        // Turn Player
        // Turn Player to face the direction of the cursor
        // The screen is a 2d world but the game is in a 3d world so we need raycasting to get the
        // accurate position of the mouse cursor in the game world.

        // 1. Cast a ray from the camera through the mouse cursor's position
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        // 2. draw a plane that's perpendicular to the y-axis and passes through a point at the gun height.
        // this would be a plane just above the ground and slicing the player at gun height.
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * 1); // 1 here represents the height of the gun
        float rayDistance;

        // 3. if the ray drawn earlier intersects the plane
        if (groundPlane.Raycast(ray, out rayDistance))
        {
            // 4. get the point where the ray intersects the plane
            Vector3 point = ray.GetPoint(rayDistance);
            // 5. turn the Player to face that point
            playerController.LookAt(point);

            // this should be limited to a length
            lineOfSight.SetPosition(1, point);
        }

        // Shoot
        if (Input.GetMouseButtonUp(0))
        {
            if (lightPower <= 0)
                return;

            lightPower -= 1;
            gunController.Shoot();
        }

        // Turn lamp on or off
        if (Input.GetMouseButtonUp(1))
        {
            EventManager.instance.TriggerEvent("NewGlowingObject", lamp);
            lamp.ToggleLight();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (lightPower < 25)
                return;

            lightPower -= 25;
            lightstickController.DropLight();
        }
    }

    // This method allows the player to carry experience and health over
    // levels. The Player's stats are stored when a new level is to be loaded.
    void UpdateStats(float newHP, float newPower, float newCell)
    {
        health = newHP;
        lightPower = newPower;
        cellCount = newCell;
    }

    public float GetHealth()
    {
        return health;
    }

    public float GetPower()
    {
        return lightPower;
    }

    public float GetCells()
    {
        return cellCount;
    }

    public void UpdateCellCount(float cellValue)
    {
        cellCount += cellValue;
    }
}