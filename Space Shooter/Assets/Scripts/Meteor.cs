using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    // Config
    [Header("Meteor")]
    [SerializeField] int health = 1000;
    [SerializeField] int scoreValue = 350;
    [SerializeField] GameObject meteorPowerUpDrop;
    [SerializeField] float powerUpDropSpeed = 5f;
    [SerializeField] float powerUpDropChance = 2f;

    GameObject powerUp;

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        StartCoroutine(damageDealer.TurnRed(gameObject));
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        if (health <= 0)
        {
            FindObjectOfType<GameSession>().AddToScore(scoreValue);
            Destroy(gameObject);
            if (Random.Range(0f, 1f) <= 1f / powerUpDropChance)
            {
                powerUp = Instantiate(meteorPowerUpDrop, transform.position, transform.rotation);
                powerUp.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -powerUpDropSpeed);
            }
        }
    }
}
