using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.WebGL;


public class PowerupUser : MonoBehaviour {
    private PlayerMovement playerMovement;
    private PlayerManager playerManager;
    private ShotgunScript shotgunScript;
    private PowerupSpawner powerupSpawner;
    public PowerupSpawner.PowerupType ownedPowerup = PowerupSpawner.PowerupType.None;
    public PowerupSpawner.PowerupType activePowerup = PowerupSpawner.PowerupType.None;

    private float healStartPercentage;
    private int healedIntervals;

    [SerializeField] private GameObject freezePrefab;

    private float passedTime = 0f;
    private Transform spawnedObjectsParent;
    private Transform particles;

    void Start() {
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        playerManager = gameObject.GetComponent<PlayerManager>();
        shotgunScript = gameObject.GetComponent<ShotgunScript>();
        powerupSpawner = GameObject.Find("PowerupSpawner").GetComponent<PowerupSpawner>();
        spawnedObjectsParent = GameObject.Find("SpawnedObjects").transform;
        particles = gameObject.transform.GetChild(4);
    }

    void Update() {
        passedTime += Time.deltaTime;

        for (int powerupIndex = 0; powerupIndex < powerupSpawner.currentPowerupsSpawned.Count; powerupIndex++) {
            PowerupSpawner.GroundPowerup powerup = powerupSpawner.currentPowerupsSpawned[powerupIndex];

            if (Vector3.Distance(powerup.gameObject.transform.position, gameObject.transform.position) <= 1.3f  && ownedPowerup == PowerupSpawner.PowerupType.None && activePowerup == PowerupSpawner.PowerupType.None) {
                ownedPowerup = powerup.powerupType;
                powerupSpawner.DestoryGroundPowerup(powerupIndex);
            }
        }

        if (activePowerup == PowerupSpawner.PowerupType.None) {
            passedTime = 0f;
        }

        if (activePowerup == PowerupSpawner.PowerupType.Heal) {
            if (passedTime >= PowerupConstants.Heal.healIntervalDuration) {
                playerManager.player.percentage -= PowerupConstants.Heal.healthRegenerated / Mathf.Ceil(PowerupConstants.Heal.duration / PowerupConstants.Heal.healIntervalDuration);
                passedTime = 0f;
                healedIntervals++;
            }

            if (passedTime + healedIntervals * PowerupConstants.Heal.healIntervalDuration >= PowerupConstants.Heal.duration || playerManager.player.percentage <= 0f) {
                playerManager.player.percentage = Mathf.Max(healStartPercentage - PowerupConstants.Heal.healthRegenerated, 0f);
                healedIntervals = 0;
                SetParticleActive(false);
                activePowerup = PowerupSpawner.PowerupType.None;
            }
        }

        if (activePowerup == PowerupSpawner.PowerupType.Rage) {
            if (passedTime >= PowerupConstants.Rage.duration) {
                SetParticleActive(false);
                activePowerup = PowerupSpawner.PowerupType.None;
            }
        }

        if (activePowerup == PowerupSpawner.PowerupType.Haste) {
            if (passedTime >= PowerupConstants.Haste.duration) {
                SetParticleActive(false);
                activePowerup = PowerupSpawner.PowerupType.None;
            }
        }

        if (activePowerup == PowerupSpawner.PowerupType.Armour) {
            if (passedTime >= PowerupConstants.Armour.duration) {
                SetParticleActive(false);
                activePowerup = PowerupSpawner.PowerupType.None;
            }
        }
    }

    public void OnPowerupUse() {
        if (ownedPowerup != PowerupSpawner.PowerupType.None) {
            activePowerup = ownedPowerup;
            
            // Particle effects here
            SetParticleActive(true);

            switch (ownedPowerup) {
                case PowerupSpawner.PowerupType.Heal: {
                    healStartPercentage = playerManager.player.percentage;
                    break;
                }

                case PowerupSpawner.PowerupType.Jetpack: {
                    if (playerMovement.IsGrounded()) {
                        playerMovement.inJetpack = true;
                        ownedPowerup = PowerupSpawner.PowerupType.None;
                    }
                    else activePowerup = PowerupSpawner.PowerupType.None;
                    break;
                }

                case PowerupSpawner.PowerupType.Freeze: {
                    StartCoroutine(UseFreeze());
                    break;
                }

                case PowerupSpawner.PowerupType.Teleport: {
                    StartCoroutine(UseTeleport());
                    break;
                }
            }

            if (ownedPowerup != PowerupSpawner.PowerupType.Jetpack) {
                ownedPowerup = PowerupSpawner.PowerupType.None;
            }
        }
    }

    public void OnPowerupDiscard() {
        ownedPowerup = PowerupSpawner.PowerupType.None;
    }

    public void SetParticleActive(bool value) {
        GameObject particleObject = particles.GetChild((int)activePowerup - 1).gameObject;
        particleObject.SetActive(value);
        if (value == true) {
            particleObject.GetComponent<ParticleSystem>().Play(true);
        }
    }

    private IEnumerator UseFreeze() {
        yield return new WaitForSeconds(1f);
        Instantiate(freezePrefab, gameObject.transform.position, Quaternion.identity, spawnedObjectsParent);
        activePowerup = PowerupSpawner.PowerupType.None;
    }
    
    public IEnumerator UseTeleport() {
        yield return new WaitForSeconds(0.3f);
        Vector3 teleportVector = Vector3.zero;
        teleportVector.x = shotgunScript.shotVector.normalized.x * -PowerupConstants.Teleport.distance;
        transform.position += teleportVector;

        yield return new WaitForSeconds(0.3f);
        SetParticleActive(false);
        activePowerup = PowerupSpawner.PowerupType.None;
    }
}