using UnityEngine;

public class GoldCoinPickup : MonoBehaviour
{
    [Header("Coin Settings")]
    public int goldValue = 10;

    private Vector3 targetPosition;
    private float flyDuration = 0.4f;
    private float elapsedTime = 0f;
    private bool isLanding = true;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;

        // Pick a random landing spot nearby on the floor
        Vector2 randomCircle = Random.insideUnitCircle * 1.5f;
        targetPosition = new Vector3(startPos.x + randomCircle.x, startPos.y, startPos.z + randomCircle.y);

        // Ensure it's a trigger so it never blocks or falls through anything
        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;

        // Remove Rigidbody if it's attached, since we are moving it via code
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) Destroy(rb);
    }

    void Update()
    {
        if (isLanding)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / flyDuration;

            if (t < 1f)
            {
                // Parabola arc math: moves horizontally to target while arcing up in the middle
                Vector3 currentPos = Vector3.Lerp(startPos, targetPosition, t);
                currentPos.y += Mathf.Sin(t * Mathf.PI) * 1f; // Arc height of 1 unit
                transform.position = currentPos;
            }
            else
            {
                transform.position = targetPosition;
                isLanding = false; // Landed safely!
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.playerGold += goldValue;
                Debug.Log("Coin collected! Current Gold: " + GameManager.Instance.playerGold);
            }

            Destroy(gameObject);
        }
    }
}