using UnityEngine;
using UnityEngine.AI;

// Needs: NavMeshAgent, Rigidbody (Is Kinematic ON, Use Gravity OFF), 1+ waypoints.
// The Trigger Cube is now only the "caught" zone. Spotting the player uses detectRange.
public class GuardAI : MonoBehaviour
{
    public Transform[] waypoints;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4.5f;
    public float detectRange = 8f;    // how far away the guard notices you (needs a clear line of sight)
    public float catchTime = 1.3f;    // keep the same as Hold Time on the Collider Event
    public float recoverTime = 1f;    // seconds for the bar to drain after you escape/hide

    public Collider catchZone;        // drag the Trigger Cube here: it (and its scripts, like the Collider Event) get switched off while you hide

    public bool isInHiding;           // set by HidingZone
    public bool chasing;
    [Range(0f, 1f)] public float danger;   // 0 = safe, 1 = caught (read by the progress bar)

    NavMeshAgent agent;
    Transform player;
    int index;
    float lastInZone = -10f;
    MonoBehaviour[] catchScripts;
    bool catchActive = true;   // switched off on the first frame (not chasing yet)

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;
        agent.SetDestination(waypoints[index].position);

        if (catchZone != null) catchScripts = catchZone.GetComponents<MonoBehaviour>();

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        // can only be caught while he is chasing you, and never while you hide
        bool canCatch = chasing && !isInHiding;
        if (catchZone != null && catchActive != canCatch) SetCatchActive(canCatch);

        UpdateDanger();

        if (!chasing && CanSee())
        {
            chasing = true;
            agent.speed = chaseSpeed;
        }

        if (chasing) Chase(); else Patrol();
    }

    // Switches the Trigger Cube's collider AND its scripts (Collider Event) on/off
    void SetCatchActive(bool on)
    {
        catchActive = on;
        catchZone.enabled = on;
        foreach (MonoBehaviour m in catchScripts) m.enabled = on;
    }

    // Close enough + nothing in the way + not hiding
    bool CanSee()
    {
        if (player == null || isInHiding) return false;
        if (Vector3.Distance(transform.position, player.position) > detectRange) return false;

        Vector3 eyes = transform.position + Vector3.up;
        Vector3 dir = (player.position + Vector3.up) - eyes;

        if (Physics.Raycast(eyes, dir, out RaycastHit hit, dir.magnitude, ~0, QueryTriggerInteraction.Ignore))
            return hit.transform.root == player.root;

        return false;
    }

    void UpdateDanger()
    {
        bool inCatchZone = Time.time - lastInZone < 0.1f;
        bool threatened = inCatchZone && chasing && !isInHiding;

        float rate = threatened ? 1f / catchTime : -1f / recoverTime;
        danger = Mathf.Clamp01(danger + rate * Time.deltaTime);
    }

    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            index = (index + 1) % waypoints.Length;
            agent.SetDestination(waypoints[index].position);
        }
    }

    void Chase()
    {
        // Player hid -> back to patrol
        if (isInHiding || player == null)
        {
            chasing = false;
            agent.speed = patrolSpeed;
            index = NearestWaypoint();
            agent.SetDestination(waypoints[index].position);
            return;
        }

        agent.SetDestination(player.position);
    }

    // The Trigger Cube (and bot's own trigger) = the "caught" zone
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")) lastInZone = Time.time;
    }

    int NearestWaypoint()
    {
        int best = 0;
        float bestDist = float.MaxValue;
        for (int i = 0; i < waypoints.Length; i++)
        {
            float d = Vector3.Distance(transform.position, waypoints[i].position);
            if (d < bestDist) { bestDist = d; best = i; }
        }
        return best;
    }
}