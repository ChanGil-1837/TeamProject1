using System.Collections;
using System.Collections.Generic;
using TeamProject.GameSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField]private Image playerHPBar;
    [SerializeField]private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI healthRegenText;
    
    [SerializeField]private Image waveBar;
    [SerializeField]private TextMeshProUGUI gradeText;
    [SerializeField]private TextMeshProUGUI gradeInfoText;
    [SerializeField]private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI waveInfoText;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateWaveUI();
    }
    
    void UpdateWaveUI()
    {
        float ratio = Mathf.Clamp01(GameManager.Instance.waveRemain / GameManager.Instance.waveDuration);
        waveBar.fillAmount = ratio;

        waveText.text = $"Wave {GameManager.Instance.currentWave}";
    }
}
