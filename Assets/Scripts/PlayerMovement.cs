using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class PlayerMovement : MonoBehaviour {
    #region Declaration
    
    [NonSerialized] public Vector2 movement;
    [NonSerialized] public bool isFacingRight = true;
    [NonSerialized] public bool isInKnockback;
    private bool jumped;
    private bool canJump = true;
    [NonSerialized] public bool inJetpack = false;
    private bool inFreeze = false;
    private float jumpPower = 12f;
    private float knockbackPower = 14f;
    private float passedTime = 0f;

    [NonSerialized] public float speed = 7.5f;
    [NonSerialized] public float acceloration = 7.5f;
    [NonSerialized] public float decceloration = 8f;
    [NonSerialized] public float velocityPower = 1.17f;
    [NonSerialized] public float gravityScale = 3f;
    [NonSerialized] public float fallGravityMultiplier = 1.6f;

    [SerializeField] private List<Transform> groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private Transform spawnedObjectsParent;
    private Rigidbody2D rb;
    [SerializeField] private Transform spriteTransform;
    private PlayerManager playerManager;
    private PowerupUser powerupUser;
    
    #endregion

    public void OnMove(InputAction.CallbackContext context) {
        movement = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context) {
        jumped = context.action.triggered;
        if (!jumped && !inJetpack) canJump = true;
    }

    void Start() {
        rb = gameObject.GetComponent<Rigidbody2D>();
        playerManager = gameObject.GetComponent<PlayerManager>();
        powerupUser = gameObject.GetComponent<PowerupUser>();
        spawnedObjectsParent = GameObject.Find("SpawnedObjects").transform;
    }

    void Update() {
        if (inJetpack) passedTime += Time.deltaTime;
        else passedTime = 0f;

        if ((isFacingRight && movement.x < 0) || (!isFacingRight && movement.x > 0)) {
            isFacingRight = !isFacingRight;
            spriteTransform.localScale = Vector3.Scale(spriteTransform.localScale, new Vector3(-1f, 1f, 1f));
        }

        if (isInKnockback && IsGrounded()) {
            isInKnockback = false;
            acceloration = 7.5f;
            decceloration = 8f;
            velocityPower = 1.17f;
        }

        bool shouldBeInFreeze = false;
        for (int i = 0; i < spawnedObjectsParent.childCount; i++) {
            GameObject spawnedObject = spawnedObjectsParent.GetChild(i).gameObject;

            if (spawnedObject.layer == LayerMask.NameToLayer("Freeze")) {
                shouldBeInFreeze = shouldBeInFreeze || 
                    Vector3.Distance(gameObject.transform.position, spawnedObject.transform.position) <= PowerupConstants.Freeze.radius;
            }
        }
        inFreeze = shouldBeInFreeze;
    }

    void FixedUpdate() {
        float targetSpeed = movement.x * speed *
            (powerupUser.activePowerup == PowerupSpawner.PowerupType.Haste ? PowerupConstants.Haste.speedMultiplier : 1f) *
            (powerupUser.activePowerup == PowerupSpawner.PowerupType.Armour ? PowerupConstants.Armour.speedReduction : 1f) *
            (inFreeze ? PowerupConstants.Freeze.speedReduction : 1f);
        float speedDifference = targetSpeed - rb.velocity.x;
        float accelorationRate = (Mathf.Abs(targetSpeed) > 0.01) ? acceloration : decceloration;
        rb.AddForce(Vector2.right * Mathf.Pow(Mathf.Abs(speedDifference) * accelorationRate, velocityPower) * Mathf.Sign(speedDifference));

        if (jumped && IsGrounded() && canJump) {
            rb.AddForce(Vector2.up * jumpPower * (powerupUser.activePowerup == PowerupSpawner.PowerupType.Haste ? PowerupConstants.Haste.jumpPowerMultiplier : 1f), ForceMode2D.Impulse);
            canJump = false;
        }

        if (!jumped && !inJetpack && rb.velocity.y > 0f) {
            rb.AddForce(Vector2.down * jumpPower * 0.5f, ForceMode2D.Impulse);
        }

        if (inJetpack) {
            // f(x)=1+e^(-d)-e^((ln(1+e^(-d))+d)x-d)
            float jetpackForce = 1 + Mathf.Exp(-PowerupConstants.Jetpack.duration) - Mathf.Exp((Mathf.Log(1 + Mathf.Exp(-PowerupConstants.Jetpack.duration)) + PowerupConstants.Jetpack.duration) * passedTime - PowerupConstants.Jetpack.duration);
            rb.AddForce(Vector2.up * PowerupConstants.Jetpack.jetpackPower * jetpackForce, ForceMode2D.Force);
            if (jetpackForce <= 0f) {
                inJetpack = false;
                powerupUser.SetParticleActive(false);
                powerupUser.activePowerup = PowerupSpawner.PowerupType.None;
            }

            if (rb.velocity.y > 7.5f) // Might have to change method later
                rb.velocity = new Vector2(rb.velocity.x, 7.5f);
        }

        if (rb.velocity.y < 0f) rb.gravityScale = gravityScale * fallGravityMultiplier;
        else rb.gravityScale = gravityScale;
    }

    public bool IsGrounded() {
        bool returnValue = false;
        float groundCheckRadius = 0.2f;
        foreach (Transform point in groundCheck) {
            returnValue = returnValue || Physics2D.OverlapCircle(point.position, groundCheckRadius, groundLayer);
        }
        return returnValue;
    }

    public void OnPlayerHit(Vector2 bulletVector, float damage, GameObject damageDealer) {
        PowerupUser damageDealerPowerups = damageDealer.GetComponent<PowerupUser>();

        isInKnockback = true;
        acceloration = 2f;
        decceloration = 2f;
        velocityPower = 1f;

        if (!playerManager.isInvulnerable) {
            if (powerupUser.activePowerup == PowerupSpawner.PowerupType.Heal) {
                powerupUser.activePowerup = PowerupSpawner.PowerupType.None;
            }

            playerManager.player.percentage += damage *
                                               (damageDealerPowerups.activePowerup == PowerupSpawner.PowerupType.Rage ? PowerupConstants.Rage.damageDealtMultiplier : 1f) *
                                               (powerupUser.activePowerup == PowerupSpawner.PowerupType.Armour ? PowerupConstants.Armour.damageReduction : 1f);

            float knockbackPowerupInfluence =
                (damageDealerPowerups.activePowerup == PowerupSpawner.PowerupType.Rage ? PowerupConstants.Rage.targetKnockbackReduction : 1f) *
                (powerupUser.activePowerup == PowerupSpawner.PowerupType.Haste ? PowerupConstants.Haste.knockbackMultipiler : 1f) *
                (powerupUser.activePowerup == PowerupSpawner.PowerupType.Armour ? PowerupConstants.Armour.knockbackReduction : 1f);

            rb.AddForce(new Vector2(bulletVector.normalized.x, bulletVector.normalized.y * 0.8f) * knockbackPower * knockbackPowerupInfluence * playerManager.player.percentage / 100f, ForceMode2D.Impulse);
        }
    }
}