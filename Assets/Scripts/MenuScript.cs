using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour {
    [NonSerialized] public static int expectedPlayerCount = 0;
    public List<GameObject> buttons;

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
    
    private void Update() {
        foreach (GameObject button in buttons) {
            if (button == EventSystem.current.currentSelectedGameObject) {
                button.GetComponent<RectTransform>().localScale = new Vector3(1.2f, 1.2f, 1f);
            }
            else {
                button.GetComponent<RectTransform>().localScale = Vector3.one;
            }
        }
    }
}
