using UnityEngine;

public class Flame : MonoBehaviour
{
    public float returnSpeed, flickerChance, drag, flickerStrength, lightFlickerStrength;
    public int flickerRarity;
    public Light pointLight;

    private float xRotation, yRotation, zRotation;
    private float xSpeed, zSpeed;
    private int frameCount = 0;
    private float intensity, targetIntensity, intensitySpeed;

    void Start()
    {
        xRotation = 0;
        yRotation = transform.localEulerAngles.y;
        zRotation = 0;
        intensity = targetIntensity = pointLight.intensity;
        intensitySpeed = 0;
    }

    void FixedUpdate()
    {
        xRotation = Mathf.Lerp(xRotation, 0, Time.deltaTime * returnSpeed) + (xSpeed * Time.deltaTime);
        zRotation = Mathf.Lerp(zRotation, 0, Time.deltaTime * returnSpeed) + (zSpeed * Time.deltaTime);
        intensity = Mathf.Lerp(intensity, targetIntensity, Time.deltaTime * returnSpeed) + intensitySpeed * Time.deltaTime;
        xSpeed = Mathf.Lerp(xSpeed, 0, Time.deltaTime * drag);// + (xSpeed * Time.deltaTime);
        zSpeed = Mathf.Lerp(zSpeed, 0, Time.deltaTime * drag);// + (zSpeed * Time.deltaTime);
        intensitySpeed = Mathf.Lerp(intensitySpeed, 0, Time.deltaTime * drag);

        if (frameCount % flickerRarity == 0) {
            if (Random.Range(0f, 1f) < flickerChance) {
                xSpeed += Random.Range(-flickerStrength, flickerStrength);
            }
            if (Random.Range(0f, 1f) < flickerChance) {
                zSpeed += Random.Range(-flickerStrength, flickerStrength);
            }
            if (Random.Range(0f, 1f) < flickerChance) {
                intensitySpeed += Random.Range(-lightFlickerStrength, lightFlickerStrength);
            }
        }

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, zRotation);
        pointLight.intensity = intensity;

        frameCount++;
    }
}
