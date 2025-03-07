using UnityEngine;

public class moveCamera : MonoBehaviour
{

    public Transform cameraPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = cameraPos.position;

    }
}
