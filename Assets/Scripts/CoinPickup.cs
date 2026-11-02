/*Tutorials used:

Tutorials used:

MASSIVE Unity Save System Tutorial!: https://www.youtube.com/watch?v=MotmZgCod9A

How to Spawn Objects in Unity [Using Instantiate]: https://www.youtube.com/watch?v=aBzpvUXibw0&t=8s

Adding Materials and Textures to Game Objects in Unity 6: https://www.youtube.com/watch?v=RnKkTW8R4A4&t=128s

Persistent data: How to save your game states and settings: https://unity.com/blog/games/persistent-data-how-to-save-your-game-states-and-settings

EVERYTHING you need to know to SAVE and LOAD your game in Unity: https://www.youtube.com/watch?v=1mf730eb5Wo

Placing and activating TRAPS in Unity 2020: https://www.youtube.com/watch?v=L2qJNZr2A_I

Documentation/ScriptReference/Random.Range: https://docs.unity3d.com/2019.1/Documentation/ScriptReference/Random.Range.html

FULL 3D ENEMY AI in 6 MINUTES! || Unity Tutorial: https://www.youtube.com/watch?v=UjkSFoLxesw

How to Fix Pink Assets from Unity Asset Store in Unity 6: https://www.youtube.com/watch?v=YLfcRwrER4A&t=322s

How to make something spin in Unity | Unity 3D tutorial: https://www.youtube.com/watch?v=K3Ap2_beGnE

State Machine in Unity 3D - Tutorial for Beginners: https://www.youtube.com/watch?v=7QIcd6_TTys

Create Obstacle - 3D Coin Collector Game in Unity Engine - Tutorial 07 https://www.youtube.com/watch?v=bcEs3cwPzD4
*/

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