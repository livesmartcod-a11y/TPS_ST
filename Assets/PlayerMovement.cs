using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("이동 설정")]
    public float speed = 12f;
    public float runSpeed = 20f;
    public float jumpHeight = 3f;
    public float gravity = -9.81f;
    public float fallMultiplier = 2.5f; // 떨어질 때 중력 배율

    [Header("지면 체크 설정")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    CharacterController controller;
    Vector3 velocity;
    bool isGrounded;

    void Awake()
    {
        // 씬이 바뀌어도 플레이어 오브젝트가 파괴되지 않도록 설정
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        // 씬 시작 시 속도 초기화
        velocity = Vector3.zero;

        // 씬 전환 후 저장된 스폰 ID가 있는지 확인합니다. 
        if (!string.IsNullOrEmpty(ScenePortal.nextSpawnID))
        {
            SpawnPoint[] spawnPoints = Object.FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);
            foreach (SpawnPoint sp in spawnPoints)
            {
                if (sp.spawnID == ScenePortal.nextSpawnID)
                {
                    // CharacterController가 켜져 있으면 위치 직접 이동이 무시될 수 있으므로 잠시 끕니다.
                    controller.enabled = false;
                    transform.position = sp.transform.position;
                    transform.rotation = sp.transform.rotation;
                    controller.enabled = true;
                    
                    Debug.Log($"SpawnPoint '{sp.spawnID}' 위치로 설정되었습니다.");
                    break;
                }
            }
            // 스폰 ID를 초기화합니다.
            ScenePortal.nextSpawnID = null;
        }
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // groundCheck가 할당되지 않았을 경우 에러 방지를 위해 업데이트를 중단합니다.
        if (groundCheck == null)
        {
            Debug.LogWarning("Ground Check 오브젝트가 할당되지 않았습니다! 플레이어 발밑에 빈 오브젝트를 생성해 할당해주세요.");
            return;
        }

        // 발밑에 지면 레이어를 가진 물체가 있는지 체크
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // WASD 입력 받기
        float x = 0;
        float z = 0;

        if (Keyboard.current.wKey.isPressed) z += 1;
        if (Keyboard.current.sKey.isPressed) z -= 1;
        if (Keyboard.current.aKey.isPressed) x -= 1;
        if (Keyboard.current.dKey.isPressed) x += 1;

        // Shift 키를 누르고 있으면 달리기 속도 적용
        float currentSpeed = Keyboard.current.leftShiftKey.isPressed ? runSpeed : speed;

        // 플레이어가 바라보는 방향 기준으로 이동 방향 계산
        Vector3 move = transform.right * x + transform.forward * z;

        // 중력 적용 (바닥에 붙어있게 함)
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f; // 바닥에 닿아있을 때 중력을 살짝 아래로 유지

        // 점프 입력 처리 (바닥에 있을 때만 가능)
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        // 중력 가속
        if (velocity.y < 0)
        {
            // 떨어지는 중일 때 중력을 더 강하게 적용하여 묵직하게 낙하
            velocity.y += gravity * fallMultiplier * Time.deltaTime;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        // 모든 이동(WASD + 중력/점프)을 하나의 벡터로 합쳐서 실제 이동
        Vector3 finalMove = (move * currentSpeed) + velocity;
        controller.Move(finalMove * Time.deltaTime);
    }

    // 에디터에서 지면 체크 범위를 시각적으로 확인하기 위한 코드
    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}