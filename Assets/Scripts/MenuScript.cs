using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour {
    [NonSerialized] public static int expectedPlayerCount = 0;
    
    public void ExitGame() {
        Application.Quit();
    }

    public void TwoPlayers() {
        expectedPlayerCount = 2;
        SceneChanger();
    }

    public void ThreePlayers() {
        expectedPlayerCount = 3;
        SceneChanger();
    }

    public void FourPlayers() {
        expectedPlayerCount = 4;
        SceneChanger();
    }

    private void SceneChanger() {
        SceneManager.LoadScene("Scenes/Game");
    }
}
