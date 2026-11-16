using UnityEngine;

// Put on each hiding zone (the object with the trigger collider, Is Trigger ticked).
// Nothing to drag in. Works with any number of robots and any number of zones.
// Works with both the old GuardAI and the new one.
public class HidingZone : MonoBehaviour
{
    static int count;                                  // how many zones the player is inside
    public static bool playerHidden => count > 0;

    bool inside;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics() { count = 0; }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !inside)
        {
            inside = true;
            count++;
            PushToGuards();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && inside)
        {
            inside = false;
            count--;
            PushToGuards();
        }
    }

    // Zone turned off / destroyed / scene reloaded while the player is inside
    void OnDisable()
    {
        if (!inside) return;
        inside = false;
        count--;
        if (gameObject.scene.isLoaded) PushToGuards();
    }

    // Tell every robot in the scene right now
    void PushToGuards()
    {
        foreach (GuardAI g in FindObjectsByType<GuardAI>(FindObjectsSortMode.None))
            g.isInHiding = playerHidden;
    }
}