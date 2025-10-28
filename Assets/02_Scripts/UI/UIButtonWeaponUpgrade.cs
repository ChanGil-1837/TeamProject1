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

            switch (upgradeType)
            {
                case UIUpgradeType.Damage:
                    targetWeapon.UpgradeDamage(upgradeAmount);
                    break;

                case UIUpgradeType.FireRate:
                    targetWeapon.UpgradeFireRate(fireRateMultiplier);
                    break;

                case UIUpgradeType.ShotCount:
                    if (targetWeapon is MultiWeapon multi)
                    {
                        multi.UpgradeShotCount(shotIncrease);
                    }
                    break;
            }
        }

        public void SetWeapon(Weapon weapon)
        {
            targetWeapon = weapon;
        }
    }

}
