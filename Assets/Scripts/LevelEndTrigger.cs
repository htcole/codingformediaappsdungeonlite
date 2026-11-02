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
using UnityEngine.SceneManagement;

public class TreasureRoomTrigger : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered || !other.CompareTag("Player"))
            return;

        triggered = true;

        // Overwrite the save file with the starting position.
        SimplePlayerMovement.SaveObject saveObject = new SimplePlayerMovement.SaveObject
            {
            position = new Vector3(0f, 2.5f, -3)
        };

        string path = Path.Combine(Application.dataPath,"Saves","saveFile.sav");

        Directory.CreateDirectory(Path.GetDirectoryName(path));

        string json = JsonUtility.ToJson(saveObject);
        File.WriteAllText(path, json);

        Debug.Log("Save overwritten with starting position: " + saveObject.position);

        //Advance to the next level.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.currentLevel++;

            Debug.Log("Advancing to Level: " + GameManager.Instance.currentLevel);
        }

        //Reload the scene.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
