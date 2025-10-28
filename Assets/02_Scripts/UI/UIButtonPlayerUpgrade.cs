using UnityEngine;
using UnityEngine.UI;

namespace UI
{

    public class UIButtonPlayerUpgrade : MonoBehaviour
    {
        public UpgradeType upgradeType;
        public ulong upgradeCost;

        [SerializeField] private Button button;
        [SerializeField] private Player playerTarget; // 인스펙터에 드래그 or 런타임 할당

        private void Awake()
        {
            if (button == null)
                button = GetComponent<Button>();
            playerTarget = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }

        private void Start()
        {
            button.onClick.AddListener(OnUpgradeClicked);
        }

        private void OnUpgradeClicked()
        {
            if (playerTarget == null)
            {
                Debug.LogWarning("UIButtonPlayerUpgrade: playerTarget이 설정되지 않음!");
                return;
            }

            playerTarget.UpgradeStats(upgradeType);
        }

        public void SetPlayer(Player player)
        {
            playerTarget = player;
        }
    }
}
