using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerSystemManager : MonoBehaviour {
    private int expectedPlayerCount;
    public List<Player> playerList = new List<Player>();
    public List<Vector2> spawnPoints = new List<Vector2>();
    public static GameObject infoPanelParent;

    [Serializable]
    public class Player {
        public int playerID;
        public GameObject gameObject;
        public float percentage;
        public int lifeCount;
        public GameObject infoPanel;

        public Player(int playerID, GameObject gameObject) {
            this.playerID = playerID;
            this.gameObject = gameObject;
            this.percentage = 0f;
            this.lifeCount = 3;
            this.infoPanel = infoPanelParent.transform.GetChild(playerID).gameObject;
            this.infoPanel.SetActive(true);
        }
    }

    void Awake() {
        infoPanelParent = GameObject.Find("PlayerInfo");
        Time.timeScale = 0f;
        expectedPlayerCount = MenuScript.expectedPlayerCount;
        expectedPlayerCount = 1;
    }

    private void Update() {
        if (expectedPlayerCount == playerList.Count && Time.timeScale == 0f) {
            Time.timeScale = 1f;
            foreach (Player player in playerList) {
                PlayerManager playerManager = player.gameObject.GetComponent<PlayerManager>();
                playerManager.SetPlayerActive(true);
                StartCoroutine(playerManager.MakeInvulnerable(3));
            }
        }

        int livingPlayers = 0;
        foreach (Player player in playerList) {
            livingPlayers += (player.lifeCount > 0) ? 2>>player.playerID : 0;
        }
        if (livingPlayers == 1 || livingPlayers == 2 || livingPlayers == 4 || livingPlayers == 8) {
            EndGame((int)Mathf.Log(livingPlayers));
        }
    }

    public void OnPlayerJoin(PlayerInput playerInput) {
        playerInput.gameObject.GetComponent<PlayerManager>().OnPlayerJoin(this);
        
    }

    private void EndGame(int winnerID) {
        
    }
}