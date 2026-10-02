using UnityEngine;

public class Hazard : MonoBehaviour
{
    public float speed = 3f;
    public int damage = 1;
    public int health = 1; // Added health so it can be destroyed by the player
    private Transform player;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (speed > 0 && player != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

//Debug print string for damage
    public void TakeDamage(int amount)
{
    health -= amount;
    if (health <= 0)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.playerGold += 10;
            Debug.Log("Hazard defeated! Current Gold: " + GameManager.Instance.playerGold);
        }
        Destroy(gameObject);
    }
}
}