using UnityEngine;
using System.Collections;

public enum Shape { Sine, Square, Triangle, Flat }

public class ElectricWire : MonoBehaviour
{

    LineRenderer lineRenderer;
    public Transform StartPoint, EndPoint;
    public int SampleRate = 300;
    public int SegmentCount = 3;
    public Shape waveform;
    [HideInInspector]
    public int phase,frequency,score;
    public float swidth = 0.1f;
    public float ewidth = 0.1f;
    public float magrate = 1f;
    [HideInInspector]
    public float magnitude,magnitude_max,pingpong;
    [HideInInspector]
    public bool pass;
    private Vector3[] points;
    private Vector3 startPos, endPos;
    private float distance;

    // Use this for initialization
    void Start()
    {
        score = 0;
        phase = 0;
        frequency = 1;
        pingpong = 0;
        pass = false;
        magnitude_max = (StartPoint.lossyScale.y / 2) - 0.15f;
        startPos = StartPoint.position; //position initialization
        startPos.y -= magnitude_max;
        endPos = EndPoint.position;
        endPos.y -= magnitude_max;
        distance = Mathf.Abs(endPos.x - startPos.x);
        lineRenderer = GetComponent<LineRenderer>();
        points = new Vector3[SampleRate];
        lineRenderer.SetWidth(swidth, ewidth);
        waveform = Shape.Sine;
    }

    void SetWave()
    {
        float x,phaseShift,t;
        switch (waveform)
        {
            case Shape.Sine:
                phaseShift = (phase - 0.5f) * Mathf.PI;
                for (int i = 0; i < SampleRate; i++)
                {
                    t = Mathf.Lerp(0, frequency * 2 * SegmentCount * Mathf.PI, (float)i / SampleRate);
                    points[i] = Vector3.Lerp(startPos, endPos, (float)i / (SampleRate - 1));
                    points[i].y += (magnitude + pingpong) * (1 + Mathf.Sin(t + phaseShift)) + Random.Range(-0.01f, 0.01f); //POINT SET
                }
                break;
            case Shape.Square:
                phaseShift = phase * Mathf.PI;
                for (int i = 0; i < SampleRate; i++)
                {
                    t = Mathf.Lerp(0, frequency * 2 * SegmentCount * Mathf.PI, (float)i / SampleRate);
                    points[i] = Vector3.Lerp(startPos, endPos, (float)i / (SampleRate - 1));
                    x = (((t + phaseShift) / Mathf.PI) % 2);
                    if (x > 1)
                    {
                        points[i].y += (magnitude + pingpong) * 2 + Random.Range(-0.01f, 0.01f); //POINT SET
                    }
                    else
                        points[i].y += Random.Range(-0.01f, 0.01f); // POINT SET
                }
                break;
            case Shape.Triangle:
                phaseShift = phase * Mathf.PI;
                for (int i = 0; i < SampleRate; i++)
                {
                    t = Mathf.Lerp(0, frequency * 2 * SegmentCount * Mathf.PI, (float)i / SampleRate);
                    points[i] = Vector3.Lerp(startPos, endPos, (float)i / (SampleRate - 1));
                    x = (((t + phaseShift) / Mathf.PI) % 2);
                    if (x < 1)
                    {
                        points[i].y += (magnitude + pingpong) * 2.4f * x + Random.Range(-0.01f, 0.01f); //POINT SET
                    }
                    else
                    {
                        points[i].y += (magnitude + pingpong) * 2.4f * (2 - x) + Random.Range(-0.01f, 0.01f); //POINT SET
                    }
                }
                break;
        }
        lineRenderer.SetVertexCount(SampleRate);
        lineRenderer.SetPositions(points);
    }

    /*void SetSineWave()
    {
        float phaseShift = (phase - 0.75f) * (Mathf.PI / 2);
        for (int i=0; i<SampleRate; i++)
        {
            var t = Mathf.Lerp(0, 2 * frequency * SegmentCount * Mathf.PI, (float)i / SampleRate);
            points[i] = Vector3.Lerp(startPos, endPos, (float)i / (SampleRate-1));
            points[i].y += magnitude * (1 + Mathf.Sin(t + phaseShift));
        }
        lineRenderer.SetVertexCount(SampleRate);
        lineRenderer.SetPositions(points);
    }

    void SetSquareWave()
    {
        float x;
        float phaseShift = phase * (Mathf.PI / 2);
        for (int i = 0; i < SampleRate; i++)
        {
            var t = Mathf.Lerp(0, 2 * frequency * SegmentCount * Mathf.PI, (float)i / SampleRate);
            points[i] = Vector3.Lerp(startPos, endPos, (float)i / (SampleRate - 1));
            x = (((t + phaseShift) / Mathf.PI) % 2);
            if (x < 1)
            {
                points[i].y += magnitude * 2;
            }
        }
        lineRenderer.SetVertexCount(SampleRate);
        lineRenderer.SetPositions(points);
    }

    void SetTriangleWave()
    {
        float x;
        float phaseShift = phase * (Mathf.PI / 2);
        for (int i = 0; i < SampleRate; i++)
        {
            var t = Mathf.Lerp(0, 2 * frequency * SegmentCount * Mathf.PI, (float)i / SampleRate);
            points[i] = Vector3.Lerp(startPos, endPos, (float)i / (SampleRate - 1));
            x = (((t + phaseShift) / Mathf.PI) % 2);
            if (x < 1)
            {
                points[i].y += magnitude * 2 * x;
            }
            else
            {
                points[i].y += magnitude * 2 * (2-x);
            }
        }
        lineRenderer.SetVertexCount(SampleRate);
        lineRenderer.SetPositions(points);
    }*/

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if(frequency > 1)
                frequency /= 2;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if(frequency < 4)
                frequency *= 2;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            phase++;
        }
        if (Input.GetKey(KeyCode.Q) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.E))
        {
            if (waveform != Shape.Sine)
            {
                magnitude = 0;
                waveform = Shape.Sine;
            }
            else
                magnitude += magrate* 2 * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.Q) && !Input.GetKey(KeyCode.E))
        {
            if (waveform != Shape.Square)
            {
                magnitude = 0;
                waveform = Shape.Square;
            }
            else
                magnitude += magrate * 2 * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.E) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.Q))
        {
            if (waveform != Shape.Triangle)
            {
                magnitude = 0;
                waveform = Shape.Triangle;
            }
            else
                magnitude += magrate * 2 * Time.deltaTime;
        }

        magnitude = Mathf.Clamp(magnitude, 0, magnitude_max);

        if (magnitude > 0.99 * magnitude_max)
        {
            pass = true;
            pingpong += 0.01f;
            pingpong = Mathf.PingPong(5 * Time.time, 0.03f);
        }
        else
        {
            pass = false;
            pingpong = 0;
        }

        SetWave(); //CALL THE FUNCTION THAT SETS THE LINE RENDERER

        magnitude -= magrate * Time.deltaTime;
    }
}
