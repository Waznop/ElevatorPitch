using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonScript : MonoBehaviour
{

    public Sprite Hair;
    public Sprite Accessory;
    public Sprite Outer;
    public Sprite Top;
    public Sprite Bottom;

    public Sprite Eyes;
    public Sprite Shoes;
    public Sprite Skin;

    public GameObject PersonPart;
    public GameObject Poof;

    // endless
    public int Origin = -1;
    public int Destination = -1;
    public float Patience = 0;
    bool inElevator = false;

    public float SmoothTime = 1;

    const float precision = 0.01f;

    Vector3 velocity = Vector3.zero;
    Vector3 target;
    bool validTarget;
    bool killAtTarget;

    public void MoveTowards(Vector3 pos, bool kill = false)
    {
        target = pos;
        validTarget = true;
        killAtTarget = kill;
    }

    public void MakePoof()
    {
        Instantiate(Poof, transform.position, Quaternion.identity);
    }

    public void FloorStopped(int floor)
    {
        if (floor == Origin && !inElevator) {
            GameLogic.ClientInteraction(true, this);
            transform.parent = ElevatorControl.Instance.transform;
            float xpos = Random.Range(-1f, 1f);
            MoveTowards(new Vector3(xpos, floor * CameraScript.PPU / 4 - 0.5f));
            inElevator = true;
        } else if (floor == Destination && inElevator) {
            Patience = GameLogic.MaxPatience;
            GameLogic.ClientInteraction(false, this);
            GameLogic.FloorStop -= FloorStopped;
            float xpos = Random.Range(3f, 5f);
            MoveTowards(new Vector3(xpos, floor * CameraScript.PPU / 4 - 0.5f), true);
            inElevator = false;
        }
    }

	void Update()
	{
        if (validTarget) {
            Vector3 newPos = Vector3.SmoothDamp(transform.position, target, ref velocity, SmoothTime);
            if (inElevator)
                newPos.y = ElevatorControl.Instance.transform.position.y - 0.5f;
            transform.position = newPos;

            if (Vector3.SqrMagnitude(transform.position - target) < precision) {
                validTarget = false;
                if (killAtTarget) {
                    MakePoof();
                    Destroy(gameObject);
                }
            }
        }

        if (GameLogic.GameStarted) {
            if (Patience > 0)
            {
                Patience -= Time.deltaTime;
                GameLogic.MinPatience = Mathf.Min(GameLogic.MinPatience, Patience);
                if (Patience <= 0)
                {
                    Patience = 0;
                    GameLogic.EndGame();
                }
            }
        }
	}

	void Start()
    {
        if (Skin)
        {
            GameObject part = Instantiate(PersonPart, gameObject.transform);
            SpriteRenderer sr = part.GetComponent<SpriteRenderer>();
            sr.sprite = Skin;
            sr.color = Random.ColorHSV(0, 0.17f, 0.2f, 0.4f, 0.6f, 1);
            sr.sortingOrder = 3;
        }

        if (Eyes)
        {
            GameObject part = Instantiate(PersonPart, gameObject.transform);
            part.transform.parent = gameObject.transform;
            SpriteRenderer sr = part.GetComponent<SpriteRenderer>();
            sr.sprite = Eyes;
            sr.color = Random.ColorHSV(0, 1, 1, 1, 0, 0.4f);
            sr.sortingOrder = 4;
        }

        if (Hair)
        {
            GameObject part = Instantiate(PersonPart, gameObject.transform);
            part.transform.parent = gameObject.transform;
            SpriteRenderer sr = part.GetComponent<SpriteRenderer>();
            sr.sprite = Hair;
            sr.color = Random.ColorHSV();
            sr.sortingOrder = 5;
        }

        if (Shoes)
        {
            GameObject part = Instantiate(PersonPart, gameObject.transform);
            part.transform.parent = gameObject.transform;
            SpriteRenderer sr = part.GetComponent<SpriteRenderer>();
            sr.sprite = Shoes;
            sr.color = Random.ColorHSV();
            sr.sortingOrder = 6;
        }

        if (Bottom)
        {
            GameObject part = Instantiate(PersonPart, gameObject.transform);
            part.transform.parent = gameObject.transform;
            SpriteRenderer sr = part.GetComponent<SpriteRenderer>();
            sr.sprite = Bottom;
            sr.color = Random.ColorHSV();
            sr.sortingOrder = 6;
        }

        if (Top)
        {
            GameObject part = Instantiate(PersonPart, gameObject.transform);
            part.transform.parent = gameObject.transform;
            SpriteRenderer sr = part.GetComponent<SpriteRenderer>();
            sr.sprite = Top;
            sr.color = Random.ColorHSV();
            sr.sortingOrder = 6;
        }

        if (Outer)
        {
            GameObject part = Instantiate(PersonPart, gameObject.transform);
            part.transform.parent = gameObject.transform;
            SpriteRenderer sr = part.GetComponent<SpriteRenderer>();
            sr.sprite = Outer;
            sr.color = Random.ColorHSV();
            sr.sortingOrder = 7;
        }

        if (Accessory)
        {
            GameObject part = Instantiate(PersonPart, gameObject.transform);
            part.transform.parent = gameObject.transform;
            SpriteRenderer sr = part.GetComponent<SpriteRenderer>();
            sr.sprite = Accessory;
            sr.color = Random.ColorHSV();
            sr.sortingOrder = 8;
        }
    }
}
