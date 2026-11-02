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

using System.IO;
using UnityEngine;
using UnityEngine.InputSystem; // Required for the new Input System

[RequireComponent(typeof(CharacterController))]
public class SimplePlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 12f;
    private CharacterController controller;

    public static SimplePlayerMovement Instance { get; private set; }

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Ensure a keyboard is connected
        if (Keyboard.current == null) return;

        float moveX = 0f;
        float moveZ = 0f;

        // Check WASD or Arrow keys using the new Input System
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX = -1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX = 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveZ = -1f;
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveZ = 1f;

        // Combine inputs into a direction vector and normalize so diagonal movement isn't faster
        Vector3 move = new Vector3(moveX, 0, moveZ).normalized;

        // Move the player
        controller.Move(move * speed * Time.deltaTime);
    }
    private void Awake() {
        Instance = this;
        /*
        string json = JsonUtility.ToJson(new SaveObject {
            position = transform.position,
        });
        Debug.Log(Application.dataPath + "\n" + Application.persistentDataPath);  
        File.WriteAllText(Application.dataPath + "/Saves/saveFile.sav" ,json);
        */
        /* string json = File.ReadAllText(Application.dataPath + "/Saves/saveFile.sav");
        SaveObject saveObject = JsonUtility.FromJson<SaveObject>(json);
        Debug.Log(saveObject.position);
        */
        
    }

    [System.Serializable]
    public class SaveObject
    {
        public Vector3 position;
    }

    public SaveObject Save()
    {
        return new SaveObject
        {
            position = transform.position
        };
    }

    public void Load(SaveObject saveObject)
    {
        CharacterController controller =
            GetComponent<CharacterController>();

        controller.enabled = false;
        transform.position = saveObject.position;
        controller.enabled = true;
    }

}