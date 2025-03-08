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
    private Transform playerFace;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        snapDist *= snapDist;
        legAnimationOffset = UnityEngine.Random.Range(0f, (float)(2 * Math.PI));
        lastSwat = -99999f;
        playerFace = GameObject.Find("Player").transform.Find("Main Camera").Find("Face");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        crawlTarget = playerFace.transform.position;

        Crawl();

        // Animate legs
        if (Time.time - lastSwat >= swatCooldown) {
            float animationTime = Time.time + legAnimationOffset;
            float animationAngle = (float)Math.Cos(animationTime * legAnimationSpeed) * legAnimationAmplitude;
            foreach (Transform leg in legs) {
                leg.localRotation = Quaternion.Euler(new Vector3(-90f, animationAngle, animationAngle));
                animationAngle = -animationAngle;
            }
        }
    }

    public static Vector3 Project(Vector3 direction, Vector3 normal) {
        return direction - Vector3.Dot(direction, normal) / normal.sqrMagnitude * normal;
    }

    void Crawl() {
        RaycastHit[] hits = Physics.RaycastAll(transform.position + (0.1f * transform.forward), -transform.up, maxGroundDist);
        Array.Sort(hits, (x,y) => x.distance.CompareTo(y.distance));
        bool isSlow = rb.linearVelocity.sqrMagnitude < 0.1f;
        if (hits.Length == 0 || Time.time - lastSwat < swatCooldown) {
            rb.isKinematic = false;
            rb.useGravity = true;
            if (isSlow) {
                transform.rotation = Quaternion.LookRotation(Project(transform.forward, Vector3.up).normalized, Vector3.up);
            }
            return;
        }
        rb.useGravity = false;
        rb.angularVelocity = new Vector3();
        RaycastHit hit = hits[0];
        float tempRotateSpeed = rotateSpeed * Time.deltaTime;

        bool suddenIncrease = false;
        // Try crawling up sudden gradient increase?
        /*
        if ((transform.position - crawlTarget).sqrMagnitude > (0.25f * 0.25f)) {
            hits = Physics.RaycastAll(transform.position, (transform.forward + transform.up).normalized, maxGroundDist);
            if (hits.Length != 0) {
                hit = hits[0];
                tempRotateSpeed = 90f;
                transform.position += transform.up * 0.2f;
                suddenIncrease = true;
            } else {
                hits = Physics.RaycastAll(transform.position, transform.forward, maxGroundDist);
                if (hits.Length != 0) {
                    hit = hits[0];
                    tempRotateSpeed = 90f;
                    transform.position += transform.up * 0.2f;
                    suddenIncrease = true;
                }
            }
        }
        */

        Vector3 normal = hit.normal;
        if (!suddenIncrease) {
            transform.position += transform.up * (idealGroundDist - hit.distance);
        }
        Vector3 direction = crawlTarget - transform.position;
        Vector3 projectedDirection = Project(direction, normal);
        if (projectedDirection.sqrMagnitude <= snapDist) {
            transform.position += projectedDirection;
            return;
        }
        projectedDirection.Normalize();
        rb.linearVelocity = crawlSpeed * projectedDirection;
        
        // Rotate towards target
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(projectedDirection, normal), tempRotateSpeed);
    }
}
