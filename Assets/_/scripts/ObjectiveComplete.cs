using UnityEngine;

// Put on whatever marks the end of an objective, then pick WHEN it counts as done.
public class ObjectiveComplete : MonoBehaviour
{
    public enum When
    {
        PlayerEntersBox,            // needs a Box Collider with Is Trigger ticked
        ObjectTurnsOn,              // the object starts OFF and your Game Creator action turns it ON
        ObjectTurnsOffOrDestroyed   // e.g. the keycard disappearing when you pick it up
    }

    public When when = When.ObjectTurnsOffOrDestroyed;
    public int objectiveNumber = 1;   // 1 = first objective in the list, 2 = second...

    void OnTriggerEnter(Collider other)
    {
        if (when == When.PlayerEntersBox && other.CompareTag("Player"))
            Done();
    }

    void OnEnable()
    {
        // ignore the normal enabling that happens while the scene loads
        if (when == When.ObjectTurnsOn && Time.timeSinceLevelLoad > 0.5f)
            Done();
    }

    void OnDisable()
    {
        // ignore the scene unloading / quitting
        if (when == When.ObjectTurnsOffOrDestroyed && gameObject.scene.isLoaded)
            Done();
    }

    void Done()
    {
        Debug.Log("Objective " + objectiveNumber + " completed by " + name);
        ObjectiveUI.Complete(objectiveNumber - 1);
    }
}
