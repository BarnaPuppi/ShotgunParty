using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class OLDPlayerMovement : MonoBehaviour
{
    #region Variable_Declaration
    
    private float horizontal;
    private float vertical;
    private float speed = 6f;
    private float maxSpeed = 18f;
    private bool isFacingRight = true;
    private bool jumped = false;
    private float jumpingPower = 12f;


    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private List<Transform> groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform shotgun;
    [SerializeField] private GameObject shotgunPellet;
    [SerializeField] private GameObject pelletParent;

    [HideInInspector] public static float shotgunCooldown = 1.3f;

    [HideInInspector] public enum State { Ready, Fired, OnCooldown };
    private State shotgunState = State.Ready;
    
    private float shotAngle;
    private Vector2 shotVector;

    private float rotation = 0f;
    private float shotgunForce = 600f;
    private bool isInShotgunKnockback = false;
    private float pelletSpeed = 20f;
    private float defaultSpread = 12f;
    private short defaultPelletCount = 5;
    private float randomSpread = 10f;
    private short randomPelletCount = 8;

    #endregion

    #region Input
    public void OnMove(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
        vertical = context.ReadValue<Vector2>().y;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        jumped = context.action.triggered;
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (shotgunState == State.Ready)
        {
            shotgunState = State.Fired;
        }
    }
    #endregion


    void Update()
    {
        #region Movement_and_Jump
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        if (jumped && IsGrounded()) 
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }

        if (!jumped && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.35f);
        }
        #endregion

        #region Rotation_and_Flip
        if (horizontal != 0 || vertical != 0)
        {
            rotation = Mathf.Rad2Deg * Mathf.Atan2(vertical, Mathf.Abs(horizontal));

            shotAngle = Mathf.Atan2(vertical, horizontal) + Mathf.PI;
            shotVector = new Vector2(shotgunForce * Mathf.Cos(shotAngle), shotgunForce * Mathf.Sin(shotAngle));

            if (horizontal < 0f)
            {
                rotation = -rotation + 180f;
            }

        }

        if ((isFacingRight && shotgun.localScale.y < 0f) || (!isFacingRight && shotgun.localScale.y > 0f))
        {
            Vector3 localScale = shotgun.localScale;
            localScale.y *= -1;
            shotgun.localScale = localScale;
        }

        Flip();
        #endregion

        #region Shotgun
        if (isInShotgunKnockback && IsGrounded())
        {
            isInShotgunKnockback = false;
        }

        if (shotgunState == State.Fired)
        {
            isInShotgunKnockback = true;
            Shoot();
        }
        #endregion

    }

    private void FixedUpdate() 
    {
        #region Movement
        if (!isInShotgunKnockback)
        {
            if (Mathf.Pow(horizontal, 2) + Mathf.Pow(vertical, 2) > 0.95 && Mathf.Abs(vertical) <= 0.75)
            {
                rb.velocity = new Vector2(horizontal * speed / Mathf.Abs(horizontal), rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
            }
        }
        else
        {
            rb.AddForce(new Vector2(horizontal * speed * 1.8f, 0));
            if (rb.velocity.magnitude > maxSpeed)
            {
                rb.velocity = rb.velocity.normalized * maxSpeed;
            }
        }
        #endregion

        #region Shotgun
        shotgun.rotation = Quaternion.Euler(new Vector3(0f, 0f, rotation));

        if (shotgunState == State.Fired)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);

            // borzalom, pls fix

            shotVector *= Mathf.Pow(0.4f, rb.velocity.y);
            
            rb.AddForce(shotVector, ForceMode2D.Force);

            shotgunState = State.OnCooldown;
            StartCoroutine(StateReset(shotgunCooldown, (x) => shotgunState = x));
        }
        #endregion
    }

    private bool IsGrounded() 
    {
        bool returnValue = false;
        float groundCheckRadius = 0.2f;
        foreach (Transform point in groundCheck)
        {
            returnValue = returnValue || Physics2D.OverlapCircle(point.position, groundCheckRadius, groundLayer);
        }
        return returnValue;
    }
    
    private void Flip() 
    {
        if ((isFacingRight && horizontal < 0) || (!isFacingRight && horizontal > 0)) {
            isFacingRight = !isFacingRight;
            this.gameObject.GetComponent<SpriteRenderer>().flipX = !(this.gameObject.GetComponent<SpriteRenderer>().flipX);
        }
    }

    public static IEnumerator StateReset(float cooldown, Action<State> stateChange)
    {
        yield return new WaitForSeconds(cooldown);
        stateChange(State.Ready);
    }

    private void Shoot()
    {
        for (int i = 0; i < defaultPelletCount + randomPelletCount; i++)
        {
            GameObject newPellet = Instantiate(shotgunPellet, this.gameObject.transform.position + new Vector3(-shotVector.normalized.x, -shotVector.normalized.y, 0f), Quaternion.identity, pelletParent.transform);
            Rigidbody2D newPelletRB = newPellet.GetComponent<Rigidbody2D>();
            newPellet.SetActive(true);

            float theta = 0;
            float x = pelletSpeed * Mathf.Cos(shotAngle - Mathf.PI);
            float y = pelletSpeed * Mathf.Sin(shotAngle - Mathf.PI);
            if (i < defaultPelletCount)
            {
                theta = -defaultSpread + 2 * defaultSpread * i / (defaultPelletCount - 1);
            }
            else
            {
                theta = UnityEngine.Random.Range(-randomSpread, randomSpread);
            }
            theta *= Mathf.Deg2Rad;

            newPelletRB.AddForce(new Vector2(x * Mathf.Cos(theta) - y * Mathf.Sin(theta), x * Mathf.Sin(theta) + y * Mathf.Cos(theta)), ForceMode2D.Impulse);
        }
    }
}