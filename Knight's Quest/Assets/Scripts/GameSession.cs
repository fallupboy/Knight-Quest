using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    [SerializeField] int startPlayerHealthPoints = 100;
    [SerializeField] int startPlayerLives = 3;
    [SerializeField] int startCoins = 0;
    [SerializeField] int startCrystals = 0;

    UI GameSessionUI;

    private void Start()
    {
        GameSessionUI = FindObjectOfType<UI>();
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Main Menu" || 
            SceneManager.GetActiveScene().name == "End Game Success" ||
            SceneManager.GetActiveScene().name == "How To Play Menu")
        {
            if (GameSessionUI.gameObject == null) { return; }
            else
            {
                GameSessionUI.gameObject.SetActive(false);
            }
        }
    }

    public void ProccessPlayerDeath()
    {
        if (PlayerStats.Lives > 1)
        {
            TakePlayerLife();
            PlayerStats.HealthPoints = 100;
        }
        else
        {
            ResetGameSession();
        }
    }
    
    public void TakePlayerLife()
    {
        PlayerStats.Lives--;
        PlayerStats.Crystals = startCrystals;
        var currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene);
    }

    public void TakePlayerHealthPoints(int amount)
    {
        FindObjectOfType<Player>().PlayTakeDamageSFX();
        PlayerStats.HealthPoints -= amount;
        if (PlayerStats.HealthPoints <= 0)
        {
            StartCoroutine(FindObjectOfType<Player>().Die());
        }
    }

    public void DecreaseCoins(int amount)
    {
        PlayerStats.Coins -= amount;
    }

    public void DecreaseCrystals(int amount)
    {
        PlayerStats.Crystals -= amount;
    }

    public void ResetGameSession()
    {
        PlayerStats.HealthPoints = startPlayerHealthPoints;
        PlayerStats.Lives = startPlayerLives;
        PlayerStats.Coins = startCoins;
        PlayerStats.Crystals = startCrystals;
        FindObjectOfType<SceneLoader>().LoadMenuScene();
        Destroy(gameObject);
    }
}
