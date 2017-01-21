using UnityEngine;
using System.Collections;

public class Minion : MonoBehaviour
{
    public Transform Checkpoint_Left, Checkpoint_Right, Anten_Left, Anten_Right;
    public ElectricWire wire;
    [HideInInspector]
    public Shape waveform;
    public float speed = 1f;
    private int phase,frequency;
    private bool active;
    private int lane;
    private float diff;

	// Use this for initialization
	void Start ()
    {
        diff = Checkpoint_Right.position.x - Checkpoint_Left.position.x;
        lane = 0;
        active = true;
        waveform = Shape.Sine;
        phase = 0;
        frequency = 2;
	}
	
	// Update is called once per frame
	void Update()
    {
        float laneX = Checkpoint_Left.position.x + diff * (lane + 0.5f) / 3;
        transform.position = new Vector3(laneX,transform.position.y, transform.position.z - speed * Time.deltaTime);
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
}
