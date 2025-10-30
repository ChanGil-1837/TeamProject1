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
        [SerializeField] private JHJ.Player player;
        [SerializeField] private NormalWeapon normalWeapon;
        [SerializeField] private MultiWeapon multiWeapon;
        [SerializeField] private BounceWeapon bounceWeapon;

        public GameObject floatingTextPrefab;
        public Canvas mainCanvas;

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
                ShowFloatingText("Not enough gold");
                return;
            }

            upgradeLevel[type]++;
            currentCost[type] = Mathf.RoundToInt(cost * data.costMultiplier);

            switch (type)
            {
                case UpgradeType.MaxHp:
                case UpgradeType.Defense:
                case UpgradeType.HpRegen:
                case UpgradeType.DetectRange: player.UpgradeStats(type, data.valueStep); break;

                // case UpgradeType.GoldMultiplier: player.GoldMultiplierUp(data.valueStep); break;

                case UpgradeType.NormalDamage: normalWeapon.UpgradeDamage((int)data.valueStep); break;
                case UpgradeType.NormalFireRate: normalWeapon.UpgradeFireRate(data.valueStep); break;

                case UpgradeType.MultiDamage: multiWeapon.UpgradeDamage((int)data.valueStep); break;
                case UpgradeType.MultiFireRate: multiWeapon.UpgradeFireRate(data.valueStep); break;
                case UpgradeType.MultiShotCount: multiWeapon.UpgradeShotCount((int)data.valueStep); break;

                case UpgradeType.BounceDamage: bounceWeapon.UpgradeDamage((int)data.valueStep); break;
                case UpgradeType.BounceFireRate: bounceWeapon.UpgradeFireRate(data.valueStep); break;
            }
            ShowFloatingText("Upgrade Complete");
        }
        private void ShowFloatingText(string message)
        {
            if (floatingTextPrefab == null || mainCanvas == null)
            {
                Debug.LogError("FloatingText Prefab 또는 Main Canvas가 할당되지 않았습니다.");
                return;
            }

            GameObject go = Instantiate(floatingTextPrefab, mainCanvas.transform); 
            
            Vector3 mousePosition = Input.mousePosition; 
            
            RectTransform rectTransform = go.GetComponent<RectTransform>();
            rectTransform.position = mousePosition + new Vector3(0, 50, 0); // Y축으로 50픽셀 위로 살짝 띄움

            FloatingText ft = go.GetComponent<FloatingText>();
            if (ft != null)
            {
                ft.Initialize(message);
            }
        }
    }

}
