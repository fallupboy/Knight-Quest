using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] float health = 100;
    [SerializeField] int scoreValue = 150;

    [Header("Shooting")]
    [SerializeField] float mintimeBetweenShots = 0.3f;
    [SerializeField] float maxTimeBetweenShots = 2f;
    [SerializeField] GameObject projectile;
    [SerializeField] float projectileSpeed = 10f;
    float shotCounter;

    [Header("SFX")]
    [SerializeField] GameObject deathVFX;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] [Range(0, 1)] float deathSFXVolume = 1f;
    [SerializeField] AudioClip shootSFX;
    [SerializeField] [Range(0, 1)] float shootSFXVolume = 0.5f;

    [Header("Drops After Death")]
    [SerializeField] GameObject coinDrop;
    [SerializeField] float coinDropChance = 3f;
    [SerializeField] float coinDropSpeed = 5f;

    private void Start()
    {
        shotCounter = Random.Range(mintimeBetweenShots, maxTimeBetweenShots);
    }

    private void Update()
    {
        CountDownAndShoot();
    }

    private void CountDownAndShoot()
    {
        shotCounter -= Time.deltaTime;
        if (shotCounter <= 0f)
        {
            Fire();
            shotCounter = Random.Range(mintimeBetweenShots, maxTimeBetweenShots);
        }
    }

    private void Fire()
    {
        GameObject laser = Instantiate(projectile, transform.position, Quaternion.identity);
        laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -projectileSpeed);
        AudioSource.PlayClipAtPoint(shootSFX, Camera.main.transform.position, shootSFXVolume);
    }

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
            Die();
        }
    }

    private void Die()
    {
        FindObjectOfType<GameSession>().AddToScore(scoreValue);
        Destroy(gameObject);
        Explode();
        AudioSource.PlayClipAtPoint(deathSFX, Camera.main.transform.position, deathSFXVolume);
        DropCoin();
    }

    private void Explode()
    {
        GameObject explosion = Instantiate(deathVFX, transform.position, transform.rotation);
        Destroy(explosion, 1f);
    }

    private void DropCoin()
    {
        if (Random.Range(0f, 1f) <= 1f / coinDropChance)
        {
            GameObject coin = Instantiate(coinDrop, transform.position, transform.rotation);
            coin.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -coinDropSpeed);
        }
    }
}
