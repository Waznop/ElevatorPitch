using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{

    public GameObject FloorPrefab;

    const float c1 = 0.37f;
    const float c2 = 0.33f;
    const float c3 = 0.29f;

    Color[] floorColors = {
        new Color(c1, c3, c3),
        new Color(c1, c2, c3),
        new Color(c1, c1, c3),
        new Color(c2, c1, c3),
        new Color(c3, c1, c3),
        new Color(c3, c1, c2),
        new Color(c3, c1, c1),
        new Color(c3, c2, c1),
        new Color(c3, c3, c1),
        new Color(c2, c3, c1),
        new Color(c1, c3, c1),
        new Color(c1, c3, c2)
    };

    GameObject[] floors;
    ArrayList floorsOn;

    public static int PosToNote(float pos)
    {
        return Mathf.RoundToInt(pos * 4 / Constants.PPU);
    }

    public static float NoteToPos(int note)
    {
        return note * Constants.PPU / 4f;
    }

    public static float ApproxNoteToPos(float note)
    {
        return note * Constants.PPU / 4f;
    }

    public static bool AtFloor(float note)
    {
        float precision = Constants.Endless ? Constants.EndlessPrecision : Constants.NormalPrecision;
        return Mathf.Abs(note - Mathf.Round(note)) < precision;
    }

    public static Color LightUp(Color color)
    {
        return color + new Color(0.5f, 0.5f, 0.5f);
    }

    public Color GetColor(int note)
    {
        return floorColors[note % floorColors.Length];
    }

    void Start()
    {
        float worldScreenWidth = (Camera.main.orthographicSize * 2 / Screen.height) * Screen.width;

        floorsOn = new ArrayList();
        floors = new GameObject[Constants.MaxNote - Constants.MinNote + 1];
        for (int note = Constants.MinNote; note <= Constants.MaxNote; note++)
        {
            Vector3 pos = new Vector3(0, NoteToPos(note));
            GameObject floor = Instantiate(FloorPrefab, pos, Quaternion.identity);
            floors[note - Constants.MinNote] = floor;

            var left = floor.transform.Find("left_floor").GetComponent<SpriteRenderer>();
            var right = floor.transform.Find("right_floor").GetComponent<SpriteRenderer>();
            var bg = floor.transform.Find("background").GetComponent<SpriteRenderer>();
            var text = floor.transform.Find("name").GetComponent<TextMesh>();

            Vector3 bgScale = bg.transform.localScale;
            bgScale.x = worldScreenWidth / bg.sprite.bounds.size.x;
            bg.transform.localScale = bgScale;

            bg.color = GetColor(note);

            Vector3 leftPos = left.transform.position;
            leftPos.x = -worldScreenWidth / 4 - 1;
            left.transform.position = leftPos;

            Vector2 leftSize = left.size;
            leftSize.x = worldScreenWidth / 2 - 2;
            left.size = leftSize;

            Vector3 rightPos = right.transform.position;
            rightPos.x = worldScreenWidth / 4 + 1;
            right.transform.position = rightPos;

            Vector2 rightSize = right.size;
            rightSize.x = worldScreenWidth / 2 - 2;
            right.size = rightSize;

            text.text = PitchManager.NoteToName(note);

            Vector3 textPos = text.transform.position;
            textPos.x = worldScreenWidth / 2 - 1;
            text.transform.position = textPos;
        }
    }

    public void LightOn(int note) {
        int idx = note - Constants.MinNote;
        GameObject floor = floors[idx];
        var bg = floor.transform.Find("background").GetComponent<SpriteRenderer>();
        bg.color = LightUp(bg.color);
        floorsOn.Add(note);
    }

    public void LightOff(int note) {
        int idx = note - Constants.MinNote;
        GameObject floor = floors[idx];
        var bg = floor.transform.Find("background").GetComponent<SpriteRenderer>();
        bg.color = GetColor(note);
        floorsOn.Remove(note);
    }

    public int FloorsOnAbove(int note) {
        int count = 0;
        foreach (int floor in floorsOn) {
            if (floor > note) count++;
        }
        return count;
    }

    public int FloorsOnUnder(int note) {
        int count = 0;
        foreach (int floor in floorsOn) {
            if (floor < note) count++;
        }
        return count;
    }
}
