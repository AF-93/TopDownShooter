using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour
{   [SerializeField] int lifePoints;
    [Range(0.1f, 5f)]
    [SerializeField] private float fireCooldown; // Tiempo de cooldown entre disparos
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

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }


    void Update()
    {
        if (!characterController.isGrounded)
        {

            verticalVelocity = gravityForce * Time.deltaTime;
        }

        float movementX = (movementInput.x * movementSpeed * Time.deltaTime);
        float movementZ = (movementInput.y * movementSpeed * Time.deltaTime);

        Vector3 finalMovement = new Vector3(movementX, verticalVelocity, movementZ);

        characterController.Move(finalMovement);

        lookTarget.y = transform.position.y;
        transform.LookAt(lookTarget);

        // Cooldown para disparo simple
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

        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float enter))
        {
            lookTarget = ray.GetPoint(enter);

        }
    }
    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed && canFire)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            canFire = false;
            fireCooldownTimer = 0f;
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
    public void OnBurst(InputAction.CallbackContext context)
    {
        if (context.performed && canBurst)
        {
            StartCoroutine(BurstFire());
            canBurst = false;
            burstTimer = 0f;
        }
        
    }
    public void LPPlayer(int valor){
        lifePoints += valor;
        Debug.Log("vida: " + lifePoints);
    }
}
