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
*/

using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton instance so any script can access the GameManager easily
    public static GameManager Instance;

    [Header("Run Progression")]
    public int currentLevel = 1; // Increases every time you beat a boss/treasure room
    public int playerGold = 0;   // Currency carried across runs
    public int extraMaxHealthPurchased = 0; // Purchased via health upgrade
    public bool hasVampiricUpgrades = false; // Purchased via leech upgrade
    public int vampiricHealAmount = 10;
    public int ExtraAttackPower = 0;
    public float Speed = 12f;


    void Awake()
    {
        // Ensure only one GameManager exists across scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps this object alive when restarting scenes!
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Call this when you want to reset after death (keeps gold/meta-progression if desired)
    public void ResetRun()
    {
        currentLevel = 1;
    }
}