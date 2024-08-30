using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PauseScript : MonoBehaviour {
    private PlayerSystemManager psm;

    private void Start() {
        psm = GameObject.Find("PlayerSystemManager").GetComponent<PlayerSystemManager>();
    }

    public void OnPause(InputAction.CallbackContext context) {
        if (psm.AllPlayersPresent() && context.action.WasReleasedThisFrame()) {
            Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        }
    }
}