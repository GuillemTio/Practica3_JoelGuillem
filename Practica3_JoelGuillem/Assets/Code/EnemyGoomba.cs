using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGoomba : MonoBehaviour, IRestartGameElement
{
    enum TState
    {
        IDLE = 0,
        PATROL,
        ALERT,
    }

    private GameObject m_Player;

    TState m_State;
    TState m_LastState;
    UnityEngine.AI.NavMeshAgent m_NavMeshAgent;
    public float m_NearPlayerRadius;
    public List<Transform> m_PatrolPositions;
    int m_CurrentPatrolPositionId = 0;
    Animator m_Animator;

    Vector3 m_StartPosition;
    Quaternion m_StartRotation;

    [Header("Attack")]
    public int m_AttackDamage = 1;
    public float m_TimeToAttack = 1f;
    float m_AttackTimer = 0;
    public float m_MinDistanceToAttack;

    public float m_WalkSpeed;
    public float m_RunSpeed;

    public GameObject m_KillParticles;


    private void Awake()
    {
        m_NavMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
        GameController.GetGameController().AddRestartGameElement(this);
    }

    void Start()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player");
        m_Animator = GetComponent<Animator>();
        SetIdleState();
    }


    private void Update()
    {
        switch (m_State)
        {
            case TState.IDLE:
                UpdateIdleState();
                break;
            case TState.PATROL:
                UpdatePatrolState();
                break;
            case TState.ALERT:
                UpdateAlertState();
                break;
            default:
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, m_NearPlayerRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_MinDistanceToAttack);
    }

    void SetIdleState()
    {
        m_State = TState.IDLE;
    }
    void SetPatrolState()
    {
        m_State = TState.PATROL;
        m_NavMeshAgent.isStopped = false;
        m_NavMeshAgent.speed = m_WalkSpeed;
        m_Animator.SetBool("PlayerDetected", false);
    }
    void SetAlertState()
    {
        m_State = TState.ALERT;
        m_NavMeshAgent.speed = m_RunSpeed;
        m_Animator.SetBool("PlayerDetected", true);
    }

    void UpdateIdleState()
    {
        SetPatrolState();
    }
    void UpdatePatrolState()
    {
        if (!m_NavMeshAgent.hasPath && m_NavMeshAgent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathComplete)
            NextPatrolPosition();
        if (IsPlayerAround())
            SetAlertState();

    }
    void UpdateAlertState()
    {
        SetNextChasePosition();
        if (!IsPlayerAround())
        {
            SetPatrolState();
        }
        if (CanAttack())
        {
            Attack();
        }
    }

    void SetNextChasePosition()
    {
        Vector3 l_PlayerPosition = m_Player.transform.position;
        Vector3 l_EnemyPosition = transform.position;
        Vector3 l_DirectionToEnemy = l_EnemyPosition - l_PlayerPosition;
        l_DirectionToEnemy.Normalize();
        Vector3 l_DesiredPosition = l_PlayerPosition + l_DirectionToEnemy;
        m_NavMeshAgent.SetDestination(l_DesiredPosition);
    }

    void NextPatrolPosition()
    {
        ++m_CurrentPatrolPositionId;
        if (m_CurrentPatrolPositionId >= m_PatrolPositions.Count)
            m_CurrentPatrolPositionId = 0;
        MoveToNextPatrolPosition();
    }

    void MoveToNextPatrolPosition()
    {
        m_NavMeshAgent.SetDestination(m_PatrolPositions[m_CurrentPatrolPositionId].position);
    }

    bool IsPlayerAround()
    {
        return DistanceToPlayer() < m_NearPlayerRadius;
    }

    private float DistanceToPlayer()
    {
        Vector3 l_PlayerPosition = m_Player.transform.position;
        Vector3 l_EnemyPosition = transform.position;
        return Vector3.Distance(l_PlayerPosition, l_EnemyPosition);
    }
    private bool CanAttack()
    {
        m_AttackTimer += Time.deltaTime;
        if(m_AttackTimer > m_TimeToAttack)
        {
            if(DistanceToPlayer() < m_MinDistanceToAttack)
            {
                m_AttackTimer = 0;
                return true;
            }

        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "MarioPunch")
            Kill();
    }

    private void Attack()
    {
        m_Player.GetComponent<MarioController>().UpdateHealth(-m_AttackDamage);
    }
    public void RestartGame()
    {
        
        transform.parent.gameObject.SetActive(true);
        m_NavMeshAgent.isStopped = true;
        m_NavMeshAgent.enabled = false;
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        m_NavMeshAgent.enabled = true;
        SetIdleState();
    }

    public void Kill()
    {
        
        GameObject l_Particles = Instantiate(m_KillParticles, transform.position, transform.rotation);
        l_Particles.GetComponent<ParticleSystem>().Play();
        transform.parent.gameObject.SetActive(false);
    }
}
