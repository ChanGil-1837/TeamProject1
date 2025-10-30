using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 50f; // 텍스트가 떠오르는 속도
    [SerializeField] private float duration = 0.5f; // 텍스트가 유지되는 시간
    private float timer;
    private TextMeshProUGUI textMesh;
    private RectTransform rectTransform;
    private Color startColor;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
        startColor = textMesh.color;
    }

    public void Initialize(string message)
    {
        textMesh.text = message;
        timer = duration;
        textMesh.color = startColor; // 초기 색상으로 설정
        // 이 함수를 호출할 때 마우스 위치로 위치 설정이 이루어져야 합니다.
    }

    private void Update()
    {
        // 텍스트를 위로 이동
        rectTransform.anchoredPosition += new Vector2(0, moveSpeed) * Time.deltaTime;

        // 시간 감소
        timer -= Time.deltaTime;

        // 페이드 아웃
        if (timer <= 0)
        {
            float fadeAmount = timer / -0.1f; // 마지막 0.1초 동안 빠르게 투명하게
            if (fadeAmount < 1)
            {
                textMesh.color = new Color(startColor.r, startColor.g, startColor.b, fadeAmount);
            }
            if (timer <= -0.1f)
            {
                // 시간 초과 시 인스턴스 파괴
                Destroy(gameObject); 
            }
        }
    }
}