using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI playerHealthPointsText;
    [SerializeField] TextMeshProUGUI playerLivesText;
    [SerializeField] TextMeshProUGUI coinsText;
    [SerializeField] TextMeshProUGUI crystalsText;

    void Update()
    {
        UpdatePlayerHealthPointsText();
        UpdatePlayerLivesText();
        UpdatePlayerCoinsText();
        UpdatePlayerCrystalsText();
    }

    public void UpdatePlayerCoinsText()
    {
        coinsText.text = PlayerStats.Coins.ToString();
    }

    public void UpdatePlayerCrystalsText()
    {
        crystalsText.text = PlayerStats.Crystals.ToString();
    }

    public void UpdatePlayerHealthPointsText()
    {
        playerHealthPointsText.text = PlayerStats.HealthPoints.ToString();
    }

    public void UpdatePlayerLivesText()
    {
        playerLivesText.text = PlayerStats.Lives.ToString();
    }
}
