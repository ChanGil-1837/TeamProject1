using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UI
{
    public enum UpgradeType
    {
        MaxHp,
        Defense,
        HpRegen,
        DetectRange,

        GoldMultiplier,
        IZA,

        NormalDamage,
        NormalFireRate,

        MultiDamage,
        MultiFireRate,
        MultiShotCount,

        BounceDamage,
        BounceFireRate
    }
    [System.Serializable]
    public struct UpgradeData
    {
        public int startCost;
        public float costMultiplier; 
        public float valueStep;  
    }

    public class UpgradeManager : MonoBehaviour
    {
        public static UpgradeManager Instance { get; private set; }
        [SerializeField] private Player.Player player;
        [SerializeField] private NormalWeapon normalWeapon;
        [SerializeField] private MultiWeapon multiWeapon;
        [SerializeField] private BounceWeapon bounceWeapon;

        [System.Serializable]
        public struct UpgradeData
        {
            public int startCost;
            public float costMultiplier;
            public float valueStep;
        }

        private Dictionary<UpgradeType, UpgradeData> config = new();
        private Dictionary<UpgradeType, int> currentCost = new();
        private Dictionary<UpgradeType, int> upgradeLevel = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            config[UpgradeType.MaxHp]          = new UpgradeData { startCost = 100, costMultiplier = 1.2f, valueStep = 10 };
            config[UpgradeType.Defense]        = new UpgradeData { startCost = 120, costMultiplier = 1.2f, valueStep = 1 };
            config[UpgradeType.HpRegen]        = new UpgradeData { startCost = 150, costMultiplier = 1.25f, valueStep = 0.5f };
            config[UpgradeType.DetectRange]    = new UpgradeData { startCost = 80, costMultiplier = 1.1f, valueStep = 1 };
            config[UpgradeType.GoldMultiplier] = new UpgradeData { startCost = 200, costMultiplier = 1.5f, valueStep = 0.1f };

            config[UpgradeType.NormalDamage]   = new UpgradeData { startCost = 100, costMultiplier = 1.3f, valueStep = 5 };
            config[UpgradeType.NormalFireRate] = new UpgradeData { startCost = 100, costMultiplier = 1.25f, valueStep = 0.9f };

            config[UpgradeType.MultiDamage]    = new UpgradeData { startCost = 150, costMultiplier = 1.3f, valueStep = 4 };
            config[UpgradeType.MultiFireRate]  = new UpgradeData { startCost = 150, costMultiplier = 1.25f, valueStep = 0.9f };
            config[UpgradeType.MultiShotCount] = new UpgradeData { startCost = 200, costMultiplier = 1.35f, valueStep = 1 };

            config[UpgradeType.BounceDamage]   = new UpgradeData { startCost = 180, costMultiplier = 1.3f, valueStep = 6 };
            config[UpgradeType.BounceFireRate] = new UpgradeData { startCost = 180, costMultiplier = 1.25f, valueStep = 0.9f };

            foreach (var type in config.Keys)
            {
                currentCost[type] = config[type].startCost;
                upgradeLevel[type] = 0;
            }
        }

        public int GetCost(UpgradeType type) => currentCost[type];
        public int GetLevel(UpgradeType type) => upgradeLevel[type];

        public void ApplyUpgrade(UpgradeType type)
        {
            if (!config.TryGetValue(type, out var data))
            {
                return;
            }

            int cost = currentCost[type];
            if (!player.SpendGold(cost))
            {
                //업그레이드 돈 부족으로 인해 불가 판정 
                return;
            }

            upgradeLevel[type]++;
            currentCost[type] = Mathf.RoundToInt(cost * data.costMultiplier);

            switch (type)
            {
                case UpgradeType.MaxHp:          
                case UpgradeType.Defense:        
                case UpgradeType.HpRegen:        
                case UpgradeType.DetectRange:    player.UpgradeStats(type,data.valueStep); break;

                // case UpgradeType.GoldMultiplier: player.GoldMultiplierUp(data.valueStep); break;

                case UpgradeType.NormalDamage:   normalWeapon.UpgradeDamage((int)data.valueStep); break;
                case UpgradeType.NormalFireRate: normalWeapon.UpgradeFireRate(data.valueStep); break;

                case UpgradeType.MultiDamage:    multiWeapon.UpgradeDamage((int)data.valueStep); break;
                case UpgradeType.MultiFireRate:  multiWeapon.UpgradeFireRate(data.valueStep); break;
                case UpgradeType.MultiShotCount: multiWeapon.UpgradeShotCount((int)data.valueStep); break;

                case UpgradeType.BounceDamage:   bounceWeapon.UpgradeDamage((int)data.valueStep); break;
                case UpgradeType.BounceFireRate: bounceWeapon.UpgradeFireRate(data.valueStep); break;
            }

        }
    }

}
