using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(UIButtonPlayerUpgrade))]
    public class UIButtonPlayerUpgradeController : MonoBehaviour
    {
        [SerializeField] private Image thumbnail;
        [SerializeField] private TextMeshProUGUI textLabel;
        [SerializeField] private TextMeshProUGUI costLabel;

        [SerializeField] private Sprite hpIcon;
        [SerializeField] private Sprite defIcon;
        [SerializeField] private Sprite regenIcon;
        [SerializeField] private Sprite detectIcon;

        private UIButtonPlayerUpgrade buttonLogic;

        private void Awake()
        {
            buttonLogic = GetComponent<UIButtonPlayerUpgrade>();
        }

        private void Start()
        {
            UpdateUI();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                if (buttonLogic == null)
                    buttonLogic = GetComponent<UIButtonPlayerUpgrade>();
                UpdateUI();
                EditorApplication.QueuePlayerLoopUpdate();
            }
        }
#endif

        public void UpdateUI()
        {
            if (buttonLogic == null) return;

            // --- 아이콘 변경 ---
            if (thumbnail)
            {
                switch (buttonLogic.upgradeType)
                {
                    case UpgradeType.MaxHp: thumbnail.sprite = hpIcon; break;
                    case UpgradeType.Defense: thumbnail.sprite = defIcon; break;
                    case UpgradeType.HpRegen: thumbnail.sprite = regenIcon; break;
                    case UpgradeType.DetectRange: thumbnail.sprite = detectIcon; break;
                }
            }

            // --- 텍스트 변경 ---
            if (textLabel)
            {
                switch (buttonLogic.upgradeType)
                {
                    case UpgradeType.MaxHp: textLabel.text = "HP +"; break;
                    case UpgradeType.Defense: textLabel.text = "DEF +"; break;
                    case UpgradeType.HpRegen: textLabel.text = "REGEN +"; break;
                    case UpgradeType.DetectRange: textLabel.text = "RANGE +"; break;
                }
            }

            // --- 비용 표시 ---
            if (costLabel)
            {
                costLabel.text = $"{buttonLogic.upgradeCost}";
            }
        }
    }
}
