using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponAnims : MonoBehaviour
{

    public Animator weaponAnimator;


    // Update is called once per frame
    void Update()
    {
        // Checks if E is pressed
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame) { 
        // Checks if it exists
        if (weaponAnimator != null)
        {
            weaponAnimator.SetTrigger("SwingTrigger");
        }

        }
    }
}
