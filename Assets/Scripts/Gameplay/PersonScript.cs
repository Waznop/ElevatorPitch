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
    public bool InElevator = false;

    public float SmoothTime = 1;

    const float precision = 0.01f;

    Vector3 velocity = Vector3.zero;
    Vector3 target;
    bool validTarget;
    bool killAtTarget;

    public void Appear(int from, int to) {
        Origin = from;
        Destination = to;
        Patience = Constants.Endless ? Constants.EndlessPatience : Constants.NormalPatience;

        float xpos = Random.Range(-5f, -3f);
        float ypos = FloorManager.NoteToPos(from) + Constants.PersonOffset;
        transform.position = new Vector3(xpos, ypos);

        MakePoof();
    }

    public void GetOnElevator() {
        InElevator = true;
        float xpos = Random.Range(-1f, 1f);
        float ypos = FloorManager.NoteToPos(Origin) + Constants.PersonOffset;
        MoveTowards(new Vector3(xpos, ypos));
    }

    public void GetOffElevator() {
        InElevator = false;
        float xpos = Random.Range(3f, 5f);
        float ypos = FloorManager.NoteToPos(Destination) + Constants.PersonOffset;
        MoveTowards(new Vector3(xpos, ypos), true);
    }

    void MoveTowards(Vector3 pos, bool kill = false)
    {
        target = pos;
        validTarget = true;
        killAtTarget = kill;
    }

    public void MakePoof()
    {
        Instantiate(Poof, transform.position, Quaternion.identity);
    }

	void Update()
	{
        if (validTarget) {
            Vector3 newPos = Vector3.SmoothDamp(transform.position, target, ref velocity, SmoothTime);

            if (transform.parent != null)
                newPos.y = transform.parent.transform.position.y + Constants.PersonOffset;

            transform.position = newPos;

            if (Vector3.SqrMagnitude(transform.position - target) < precision) {
                validTarget = false;
                if (killAtTarget) {
                    MakePoof();
                    Destroy(gameObject);
                }
            }
        }

        if (Constants.GameOn) {
            if (Patience > 0)
            {
                Patience -= Time.deltaTime;
                if (Patience <= 0)
                {
                    Patience = 0;
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
