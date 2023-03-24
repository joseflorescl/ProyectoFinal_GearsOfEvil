using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerStates { Idle, Walking, Running, Jump, Death, Victory};

public class PlayerWarriorController : MonoBehaviour, IDamageable
{
    [Header("Motion Settings")]
    public float walkingSpeed = 2;
    public float runningSpeed = 4;
    public float turnSpeed = 90;
    public float jumpForce = 4;

    [Header("Gameplay Settings")]    
    public int maxHealth = 10;
    public int initialAmmo = 20;
    public float fireRate = 0.2f;
    public int healthPowerUp = 1;
    public int ammoPowerUp = 5;

    [Header("Ground Checking")]
    public LayerMask layerIsGround;
    public float checkGroundRadius = 0.1f;
    public float checkGroundMaxDistance = 0.1f;
    public Transform checkGroundOrigin;

    [Header("Fire Gun Checking")]
    public LayerMask layerIsDamageable;
    public float checkFireRadius = 1f;
    public float checkFireMaxDistance = 10f;
    public Transform checkFireOrigin;

    [Header("Managers")]
    public AudioManager audioManager;
    public VFXManager vfxManager;
    public UIManager uiManager;
    public GameManager gameManager;

    private PlayerStates playerState;
    private float motionSpeed;
    private float turnInput;
    private float movementInput;
    private float nextFire = 0.0f;
    private int currentAmmo;
    private Rigidbody rb;
    private Animator anim;

    private float InputSpeed => Mathf.Abs(movementInput);
    private bool InputIsRunning => InputSpeed > 0 && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));

    public int Health { get; set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        StartPlayerLife();
    }

    private void StartPlayerLife()
    {
        playerState = PlayerStates.Idle;
        motionSpeed = 0;
        Health = maxHealth;
        currentAmmo = initialAmmo;

        uiManager.UpdateHealth((float)Health / (float)maxHealth);
        uiManager.UpdateAmmo(currentAmmo);

      
    }

    public void ResetPlayerLife()
    {
        StartPlayerLife();
        anim.SetTrigger("Respawn");
    }

    private void Update()
    {
        if (playerState == PlayerStates.Death || playerState == PlayerStates.Victory)
            return;

        turnInput = Input.GetAxis("Horizontal");
        movementInput = Input.GetAxis("Vertical");

        anim.SetFloat("Move Speed", InputSpeed);

        if (Input.GetKeyDown(KeyCode.Space))
            Jump();

        if (Input.GetKeyDown(KeyCode.LeftControl) && Time.time > nextFire)
            Fire();

        switch (playerState)
        {
            case PlayerStates.Idle:
                IdleState();
                break;
            case PlayerStates.Walking:
                WalkingState();
                break;
            case PlayerStates.Running:
                RunningState();
                break;
            case PlayerStates.Jump:
                break;
            default:
                break;
        }
    }

    private void IdleState()
    {
        if (InputSpeed > 0)
        {
            playerState = PlayerStates.Walking;
            motionSpeed = walkingSpeed;
        }
    }

    private void WalkingState()
    {
        if (InputIsRunning)
        {
            playerState = PlayerStates.Running;
            motionSpeed = runningSpeed;
            anim.SetBool("Is Running", true);
        }
        else if (InputSpeed == 0)
        {
            playerState = PlayerStates.Idle;
            motionSpeed = 0;
        }
    }

    private void RunningState()
    {
        if (!InputIsRunning)
        {
            anim.SetBool("Is Running", false);
            playerState = PlayerStates.Walking;
            motionSpeed = walkingSpeed;
        }
    }

    private void Jump()
    {
        if (Physics.SphereCast(checkGroundOrigin.position, checkGroundRadius, Vector3.down, out RaycastHit hitInfo, checkGroundMaxDistance, layerIsGround))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            anim.SetTrigger("Jump");
            audioManager.PlayerVoiceJumping();
        }
    }

    private void Fire()
    {
        nextFire = Time.time + fireRate;

        if (currentAmmo <= 0)
        {
            audioManager.PlayerGunNoAmmo();
        }
        else
        {
            currentAmmo--;            
            uiManager.UpdateAmmo(currentAmmo);

            anim.SetTrigger("Fire");
            audioManager.PlayerGunFire();
            vfxManager.GunFire();

            // Notar que también se puede detectar un disparo en un collider Default: si disparo a un muro, la idea es que si hay un enemigo detrás no lo detectemos.
            if (Physics.SphereCast(checkFireOrigin.position, checkFireRadius, checkFireOrigin.forward, out RaycastHit hitInfo, checkFireMaxDistance, layerIsDamageable))
            {
                //print("Fire: " + hitInfo.transform.name);
                if (hitInfo.transform.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(1);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (playerState == PlayerStates.Death || playerState == PlayerStates.Victory)
            return;

        Turn();
        Move();
    }

    private void Turn()
    {
        // el turnInput nos da el delta de giro a aplicar al player
        float deltaTurn = turnInput * turnSpeed * Time.fixedDeltaTime; // Este número indica el delta de rotación a aplicar
        Quaternion deltaRotation = Quaternion.Euler(0, deltaTurn, 0); // Este es el delta de rotación a sumar a la rotación actual del player
        rb.MoveRotation(rb.rotation * deltaRotation); // Para agregar el delta de rotación se debe multiplicar por la rotación actual.
    }
    private void Move()
    {
        Vector3 movement = transform.forward * motionSpeed * movementInput;
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Health"))
        {
            Destroy(other.gameObject);
            Health = Mathf.Clamp(Health + healthPowerUp, 0, maxHealth);
            uiManager.UpdateHealth((float)Health / (float)maxHealth);            
            audioManager.PlayerGetHealth();
        }
        else if (other.CompareTag("Ammo"))
        {
            Destroy(other.gameObject);
            currentAmmo += ammoPowerUp;
            uiManager.UpdateAmmo(currentAmmo);
            audioManager.PlayerGetAmmo();
        }
    }

    //private void OnDrawGizmosSelected()
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Debug.DrawLine(checkGroundOrigin.position, checkGroundOrigin.position + Vector3.down * checkGroundMaxDistance);
        Gizmos.DrawWireSphere(checkGroundOrigin.position + Vector3.down * checkGroundMaxDistance, checkGroundRadius);

        Gizmos.color = Color.red;
        Debug.DrawLine(checkFireOrigin.position, checkFireOrigin.position + checkFireOrigin.forward * checkFireMaxDistance);
        Gizmos.DrawWireSphere(checkFireOrigin.position + checkFireOrigin.forward * checkFireMaxDistance, checkFireRadius);
    }


    public void FootstepWalking()
    {
        audioManager.PlayerFootstepWalking();
    }

    public void FootstepRunning()
    {
        audioManager.PlayerFootstepRunning();
    }

    public void FallingJumpSound()
    {
        audioManager.PlayerFallingJump();
    }
    
    public void TakeDamage(int damage)
    {
        if (Health > 0)
        {
            Health -= damage;
            uiManager.UpdateHealth((float)Health / (float)maxHealth);            
            if (Health <= 0)
            {
                Died();
            }
            else
            {
                audioManager.PlayerTakeDamage();
                anim.SetTrigger("Take Damage");
            }
        }
    }

    public void Died()
    {
        audioManager.PlayerDied();
        anim.SetTrigger("Death");
        playerState = PlayerStates.Death;
        gameManager.OnPlayerDeath();
    }

    public void PlayerFallingGround()
    {
        audioManager.PlayerFallingGround();
    }

    public void SetVictoryState()
    {
        playerState = PlayerStates.Victory;
        anim.SetTrigger("Victory");
    }
}
