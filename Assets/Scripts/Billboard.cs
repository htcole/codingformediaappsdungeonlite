using UnityEngine;

public class Billboard : MonoBehaviour
{
    void LateUpdate()
    {
        // Face the camera while keeping upright
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
            Camera.main.transform.rotation * Vector3.up);
    }
}