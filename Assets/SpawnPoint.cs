using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Header("스폰 설정")]
    public string spawnID; // 포탈이나 생성용 ID
    public Color gizmoColor = Color.green;

    // 씬 뷰에서 항상 가시성을 유지
    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        // 캐릭터 크기(약 2m)의 박스를 그려 위치 확인을 돕습니다. 
        Vector3 center = transform.position + Vector3.up * 1f;
        Gizmos.DrawWireCube(center, new Vector3(0.6f, 2f, 0.6f));
        
        // 캐릭터가 바라보는 정면 방향을 화살표로 표시합니다. 
        Gizmos.DrawRay(center, transform.forward * 1f);
    }
}