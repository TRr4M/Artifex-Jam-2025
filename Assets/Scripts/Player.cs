using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float speed, maxSpeed, speedPenalty, snapSpeed, groundDrag, jumpPower;
    public Transform orientation;

    private Rigidbody rb;
    private float drag;
    public LayerMask groundLayers;

    void Start() {
        rb = GetComponent<Rigidbody>();
        drag = rb.linearDamping;
    }

    void FixedUpdate()
    {
        Vector3 newVeloChange;
        newVeloChange = orientation.forward * Input.GetAxis("Vertical");
        newVeloChange += orientation.right * Input.GetAxis("Horizontal");
        bool onGround = IsOnGround();
        if (!onGround) newVeloChange = new Vector3();
        else if (newVeloChange.sqrMagnitude > 1) newVeloChange.Normalize();
        bool moving = newVeloChange.sqrMagnitude > 0;
        newVeloChange *= speed;
        rb.linearVelocity += newVeloChange * Time.deltaTime;
        if (!moving && onGround && rb.linearVelocity.sqrMagnitude < snapSpeed * snapSpeed) {
            rb.linearVelocity = new Vector3();
        }
        else if (onGround && rb.linearVelocity.sqrMagnitude > maxSpeed * maxSpeed) {
            float mag = rb.linearVelocity.magnitude, exceedBy = mag - maxSpeed;
            Vector3 dv = speedPenalty * Time.deltaTime * rb.linearVelocity / mag;
            if (dv.sqrMagnitude < exceedBy * exceedBy) rb.linearVelocity -= dv;
            else rb.linearVelocity = rb.linearVelocity * maxSpeed / mag;
        }
        if (onGround) rb.linearDamping = groundDrag;
        else rb.linearDamping = drag;

        if (onGround && Input.GetKey(KeyCode.Space)) {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpPower, rb.linearVelocity.z);
        }
    }

    bool IsOnGround() {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayers);
        //return Physics.CheckSphere(transform.position - 0.65f*transform.up, 0.49f, groundLayers);
    }
}
