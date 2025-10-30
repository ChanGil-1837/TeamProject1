using System.Collections;
using System.Collections.Generic;
using TeamProject.GameSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField]private JHJ.Player player;
    [SerializeField]private Image playerHPBar;
    [SerializeField]private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI healthRegenText;
    
    [SerializeField]private Image waveBar;
    [SerializeField]private TextMeshProUGUI gradeText;
    [SerializeField]private TextMeshProUGUI gradeInfoText;
    [SerializeField]private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI waveInfoText;
    
    void Start()
    {
        player = FindObjectOfType<JHJ.Player>();
        if (player != null)
        {
            player.OnStatsChanged += UpdatePlayerUI;
            player.OnGoldChanged += UpdateGoldUI;
            UpdatePlayerUI(); // Initial UI update
            UpdateGoldUI(player.Gold); // Initial Gold UI update
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnWaveChanged += UpdateWaveUI;
            UpdateWaveUI();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    void UpdateWaveUI()
    {
        if (GameManager.Instance == null) return;

        float ratio = Mathf.Clamp01(GameManager.Instance.waveRemain / GameManager.Instance.waveDuration);
        waveBar.fillAmount = ratio;

        waveText.text = $"Wave {GameManager.Instance.currentWave}";
    }

    void UpdatePlayerUI()
    {
        if (player == null) return;

        playerHPBar.fillAmount = player.CurrentHp / player.MaxHp;
        damageText.text = "Damage "+player.TotalDPS.ToString("F1") + "/s";
        healthRegenText.text = "Health Regen "+player.HpRegen.ToString("F1");
    }

    void UpdateGoldUI(int gold)
    {
        gradeInfoText.text = $"Gold: {gold}";
    }
    
}
