using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] int damage = 100;

    public IEnumerator TurnRed(GameObject gameObject)
    {
        SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer>();
        Color currColor = sprite.material.color;
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        sprite.color = currColor;
    }

    public int GetDamage()
    {
        return damage;
    }

    public void Hit()
    {
        Destroy(gameObject);
    }
}
