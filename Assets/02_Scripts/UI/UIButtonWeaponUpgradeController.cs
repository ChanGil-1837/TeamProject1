using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace UI
{
    [ExecuteAlways] // 에디터에서도 갱신되게
    [RequireComponent(typeof(UIButtonWeaponUpgrade))]
    public class UIButtonWeaponUpgradeController : MonoBehaviour
    {
        [SerializeField] private Image thumbnail;
        [SerializeField] private TextMeshProUGUI textLabel;
        [SerializeField] private TextMeshProUGUI costLabel;

        [SerializeField] private Sprite normalIcon;
        [SerializeField] private Sprite multiIcon;
        [SerializeField] private Sprite boundIcon;

        private UIButtonWeaponUpgrade buttonLogic;

        private void Awake()
        {
            buttonLogic = GetComponent<UIButtonWeaponUpgrade>();
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
                {
                    buttonLogic = GetComponent<UIButtonWeaponUpgrade>();
                }
                    
                UpdateUI();
                EditorApplication.QueuePlayerLoopUpdate();
            }
        }
    #endif

        public void UpdateUI()
        {
            if (buttonLogic == null) return;

            switch (buttonLogic.weaponType)
            {
                case WeaponType.Normal:
                    if (thumbnail) thumbnail.sprite = normalIcon;
                    break;
                case WeaponType.Multi:
                    if (thumbnail) thumbnail.sprite = multiIcon;
                    break;
                case WeaponType.Bounce:
                    if (thumbnail) thumbnail.sprite = boundIcon;
                    break;
            }

            if (textLabel)
            {
                switch (buttonLogic.upgradeType)
                {
                    case UIUpgradeType.Damage:
                        textLabel.text = $"ATK +{buttonLogic.upgradeAmount:F0}";
                        break;
                    case UIUpgradeType.FireRate:
                        float percent = (1 - buttonLogic.fireRateMultiplier) * 100f;
                        textLabel.text = $"SPD +{percent:F0}%";
                        break;
                    case UIUpgradeType.ShotCount:
                        textLabel.text = $"SHOT COUNT +{buttonLogic.shotIncrease}";
                        break;
                }
            }

            if(costLabel)
            {
                costLabel.text = $"{buttonLogic.upgradeCost}";
            }
        }
    }



}
