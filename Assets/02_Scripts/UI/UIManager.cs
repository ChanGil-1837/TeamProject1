using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public enum UIContentsType
{
    WEAPON,HEALTH,UTIL,NONE,
}

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button weaponBtn;
    [SerializeField] private Button healthBtn;
    [SerializeField] private Button utilBtn;
    [SerializeField] private Button tempBtn;
    [SerializeField] private Button closeBtn;
    // Start is called before the first frame update

    [SerializeField] private GameObject contentPanel;

    [SerializeField] private SerializedDictionary<UIContentsType, GameObject> uiContentsDic;

    private UIContentsType contentType = UIContentsType.NONE;

    [SerializeField] private bool isPanelOpen = false;

    const float CLOSEDSIZE = 80f;
    const float OPENSIZE = 720f;
    const float ANIMDURATION = 0.35f;
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

    void Start()
    {
        weaponBtn.onClick.AddListener(() =>
        {
            ContentType = UIContentsType.WEAPON;
        });
        healthBtn.onClick.AddListener(() =>
        {
            ContentType = UIContentsType.HEALTH;
        });
        utilBtn.onClick.AddListener(() =>
        {
            ContentType = UIContentsType.UTIL;
        });
        closeBtn.onClick.AddListener(() =>
        {
            if (!isPanelOpen)
            {
                isPanelOpen = true;
                OpenPanel();
            }
            else
            {
                isPanelOpen = false;
                ClosePanel();
            }
        });
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ChangeContent(UIContentsType type)
    {
        // 모든 콘텐츠 비활성화
        foreach (var kvp in uiContentsDic)
        {
            if (kvp.Value != null)
                kvp.Value.SetActive(false);
        }

        // 선택한 타입의 콘텐츠만 활성화
        if (uiContentsDic.ContainsKey(type) && uiContentsDic[type] != null)
        {
            uiContentsDic[type].SetActive(true);
        }
    }
    void OpenPanel()
    {
        RectTransform rect = contentPanel.GetComponent<RectTransform>();
        if (rect != null)
        {
            // DOTween으로 높이만 변경
            rect.DOSizeDelta(new Vector2(rect.sizeDelta.x, OPENSIZE), ANIMDURATION)
                .SetEase(Ease.OutCubic);
        }
    }

    void ClosePanel()
    {
        RectTransform rect = contentPanel.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.DOSizeDelta(new Vector2(rect.sizeDelta.x, CLOSEDSIZE), ANIMDURATION)
                .SetEase(Ease.InCubic);
        }
    }
}
