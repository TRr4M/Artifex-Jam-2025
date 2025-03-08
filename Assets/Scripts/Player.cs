using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed, maxSpeed, speedPenalty, snapSpeed, groundDrag, jumpPower, swatRange;

    private Rigidbody rb;
    private float drag;
    public float mouseSensitivity;
    private float xRotation;
    private float yRotation;
    private Transform mainCamera;

    public LayerMask groundLayers;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        mainCamera = GameObject.Find("Main Camera").transform;

        rb = GetComponent<Rigidbody>();
        drag = rb.linearDamping;
    }

    void FixedUpdate()
    {
        Vector3 newVeloChange;
        newVeloChange = transform.forward * Input.GetAxis("Vertical");
        newVeloChange += transform.right * Input.GetAxis("Horizontal");
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

    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * mouseSensitivity;
        yRotation += mouseX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * mouseSensitivity;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 90f);
        transform.rotation = Quaternion.Euler(0, yRotation, 0);
        mainCamera.localRotation = Quaternion.Euler(xRotation, 0, 0);

        if (Input.GetKeyDown(KeyCode.E)) { // Swat spider
            RaycastHit[] hits = Physics.RaycastAll(mainCamera.position, mainCamera.forward, swatRange);
            System.Array.Sort(hits, (x,y) => x.distance.CompareTo(y.distance));
            if (hits.Length > 0) {
                RaycastHit hit = hits[0];
                GameObject spiderGameObject = hit.collider.gameObject;
                if (spiderGameObject.CompareTag("spider")) {
                    spiderGameObject = spiderGameObject.transform.parent.parent.gameObject;
                    spiderGameObject.GetComponent<Spider>().lastSwat = Time.time;
                    Rigidbody spiderRB = spiderGameObject.GetComponent<Rigidbody>();
                    spiderRB.linearVelocity = 6.5f * Spider.Project(mainCamera.forward, Vector3.up).normalized + 6 * Vector3.up;
                }
            }
        }
    }

    bool IsOnGround() {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayers);
        //return Physics.CheckSphere(transform.position - 0.65f*transform.up, 0.49f, groundLayers);
    }
}
