using JHJ;
using UnityEngine;

public class LineRenderer3D : MonoBehaviour
{
    // === 새로 추가된 변수 ===
    [Header("Circle Properties")]
    public float radius = 5f;               // 원의 반지름
    public int segments = 60;               // 원을 구성하는 선분(꼭짓점)의 개수 (해상도)

    [Header("Appearance")]
    public float circleThickness = 0.1f;    // **원의 경계 두께**
    public Material circleMaterial;          // **Line Renderer에 적용할 마테리얼**
    // ======================
    public Transform target; //플레이어 위치
    public string rangeField = ("_detectRange");
    public float refreshTime = 0.1f;

    private LineRenderer lineRenderer;
    private float timer;
    private float prevRadius;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("라인 렌더러 없음");
            return;
        }

        // --- 새로 추가된 설정 ---
        // 1. 선 두께 설정
        lineRenderer.startWidth = circleThickness;
        lineRenderer.endWidth = circleThickness;

        // 2. 마테리얼 설정
        if (circleMaterial != null)
        {
            lineRenderer.material = circleMaterial;
        }

        // Loop 설정을 위한 확인 (인스펙터에서 체크 필요)
        lineRenderer.positionCount = segments;
        // -------------------------

        CreatePoints();
    }
    private void Update()
    {
        if (target == null)
        {
            return;
        }
        timer += Time.deltaTime;
        if (timer >= refreshTime)
        {
            timer = 0;
            SyncTarget();
        }
        if (Mathf.Approximately(prevRadius, radius) == false)
        {
            prevRadius = radius;
            CreatePoints();
        }
    }
    public void UpdateSync()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
 
        CreatePoints();
    }
    private void SyncTarget()
    {
        Player player = target.GetComponent<Player>();
        if (player == null)
        {
            return;
        }
        radius = player.DetectRange;
    }

    void CreatePoints()
    {
        // 원의 중심은 이 GameObject의 Transform 위치 (transform.position)

        for (int i = 0; i < segments; i++)
        {
            // 360도를 segments 개수만큼 나눈 각도를 계산
            float angle = i * 360f / segments;

            // 각도를 라디안으로 변환
            float rad = angle * Mathf.Deg2Rad;

            // X, Z 평면 (수평)에서 원 위의 점의 위치 계산
            float x = radius * Mathf.Cos(rad);
            float z = radius * Mathf.Sin(rad);
            float y = 0f; // 수평 원이므로 Y축 값은 0 (또는 중심의 y값)

            // Line Renderer의 꼭짓점 위치 설정
            // transform.position을 더하여 GameObject의 위치를 중심으로 원을 그림
            Vector3 point = transform.position + new Vector3(x, y, z);
            lineRenderer.SetPosition(i, point);
        }
    }

    // 인스펙터에서 반지름이나 해상도를 변경하면 실시간으로 업데이트
    void OnValidate()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        if (lineRenderer != null)
        {
            // 선 두께 업데이트
            lineRenderer.startWidth = circleThickness;
            lineRenderer.endWidth = circleThickness;

            // 꼭짓점 개수 업데이트 및 재계산
            if (lineRenderer.positionCount != segments)
            {
                lineRenderer.positionCount = segments;
            }
            CreatePoints();
        }
    }
}