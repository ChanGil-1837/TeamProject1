using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro; // TextMeshPro 사용을 위해 추가

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UI
{
    public enum UIContentsType
    {
        WEAPON,
        HEALTH,
        UTIL,
        NONE,
    }

    [ExecuteAlways]
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; } // 싱글톤 인스턴스

        [Header("Event Text Settings")]
        [SerializeField] private GameObject eventTextPrefab; // 이벤트 메시지를 표시할 TextMeshPro UI 프리팹
        [SerializeField] private Transform mainCanvasTransform; // UI가 생성될 부모 캔버스

        [Header("Upgrade Panel Settings")]
        [SerializeField] private Button weaponBtn;
        [SerializeField] private Button healthBtn;
        [SerializeField] private Button utilBtn;
        [SerializeField] private Button tempBtn;
        [SerializeField] private Button closeBtn;

        [SerializeField] private GameObject contentPanel;
        [SerializeField] private SerializedDictionary<UIContentsType, GameObject> uiContentsDic;

        [SerializeField] private bool isPanelOpen = false;
        private UIContentsType contentType = UIContentsType.NONE;

        private const float CLOSEDSIZE = 80f;
        private const float OPENSIZE = 720f;
        private const float ANIMDURATION = 0.35f;

        public UIContentsType ContentType
        {
            get => contentType;
            set
            {
                if (value == UIContentsType.NONE)
                {
                    // NONE이면 모든 콘텐츠 끄기
                    foreach (var kvp in uiContentsDic)
                    {
                        if (kvp.Value != null)
                            kvp.Value.SetActive(false);
                    }
                    contentType = UIContentsType.NONE;
                    return;
                }

                if (!isPanelOpen)
                {
                    isPanelOpen = true;
                    OpenPanel();
                }

                if (contentType != value)
                {
                    ChangeContent(value);
                    contentType = value;
                }
            }
        }

        private void Awake()
        {
            // 싱글톤 설정
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif
            weaponBtn.onClick.AddListener(() => { ContentType = UIContentsType.WEAPON; });
            healthBtn.onClick.AddListener(() => { ContentType = UIContentsType.HEALTH; });
            utilBtn.onClick.AddListener(() => { ContentType = UIContentsType.UTIL; });

            closeBtn.onClick.AddListener(() =>
            {
                // 닫기 전용
                ClosePanel();
                ContentType = UIContentsType.NONE;
                isPanelOpen = false;
            });
        }

        /// <summary>
        /// 화면에 이벤트 텍스트를 페이드인/아웃 효과와 함께 표시하고, 그 동안 시간을 느리게 합니다.
        /// </summary>
        /// <param name="message">표시할 메시지</param>
        /// <param name="duration">전체 효과 지속 시간</param>
        /// <param name="fadeInTime">페이드인 시간</param>
        /// <param name="fadeOutTime">페이드아웃 시간</param>
        public void ShowEventText(string message, float duration, float fadeInTime = 0.5f, float fadeOutTime = 0.5f)
        {
            if (eventTextPrefab == null)
            {
                Debug.LogError("Event Text Prefab is not assigned in UIManager.");
                return;
            }
            if (mainCanvasTransform == null)
            {
                Debug.LogError("Main Canvas Transform is not assigned in UIManager.");
                return;
            }

            // TimeManager를 호출하여 시간 느리게 만들기
            TeamProject.GameSystem.TimeManager.Instance.SlowDownForDuration(duration);

            // 프리팹 생성
            GameObject textObject = Instantiate(eventTextPrefab, mainCanvasTransform);
            TMP_Text textComponent = textObject.GetComponent<TMP_Text>();

            if (textComponent == null)
            {
                Debug.LogError("Event Text Prefab must have a TMP_Text component.");
                Destroy(textObject);
                return;
            }

            textComponent.text = message;
            textComponent.alpha = 0; // 초기 알파값을 0으로 설정
            
            // DOTween 시퀀스를 사용하여 애니메이션 체인 만들기
            Sequence textSequence = DOTween.Sequence();
            textSequence.SetUpdate(true); // Time.timeScale의 영향을 받지 않도록 설정

            // 1. 페이드인
            textSequence.Append(textComponent.DOFade(1f, fadeInTime));
            
            // 2. 잠시 대기
            float holdTime = duration - (fadeInTime + fadeOutTime);
            if (holdTime > 0)
            {
                textSequence.AppendInterval(holdTime);
            }

            // 3. 페이드아웃
            textSequence.Append(textComponent.DOFade(0f, fadeOutTime));

            // 4. 애니메이션 완료 후 오브젝트 파괴
            textSequence.OnComplete(() => {
                Destroy(textObject);
            });
        }

        private void ChangeContent(UIContentsType type)
        {
            foreach (var kvp in uiContentsDic)
            {
                if (kvp.Value != null)
                    kvp.Value.SetActive(false);
            }

            if (uiContentsDic.ContainsKey(type) && uiContentsDic[type] != null)
            {
                uiContentsDic[type].SetActive(true);
            }
        }

        private void OpenPanel()
        {
            RectTransform rect = contentPanel?.GetComponent<RectTransform>();
            if (rect == null) return;

            if (Application.isPlaying)
                rect.DOSizeDelta(new Vector2(rect.sizeDelta.x, OPENSIZE), ANIMDURATION).SetEase(Ease.OutCubic);
            else
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, OPENSIZE);
        }

        private void ClosePanel()
        {
            RectTransform rect = contentPanel?.GetComponent<RectTransform>();
            if (rect == null) return;

            if (Application.isPlaying)
                rect.DOSizeDelta(new Vector2(rect.sizeDelta.x, CLOSEDSIZE), ANIMDURATION).SetEase(Ease.InCubic);
            else
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, CLOSEDSIZE);

            // 닫을 때 콘텐츠 전부 비활성화
            foreach (var kvp in uiContentsDic)
            {
                if (kvp.Value != null)
                    kvp.Value.SetActive(false);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (contentPanel == null)
                return;

            if (isPanelOpen)
                OpenPanel();
            else
                ClosePanel();

            if (!Application.isPlaying)
                EditorApplication.QueuePlayerLoopUpdate();
        }
#endif
    }
}