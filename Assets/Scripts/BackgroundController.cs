using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private GameObject bg;
    private List<PlayerSystemManager.Player> playerList;
    private float maxDistance = 4;

    void Start() {
        bg = this.gameObject;
        playerList = GameObject.Find("PlayerSystemManager").GetComponent<PlayerSystemManager>().playerList;
    }

    void Update() {
        Vector3 avgPos = Vector3.zero;
        int playerCount = 0;
        foreach (PlayerSystemManager.Player player in playerList) {
            if (player.lifeCount > 0) {
                avgPos += player.gameObject.transform.position;
                playerCount++;
            }
        }
        avgPos /= playerCount + 1;
        bg.transform.position = Vector3.ClampMagnitude(avgPos, maxDistance) * 0.3f;
    }
}
