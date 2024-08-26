using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject cam;
    private List<PlayerSystemManager.Player> playerList;
    private Vector3 offset = new Vector3(0f, 0f, -10f);
    private float maxDistance = 4;

    void Start() {
        cam = this.gameObject;
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
        cam.transform.position = Vector3.ClampMagnitude(avgPos, maxDistance) + offset;
    }
}
