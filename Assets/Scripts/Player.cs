using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float speed, maxSpeed, speedPenalty, snapSpeed, groundDrag, jumpPower, swatRange;

    private Rigidbody rb;
    private float drag;
    public float mouseSensitivity;
    private float xRotation;
    private float yRotation;
    private Transform mainCamera;
    public RawImage healthBar;
    public CanvasGroup deathScreen;

    private VolumeProfile volume;
    private Vignette vignette;
    private ChromaticAberration chromaticAberration;
    private WhiteBalance whiteBalance;
    private bool dead = false;
    private AudioSource slap;

    public LayerMask groundLayers;
    public TextMeshProUGUI healthText;
    public int health = 100;
    public bool win;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        mainCamera = GameObject.Find("Main Camera").transform;
        xRotation = 0f;
        yRotation = 180f;
        slap = GameObject.Find("Slap Sound").GetComponent<AudioSource>();

        rb = GetComponent<Rigidbody>();
        drag = rb.linearDamping;

        dead = false;
        health = 100;
        volume = GameObject.Find("Global Volume").GetComponent<Volume>().profile;
        volume.TryGet(out chromaticAberration);
        volume.TryGet(out vignette);
        volume.TryGet(out whiteBalance);
        deathScreen.alpha = 0f;
        win = false;
    }

    void FixedUpdate()
    {
        if (!dead) {
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

        // Health stuff
        if (transform.position.y < -200 && Time.frameCount % 5 == 0) {
            health -= 1;
        }
        if (health < 0) {
            health = 0;
        }
        healthText.text = health.ToString();
        healthBar.rectTransform.localScale = new Vector3(health / 100f, healthBar.rectTransform.localScale.y, healthBar.rectTransform.localScale.z);
        chromaticAberration.intensity.Override(Mathf.Lerp(1f, 0f, health / 100f));
        vignette.intensity.Override(Mathf.Lerp(0.4f, 0.15f, health / 100f));
        whiteBalance.temperature.Override(Mathf.Lerp(100f, -30f, health / 100f));

        if (health <= 0) {
            dead = true;
            rb.constraints = RigidbodyConstraints.None;
        }

        if (dead) {
            deathScreen.alpha = Mathf.Lerp(deathScreen.alpha, 1f, Time.deltaTime * 0.1f);
        }
    }

    void Update()
    {
        if (!dead) {
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
                        slap.Play();
                        spiderGameObject = spiderGameObject.transform.parent.parent.gameObject;
                        spiderGameObject.GetComponent<Spider>().lastSwat = Time.time;
                        Rigidbody spiderRB = spiderGameObject.GetComponent<Rigidbody>();
                        spiderRB.linearVelocity = 6.5f * Spider.Project(mainCamera.forward, Vector3.up).normalized + 6 * Vector3.up;
                    }
                }
            }
        } else {
            if (Input.GetKey(KeyCode.R)) {
                SceneManager.LoadScene(1);
            }
            if (Input.GetKey(KeyCode.M)) {
                SceneManager.LoadScene(0);
            }
        }
    }

    bool IsOnGround() {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayers);
        //return Physics.CheckSphere(transform.position - 0.65f*transform.up, 0.49f, groundLayers);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WinZone")) {
            win = true;
        }
    }
}
