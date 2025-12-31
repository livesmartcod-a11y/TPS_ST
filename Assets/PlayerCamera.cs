using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [Header("회전 설정")]
    public float mouseSensitivity = 100f;

    [Header("플레이어 본체 참조")]
    public Transform playerBody;

    [Header("수직 회전 제한")]
    public float minPitch = -90f;
    public float maxPitch = 90f;

    float xRotation = 0f;

    void Start()
    {
        // 게임 시작 시 마우스 커서를 화면 중앙에 고정하고 숨깁니다. 
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Mouse.current == null) return;

        // 새로운 Input System을 사용하여 마우스 델타(움직임) 값 받기
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime;

        // 수직 회전(Pitch) 계산: 마우스를 위로 올리면 xRotation 값이 작아지도록 설정합니다.
        xRotation -= mouseY;
        // 위아래로 너무 많이 회전하지 않도록 -90도에서 90도 사이로 제한(Clamp)합니다.
        xRotation = Mathf.Clamp(xRotation, minPitch, maxPitch);

        // 카메라의 수직 회전 적용 (상하 회전)
        // 부모인 Camera Holder는 고정되고 카메라만 위아래로 움직입니다.
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 플레이어 본체 회전 적용 (좌우 회전)
        // 본체 전체가 회전하므로 자식인 Camera Holder와 카메라도 함께 회전합니다.
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
