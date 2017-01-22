using UnityEngine;
using System.Collections;

public class Minion : MonoBehaviour
{
    public Transform Checkpoint_Left, Checkpoint_Right, Anten_Left, Anten_Right;
    public LineRenderer Cable;
    public ElectricWire wire;
    public int lane = 0;
    public int phase = 0;
    public int frequency = 1;
    public Shape waveform;
    public float speed = 1f;
    private float magnitude, pingpong;
    private int SampleRate,newLane;
    private bool active;
    private float diff,laneX;
    private Vector3[] points;
    private Vector3 startPos, endPos;
    private bool CoroutineDone,isDead;
    private float horLerpTime = 1f;
    private float currentHorLerpTime;
    private float verLerpTime = 0.5f;
    private float currentVerLerpTime;
    private float minionY;
    private float deathTimer, deathTimerMax;
    private Animator anim;
    private GameObject smoke,shock;
    public Texture shockTexture;
    public MeshRenderer surat;
    public GameObject zapSkeletonObj;

    public float LaneX
    {
        get
        {
            return Checkpoint_Left.position.x + diff * (lane + 0.5f) / 3;
        }
    }

	// Use this for initialization
	void Start ()
    {
        magnitude = 0.2f;
        SampleRate = 150;
        minionY = 0.05f;
        diff = Checkpoint_Right.position.x - Checkpoint_Left.position.x;
        lane = Random.Range(0, 3);
        phase = Random.Range(0, 2);
        frequency = Random.Range(1, 3);
        Cable = GetComponentInChildren<LineRenderer>();
        anim = GetComponent<Animator>();
        deathTimer = 0;
        deathTimerMax = 1.5f;
        smoke = null;
        shock = null;

        anim.SetBool("isDead", false);

        active = true;
        points = new Vector3[SampleRate+2];
        laneX = Checkpoint_Left.position.x + diff * (lane + 0.5f) / 3;
        newLane = lane;
        transform.position = new Vector3(laneX, 0.05f, 3f);
        isDead = false;
        CoroutineDone = true;
    }
	
	// Update is called once per frame
	void Update()
    {
        startPos = Anten_Left.position;
        endPos = Anten_Right.position;

        if(!isDead)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - speed * Time.deltaTime);
        }

        if (active && !isDead && (transform.position.z <= Checkpoint_Left.position.z))
        {
            active = false;
            HitGate();
        }
        else if(transform.position.z <= (Checkpoint_Left.position.z - 2))
            Destroy(this.gameObject);


        if (CoroutineDone && Input.GetKeyDown("1"))
            StartCoroutine(SwapLane(0));
        else if (CoroutineDone && Input.GetKeyDown("2"))
            StartCoroutine(SwapLane(1));
        else if (CoroutineDone && Input.GetKeyDown("3"))
            StartCoroutine(SwapLane(2));

        SetWave();
    }

    IEnumerator SwapLane(int destination)
    {
        CoroutineDone = false;
        currentHorLerpTime = 0;
        lane = destination;
        float newX,jumpdist = 0;
        var targetX = LaneX;
        Vector3 p = transform.position;
        var originalY = p.y;
        float diff = Mathf.Abs(transform.position.x - targetX);
        float originalDiff = diff;
        while (diff > 0.001f)
        {
            var percx = currentHorLerpTime / horLerpTime;
            p = transform.position;
            newX = Mathf.Lerp(p.x, targetX, percx);
            jumpdist = Mathf.Sin((originalDiff-diff) * Mathf.PI) * 0.5f;
            transform.position = new Vector3(newX, originalY + jumpdist, p.z);
            currentHorLerpTime += Time.deltaTime;
            currentVerLerpTime += Time.deltaTime;
            diff = Mathf.Abs(transform.position.x - targetX);
            yield return null;
        }
        transform.position = new Vector3(p.x, originalY, p.z);
        CoroutineDone = true;
    }

    void HitGate()
    {
        if (wire.pass && waveform == wire.waveform && phase == wire.phase && frequency == wire.frequency)
        {
            //Debug.Log("Pass");
            wire.Score(true);
        }
        else
        {
            //Debug.Log("SHOCK!");
            anim.SetBool("isDead", true);
            isDead = true;
            wire.Score(false);
            surat.material.mainTexture = shockTexture;
            StartCoroutine(DeathCoroutine(1f));
        }
    }

    IEnumerator DeathCoroutine(float duration)
    {
        shock = (GameObject)Instantiate(wire.shock, transform.position, transform.rotation);
        shock.transform.Translate(0, 0.6f, 0);
        StartCoroutine(ZapCoroutine(8));
        yield return new WaitForSeconds(duration);
        Destroy(shock);
        Cable.gameObject.active = false;
        smoke = (GameObject)Instantiate(wire.smoke, transform.position, transform.rotation);
        foreach (var r in GetComponentsInChildren<MeshRenderer>())
        {
            r.enabled = false;
        }
        yield return new WaitForSeconds(duration);
        Destroy(smoke);
        Destroy(this.gameObject);
    }

    IEnumerator ZapCoroutine(int count)
    {
        
        while(count > 0)
        {
            zapSkeletonObj.SetActive(!zapSkeletonObj.activeInHierarchy);
            foreach (var r in GetComponentsInChildren<MeshRenderer>())
            {
                if(r.gameObject != zapSkeletonObj)
                {
                    r.enabled = !zapSkeletonObj.activeInHierarchy;
                }
            }
            count--;
            yield return new WaitForSeconds(0.03f);
        }
    }

    void SetWave()
    {
        float x, phaseShift, t;
        points[0] = startPos;
        points[SampleRate + 1] = endPos;
        switch (waveform)
        {
            case Shape.Sine:
                phaseShift = (phase - 0.5f) * Mathf.PI;
                for (int i = 1; i < SampleRate+1; i++)
                {
                    t = Mathf.Lerp(0, frequency * 2 * Mathf.PI, (float)i / SampleRate);
                    points[i] = Vector3.Lerp(startPos, endPos, (float)i / (SampleRate - 1));
                    points[i].y += magnitude * 0.8f * (1 + Mathf.Sin(t + phaseShift)); //POINT SET
                }
                break;
            case Shape.Square:
                phaseShift = phase * Mathf.PI;
                for (int i = 1; i < SampleRate+1; i++)
                {
                    t = Mathf.Lerp(0, frequency * 2 * Mathf.PI, (float)i / SampleRate);
                    points[i] = Vector3.Lerp(startPos, endPos, (float)i / (SampleRate - 1));
                    x = (((t + phaseShift) / Mathf.PI) % 2);
                    if (x > 1)
                    {
                        points[i].y += magnitude * 2; //POINT SET
                    }
                }
                break;
            case Shape.Triangle:
                phaseShift = phase * Mathf.PI;
                for (int i = 1; i < SampleRate+1; i++)
                {
                    t = Mathf.Lerp(0, frequency * 2 * Mathf.PI, (float)i / SampleRate);
                    points[i] = Vector3.Lerp(startPos, endPos, (float)i / (SampleRate - 1));
                    x = (((t + phaseShift) / Mathf.PI) % 2);
                    if (x < 1)
                    {
                        points[i].y += magnitude * 2.4f * x; //POINT SET
                    }
                    else
                    {
                        points[i].y += magnitude * 2.4f * (2 - x); //POINT SET
                    }
                }
                break;
        }
        Cable.SetVertexCount(SampleRate+2);
        Cable.SetPositions(points);
    }
}
