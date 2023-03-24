using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyStates { Idle, Patrol, Chase, Attack, Death };

public class EnemyController : MonoBehaviour, IDamageable
{
    [Header("Motion Settings")]
    public float walkingSpeed = 2;
    public float runningSpeed = 4;
    public float turnSpeedSlerp = 20;
    public float minDistanceToWaypoint = 0.3f;
    public float minDistanceToChase = 20f;
    public float minDistanceToAttack = 2f;

    [Header("Gameplay Settings")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float minSecondsInIdle = 1;
    [SerializeField] private float maxSecondsInIdle = 5;
    [SerializeField] private GameObject wayPointsPatrol; // Estos puntos se crean en modo edit dentro del transform del enemigo, pero en el start se sacan, para que cuando se mueva el enemigo esos puntos no se muevan con el.
    [SerializeField] private int damageAttack = 1;
    [SerializeField] private float secondsToCelebrate = 5f;

    [Header("Managers")]
    public AudioManager audioManager;
    public GameManager gameManager;

    public int Health { get; set; }

    private GameObject player;
    public EnemyStates enemyState;  // dejarlo private!   
    private float secondsInIdle;
    public Vector3[] waypointsArray; // dejarlo private!
    private int currentWaypoint;
    private float motionSpeed;
    private Quaternion targetRotation;
    private Animator anim;
    private Rigidbody rb;
    private AudioSource audioSource;
    private Collider coll;
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        coll = GetComponent<Collider>();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Health = maxHealth;
        CreateWaypointsArray();
        StartEnemyState();
    }

    private void StartEnemyState()
    {
        enemyState = EnemyStates.Idle;
        secondsInIdle = Random.Range(minSecondsInIdle, maxSecondsInIdle);
        transform.LookAt(waypointsArray[currentWaypoint]);
        targetRotation = Quaternion.LookRotation(waypointsArray[currentWaypoint] - transform.position);
        motionSpeed = 0;
    }

    public void ResetEnemyState()
    {
        // No se incluye en este reseteo el resetear la salud: si el player ya le había quitado salud eso se respetará al hacer un Respawn
        StartEnemyState();
        anim.SetTrigger("Respawn");
    }

    private void CreateWaypointsArray()
    {
        wayPointsPatrol.transform.parent = null;
        currentWaypoint = 0;
        // Para que el código quede más limpio y por performance, se pasa la lista de waypoints desde wayPointsPatrol al array waypointsArray
        waypointsArray = new Vector3[wayPointsPatrol.transform.childCount];
        for (int i = 0; i < waypointsArray.Length; i++)
        {
            waypointsArray[i] = wayPointsPatrol.transform.GetChild(i).position;
        }

        Destroy(wayPointsPatrol);
    }

   

    public void Update()
    {
        switch (enemyState)
        {
            case EnemyStates.Idle:
                IdleState();
                break;
            case EnemyStates.Patrol:
                PatrolState();
                break;
            case EnemyStates.Chase:
                ChaseState();
                break;
            case EnemyStates.Attack:
                // La lógica del ataque se llama en el evento de sonido, para que calce mejor. De ataque se pasa a Idle.                
                break;
            case EnemyStates.Death:
                break;
            default:
                break;
        }
    }

    private void IdleState()
    {
        secondsInIdle -= Time.deltaTime;
        if (secondsInIdle <= 0f)
        {
            enemyState = EnemyStates.Patrol;
            motionSpeed = walkingSpeed;
            anim.SetBool("Is Patrolling", true);
            anim.SetBool("Is Chasing", false);
        }
    }

    private void PatrolState()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= minDistanceToChase)
        {
            enemyState = EnemyStates.Chase;
            motionSpeed = runningSpeed;
            targetRotation = Quaternion.LookRotation(player.transform.position - transform.position);
            anim.SetBool("Is Patrolling", false);
            anim.SetBool("Is Chasing", true);
        }
        else if (Vector3.Distance(transform.position, waypointsArray[currentWaypoint]) <= minDistanceToWaypoint)
        {
            currentWaypoint = (currentWaypoint + 1) % waypointsArray.Length;
            targetRotation = Quaternion.LookRotation(waypointsArray[currentWaypoint] - transform.position);
        }
    }

    private void ChaseState()
    {
        targetRotation = Quaternion.LookRotation(player.transform.position - transform.position);
        if (Vector3.Distance(transform.position, player.transform.position) <= minDistanceToAttack)
        {
            enemyState = EnemyStates.Attack;
            motionSpeed = 0f;
            anim.SetBool("Is Patrolling", false);
            anim.SetBool("Is Chasing", false);
            anim.SetTrigger("Attack");
        }
    }

    private void Attack()
    {
        IDamageable playerDamageable = player.GetComponent<IDamageable>();
        playerDamageable.TakeDamage(damageAttack);
        if (playerDamageable.Health <= 0)
        {
            Invoke(nameof(Victory), secondsToCelebrate);
        }
        else
        {
            enemyState = EnemyStates.Idle;
            secondsInIdle = minSecondsInIdle;
        }
        
    }

    private void FixedUpdate()
    {
        switch (enemyState)
        {
            case EnemyStates.Patrol:
            case EnemyStates.Chase:
                Turn();
                Move();
                break;
            default:
                break;
        }
        
    }

    private void Turn()
    {
        rb.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * turnSpeedSlerp);
    }

    private void Move()
    {
        Vector3 movement = transform.forward * motionSpeed;
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
    }



    public void TakeDamage(int damage)
    {
        if (Health > 0)
        {
            Health -= damage;
            if (Health <= 0)
            {
                Died();
            }
            else
            {
                audioManager.EnemyTakeDamage(audioSource);
                anim.SetTrigger("Take Damage");
            }
        }
    }

    private void Victory()
    {
        anim.SetTrigger("Victory");
        audioManager.EnemyVictory(audioSource);
    }

    public void Died()
    {
        motionSpeed = 0f;
        audioManager.EnemyDied(audioSource);
        anim.SetTrigger("Death");
        enemyState = EnemyStates.Death;
        coll.enabled = false;
        rb.isKinematic = true;
        gameManager.OnEnemyDeath();

        //Nos aseguramos que el enemigo no reviva:
        this.enabled = false;
    }



    public void InitTakeDamage() // Llamada en evento de animación
    {
        motionSpeed = 0f;
    }

    public void EndTakeDamage() // Llamada en evento de animación
    {
        switch (enemyState)
        {
            case EnemyStates.Idle:
                motionSpeed = 0f;
                break;
            case EnemyStates.Patrol:
                motionSpeed = walkingSpeed;
                break;
            case EnemyStates.Chase:
                motionSpeed = runningSpeed;
                break;
            case EnemyStates.Attack:
                motionSpeed = 0f;
                break;
            default:
                break;
        }
    }


    public void FootstepWalking()
    {
        audioManager.EnemyFootstepWalking(audioSource);
    }

    public void FootstepRunning()
    {
        audioManager.EnemyFootstepRunning(audioSource);
    }

    public void AttackSound()
    {
        audioManager.EnemyAttack(audioSource);
        Attack();
    }

    public void EnemyRoar()
    {
        audioManager.EnemyRoar(audioSource);
    }

    public void EnemyFallingGround()
    {
        audioManager.EnemyFallingGround(audioSource);
    }

}
