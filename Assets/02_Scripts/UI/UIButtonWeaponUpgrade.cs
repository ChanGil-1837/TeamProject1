using UnityEngine;
using UnityEngine.UI;

public enum UpgradeType
{
    Damage,
    FireRate,
    ShotCount
}

public class UIButtonWeaponUpgrade : MonoBehaviour
{
    public UpgradeType upgradeType;
    public WeaponType weaponType;
    public ulong upgradeCost;
    public float upgradeAmount = 1f;
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
            case UpgradeType.Damage:
                // targetWeapon.UpgradeDamage(upgradeAmount);
                break;

            case UpgradeType.FireRate:
                // targetWeapon.UpgradeFireRate(fireRateMultiplier);
                break;

            case UpgradeType.ShotCount:
                // if (targetWeapon is MultiShotWeapon multi)
                //{
                //     multi.UpgradeShotCount(shotIncrease);
                //}
                break;
        }
    }

    public void SetWeapon(Weapon weapon)
    {
        targetWeapon = weapon;
    }
}
