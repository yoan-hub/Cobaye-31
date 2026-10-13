using UnityEngine;

// Put on each hiding zone (the object with the trigger collider).
// Drag Robot-1 into the "guard" slot.
public class HidingZone : MonoBehaviour
{
    public GuardAI guard;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) guard.isInHiding = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) guard.isInHiding = false;
    }
}
