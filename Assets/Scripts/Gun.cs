using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject barrel; //spot to spawn bullet
    [SerializeField] private float bulletSpeed = 5f;

    void Update()
    {
        if (Keyboard.current.eKey.isPressed)
        {
            Shoot();

        }
    }

    void Shoot()
    {
        GameObject bul = Instantiate(bullet, barrel.transform.position, Quaternion.identity);
        Rigidbody rb = bul.GetComponent<Rigidbody>();

        rb.AddForce(Vector3.forward * bulletSpeed, ForceMode.Force);

        Destroy(bul, 3f);
    }
}
