using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] int lifePoints;
    [Range(0.1f, 5f)]
    [SerializeField] private float fireCooldown;
    private bool canFire = true;
    private float fireCooldownTimer = 0f;
    
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    
    [Range(1f, 5f)]
    [SerializeField] int burstCount;
    [Range(0.1f, 2f)]
    [SerializeField] float burstDelay;
    
    [SerializeField] float gravityForce;
    [Range(1f, 10f)]
    [SerializeField] float movementSpeed;
    
    private bool canBurst = true;
    [Range(0f, 5f)]
    [SerializeField] float burstCooldown;
    [SerializeField] float burstTimer;
    
    private float verticalVelocity;
    private Vector2 movementInput;
    private Vector2 mousePosition;
    private Vector3 lookTarget;
    private CharacterController characterController;

    private ICommand fireCommand;
    private ICommand burstCommand;
    private ICommand moveCommand;
    private ICommand lookCommand;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        fireCommand = new FireCommand(this);
        burstCommand = new BurstFireCommand(this);
        
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.OnPlayerDied += OnPlayerDied;
        }
    }

    void Update()
    {
        verticalVelocity = -9.81f*Time.deltaTime;

        moveCommand = new MoveCommand(this, movementInput);
        moveCommand.Execute();

        if (lookCommand != null)
        {
            lookCommand.Execute();
        }

        if (!canFire)
        {
            fireCooldownTimer += Time.deltaTime;
            if (fireCooldownTimer >= fireCooldown)
            {
                canFire = true;
                fireCooldownTimer = 0f;
            }
        }

        if (!canBurst)
        {
            burstTimer += Time.deltaTime;
            if (burstTimer >= burstCooldown)
            {
                canBurst = true;
                burstTimer = 0f;
            }
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnMouseLook(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>();
        lookCommand = new LookCommand(this, mousePosition);
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed && canFire && GameManager.Instance.IsGameActive())
        {
            fireCommand.Execute();
        }
    }

    public void OnBurst(InputAction.CallbackContext context)
    {
        if (context.performed && canBurst && GameManager.Instance.IsGameActive())
        {
            burstCommand.Execute();
        }
    }

    public void ExecuteFire()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        canFire = false;
        fireCooldownTimer = 0f;
    }

    public void ExecuteBurstFire()
    {
        StartCoroutine(BurstFire());
        canBurst = false;
        burstTimer = 0f;
    }

    public void ExecuteMove(Vector2 direction)
    {
        float movementX = (direction.x * movementSpeed * Time.deltaTime);
        float movementZ = (direction.y * movementSpeed * Time.deltaTime);

        Vector3 finalMovement = new Vector3(movementX, verticalVelocity, movementZ);

        characterController.Move(finalMovement);
    }

    public void ExecuteLook(Vector2 screenMousePos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenMousePos);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float enter))
        {
            lookTarget = ray.GetPoint(enter);
            lookTarget.y = transform.position.y;
            transform.LookAt(lookTarget);
        }
    }

    private System.Collections.IEnumerator BurstFire()
    {
        for (int i = 0; i < burstCount; i++)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            yield return new WaitForSeconds(burstDelay);
        }
    }

    public void LPPlayer(int valor)
    {
        lifePoints += valor;
        Debug.Log("Vida: " + lifePoints);

        GameEvents.Instance?.RaisePlayerHealthChanged(lifePoints);

        if (lifePoints <= 0)
        {
            GameEvents.Instance?.RaisePlayerDied();
        }
    }

    private void OnPlayerDied()
    {
        enabled = false;
    }

    private void OnDestroy()
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.OnPlayerDied -= OnPlayerDied;
        }
    }
}
