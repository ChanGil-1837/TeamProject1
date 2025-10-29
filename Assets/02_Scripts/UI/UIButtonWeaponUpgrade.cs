using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public enum UIUpgradeType
    {
        Damage,
        FireRate,
        ShotCount
    }

    public class UIButtonWeaponUpgrade : MonoBehaviour
    {
        public UIUpgradeType upgradeType;
        public WeaponType weaponType;
        public ulong upgradeCost;
        public int upgradeAmount = 1;
        public float fireRateMultiplier = 0.9f;
        public int shotIncrease = 1;

        [SerializeField] private Button button;
        [SerializeField] private Weapon targetWeapon; // 인스펙터에 드래그하거나, 런타임에 할당
        private void Awake()
        {
            if (button == null)
                button = GetComponent<Button>();

        }

        private void Start()
        {
            button.onClick.AddListener(ApplyUpgrade);
        }

        private void ApplyUpgrade()
        {
            if (targetWeapon == null)
            {
                Debug.LogWarning("WeaponUpgradeButton: targetWeapon이 할당되지 않음!");
                return;
            }
            UpgradeType type = UpgradeTypeConverter.ToUpgradeType(weaponType, upgradeType);

            UpgradeManager.Instance.ApplyUpgrade(type);
        }

        public void SetWeapon(Weapon weapon)
        {
            targetWeapon = weapon;
        }

        public static class UpgradeTypeConverter
        {
            public static UpgradeType ToUpgradeType(WeaponType weaponType, UIUpgradeType uiType)
            {
                switch (weaponType)
                {
                    case WeaponType.Normal:
                        return uiType switch
                        {
                            UIUpgradeType.Damage => UpgradeType.NormalDamage,
                            UIUpgradeType.FireRate => UpgradeType.NormalFireRate,
                            _ => throw new System.ArgumentOutOfRangeException()
                        };

                    case WeaponType.Multi:
                        return uiType switch
                        {
                            UIUpgradeType.Damage => UpgradeType.MultiDamage,
                            UIUpgradeType.FireRate => UpgradeType.MultiFireRate,
                            UIUpgradeType.ShotCount => UpgradeType.MultiShotCount,
                            _ => throw new System.ArgumentOutOfRangeException()
                        };

                    case WeaponType.Bounce:
                        return uiType switch
                        {
                            UIUpgradeType.Damage => UpgradeType.BounceDamage,
                            UIUpgradeType.FireRate => UpgradeType.BounceFireRate,
                            _ => throw new System.ArgumentOutOfRangeException()
                        };

                    default:
                        throw new System.ArgumentOutOfRangeException(nameof(weaponType), weaponType, null);
                }
            }
        }
    }

}
