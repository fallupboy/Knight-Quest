using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] int scorePerCoin = 150;
    [SerializeField] AudioClip[] pickupSounds;

    private void OnTriggerEnter2D(Collider2D player)
    {
        if (player.transform.CompareTag("Player"))
        {
            FindObjectOfType<Player>().PickUpCoin(scorePerCoin);
            AudioSource.PlayClipAtPoint(pickupSounds[Random.Range(0, pickupSounds.Length)], Camera.main.transform.position);
            Destroy(gameObject);
        }
    }
}
