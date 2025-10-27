using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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

    [ExecuteAlways] // 에디터 모드에서도 실행 가능
    public class UIManager : MonoBehaviour
    {
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

        private void Start()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            weaponBtn.onClick.AddListener(() => { ContentType = UIContentsType.WEAPON; });
            healthBtn.onClick.AddListener(() => { ContentType = UIContentsType.HEALTH; });
            utilBtn.onClick.AddListener(() => { ContentType = UIContentsType.UTIL; });

            closeBtn.onClick.AddListener(() =>
            {
                isPanelOpen = !isPanelOpen;

                if (isPanelOpen)
                {
                    OpenPanel();
                }
                else
                {
                    ClosePanel();
                }
            });
        }

        private void ChangeContent(UIContentsType type)
        {
            foreach (var kvp in uiContentsDic)
            {
                if (kvp.Value != null)
                {
                    kvp.Value.SetActive(false);
                }
            }

            if (uiContentsDic.ContainsKey(type) && uiContentsDic[type] != null)
            {
                uiContentsDic[type].SetActive(true);
            }
        }

        private void OpenPanel()
        {
            RectTransform rect = contentPanel?.GetComponent<RectTransform>();
            if (rect == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                rect.DOSizeDelta(new Vector2(rect.sizeDelta.x, OPENSIZE), ANIMDURATION)
                    .SetEase(Ease.OutCubic);
            }
            else
            {
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, OPENSIZE);
            }
        }

        private void ClosePanel()
        {
            RectTransform rect = contentPanel?.GetComponent<RectTransform>();
            if (rect == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                rect.DOSizeDelta(new Vector2(rect.sizeDelta.x, CLOSEDSIZE), ANIMDURATION)
                    .SetEase(Ease.InCubic);
            }
            else
            {
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, CLOSEDSIZE);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (contentPanel == null)
            {
                return;
            }

            // 인스펙터에서 isPanelOpen 토글 시 즉시 반영
            if (isPanelOpen)
            {
                OpenPanel();
            }
            else
            {
                ClosePanel();
            }

            // 에디터 뷰 갱신
            if (!Application.isPlaying)
            {
                EditorApplication.QueuePlayerLoopUpdate();
            }
        }
#endif
    }
}
