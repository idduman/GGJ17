using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum Shape { Sine, Square, Triangle, Flat }

public class ElectricWire : MonoBehaviour
{
    public Text scoreText;
    public Material Electric_Sine, Electric_Square, Electric_Triangle;
    public GameObject minion_pink,minion_green,minion_purple;
    public GameObject smoke,shock;
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
    public float spawntime = 2.5f;
    [HideInInspector]
    public float magnitude,magnitude_max,pingpong;
    [HideInInspector]
    public bool pass;
    public AudioSource Buzz;
    [HideInInspector]
    public int health;
    private Vector3[] points;
    private Vector3 startPos, endPos;
    private float distance,timer;
    private GameObject m;

    // Use this for initialization
    void Start()
    {
        score = 0;
        phase = 0;
        frequency = 1;
        pingpong = 0;
        timer = 1;
        pass = false;
        magnitude_max = (StartPoint.localScale.y / 2) - 0.15f;
        startPos = StartPoint.position; //position initialization
        startPos.y -= magnitude_max;
        endPos = EndPoint.position;
        endPos.y -= magnitude_max;
        distance = Mathf.Abs(endPos.x - startPos.x);
        lineRenderer = GetComponent<LineRenderer>();
        points = new Vector3[SampleRate];
        lineRenderer.SetWidth(swidth, ewidth);
        waveform = Shape.Sine;
        health = 15;
        scoreText.text = "0";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if(frequency > 1)
                frequency /= 2;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if(frequency < 2)
                frequency *= 2;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            phase++;
            phase %= 2;
        }
        if (Input.GetKey(KeyCode.Q) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.E))
        {
            if (waveform != Shape.Sine)
            {
                magnitude = 0;
                waveform = Shape.Sine;
                lineRenderer.material = Electric_Sine;
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
                lineRenderer.material = Electric_Square;
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
                lineRenderer.material = Electric_Triangle;
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

        if(timer >= spawntime)
        {
            timer = 0;
            SpawnMinion();
        }
        timer += Time.deltaTime;

        Buzz.volume = 0.02f + 0.07f * (magnitude / magnitude_max);

        SetWave(); //CALL THE FUNCTION THAT SETS THE LINE RENDERER

        magnitude -= magrate * Time.deltaTime;
    }

    void SetWave()
    {
        float x, phaseShift, t;
        switch (waveform)
        {
            case Shape.Sine:
                phaseShift = (phase - 0.5f) * Mathf.PI;
                for (int i = 0; i < SampleRate; i++)
                {
                    t = Mathf.Lerp(0, frequency * 2 * SegmentCount * Mathf.PI, (float)i / SampleRate);
                    points[i] = Vector3.Lerp(startPos, endPos, (float)i / (SampleRate - 1));
                    points[i].y += (magnitude + pingpong) * 0.8f * (1 + Mathf.Sin(t + phaseShift)) + Random.Range(-0.01f, 0.01f); //POINT SET
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

    void SpawnMinion()
    {
        Quaternion rot = Quaternion.identity;
        rot.eulerAngles = new Vector3(0, 180, 0);
        int w = Random.Range(0, 3);
        switch (w)
        {
            case 0:
                m = (GameObject)Instantiate(minion_green, new Vector3(0, 0.05f, 3), rot);
                m.GetComponent<Minion>().waveform = Shape.Sine;
                break;
            case 1:
                m = (GameObject)Instantiate(minion_pink, new Vector3(0, 0.05f, 3), rot);
                m.GetComponent<Minion>().waveform = Shape.Square;
                break;
            case 2:
                m = (GameObject)Instantiate(minion_purple, new Vector3(0, 0.05f, 3), rot);
                m.GetComponent<Minion>().waveform = Shape.Triangle;
                break;
        }
        m.GetComponent<Minion>().Checkpoint_Left = StartPoint;
        m.GetComponent<Minion>().Checkpoint_Right = EndPoint;
        m.GetComponent<Minion>().wire = this;
    }

    public void Score(bool pass)
    {
        if (pass)
        {
            health++;
            score++;
            scoreText.text = score.ToString();
        }
        else
        {
            health -= 5;
        }
        Debug.Log("Score: " + score);
    }
}
