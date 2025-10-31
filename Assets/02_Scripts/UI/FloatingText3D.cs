
using UnityEngine;
using TMPro;
using System.Collections;

public class FloatingText3D : MonoBehaviour
{
    [Tooltip("페이드인 되는 시간")]
    public float fadeInDuration = 0.5f;

    [Tooltip("떠오르는 시간")]
    public float floatDuration = 1.0f;

    [Tooltip("페이드아웃 되는 시간")]
    public float fadeOutDuration = 0.5f;

    [Tooltip("떠오르는 거리")]
    public float floatDistance = 1.0f;

    private TextMeshPro textMesh;
    private Vector3 startPosition;

    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        if (textMesh == null)
        {
            Debug.LogError("TextMeshPro 컴포넌트를 찾을 수 없습니다. FloatingText3D 스크립트는 TextMeshPro 오브젝트에 추가해야 합니다.");
            enabled = false;
        }
    }

    void Start()
    {
        startPosition = transform.position;
        StartCoroutine(AnimateText());
    }

    private IEnumerator AnimateText()
    {
        // 초기 알파값 0으로 설정
        Color originalColor = textMesh.color;
        textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);

        // 1. 페이드인
        float fadeInTimer = 0f;
        while (fadeInTimer < fadeInDuration)
        {
            fadeInTimer += Time.deltaTime;
            float alpha = Mathf.Clamp01(fadeInTimer / fadeInDuration);
            textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1);

        // 2. 떠오르기
        float floatTimer = 0f;
        Vector3 endPosition = startPosition + new Vector3(0, floatDistance, 0);
        while (floatTimer < floatDuration)
        {
            floatTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(floatTimer / floatDuration);
            transform.position = Vector3.Lerp(startPosition, endPosition, progress);
            yield return null;
        }
        transform.position = endPosition;

        // 3. 페이드아웃
        float fadeOutTimer = 0f;
        while (fadeOutTimer < fadeOutDuration)
        {
            fadeOutTimer += Time.deltaTime;
            float alpha = 1 - Mathf.Clamp01(fadeOutTimer / fadeOutDuration);
            textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);

        // 4. 오브젝트 파괴
        Destroy(gameObject);
    }

    /// <summary>
    /// 텍스트 내용을 설정합니다.
    /// </summary>
    /// <param name="text">표시할 텍스트</param>
    public void SetText(string text)
    {
        if (textMesh != null)
        {
            textMesh.text = text;
        }
    }
}
