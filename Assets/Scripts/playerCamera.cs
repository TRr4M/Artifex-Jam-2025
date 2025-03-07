using UnityEngine;

public class playerCamera : MonoBehaviour
{

    public Transform orientation;
    public float mouseSensitivity;
    float xRotation;
    float yRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {

        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * mouseSensitivity;

        yRotation += mouseX;

        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);


    }
}
