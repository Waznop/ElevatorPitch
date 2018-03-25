using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{

    AudioSource source;

    public AudioClip DoClip;
    public AudioClip PointClip;

    float refFreq;

    public static int MinNote = 48; // C3
    public static int MaxNote = 72; // C5

    const float maxTimer = 5;
    float timer = 0;

    const float spawnDecay = 0.95f;
    float spawnTime = 5;
    float spawnTimer = 0;
    public static float MaxPatience = 30;
    public static float MinPatience = 0;

    static float pauseTime = 1;
    static float pauseTimer = 0;

    public static bool GameStarted = false;

    public Image UpArrow;
    public Image DownArrow;
    public Text UpText;
    public Text DownText;
    public Slider TimerBar;
    public Text ScoreText;

    public ElevatorControl Elevator;

    PersonFactory factory;
    GameObject client;

    public const string MinNoteKey = "minNote";
    public const string MaxNoteKey = "maxNote";
    public const string LastscoreKey = "lastscore";
    public const string HighscoreKey = "highscore";

    const float penalty = 1;

    static int totalScore = 0;

    int curIdx = -1;
    int targetIdx = 0;

    public static int[] Level;
    int levelLength;

    bool endless;

    public delegate void FloorStopHandler(int floor);
    public static event FloorStopHandler FloorStop; 

	void Awake()
	{
        if (PlayerPrefs.HasKey(MinNoteKey))
        {
            MinNote = PlayerPrefs.GetInt(MinNoteKey);
        }

        if (PlayerPrefs.HasKey(MaxNoteKey))
        {
            MaxNote = PlayerPrefs.GetInt(MaxNoteKey);
        }
	}

	void Start()
    {
        source = GetComponent<AudioSource>();
        refFreq = PitchManager.MidiToFreq(60); // C4
        factory = GetComponent<PersonFactory>();

        if (Level != null) {
            levelLength = Level.Length;
        } else {
            endless = true;
        }

        TimerBar.gameObject.SetActive(false);

        Invoke("StartGame", 1);
    }

    void SpawnEndless() {
        spawnTime *= spawnDecay;
        spawnTimer = spawnTime;

        if (!TimerBar.IsActive()) {
            TimerBar.gameObject.SetActive(true);
        }

        int from = Random.Range(MinNote, MaxNote + 1);
        int to = Random.Range(MinNote, MaxNote + 1);
        while (to == from) {
            to = Random.Range(MinNote, MaxNote + 1);
        }

        FloorManager.LightOn(from);
        GameObject person = factory.CreatePerson();

        float targetClientY = from * CameraScript.PPU / 4 - 0.5f;
        float xpos = Random.Range(-5f, -3f);
        person.transform.position = new Vector3(xpos, targetClientY);
        var ps = person.GetComponent<PersonScript>();
        ps.MakePoof();
        ps.Origin = from;
        ps.Destination = to;
        ps.Patience = MaxPatience;
        FloorStop += ps.FloorStopped;
    }

    public static void ClientInteraction(bool gettingOn, PersonScript person) {
        GameStarted = false;
        pauseTimer = pauseTime;

        if (gettingOn) {
            FloorManager.LightOff(person.Origin);
            FloorManager.LightOn(person.Destination);
        } else {
            totalScore += (int)(person.Patience * 10);
            MinPatience = MaxPatience;
            FloorManager.LightOff(person.Destination);
        }
    }

	void Update()
	{
        ScoreText.text = totalScore.ToString();

        if (GameStarted) {

            if (endless) {
                if (spawnTimer > 0)
                {
                    spawnTimer -= Time.deltaTime;
                    if (spawnTimer <= 0)
                    {
                        spawnTimer = 0;
                        SpawnEndless();
                    }
                }

                TimerBar.value = MinPatience / MaxPatience;

                int curFloor = Mathf.RoundToInt(Elevator.transform.position.y * 4f / CameraScript.PPU);
                int floorsAbove = FloorManager.FloorsOnAbove(curFloor);
                int floorsUnder = FloorManager.FloorsOnUnder(curFloor);

                if (floorsAbove > 0) {
                    UpArrow.enabled = true;
                    UpText.enabled = true;
                    UpText.text = floorsAbove.ToString();
                } else {
                    UpArrow.enabled = false;
                    UpText.enabled = false;
                }

                if (floorsUnder > 0) {
                    DownArrow.enabled = true;
                    DownText.enabled = true;
                    DownText.text = floorsUnder.ToString();
                } else {
                    DownArrow.enabled = false;
                    DownText.enabled = false;
                }
            } else {
                if (timer > 0)
                {
                    timer -= Time.deltaTime;
                    TimerBar.value = timer / maxTimer;
                    if (timer <= 0)
                    {
                        timer = 0;
                        EndGame();
                    }
                }

                int targetNote = Level[targetIdx];
                float target = targetNote * CameraScript.PPU / 4f;
                float current = Elevator.transform.position.y;

                if (target > current)
                {
                    UpArrow.color = FloorManager.GetColor(targetNote);
                    UpArrow.color = FloorManager.LightUp(UpArrow.color);
                    UpArrow.enabled = true;
                    DownArrow.enabled = false;
                }
                else if (target < current)
                {
                    DownArrow.color = FloorManager.GetColor(targetNote);
                    DownArrow.color = FloorManager.LightUp(DownArrow.color);
                    UpArrow.enabled = false;
                    DownArrow.enabled = true;
                }
                else
                {
                    UpArrow.enabled = false;
                    DownArrow.enabled = false;
                }
            }
        } else if (endless) {
            if (pauseTimer > 0) {
                pauseTimer -= Time.deltaTime;
                if (pauseTimer <= 0) {
                    pauseTimer = 0;
                    GameStarted = true;
                }
            }
        }
	}

    void GetPoint()
    {
        totalScore += (int)(timer * 10);
        source.clip = PointClip;
        source.pitch = 1;
        source.volume = 0.6f;
        source.Play();
    }

	void PlayNote(int midi)
    {
        float targetFreq = PitchManager.MidiToFreq(midi);
        source.clip = DoClip;
        source.pitch = targetFreq / refFreq;
        source.volume = 1;
        source.Play();
    }

    void StartGame()
    {
        if (endless)
        {
            MinPatience = MaxPatience;
            spawnTimer = spawnTime;
            GameStarted = true;
        }
        else
        {
            UpdateTarget();
        }
    }

    void ReachTarget(int target)
    {
        GameStarted = false;
        UpArrow.enabled = false;
        DownArrow.enabled = false;

        GetPoint();

        PersonScript ps = client.GetComponent<PersonScript>();
        float targetClientY = target * CameraScript.PPU / 4 - 0.5f;

        if (targetIdx % 2 == 0) { // getting on
            ps.transform.parent = Elevator.transform;
            ps.MoveTowards(new Vector3(0, targetClientY));
        } else { // getting off
            ps.MoveTowards(new Vector3(4, targetClientY), true);
        }

        StartCoroutine(AdvanceTarget(target));
    }

    void UpdateTarget()
    {
        if (curIdx < targetIdx)
        {
            int target = Level[targetIdx];
            PlayNote(target);
            curIdx = targetIdx;
            FloorManager.LightOn(target);
            TimerBar.gameObject.SetActive(false);

            if (client == null) {
                client = factory.CreatePerson();
                float targetClientY = target * CameraScript.PPU / 4 - 0.5f;
                client.transform.position = new Vector3(-4, targetClientY);
                var ps = client.GetComponent<PersonScript>();
                ps.MakePoof();
            }
        }

        Invoke("ResumeGame", 1);
    }

    IEnumerator AdvanceTarget(int target)
    {
        yield return new WaitForSeconds(1);

        FloorManager.LightOff(target);
        targetIdx++;

        if (targetIdx < levelLength)
        {
            UpdateTarget();
        }
        else
        {
            EndGame();
        }
    }

    public static void EndGame()
    {
        PlayerPrefs.SetInt(LastscoreKey, totalScore);

        if (PlayerPrefs.HasKey(HighscoreKey))
        {
            int hs = PlayerPrefs.GetInt(HighscoreKey);
            if (totalScore > hs)
            {
                PlayerPrefs.SetInt(HighscoreKey, totalScore);
            }
        }
        else
        {
            PlayerPrefs.SetInt(HighscoreKey, totalScore);
        }

        SceneManager.LoadScene("Menu");
    }

    void ResumeGame()
    {
        source.Stop();
        timer = maxTimer;
        TimerBar.gameObject.SetActive(true);
        GameStarted = true;
    }

    public void StoppedAt(int midi)
    {
        if (!GameStarted) return;

        if (endless) {
            if (FloorStop != null)
                FloorStop(midi);
        } else {
            if (targetIdx < levelLength)
            {
                int target = Level[targetIdx];
                if (target == midi)
                {
                    ReachTarget(target);
                }
                else
                {
                    timer -= penalty;
                    if (timer <= 0)
                    {
                        timer = 0;
                        EndGame();
                    }
                }
            }
        }
    }
}
