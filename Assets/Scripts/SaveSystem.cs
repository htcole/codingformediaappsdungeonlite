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
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class SaveSystem : MonoBehaviour
{
   
private void Update()
{
    if (Keyboard.current == null)
        return;

    if (Keyboard.current.tKey.wasPressedThisFrame)
    {
        SaveGame();
    }

    if (Keyboard.current.yKey.wasPressedThisFrame)
    {
      LoadGame();
    }
}

private bool LoadGame()
{
    string path = Application.dataPath + "/Saves/saveFile.sav";

    if (!File.Exists(path))
    {
        Debug.LogError("Save file not found: " + path);
        return false;
    }

    string json = File.ReadAllText(path);

    SimplePlayerMovement.SaveObject saveObject =
        JsonUtility.FromJson<SimplePlayerMovement.SaveObject>(json);

    if (saveObject == null)
    {
        Debug.LogError("Failed to load save data.");
        return false;
    }

    if (SimplePlayerMovement.Instance == null)
    {
        Debug.LogError("Player instance was not found!");
        return false;
    }


        DungeonGenerator dungeon = Object.FindAnyObjectByType<DungeonGenerator>();

        if (dungeon == null)
        {
            Debug.LogError("DungeonGenerator was not found!");
            return false;
        }

        // Spawn all rooms leading up to the saved position.
        dungeon.SpawnRoomsUpToPosition(saveObject.position);

        // Then restore the player's position.
        SimplePlayerMovement.Instance.Load(saveObject);

        Debug.Log("Player position loaded: " + saveObject.position);

        return true;

    }

    private void SaveGame()
    {
        SimplePlayerMovement.SaveObject saveObject = SimplePlayerMovement.Instance.Save();
        string json = JsonUtility.ToJson(saveObject);
        File.WriteAllText(Application.dataPath + "/Saves/saveFile.sav", json);
    }
}

