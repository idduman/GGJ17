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
    private bool CoroutineDone;
    private float horLerpTime = 1f;
    private float currentHorLerpTime;
    private float verLerpTime = 0.5f;
    private float currentVerLerpTime;
    private float minionY;

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
        SampleRate = 100;
        minionY = 0.05f;
        diff = Checkpoint_Right.position.x - Checkpoint_Left.position.x;
        active = true;
        points = new Vector3[SampleRate+2];
        laneX = Checkpoint_Left.position.x + diff * (lane + 0.5f) / 3;
        newLane = lane;
        transform.position = new Vector3(laneX, 0.05f, 3f);
        CoroutineDone = true;
    }
	
	// Update is called once per frame
	void Update()
    {
        if(newLane != lane)
        {

        }
        else
        {

        }

        transform.position = new Vector3(transform.position.x ,transform.position.y, transform.position.z - speed * Time.deltaTime);
        startPos = Anten_Left.position;
        endPos = Anten_Right.position;
        if (active && (transform.position.z <= Checkpoint_Left.position.z))
        {
            active = false;
            HitGate();
        }
        else if(transform.position.z <= (Checkpoint_Left.position.z - 1))
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
        while(Mathf.Abs(transform.position.x - targetX) > 0.001f)
        {
            var percx = currentHorLerpTime / horLerpTime;
            p = transform.position;
            newX = Mathf.Lerp(p.x, targetX, percx);
            jumpdist = Mathf.Sin(percx * 2 * Mathf.PI);
            transform.position = new Vector3(newX, originalY + jumpdist, p.z);
            currentHorLerpTime += Time.deltaTime;
            currentVerLerpTime += Time.deltaTime;
            yield return null;
        }
        CoroutineDone = true;
    }

    void HitGate()
    {
        if (wire.pass && waveform == wire.waveform && phase == wire.phase && frequency == wire.frequency)
        {
            Debug.Log("Pass");
            wire.score += 1;
        }
        else
        {
            Debug.Log("SHOCK!");
            Destroy(this.gameObject);
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
                    points[i].y += magnitude * (1 + Mathf.Sin(t + phaseShift)); //POINT SET
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
