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
    private int SampleRate;
    private bool active;
    private float diff;
    private Vector3[] points;
    private Vector3 startPos, endPos;

	// Use this for initialization
	void Start ()
    {
        magnitude = 0.2f;
        SampleRate = 100;
        diff = Checkpoint_Right.position.x - Checkpoint_Left.position.x;
        active = true;
        points = new Vector3[SampleRate+2];
    }
	
	// Update is called once per frame
	void Update()
    {
        float laneX = Checkpoint_Left.position.x + diff * (lane + 0.5f) / 3;
        transform.position = new Vector3(laneX,transform.position.y, transform.position.z - speed * Time.deltaTime);
        startPos = Anten_Left.position;
        endPos = Anten_Right.position;
        if (active && (transform.position.z <= Checkpoint_Left.position.z))
        {
            active = false;
            HitGate();
        }
        else if(transform.position.z <= (Checkpoint_Left.position.z - 1))
            Destroy(this.gameObject);

        if (Input.GetKeyDown("1"))
            lane = 0;
        else if (Input.GetKeyDown("2"))
            lane = 1;
        else if (Input.GetKeyDown("3"))
            lane = 2;
        SetWave();
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
