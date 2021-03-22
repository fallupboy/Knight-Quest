using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] AudioClip pickUpSound;

    Player player;

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player.RestoreHealth();
            AudioSource.PlayClipAtPoint(pickUpSound, Camera.main.transform.position);
            Destroy(gameObject);
        }
    }
}
