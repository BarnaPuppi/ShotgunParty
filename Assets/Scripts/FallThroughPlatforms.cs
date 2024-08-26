using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class FallThroughPlatforms : MonoBehaviour {
    private Collider2D platformCollider;
    private PlayerSystemManager psm;
    private Dictionary<PlayerMovement, bool> playerOnPlatform = new Dictionary<PlayerMovement, bool>();

    private void Start() {
        platformCollider = GetComponent<Collider2D>();
        psm = GameObject.Find("PlayerSystemManager").GetComponent<PlayerSystemManager>();
    }

    private void Update() {
        if (playerOnPlatform.Count != psm.playerList.Count) {
            playerOnPlatform.Add(psm.playerList[^1].gameObject.GetComponent<PlayerMovement>(), false);
        }

        foreach (var (playerMovement, isOnPlatform) in new Dictionary<PlayerMovement, bool>(playerOnPlatform)) {
            if (isOnPlatform && playerMovement.movement.y < -0.5f) {
                StartCoroutine(DisableCollider());
            }
        }
    }

    private void SetPlayerOnPlatform(Collision2D other, bool value) {
        var player = other.gameObject.GetComponent<PlayerMovement>();
        if (player != null) {
            playerOnPlatform[player] = value;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        SetPlayerOnPlatform(other, true);
    }

    private void OnCollisionExit2D(Collision2D other) {
        SetPlayerOnPlatform(other, false);
    }

    private IEnumerator DisableCollider() {
        platformCollider.enabled = false;
        yield return new WaitForSeconds(0.5f);
        platformCollider.enabled = true;
    }
}
