using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShotgunScript : MonoBehaviour {
    public enum State {
        Ready,
        Fired,
        OnCooldown
    };

    private State shotgunState = State.Ready;

    [SerializeField] private Transform shotgun;
    [SerializeField] private Transform firingPoint;
    [SerializeField] private GameObject shotgunPellet;
    [SerializeField] private GameObject pelletParent;
    [SerializeField] private Rigidbody2D playerRB;
    [SerializeField] private SpriteRenderer shotgunSpriteRenderer;
    private PlayerMovement playerMovement;
    private PlayerManager playerManager;
    private PowerupUser powerupUser;

    private Vector2 movement;
    private float shotgunCooldown = 0.9f;
    private float shotAngle = 0f;
    public Vector2 shotVector;
    private float rotation;
    private float pelletSpeed = 1e-3f;
    private float defaultSpread = 12f;
    private short defaultPelletCount = 5;
    private float randomSpread = 10f;
    private short randomPelletCount = 8;

    private float shotgunPower = 15f;


    public void OnFire(InputAction.CallbackContext context) {
        if (shotgunState == State.Ready) shotgunState = State.Fired;
        if (playerManager.isInvulnerable) StartCoroutine(playerManager.MakeInvulnerable(0));
    }

    void Start() {
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        playerRB = gameObject.GetComponent<Rigidbody2D>();
        playerManager = gameObject.GetComponent<PlayerManager>();
        powerupUser = gameObject.GetComponent<PowerupUser>();

        shotVector = playerMovement.isFacingRight ? Vector2.right : Vector2.left;
    }


    void Update() {
        movement = playerMovement.movement;

        if (movement.x != 0 || movement.y != 0) {
            rotation = Mathf.Rad2Deg * Mathf.Atan2(movement.y, Mathf.Abs(movement.x));
            shotAngle = Mathf.Atan2(movement.y, movement.x) + Mathf.PI;
            shotVector = new Vector2(shotgunPower * Mathf.Cos(shotAngle), shotgunPower * Mathf.Sin(shotAngle));

            if (movement.x < 0f) {
                rotation = -rotation + 180f;
            }
        }

        if ((playerMovement.isFacingRight && shotgun.localScale.y < 0f) || (!playerMovement.isFacingRight && shotgun.localScale.y > 0f)) {
            Vector3 localScale = shotgun.localScale;
            localScale.y *= -1;
            shotgun.localScale = localScale;
        }
    }

    void FixedUpdate() {
        shotgun.rotation = Quaternion.Euler(new Vector3(0f, 0f, rotation));

        if (shotgunState == State.Fired) {
            shotgunSpriteRenderer.color = new Color(0.75f, 0.75f, 0.75f, 0.87f);
            shotgunState = State.OnCooldown;
            StartCoroutine(StateReset(x => shotgunState = x, shotgunCooldown * (powerupUser.activePowerup == PowerupSpawner.PowerupType.Rage ? PowerupConstants.Rage.reloadReduction : 1f)));
            playerMovement.isInKnockback = true;

            playerMovement.acceloration = 2f;
            playerMovement.velocityPower = 1f;

            playerRB.velocity = Vector2.zero;
            playerRB.totalForce = Vector2.zero;
            playerRB.AddForce(shotVector * (playerManager.player.percentage > 100 ? (playerManager.player.percentage / 100 - 1) / 4 + 1 : 1), ForceMode2D.Impulse);
            
            ShootPellets();
        }
    }

    private IEnumerator StateReset(Action<State> stateChange, float cooldown) {
        yield return new WaitForSeconds(cooldown);
        stateChange(State.Ready);
        shotgunSpriteRenderer.color = Color.white;
    }

    private void ShootPellets() {
        for (int i = 0; i < defaultPelletCount + randomPelletCount; i++) {
            GameObject newPellet = Instantiate(shotgunPellet, firingPoint.position, Quaternion.identity, pelletParent.transform);

            Rigidbody2D newPelletRB = newPellet.GetComponent<Rigidbody2D>();
            PelletScript newPelletScript = newPellet.GetComponent<PelletScript>();
            newPelletScript.originalPlayer = gameObject;
            newPellet.SetActive(true);

            float theta;
            float x = pelletSpeed * Mathf.Cos(shotAngle - Mathf.PI);
            float y = pelletSpeed * Mathf.Sin(shotAngle - Mathf.PI);
            if (i < defaultPelletCount) {
                theta = -defaultSpread + 2 * defaultSpread * i / (defaultPelletCount - 1);
            }
            else {
                theta = UnityEngine.Random.Range(-randomSpread, randomSpread);
            }

            theta *= Mathf.Deg2Rad;

            Vector2 travelVector = new Vector2(x * Mathf.Cos(theta) - y * Mathf.Sin(theta), x * Mathf.Sin(theta) + y * Mathf.Cos(theta));
            newPelletScript.travelVector = travelVector.normalized;
            newPelletRB.AddForce(travelVector, ForceMode2D.Impulse);
        }
    }
}