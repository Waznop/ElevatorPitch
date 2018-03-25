using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorControl : MonoBehaviour
{

    public float SmoothTime = 0.1f;

    public GameLogic gameLogic;

    const string stopTrigger = "Stopping";
    const string suddenRunTrigger = "SuddenRun";

    const float precision = 0.01f;

    Animator animator;

    Vector3 velocity = Vector3.zero;

    public static ElevatorControl Instance;

    static int GetFloor(Vector3 pos) {
        return (int)Mathf.Round(pos.y * 4 / CameraScript.PPU);
    }

	void Awake()
	{
        if (Instance == null) {
            Instance = this;
        }
	}

	void Start()
    {
        animator = GetComponent<Animator>();
        transform.position = new Vector3(0, GameLogic.MinNote * CameraScript.PPU / 4);
    }

    void Update()
    {
        if (!GameLogic.GameStarted) return;

        int floor = Mathf.Min(PitchManager.MidiNote, GameLogic.MaxNote);
        floor = Mathf.Max(floor, GameLogic.MinNote);
        Vector3 target = new Vector3(0, floor * CameraScript.PPU / 4);

        Vector3 prevVelocity = velocity;
        Vector3 prevPos = transform.position;
        transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, SmoothTime);

        bool was0 = Vector3.SqrMagnitude(prevVelocity - Vector3.zero) < precision;
        bool is0 = Vector3.SqrMagnitude(velocity - Vector3.zero) < precision;

        if (was0 && !is0) {
            animator.SetTrigger(suddenRunTrigger);
        } else if (!was0 && is0) {
            animator.SetTrigger(stopTrigger);
            gameLogic.StoppedAt(floor);
        }
    }
}
