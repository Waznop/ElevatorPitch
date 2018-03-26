using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    public float SmoothTime = 0.3f;

    Vector3 velocity = Vector3.zero;
    Transform target;

    float minHeight;
    float maxHeight;

	void Awake()
	{
        Camera cam = gameObject.GetComponent<Camera>();
        int mult = Constants.Endless ? 2 : 8;
        cam.orthographicSize = Screen.height / (Constants.PPU * mult);
	}

	void Start()
    {
        Camera cam = gameObject.GetComponent<Camera>();

        minHeight = FloorManager.NoteToPos(Constants.MinNote) + cam.orthographicSize - 2;
        maxHeight = FloorManager.NoteToPos(Constants.MaxNote) - cam.orthographicSize + 2;

        target = FindObjectOfType<ElevatorControl>().transform;
        transform.position = new Vector3(0, minHeight);
    }

    void LateUpdate()
    {
        Vector3 targetPos = target.position;
        targetPos.y = Mathf.Max(targetPos.y, minHeight);
        targetPos.y = Mathf.Min(targetPos.y, maxHeight);
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, SmoothTime);
    }

}
