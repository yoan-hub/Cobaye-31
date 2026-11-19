using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

// Needs: NavMeshAgent, Rigidbody (Is Kinematic ON, Use Gravity OFF), 1+ waypoints,
// and a Trigger Cube (Is Trigger) as a child = the "caught" zone.
// Delete / untick the Game Creator "Collider Event" on the Trigger Cube: this script now handles the catch.
public class GuardAI : MonoBehaviour
{
    public Transform[] waypoints;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4.5f;
    public float detectRange = 8f;          // how far away the guard notices you (needs a clear line of sight)
    public float catchTime = 1.3f;          // seconds inside the catch zone, while he chases you, before you lose
    public float graceAfterHiding = 1.5f;   // after you leave a hiding spot, he can't spot or catch you for this long
    public float recoverTime = 1f;          // seconds for the bar to drain after you escape/hide
    public string loseScene = "loser";      // scene loaded when you are caught

    public bool isInHiding;                 // filled in automatically from HidingZone (just for you to see)
    public bool chasing;
    [Range(0f, 1f)] public float danger;    // 0 = safe, 1 = caught (also drives the progress bar)

    NavMeshAgent agent;
    Transform player;
    int index;
    float lastInZone = -10f;
    float graceUntil;
    bool caught;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;
        agent.SetDestination(waypoints[index].position);

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (caught) return;

        isInHiding = HidingZone.playerHidden;   // set by the hiding zones, same for every robot
        if (isInHiding) graceUntil = Time.time + graceAfterHiding;   // restarts the grace period while you hide

        UpdateDanger();
        if (caught) return;

        if (!chasing && CanSee())
        {
            chasing = true;
            agent.speed = chaseSpeed;
        }

        if (chasing) Chase(); else Patrol();
    }

    // Close enough + nothing in the way + not hiding + not in the grace period
    bool CanSee()
    {
        if (player == null || isInHiding || Time.time < graceUntil) return false;
        if (Vector3.Distance(transform.position, player.position) > detectRange) return false;

        Vector3 eyes = transform.position + Vector3.up;
        Vector3 dir = (player.position + Vector3.up) - eyes;

        if (Physics.Raycast(eyes, dir, out RaycastHit hit, dir.magnitude, ~0, QueryTriggerInteraction.Ignore))
            return hit.transform.root == player.root;

        return false;
    }

    // The ONLY clock: fills while you stand in the catch zone, drains otherwise. Full = you lose.
    void UpdateDanger()
    {
        bool inCatchZone = Time.time - lastInZone < 0.1f;
        bool threatened = inCatchZone && chasing && !isInHiding && Time.time >= graceUntil;

        float rate = threatened ? 1f / catchTime : -1f / recoverTime;
        danger = Mathf.Clamp01(danger + rate * Time.deltaTime);

        if (danger >= 1f) Catch();
    }

    void Catch()
    {
        caught = true;
        agent.isStopped = true;
        Debug.Log("GuardAI: " + name + " caught the player in " + SceneManager.GetActiveScene().name);

        ScreenFade fade = FindFirstObjectByType<ScreenFade>();
        if (fade != null) fade.FadeAndLoad(loseScene, 0.2f);
        else SceneManager.LoadScene(loseScene);
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

    // The Trigger Cube (and the bot's own trigger) = the "caught" zone
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