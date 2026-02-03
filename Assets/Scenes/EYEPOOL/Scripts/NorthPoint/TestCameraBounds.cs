using UnityEngine;

public class TestCameraBounds : MonoBehaviour
{
    float distance;
    float height;
    float fov;
    [SerializeField] Camera mainCamera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fov = mainCamera.fieldOfView;
        height = transform.position.y - mainCamera.transform.position.y;
        distance = 0.5f * height / Mathf.Tan(0.5f * fov * Mathf.Deg2Rad);
        Debug.Log("Distance: " + distance);
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
