using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VectorGraphics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.Windows;

public class PlayerManager : MonoBehaviour {
    public PlayerSystemManager.Player player;
    public SpriteRenderer[] bodypartRenderers = new SpriteRenderer[5];
    public Sprite[] bodyparts = new Sprite[20];
    public Sprite[] playerIcons = new Sprite[4];
    [NonSerialized] public PlayerSystemManager psm;
    private PlayerMovement playerMovement;
    public PowerupUser powerupUser;
    private TMP_Text percentageText;
    public GameObject deadPlayerObject;
    public bool isInvulnerable;
    private bool isAlive = true;

    void Start() {
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        powerupUser = gameObject.GetComponent<PowerupUser>();
    }

    void Update() {
        if (player.gameObject.transform.position.y < -9f && isAlive) {
            isAlive = false;
            powerupUser.ownedPowerup = PowerupSpawner.PowerupType.None;
            powerupUser.activePowerup = PowerupSpawner.PowerupType.None;
            player.percentage = 0f;
            player.lifeCount--;
            // Remove heart from UI
            player.infoPanel.transform.GetChild(3).GetChild(player.lifeCount).gameObject.SetActive(false);
            if (player.lifeCount > 0) {
                StartCoroutine(Respawn());
            }
            else {
                Destroy(player.gameObject);
                player.gameObject = deadPlayerObject;
            }
        }

        percentageText.text = $"{player.percentage.ToString("0.0", CultureInfo.InvariantCulture)}%";
    }

    public void OnPlayerJoin(PlayerSystemManager caller) {
        psm = caller;
        player = new PlayerSystemManager.Player(psm.playerList.Count, gameObject);
        psm.playerList.Add(player);
        ChangeColor();

        percentageText = player.infoPanel.transform.GetChild(1).gameObject.GetComponent<TMP_Text>();
        player.percentage = 0f;

        // Set player's icon on UI
        player.infoPanel.transform.GetChild(0).GetComponent<SVGImage>().sprite = playerIcons[player.playerID];
        
        SetPlayerActive(false);
    }

    public void SetPlayerActive(bool value) {
        gameObject.transform.GetChild(1).gameObject.SetActive(value);
        gameObject.transform.GetChild(3).gameObject.SetActive(value);
        gameObject.GetComponent<PlayerMovement>().enabled = value;
        gameObject.GetComponent<ShotgunScript>().enabled = value;
    }
    
    public IEnumerator Respawn() {
        yield return new WaitForSeconds(3);
        player.gameObject.transform.position = new Vector3(0f, -2f, 0f);
        StartCoroutine(MakeInvulnerable(3));
        isAlive = true;
    }

    public IEnumerator MakeInvulnerable(int seconds) {
        isInvulnerable = true;
        yield return new WaitForSeconds(seconds);
        isInvulnerable = false;
    }
    
    public void ChangeColor() {
        for (int i = 0; i < 5; i++) {
            bodypartRenderers[i].sprite = bodyparts[5 * player.playerID + i];
        }
    }
}