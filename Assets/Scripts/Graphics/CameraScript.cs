using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    public float SmoothTime = 0.3f;

    public const int PPU = 16;

    Vector3 velocity = Vector3.zero;
    Transform target;

    float minHeight;
    float maxHeight;

	void Awake()
	{
        int mult = GameLogic.Level == null ? 4 : 8;

        Camera cam = gameObject.GetComponent<Camera>();
        cam.orthographicSize = Screen.height / (PPU * mult);
	}

	void Start()
    {
        Camera cam = gameObject.GetComponent<Camera>();

        minHeight = GameLogic.MinNote * PPU / 4 + cam.orthographicSize - 2;
        maxHeight = GameLogic.MaxNote * PPU / 4 - cam.orthographicSize + 2;

        target = FindObjectOfType<ElevatorControl>().transform;
        // transform.position = new Vector3(0, GameLogic.MinNote * PPU / 4);
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
