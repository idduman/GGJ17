using UnityEngine;
using System.Collections;

public class ElectricWire : MonoBehaviour
{

    LineRenderer lineRenderer;
    public Transform StartPoint, EndPoint;
    public int SampleRate = 300;
    public int SegmentCount = 3;
    public float swidth = 0.1f;
    public float ewidth = 0.1f;
    private enum Shape {Sine,Square,Triangle,Flat}
    private Shape waveform;
    private float phase,frequency,magnitude;
    private Vector3[] points;
    private Vector3 startPos, endPos;
    private float distance;

    // Use this for initialization
    void Start()
    {
        phase = 0;
        frequency = 1;
        magnitude = StartPoint.lossyScale.y / 2;
        startPos = StartPoint.position; //position initialization
        startPos.y -= magnitude;
        endPos = EndPoint.position;
        endPos.y -= magnitude;
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
                phaseShift = (phase - 1f) * (Mathf.PI / 2);
                for (int i = 0; i < SampleRate; i++)
                {
                    t = Mathf.Lerp(0, 2 * frequency * SegmentCount * Mathf.PI, (float)i / SampleRate);
                    points[i] = Vector3.Lerp(startPos, endPos, (float)i / (SampleRate - 1));
                    points[i].y += magnitude * (1 + Mathf.Sin(t + phaseShift));
                }
                break;
            case Shape.Square:
                phaseShift = phase * (Mathf.PI);
                for (int i = 0; i < SampleRate; i++)
                {
                    t = Mathf.Lerp(0, 2 * frequency * SegmentCount * Mathf.PI, (float)i / SampleRate);
                    points[i] = Vector3.Lerp(startPos, endPos, (float)i / (SampleRate - 1));
                    x = (((t + phaseShift) / Mathf.PI) % 4);
                    if (x < 2)
                    {
                        points[i].y += magnitude * 2;
                    }
                }
                break;
            case Shape.Triangle:
                phaseShift = phase * (Mathf.PI / 2);
                for (int i = 0; i < SampleRate; i++)
                {
                    t = Mathf.Lerp(0, 2 * frequency * SegmentCount * Mathf.PI, (float)i / SampleRate);
                    points[i] = Vector3.Lerp(startPos, endPos, (float)i / (SampleRate - 1));
                    x = (((t + phaseShift) / Mathf.PI) % 2);
                    if (x < 1)
                    {
                        points[i].y += magnitude * 2 * x;
                    }
                    else
                    {
                        points[i].y += magnitude * 2 * (2 - x);
                    }
                }
                break;
        }
        lineRenderer.SetVertexCount(SampleRate);
        lineRenderer.SetPositions(points);
    }

    void SetSineWave()
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
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            //currentSegmentCount /= 2;
            frequency /= 2;
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //currentSegmentCount *= 2;
            frequency *= 2;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            phase += 2;
        }
        SetWave();
        if (Input.GetKey(KeyCode.Q))
            waveform = Shape.Sine;
        if (Input.GetKey(KeyCode.W))
            waveform = Shape.Square;
        if (Input.GetKey(KeyCode.E))
            waveform = Shape.Triangle;
    }
}
