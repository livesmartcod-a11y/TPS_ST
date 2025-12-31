using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePortal : MonoBehaviour
{
    [Header("포탈 설정")]
    public string sceneToLoad;      // 이동할 씬 이름 (Build Settings에 등록되어 있어야 함)
    public float radius = 1.5f;     // 플레이어 감지 반경
    public LayerMask playerLayer;   // 플레이어 오브젝트의 레이어
    public Color portalColor = Color.red;
    public string targetSpawnID;    // 이동할 씬에서 사용할 스폰 지점 ID

   // 다음 씬으로 정보를 넘길 수 있도록 정적(static) 변수를 사용합니다.
    public static string nextSpawnID;

    private bool isTransitioning = false; // 씬 전환 중인지 확인하는 변수

    void Update()
    {
        if (isTransitioning) return; // 이미 전환 중이라면 아래 코드를 실행하지 않음

        // 포탈의 위치에서 반경 내에 플레이어 레이어를 가진 오브젝트가 있는지 체크
        if (Physics.CheckSphere(transform.position, radius, playerLayer))
        {
            if (string.IsNullOrEmpty(sceneToLoad) || SceneManager.GetActiveScene().name == sceneToLoad)
            {
                return;
            }

            isTransitioning = true;
            nextSpawnID = targetSpawnID; // 스폰 ID를 정적 변수에 저장
            Debug.Log($"{sceneToLoad} 씬으로 이동을 시작합니다.");
            SceneManager.LoadSceneAsync(sceneToLoad);
        }
    }

        // 에디터 뷰에서 포탈의 범위를 시각적으로 표시
    void OnDrawGizmos()
    {
        // 포탈 범위 표시 (반투명 구체)
        Gizmos.color = new Color(portalColor.r, portalColor.g, portalColor.b, 0.3f);
        Gizmos.DrawSphere(transform.position, radius);

        // 테두리 선 표시
        Gizmos.color = portalColor;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}