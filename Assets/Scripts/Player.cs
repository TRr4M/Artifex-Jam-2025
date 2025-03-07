using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float speed, maxSpeed, speedPenalty, snapSpeed, movingDrag;
    public Transform orientation;

    private Rigidbody rb;
    private Vector3 velocityChange = Vector3.zero;
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
        if (newVeloChange.sqrMagnitude > 1) newVeloChange.Normalize();
        bool moving = newVeloChange.sqrMagnitude > 0;
        float tempMaxSpeed = moving ? maxSpeed * newVeloChange.magnitude : maxSpeed;
        newVeloChange *= speed;
        velocityChange = newVeloChange;
        rb.linearVelocity += newVeloChange * Time.deltaTime;
        if (!moving && rb.linearVelocity.sqrMagnitude < snapSpeed * snapSpeed) {
            rb.linearVelocity = new Vector3();
        }
        else if (rb.linearVelocity.sqrMagnitude > tempMaxSpeed * tempMaxSpeed) {
            float mag = rb.linearVelocity.magnitude, exceedBy = mag - tempMaxSpeed;
            Vector3 dv = speedPenalty * Time.deltaTime * rb.linearVelocity / mag;
            if (dv.sqrMagnitude < exceedBy * exceedBy) rb.linearVelocity -= dv;
            else rb.linearVelocity = rb.linearVelocity * tempMaxSpeed / mag;
        }
        if (moving) rb.linearDamping = movingDrag;
        else rb.linearDamping = drag;
    }

    bool isOnGround() {
        return Physics.Raycast(transform.position, Vector3.down, 0.5f, groundLayers);
    }
}
