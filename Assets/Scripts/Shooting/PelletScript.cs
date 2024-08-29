using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class PelletScript : MonoBehaviour {
    [NonSerialized] public GameObject originalPlayer;
    [NonSerialized] public Vector2 travelVector;
    private float damage = 0.9f;
    private Rigidbody2D rb;

    void OnEnable() {
        StartCoroutine(Disappear());
        rb = gameObject.GetComponent<Rigidbody2D>();
        GetComponent<ParticleSystem>().Play();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        // gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GameObject hitTarget = collision.gameObject;

        if (hitTarget.CompareTag("Player") && hitTarget != originalPlayer) {
            hitTarget.GetComponent<PlayerMovement>().OnPlayerHit(travelVector, damage, originalPlayer);
            Destroy(gameObject);
        }

        if (hitTarget != originalPlayer) {
            Destroy(gameObject);
        }
    }

    private IEnumerator Disappear() {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}