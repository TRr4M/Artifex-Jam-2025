using System;
using System.Collections.Generic;
using UnityEngine;

public class Spider : MonoBehaviour
{
    public Vector3 crawlTarget;
    public float idealGroundDist;
    public float maxGroundDist;
    public float crawlSpeed;
    public float snapDist;
    public float rotateSpeed;
    public float legAnimationSpeed;
    public float legAnimationAmplitude;
    public List<Transform> legs;
    public float swatCooldown;
    public float lastSwat;

    private Rigidbody rb;
    private float legAnimationOffset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        snapDist *= snapDist;
        legAnimationOffset = UnityEngine.Random.Range(0f, (float)(2 * Math.PI));
        lastSwat = -99999f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Temp
        crawlTarget = GameObject.Find("Player").transform.position;

        Crawl();

        // Animate legs
        float animationTime = Time.time + legAnimationOffset;
        float animationAngle = (float)Math.Cos(animationTime * legAnimationSpeed) * legAnimationAmplitude;
        foreach (Transform leg in legs) {
            leg.localRotation = Quaternion.Euler(new Vector3(-90f, animationAngle, 0));
            animationAngle = -animationAngle;
        }
    }

    Vector3 Project(Vector3 direction, Vector3 normal) {
        return direction - Vector3.Dot(direction, normal) / normal.sqrMagnitude * normal;
    }

    void Crawl() {
        RaycastHit[] hits = Physics.RaycastAll(transform.position + (0.1f * transform.forward), -transform.up, maxGroundDist);
        Array.Sort(hits, (x,y) => x.distance.CompareTo(y.distance));
        if (hits.Length == 0 || Time.time - lastSwat < swatCooldown) {
            rb.isKinematic = false;
            rb.useGravity = true;
            if (rb.linearVelocity.sqrMagnitude < 0.1f) {
                transform.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);
            }
            return;
        }
        // rb.isKinematic = true;
        rb.useGravity = false;
        rb.angularVelocity = new Vector3();
        RaycastHit hit = hits[0];
        Vector3 normal = hit.normal;
        transform.position += transform.up * (idealGroundDist - hit.distance);
        Vector3 direction = crawlTarget - transform.position;
        Vector3 projectedDirection = Project(direction, normal);
        if (projectedDirection.sqrMagnitude <= snapDist) {
            transform.position += projectedDirection;
            return;
        }
        projectedDirection.Normalize();
        rb.linearVelocity = crawlSpeed * projectedDirection;
        
        // Rotate towards target
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(projectedDirection, normal), rotateSpeed * Time.deltaTime);
    }
}
