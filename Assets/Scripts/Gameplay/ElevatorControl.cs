using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorControl : MonoBehaviour
{

    public float SmoothTime = 0.1f;

    const string stopTrigger = "Stopping";
    const string suddenRunTrigger = "SuddenRun";

    Animator animator;
    GameLogic gameLogic;

    Vector3 velocity = Vector3.zero;

	void Start()
    {
        animator = GetComponent<Animator>();
        gameLogic = GetComponent<GameLogic>();
        transform.position = new Vector3(0, FloorManager.NoteToPos(Constants.MinNote));
    }

    void Update()
    {
        if (!Constants.GameOn) return;

        int note = Mathf.Min(PitchManager.MidiNote, Constants.MaxNote);
        note = Mathf.Max(note, Constants.MinNote);
        Vector3 target = new Vector3(0, FloorManager.NoteToPos(note));

        Vector3 prevVelocity = velocity;
        Vector3 prevPos = transform.position;
        transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, SmoothTime);

        bool was0 = Vector3.SqrMagnitude(prevVelocity - Vector3.zero) < Constants.VecPrecision;
        bool is0 = Vector3.SqrMagnitude(velocity - Vector3.zero) < Constants.VecPrecision;

        if (was0 && !is0) {
            animator.SetTrigger(suddenRunTrigger);
        } else if (!was0 && is0) {
            animator.SetTrigger(stopTrigger);
            gameLogic.StoppedAt(note);
        }
    }
}
