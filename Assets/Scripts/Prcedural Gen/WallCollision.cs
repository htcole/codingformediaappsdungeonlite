using UnityEngine;

public class WallCollision : MonoBehaviour
{
    void Start()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, .01f);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Wall"))
            {
                Destroy(gameObject);
                return;
            }
        }
    }
}