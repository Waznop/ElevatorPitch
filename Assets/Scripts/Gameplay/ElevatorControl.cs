﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorControl : MonoBehaviour
{

    public float SmoothTime = 0.1f;

    const string stopTrigger = "Stopping";

    Animator animator;
    GameLogic gameLogic;

    Vector3 velocity = Vector3.zero;

    float stopTimer = 0;
    int stopFloor = -1;

    void Start()
    {
        animator = GetComponent<Animator>();
        gameLogic = GetComponent<GameLogic>();
        transform.position = new Vector3(0, FloorManager.NoteToPos(Constants.MinNote));
    }

    void Update()
    {
        if (!Constants.GameOn) return;

        float note = PitchManager.PitchToNote(PitchManager.PitchValue);
        note = Mathf.Min(note, Constants.MaxNote);
        note = Mathf.Max(note, Constants.MinNote);
        Vector3 target = new Vector3(0, FloorManager.ApproxNoteToPos(note));

        Vector3 prevVelocity = velocity;
        Vector3 prevPos = transform.position;
        transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, SmoothTime);

        bool was0 = Vector3.SqrMagnitude(prevVelocity - Vector3.zero) < Constants.VecPrecision;
        bool is0 = Vector3.SqrMagnitude(velocity - Vector3.zero) < Constants.VecPrecision;

        if (!was0 && is0)
        {
            if (FloorManager.AtFloor(note))
            {
                int floor = Mathf.RoundToInt(note);
                stopTimer = Constants.StopDelay;
                stopFloor = floor;
            }
        }
        else if (was0 && is0)
        {
            int curFloor = FloorManager.PosToNote(transform.position.y);
            if (curFloor == stopFloor && stopTimer > 0)
            {
                stopTimer -= Time.deltaTime;
                if (stopTimer <= 0)
                {
                    stopTimer = 0;
                    PitchManager.PitchValue = PitchManager.NoteToPitch(stopFloor);
                    Vector3 floorPos = new Vector3(0, FloorManager.NoteToPos(stopFloor));
                    transform.position = floorPos;
                    animator.SetTrigger(stopTrigger);
                    gameLogic.StoppedAt(stopFloor);
                }
            }
        }
    }
}
