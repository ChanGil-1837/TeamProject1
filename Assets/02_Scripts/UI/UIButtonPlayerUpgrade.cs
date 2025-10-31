using UnityEngine;
using UnityEngine.UI;

namespace UI
{

    public class UIButtonPlayerUpgrade : MonoBehaviour
    {
        public UpgradeType upgradeType;
        public ulong upgradeCost;

        [SerializeField] private Button button;
        [SerializeField] private JHJ.Player playerTarget; // 인스펙터에 드래그 or 런타임 할당


        private void Awake()
        {
            if (button == null)
                button = GetComponent<Button>();
            playerTarget = GameObject.FindGameObjectWithTag("Player").GetComponent<JHJ.Player>();
        }

        private void Start()
        {
            button.onClick.AddListener(OnUpgradeClicked);
        }

        private void OnUpgradeClicked()
        {
            if (playerTarget == null)
            {
                return;
            }
            UpgradeManager.Instance.ApplyUpgrade(upgradeType);

        }

        public void SetPlayer(JHJ.Player player)
        {
            playerTarget = player;
        }
    }
}
