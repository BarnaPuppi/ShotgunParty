using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Animation : MonoBehaviour {
    [SerializeField] private Transform rLeg;
    [SerializeField] private Transform lLeg;
    [SerializeField] private GameObject player;
    
    private Rigidbody2D playerRB;
    private PlayerMovement playerMovement; 
        
    private float rLegDefault = 27.81f;
    private float lLegDefault = -32.45f;
    private float t = 0;
    private float walkingSpeed = 10f;
    private float amplitude = 30f;

    void Start() {
        playerRB = player.GetComponent<Rigidbody2D>();
        playerMovement = player.GetComponent<PlayerMovement>();
    }
    
    void Update() {
        if (!playerMovement.IsGrounded()) {
            t = 0;
            rLeg.rotation = Quaternion.Euler(new Vector3(0f, 0f, - amplitude * (playerMovement.isFacingRight ? 1 : -1)));
            lLeg.rotation = Quaternion.Euler(new Vector3(0f, 0f, amplitude * (playerMovement.isFacingRight ? 1 : -1)));
        }
        else if (Mathf.Abs(playerRB.velocity.x) > 0.1f) {
            t += Time.deltaTime * walkingSpeed;
            rLeg.rotation = Quaternion.Euler(new Vector3(0f, 0f, - amplitude * Mathf.Sin(t) + rLegDefault * (playerMovement.isFacingRight ? 1 : -1)));
            lLeg.rotation = Quaternion.Euler(new Vector3(0f, 0f, amplitude * Mathf.Sin(t) + lLegDefault * (playerMovement.isFacingRight ? 1 : -1)));
        }
        else {
            t = 0;
            rLeg.rotation = Quaternion.Euler(new Vector3(0f, 0f, rLegDefault * (playerMovement.isFacingRight ? 1 : -1)));
            lLeg.rotation = Quaternion.Euler(new Vector3(0f, 0f, lLegDefault * (playerMovement.isFacingRight ? 1 : -1)));
        }
        
    }
}
