using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

public class PowerupSpawner : MonoBehaviour {
    private static int maxPowerupsSpawned = 4;
    private static float spawnTime = 6f;
    private static float powerupVanishTime = 10f;
    private static float passedTime = 0f;

    public List<Vector2> availableSpawnPoints = new List<Vector2>();
    public List<GroundPowerup> currentPowerupsSpawned = new List<GroundPowerup>();
    public List<Sprite> groundPowerupSprites;
    public GameObject defaultGroundPowerup;

    [SerializeField] private GameObject groundPowerupsParent;
    // Make into array later

    public enum PowerupType {
        None,
        Heal,
        Rage,
        Haste,
        Jetpack,
        Armour,
        Freeze,
        Teleport
    }

    [Serializable]
    public class GroundPowerup {
        public PowerupType powerupType;
        public GameObject gameObject;
        public float timeBeforeVanish;

        public GroundPowerup(PowerupType powerupType, float timeBeforeVanish) {
            this.powerupType = powerupType;
            this.timeBeforeVanish = timeBeforeVanish;
        }
    }

    void Update() {
        for (int powerupIndex = 0; powerupIndex < currentPowerupsSpawned.Count; powerupIndex++) {
            if ((currentPowerupsSpawned[powerupIndex].timeBeforeVanish -= Time.deltaTime) <= 0f)
                DestoryGroundPowerup(powerupIndex);
        }

        if (currentPowerupsSpawned.Count < maxPowerupsSpawned)
            passedTime += Time.deltaTime;

        if (passedTime >= spawnTime) {
            passedTime = 0f;
            SpawnPowerup();
        }
    }

    private void SpawnPowerup() {
        var randomPowerupIndex = UnityEngine.Random.Range(1, Enum.GetValues(typeof(PowerupType)).Length);
        GroundPowerup newGroundPowerup = new GroundPowerup((PowerupType)randomPowerupIndex, powerupVanishTime);
        currentPowerupsSpawned.Add(newGroundPowerup);
        var randomPosition = availableSpawnPoints[UnityEngine.Random.Range(0, availableSpawnPoints.Count)];
        newGroundPowerup.gameObject = Instantiate(defaultGroundPowerup, randomPosition, Quaternion.identity, groundPowerupsParent.transform);
        newGroundPowerup.gameObject.GetComponent<SpriteRenderer>().sprite = groundPowerupSprites[randomPowerupIndex - 1];
        availableSpawnPoints.Remove(randomPosition);
    }

    public void DestoryGroundPowerup(int powerupIndex) {
        availableSpawnPoints.Add(currentPowerupsSpawned[powerupIndex].gameObject.transform.position);
        Destroy(currentPowerupsSpawned[powerupIndex].gameObject);
        currentPowerupsSpawned.RemoveAt(powerupIndex);
    }
}