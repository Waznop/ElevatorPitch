using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{

    public GameObject FloorPrefab;

    const float c1 = 0.37f;
    const float c2 = 0.33f;
    const float c3 = 0.29f;

    static GameObject[] floors;

    static ArrayList floorsOn = new ArrayList();

    public static Color[] FloorColors = {
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

    public static Color GetColor(int midi)
    {
        return FloorColors[midi % FloorColors.Length];
    }

    public static Color LightUp(Color color)
    {
        return color + new Color(0.5f, 0.5f, 0.5f);
    }

    void Start()
    {
        float worldScreenWidth = (Camera.main.orthographicSize * 2 / Screen.height) * Screen.width;

        floors = new GameObject[GameLogic.MaxNote - GameLogic.MinNote + 1];
        for (int i = GameLogic.MinNote; i <= GameLogic.MaxNote; i++)
        {
            Vector3 pos = new Vector3(0, i * CameraScript.PPU / 4);
            GameObject floor = Instantiate(FloorPrefab, pos, Quaternion.identity);
            floors[i - GameLogic.MinNote] = floor;
            var left = floor.transform.Find("left_floor").GetComponent<SpriteRenderer>();
            var right = floor.transform.Find("right_floor").GetComponent<SpriteRenderer>();
            var bg = floor.transform.Find("background").GetComponent<SpriteRenderer>();

            Vector3 bgScale = bg.transform.localScale;
            bgScale.x = worldScreenWidth / bg.sprite.bounds.size.x;
            bg.transform.localScale = bgScale;

            bg.color = GetColor(i);

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
        }
    }

    public static void LightOn(int note) {
        int idx = note - GameLogic.MinNote;
        GameObject floor = floors[idx];
        var bg = floor.transform.Find("background").GetComponent<SpriteRenderer>();
        bg.color = LightUp(bg.color);
        floorsOn.Add(note);
    }

    public static void LightOff(int note) {
        int idx = note - GameLogic.MinNote;
        GameObject floor = floors[idx];
        var bg = floor.transform.Find("background").GetComponent<SpriteRenderer>();
        bg.color = GetColor(note);
        floorsOn.Remove(note);
    }

    public static int FloorsOnAbove(int floor) {
        int count = 0;
        foreach (int f in floorsOn) {
            if (f > floor) count++;
        }
        return count;
    }

    public static int FloorsOnUnder(int floor) {
        int count = 0;
        foreach (int f in floorsOn) {
            if (f < floor) count++;
        }
        return count;
    }
}
