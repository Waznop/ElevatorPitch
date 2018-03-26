using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour {

    public Text Lastscore;
    public Text Highscore;

	public void Play()
    {
        Constants.Level = LevelManager.GenerateLevel(10);
        Constants.Endless = false;
        SceneManager.LoadScene("Main");
    }

	void Start()
	{
        if (PlayerPrefs.HasKey(GameLogic.HighscoreKey)) {
            int hs = PlayerPrefs.GetInt(GameLogic.HighscoreKey);
            Highscore.text = "Highest: " + hs.ToString();
        }

        if (PlayerPrefs.HasKey(GameLogic.LastscoreKey)) {
            int ls = PlayerPrefs.GetInt(GameLogic.LastscoreKey);
            Lastscore.text = "Latest: " + ls.ToString();
        }
	}
}
