using System;
using UnityEngine;

public class MarioController : MonoBehaviour, IRestartGameElement
{
    Animator m_Animator;
    CharacterController m_CharacterController;
    public Camera m_Camera;
    public float m_WalkSpeed;
    public float m_RunSpeed;
    float m_VerticalSpeed = 0f;
    [Range(0f, 1f)]
    public float m_LerpRotation;
    Vector3 m_StartPosition;
    Quaternion m_StartRotation;

    public ParticleSystem m_RunParticles;

    Checkpoint m_CurrentCheckpoint;

    public UIMario m_UI;

    float m_specialIdleTimer;
    public float m_TimeToSpecialIdle;

    [Header("DebugInput")]

    bool m_AngleLocked = false;
    bool m_AimLocked = true;
    public KeyCode m_DebugLockAngleKeyCode = KeyCode.I;
    public KeyCode m_DebugLockKeyCode = KeyCode.O;

    [Header("Punch")]
    int m_CurrentPunchId = 0;
    public GameObject m_LeftHandHitCollider;
    public GameObject m_RightHandHitCollider;
    public GameObject m_RightFootHitCollider;
    public float m_PunchComboMaxTime;
    float m_LastPunchTime;

    [Header("Elevator")]
    Collider m_CurrentElevator;
    public float m_MinDotToAttachElevator = 0.98f;

    public float m_BridgeForce;

    [Header("Jump")]
    public KeyCode m_JumpKeyCode = KeyCode.Space;
    bool m_OnGround;
    bool m_CanWallJump;
    bool m_IsWallJumping;
    public float m_TimeToWallJumpAgain = 1f;
    float m_WallJumpTimer = 0;
    int m_CurrentJumpId = 0;
    public float m_JumpHorizontalSpeed;
    public float m_JumpSpeed;
    public float m_DoubleJumpSpeed;
    public float m_TripleJumpSpeed;
    public float m_LongJumpSpeed;
    public float m_JumpOverEnemySpeed;
    public float m_JumpComboMaxTime;
    float m_LastJumpTime;

    [Header("Crouch")]
    public KeyCode m_CrouchKeyCode = KeyCode.LeftControl;
    bool m_IsCrouched;

    [Header("Goomba")]
    public float m_MinDotToKillGoomba = 0.8f;
    public float m_MinVerticalSpeedToKillGoomba = 0.25f;

    [Header("Health")]
    int m_CurrentHealth;
    int m_MaxHealth = 8;
    int m_CurrentLifes;
    public int m_MaxLifes = 3;
    public GameObject m_PickParticles;

    [Header("DeathScreen")]
    public GameObject m_DeathScreen;
    public GameObject m_RetryButton;
    public GameObject m_ExitButton;

    [Header("Sounds")]
    public AudioClip m_DieSound;
    public AudioClip m_JumpSound;
    public AudioClip m_DoubleJumpSound;
    public AudioClip m_TripleJumpSound;
    public AudioClip m_LongJumpSound;
    public AudioClip m_CoinSound;
    public AudioClip m_StarSound;
    public AudioClip m_StartLevelSound;
    public AudioClip m_LevelMusic;
    public AudioClip m_KillGoombaSound;
    public AudioClip m_TakeDamageSound;

    public enum TPunchHitColliderType
    {
        LEFT_HAND = 0,
        RIGHT_HAND,
        RIGHT_FOOT
    }

    bool m_IsOnPunch;

    void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_CharacterController = GetComponent<CharacterController>();
        GameController.GetGameController().AddRestartGameElement(this);
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
        m_LeftHandHitCollider.SetActive(false);
        m_RightFootHitCollider.SetActive(false);
        m_RightHandHitCollider.SetActive(false);

        m_CurrentHealth = m_MaxHealth;
        m_CurrentLifes = m_MaxLifes;
        m_UI.UpdateLifeText(m_CurrentLifes);

        AudioManager.instance.PlaySound(m_StartLevelSound);
        AudioManager.instance.PlayLevelMusic(m_LevelMusic, 0.2f);
    }



    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(m_DebugLockAngleKeyCode))
            m_AngleLocked = !m_AngleLocked;
        if (Input.GetKeyDown(m_DebugLockKeyCode))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
            m_AimLocked = Cursor.lockState == CursorLockMode.Locked;
        }
#endif
        if (Input.GetKey(m_CrouchKeyCode) && m_OnGround)
        {
            m_IsCrouched = true;
        }
        else
        {
            m_IsCrouched = false;

        }
        m_Animator.SetBool("IsCrouched", m_IsCrouched);

        if (!m_IsCrouched)
        {
            Vector3 l_Right = m_Camera.transform.right;
            Vector3 l_Forward = m_Camera.transform.forward;

            l_Right.y = 0f;
            l_Right.Normalize();

            l_Forward.y = 0f;
            l_Forward.Normalize();

            Vector3 l_Movement = Vector3.zero;
            float l_AnimationSpeed = 0f;
            float l_Speed = 0f;

            if (!m_IsWallJumping)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    l_Movement = l_Forward;
                    l_AnimationSpeed = 0.3f;
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    l_Movement = -l_Forward;
                    l_AnimationSpeed = 0.3f;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    l_Movement += l_Right;
                    l_AnimationSpeed = 0.3f;
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    l_Movement -= l_Right;
                    l_AnimationSpeed = 0.3f;
                }
            }
            if (l_AnimationSpeed == 0.3f)
            {
                l_Speed = m_WalkSpeed;
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    if (m_RunParticles.isStopped)
                    {
                        m_RunParticles.Play();
                    }
                    l_AnimationSpeed = 1f;
                    l_Speed = m_RunSpeed;
                }
            }

            l_Movement.Normalize();
            if (l_Speed > 0.0f)
            {
                Quaternion l_DesiredRotation = Quaternion.LookRotation(l_Movement);
                transform.rotation = Quaternion.Lerp(transform.rotation, l_DesiredRotation, m_LerpRotation);
            }
            l_Movement = l_Movement * l_Speed * Time.deltaTime;
            m_VerticalSpeed += Physics.gravity.y * Time.deltaTime;
            l_Movement.y = m_VerticalSpeed * Time.deltaTime;
            CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Movement);
            if ((l_CollisionFlags & CollisionFlags.CollidedBelow) != 0)
            {
                m_VerticalSpeed = 0f;
                m_OnGround = true;
            }
            else
            {
                m_OnGround = false;
            }
            if ((l_CollisionFlags & CollisionFlags.CollidedSides) != 0 && !m_OnGround)
            {
                m_CanWallJump = true;
            }
            else
            {
                m_CanWallJump = false;
            }

            m_Animator.SetBool("OnGround", m_OnGround);
            m_Animator.SetFloat("Speed", l_AnimationSpeed);

            //Special Idle
            if (!Input.anyKey)
            {
                m_specialIdleTimer += Time.deltaTime;
                if (m_specialIdleTimer > m_TimeToSpecialIdle)
                {
                    m_Animator.SetTrigger("SpecialIdle");
                    m_specialIdleTimer = 0;
                }
            }
            else
                m_specialIdleTimer = 0;

            if (CanPunch() && Input.GetMouseButtonUp(0))
            {
                if (!MustPunchCombo())
                    m_CurrentPunchId = 0;
                Punch(m_CurrentPunchId);
                m_CurrentPunchId++;
                if (m_CurrentPunchId > 2)
                    m_CurrentPunchId = 0;
            }

            if ((!(l_AnimationSpeed > 0.3) || (!m_OnGround))&& m_RunParticles.isPlaying)
            {
                m_RunParticles.Stop();
            }
        }


        if (CanGroundJump() && Input.GetKeyDown(m_JumpKeyCode))
        {

            if (!MustJumpCombo())
                m_CurrentJumpId = 0;

            if (m_IsCrouched)
            {
                m_IsCrouched = false;
                m_Animator.SetBool("IsCrouched", false);
                m_CurrentJumpId = 3;

                m_OnGround = false;
            }

            Jump(m_CurrentJumpId);
            m_CurrentJumpId++;

            if (m_CurrentJumpId > 2)
                m_CurrentJumpId = 0;
        }
        else if (m_CanWallJump && Input.GetKeyDown(m_JumpKeyCode) && !m_IsWallJumping)
        {
            m_IsWallJumping = true;
            m_CurrentJumpId = 0;
            Jump(m_CurrentJumpId);
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + 180, transform.rotation.eulerAngles.z));
        }

        UpdateElevator();
        if (m_IsWallJumping)
        {
            MovePlayerFromWall();
            UpdateWallJumpTimer();
        }


    }

    private void LateUpdate()
    {
        Vector3 l_EulerAngles = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0f, l_EulerAngles.y, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "DeathZone")
        {
            Kill();
        }
        else if (other.tag == "Elevator")
        {
            if (CanAttachElevator(other))
                AttachElevator(other);
        }
        else if (other.tag == "Goomba")
        {
            if (CanKillGoomba(other))
            {
                other.transform.parent.GetComponent<EnemyGoomba>().Kill();
                AudioManager.instance.PlaySound(m_KillGoombaSound);
                JumpOverEnemy();
            }
        }
        else if (other.tag == "Checkpoint")
            m_CurrentCheckpoint = other.GetComponent<Checkpoint>();
        else if (other.tag == "Coin")
        {
            other.GetComponent<Coin>().Pick();
            AudioManager.instance.PlaySound(m_CoinSound);
        }
        else if (other.tag == "Star")
        {
            AudioManager.instance.PlaySound(m_StarSound);
            GameObject l_Particles = Instantiate(m_PickParticles, transform.position, transform.rotation);
            l_Particles.GetComponent<ParticleSystem>().Play();
            UpdateHealth(1);
            other.gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Elevator")
        {
            if (other == m_CurrentElevator)
                DetachElevator();
        }
    }

    bool CanKillGoomba(Collider Goomba)
    {
        Vector3 l_DirectionToMario = transform.position - Goomba.transform.position;
        l_DirectionToMario.Normalize();
        return Vector3.Dot(Vector3.up, l_DirectionToMario) > m_MinDotToKillGoomba && m_VerticalSpeed < m_MinVerticalSpeedToKillGoomba;
    }
    void JumpOverEnemy()
    {
        m_VerticalSpeed = m_JumpOverEnemySpeed;
    }
    void Kill()
    {
        m_CharacterController.enabled = false;
        m_Animator.SetTrigger("Die");
        m_Animator.SetBool("IsDead", true);
        AudioManager.instance.PlaySound(m_DieSound);

        m_CurrentLifes--;
        m_UI.UpdateLifeText(m_CurrentLifes);
        Invoke("ShowDeathScreenDelayed", 2.0f);
    }

    void ShowDeathScreenDelayed()
    {
        if (m_CurrentLifes != 0)
        {
            ShowDeathScreen();
            m_RetryButton.SetActive(true);
            m_ExitButton.SetActive(true);
        }
        else
        {
            ShowDeathScreen();
            m_RetryButton.SetActive(false);
            m_ExitButton.SetActive(true);
        }
    }

    public void RestartGame()
    {
        HideDeathScreen();
        m_CharacterController.enabled = false;
        if (m_CurrentCheckpoint == null)
        {

            transform.position = m_StartPosition;
            transform.rotation = m_StartRotation;
        }
        else
        {
            transform.position = m_CurrentCheckpoint.m_StartPosition.position;
            transform.rotation = m_CurrentCheckpoint.m_StartPosition.rotation;
        }
        m_CurrentHealth = m_MaxHealth;
        m_CharacterController.enabled = true;
        m_Animator.SetBool("IsDead", false);
    }
    bool CanPunch()
    {
        return !m_IsOnPunch;
    }
    bool MustPunchCombo()
    {
        return (Time.time - m_LastPunchTime) < m_PunchComboMaxTime;
    }
    void Punch(int PunchId)
    {
        m_Animator.SetTrigger("Punch");
        m_Animator.SetInteger("PunchType", PunchId);
        m_IsOnPunch = true;
        m_LastPunchTime = Time.time;
    }
    public void SetPunchHitColliderActive(TPunchHitColliderType punchHitColliderType, bool Active)
    {
        if (punchHitColliderType == TPunchHitColliderType.LEFT_HAND)
            m_LeftHandHitCollider.SetActive(Active);
        else if (punchHitColliderType == TPunchHitColliderType.RIGHT_HAND)
            m_RightHandHitCollider.SetActive(Active);
        else if (punchHitColliderType == TPunchHitColliderType.RIGHT_FOOT)
            m_RightFootHitCollider.SetActive(Active);
    }
    public void SetIsOnPunch(bool IsOnPunch)
    {
        m_IsOnPunch = IsOnPunch;
    }
    bool CanGroundJump()
    {
        return m_OnGround;
    }
    bool MustJumpCombo()
    {
        return (Time.time - m_LastJumpTime) < m_JumpComboMaxTime;
    }

    void Jump(int JumpId)
    {
        if (m_IsWallJumping)
            m_Animator.SetTrigger("WallJump");
        else
            m_Animator.SetTrigger("Jump");

        m_Animator.SetInteger("JumpType", JumpId);
        if (JumpId == 0)
        {
            m_VerticalSpeed = m_JumpSpeed;
            AudioManager.instance.PlaySound(m_JumpSound);
        }
        else if (JumpId == 1)
        {
            m_VerticalSpeed = m_DoubleJumpSpeed;
            AudioManager.instance.PlaySound(m_DoubleJumpSound);
        }
        else if (JumpId == 2)
        {
            m_VerticalSpeed = m_TripleJumpSpeed;
            AudioManager.instance.PlaySound(m_TripleJumpSound);
        }
        else if (JumpId == 3)
        {
            m_VerticalSpeed = m_LongJumpSpeed;
            AudioManager.instance.PlaySound(m_LongJumpSound);
        }
        m_LastJumpTime = Time.time;
    }
    bool CanAttachElevator(Collider Elevator)
    {
        if (m_CurrentElevator == null)
        {
            return Vector3.Dot(Elevator.transform.forward, Vector3.up) >= m_MinDotToAttachElevator;
        }
        return false;
    }
    void AttachElevator(Collider Elevator)
    {
        m_CurrentElevator = Elevator;
        transform.SetParent(Elevator.transform);
    }
    void DetachElevator()
    {
        m_CurrentElevator = null;
        transform.SetParent(null);
    }
    void UpdateElevator()
    {
        if (m_CurrentElevator != null)
        {
            if (Vector3.Dot(m_CurrentElevator.transform.forward, Vector3.up) < m_MinDotToAttachElevator)
                DetachElevator();
        }

    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.tag == "Bridge")
        {
            hit.transform.GetComponent<Rigidbody>().AddForceAtPosition(-hit.normal * m_BridgeForce, hit.point);
        }
    }

    public void Step(AnimationEvent animationEvent)
    {
        AudioClip l_AudioClip = (AudioClip) animationEvent.objectReferenceParameter;
    }

    public void UpdateHealth(int healthPoints)
    {
        if (healthPoints < 0)
        {
            m_Animator.SetTrigger("Hit");
            AudioManager.instance.PlaySound(m_TakeDamageSound); //SONIDOOO
        }
        m_CurrentHealth += healthPoints;
        m_CurrentHealth = Math.Clamp(m_CurrentHealth, 0, m_MaxHealth);
        if (m_CurrentHealth == 0)
        {
            Kill();
        }
        else
        {
            m_UI.ShowLife(m_CurrentHealth);
        }
    }

    void UpdateWallJumpTimer()
    {
        m_WallJumpTimer += Time.deltaTime;
        if (m_WallJumpTimer > m_TimeToWallJumpAgain)
        {
            m_WallJumpTimer = 0;
            m_IsWallJumping = false;
        }
    }

    void MovePlayerFromWall()
    {
        m_CharacterController.Move(new Vector3(m_JumpHorizontalSpeed * transform.forward.x * Time.deltaTime, 0, m_JumpHorizontalSpeed * transform.forward.z * Time.deltaTime));
    }

    public void ShowDeathScreen()
    {
        m_DeathScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void HideDeathScreen()
    {
        Debug.Log("HideDeathScreen() called");
        m_DeathScreen.SetActive(false);
        m_RetryButton.SetActive(false);
        m_ExitButton.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void RetryButtonClicked()
    {
        GameController.GetGameController().RestartGame();
    }

    public void ExitButtonClicked()
    {
        Debug.Log("Exit button clicked");
        Application.Quit();
    }
}
