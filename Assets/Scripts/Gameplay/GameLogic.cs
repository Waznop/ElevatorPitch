using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{

    PersonFactory factory;
    FloorManager floorManager;
    AudioSource source;
    float refFreq;

    public AudioClip DoClip;
    public AudioClip PointClip;
    public AudioClip DingClip;

    public Image UpArrow;
    public Image DownArrow;
    public Text UpText;
    public Text DownText;
    public Slider TimerBar;
    public Image TimerFill;
    public Text ScoreText;
    public Image EndPanel;
    public Text EndText;

    // normal
    int curIdx;
    int levelLen;

    // endless
    float spawnTime;
    float spawnTimer;

    // both
    ArrayList clients;
    float timer;
    int score;
    bool gameEnded;

    Vector3 timerBarPos;

    void Start()
    {
        factory = GetComponent<PersonFactory>();
        floorManager = GetComponent<FloorManager>();
        source = GetComponent<AudioSource>();
        refFreq = PitchManager.NoteToPitch(60); // C4

        Constants.GameOn = false;
        score = 0;
        timer = 0;
        gameEnded = false;
        timerBarPos = TimerBar.transform.position;
        clients = new ArrayList();

        if (Constants.Endless)
        {
            spawnTime = Constants.EndlessInitSpawnTime;
            timer = Constants.EndlessPatience;
        }
        else
        {
            curIdx = 0;
            levelLen = Constants.Level.Length;
            timer = Constants.NormalPatience;
        }

        Invoke("StartGame", 1);
    }

    void StartGame()
    {
        if (Constants.Endless)
        {
            TimerBar.gameObject.SetActive(true);
            Spawn();
            Constants.GameOn = true;
        }
        else
        {
            UpdateTarget();
        }
    }

    void Spawn()
    {

        if (Constants.Endless)
        {

            spawnTimer = spawnTime;
            spawnTime *= Constants.EndlessSpawnDecay;

            int from = Random.Range(Constants.MinNote, Constants.MaxNote + 1);
            int to = Random.Range(Constants.MinNote, Constants.MaxNote + 1);
            while (to == from)
            {
                to = Random.Range(Constants.MinNote, Constants.MaxNote + 1);
            }

            floorManager.LightOn(from, true);
            PersonScript person = factory.CreatePerson();
            person.Appear(from, to);
            clients.Add(person);

        }
        else
        {

            int from = Constants.Level[curIdx];
            int to = Constants.Level[curIdx + 1];

            string noteName = PitchManager.NoteToName(from);
            UpText.text = noteName;
            DownText.text = noteName;
            floorManager.LightOn(from, true);
            PlayNote(from);
            PersonScript person = factory.CreatePerson();
            person.Appear(from, to);
            clients.Add(person);

        }
    }

    void Update()
    {
        if (gameEnded && (Input.touchCount > 0 || Input.GetMouseButtonUp(0)))
        {
            Initiate.Fade("HexartMenu", Color.black, 3);
        }

        ScoreText.text = score.ToString();

        if (Constants.GameOn)
        {

            if (Constants.Endless)
            {

                if (spawnTimer > 0)
                {
                    spawnTimer -= Time.deltaTime;
                    if (spawnTimer <= 0)
                    {
                        spawnTimer = 0;
                        Spawn();
                    }
                }

                int note = FloorManager.PosToNote(transform.position.y);
                int floorsAbove = floorManager.FloorsOnAbove(note);
                int floorsUnder = floorManager.FloorsOnUnder(note);

                if (floorsAbove > 0)
                {
                    UpArrow.enabled = true;
                    UpText.enabled = true;
                    UpText.text = floorsAbove.ToString();
                }
                else
                {
                    UpArrow.enabled = false;
                    UpText.enabled = false;
                }

                if (floorsUnder > 0)
                {
                    DownArrow.enabled = true;
                    DownText.enabled = true;
                    DownText.text = floorsUnder.ToString();
                }
                else
                {
                    DownArrow.enabled = false;
                    DownText.enabled = false;
                }

                TimerBar.value = timer / Constants.EndlessPatience;
            }
            else
            {

                int targetNote = Constants.Level[curIdx];
                int curNote = FloorManager.PosToNote(transform.position.y);

                if (targetNote > curNote)
                {
                    UpArrow.color = floorManager.GetColor(targetNote);
                    UpArrow.color = FloorManager.LightUp(UpArrow.color);
                    UpArrow.enabled = true;
                    UpText.enabled = true;
                    DownArrow.enabled = false;
                    DownText.enabled = false;
                }
                else if (targetNote < curNote)
                {
                    DownArrow.color = floorManager.GetColor(targetNote);
                    DownArrow.color = FloorManager.LightUp(DownArrow.color);
                    UpArrow.enabled = false;
                    UpText.enabled = false;
                    DownArrow.enabled = true;
                    DownText.enabled = true;
                }
                else
                {
                    UpArrow.enabled = false;
                    UpText.enabled = false;
                    DownArrow.enabled = false;
                    DownText.enabled = false;
                }

                TimerBar.value = timer / Constants.NormalPatience;
            }

            foreach (PersonScript person in clients)
            {
                timer = Mathf.Min(timer, person.Patience);
            }

            TimerFill.color = Color.Lerp(Color.red, Color.yellow, TimerBar.value);
            if (TimerBar.value < Constants.CriticalTimer)
            {
                TimerBar.transform.position = timerBarPos + 3 * Random.insideUnitSphere;
            }

            if (timer <= 0)
            {
                timer = 0;
                TimerBar.transform.position = timerBarPos;
                EndGame();
            }
        }
    }

    IEnumerator ResumeGame(ArrayList toLightOn, int toLightOff)
    {
        yield return new WaitForSeconds(1);

        floorManager.LightOff(toLightOff);
        foreach (int note in toLightOn)
        {
            floorManager.LightOn(note, false);
            if (!Constants.Endless)
            {
                PlayNote(note);
                string noteName = PitchManager.NoteToName(note);
                UpText.text = noteName;
                DownText.text = noteName;
            }
        }

        if (Constants.Endless)
        {
            Constants.GameOn = true;
        }
        else
        {
            if (toLightOn.Count > 0 || clients.Count == 0)
            {
                curIdx++;
                UpdateTarget();
            }
            else
            {
                Constants.GameOn = true;
            }
        }
    }

    void NormalResumeGame()
    {
        source.Stop();
        TimerBar.gameObject.SetActive(true);
        Constants.GameOn = true;
    }

    void UpdateTarget()
    {
        if (curIdx < levelLen)
        {
            if (clients.Count == 0)
            {
                Spawn();
            }
            TimerBar.gameObject.SetActive(false);
            Invoke("NormalResumeGame", 1);
        }
        else
        {
            EndGame();
        }
    }

    public void StoppedAt(int note)
    {
        Constants.GameOn = false;

        ArrayList toLightOn = new ArrayList();
        ArrayList toRemove = new ArrayList();
        bool playedDing = false;
        foreach (PersonScript person in clients)
        {
            if (note == person.Origin && !person.InElevator)
            {
                person.transform.parent = transform;
                person.GetOnElevator();
                toLightOn.Add(person.Destination);
            }
            else if (note == person.Destination && person.InElevator)
            {
                toRemove.Add(person);
                person.transform.parent = null;
                person.GetOffElevator();
                timer = Constants.Endless ? Constants.EndlessPatience : Constants.NormalPatience;

                IncreaseScore((int)person.Patience);
                if (!playedDing) {
                    playedDing = true;
                    PlayDing(true);
                }
            }
        }

        if (!playedDing) {
            playedDing = true;
            PlayDing();
        }

        foreach (PersonScript person in toRemove)
            clients.Remove(person);

        StartCoroutine(ResumeGame(toLightOn, note));
    }

    void IncreaseScore(int inc)
    {
        score += inc;
    }

    void PlayDing(bool point = false)
    {
        source.clip = point ? PointClip : DingClip;
        source.pitch = 1;
        source.volume = 0.6f;
        source.Play();
    }

    void PlayNote(int note)
    {
        float targetFreq = PitchManager.NoteToPitch(note);
        source.clip = DoClip;
        source.pitch = targetFreq / refFreq;
        source.volume = 1;
        source.Play();
    }

    public void EndGame()
    {
        Constants.GameOn = false;

        int hs = 0;
        if (PlayerPrefs.HasKey(Constants.LevelKey))
        {
            hs = PlayerPrefs.GetInt(Constants.LevelKey);
        }

        if (score > hs)
        {
            PlayerPrefs.SetInt(Constants.LevelKey, score);
            hs = score;
        }

        EndPanel.gameObject.SetActive(true);
        EndText.text = Constants.LevelKey +
            "\n\nScore: " + score.ToString() +
            "\nHighscore: " + hs.ToString();

        gameEnded = true;
    }

}
