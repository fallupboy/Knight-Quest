using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [SerializeField] int coinValue = 1;
    [SerializeField] int crystalValue = 1;
    [SerializeField] AudioClip coinPickUpSFX;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == FindObjectOfType<Player>().myButtomCollider)
        {
            if (CompareTag("Crystal"))
            {
                AudioSource.PlayClipAtPoint(coinPickUpSFX, transform.position);
                PlayerStats.Crystals += crystalValue;
                Destroy(gameObject);
            }
            else if (CompareTag("Coin"))
            {
                AudioSource.PlayClipAtPoint(coinPickUpSFX, transform.position);
                PlayerStats.Coins += coinValue;
                Destroy(gameObject);
            }
        }
    }
}
