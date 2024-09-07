using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;

public class PlayerSystemManager : MonoBehaviour {
    public List<Player> playerList = new List<Player>();
    public List<Vector2> spawnPoints = new List<Vector2>();
    public static GameObject infoPanelParent;
    public GameObject waitingText;

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
        waitingText.SetActive(true);
        MenuScript.expectedPlayerCount = 2;
    }

    private void Update() {
        if (AllPlayersPresent() && Time.timeScale == 0f) {
            Time.timeScale = 1f;
            waitingText.SetActive(false);
            GetComponent<PlayerInputManager>().DisableJoining();
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
        if (livingPlayers is 1 or 2 or 4 or 8) {
            //StartCoroutine(EndGame((int)Mathf.Log(livingPlayers)));
        }
    }

    public void OnPlayerJoin(PlayerInput playerInput) {
        playerInput.gameObject.GetComponent<PlayerManager>().OnPlayerJoin(this);
    }

    private IEnumerator EndGame(int winnerID) {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("Scenes/Menu");
    }

    public bool AllPlayersPresent() {
        return MenuScript.expectedPlayerCount == playerList.Count;
    }
}