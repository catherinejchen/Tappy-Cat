using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public delegate void GameDelegate();
    public static event GameDelegate OnGameStarted;
    public static event GameDelegate OnGamePaused;
    public static event GameDelegate OnGameOverConfirmed;
    public static GameManager Instance;

    public GameObject start_page;
    public GameObject game_over_page;
    public GameObject game_page;
    public GameObject paused_page;
    public GameObject settings_page;
    public Text score_text;
    public Text game_over_score;

    enum PageState {
        Game,
        Start,
        Paused,
        GameOver,
        Settings
    }

    int score = 0;
    bool game_over = true;

    public bool GameOver{
        get{
            return game_over;
        }
    }

    void Awake(){
        Instance = this;
    }

    void OnEnable(){
        TapController.OnPlayerScored += OnPlayerScored;
        TapController.OnPlayerDied += OnPlayerDied;
    }

    void OnDisable(){
        TapController.OnPlayerScored -= OnPlayerScored;
        TapController.OnPlayerDied -= OnPlayerDied;
    }

    void OnPlayerScored(){
        score++;
        score_text.text = score.ToString();
    }
    void OnPlayerDied(){
        int saved_score;
        game_over_score.text = "Score: " + score.ToString();
        game_over = true;
        saved_score = PlayerPrefs.GetInt("Highscore");
        if (score > saved_score){
            PlayerPrefs.SetInt("Highscore", score);
        }
        SetPageState(PageState.GameOver);
    }

    void SetPageState(PageState state){
        switch (state){
            case PageState.Game:
                start_page.SetActive(false);
                paused_page.SetActive(false);
                game_page.SetActive(true);
                game_over_page.SetActive(false);
                settings_page.SetActive(false);
                break;

            case PageState.Start:
                start_page.SetActive(true);
                paused_page.SetActive(false);
                game_page.SetActive(false);
                game_over_page.SetActive(false);
                settings_page.SetActive(false);
                break;

            case PageState.Paused:
                start_page.SetActive(false);
                paused_page.SetActive(true);
                game_page.SetActive(true);
                game_over_page.SetActive(false);
                settings_page.SetActive(false);
                break;

            case PageState.GameOver:
                start_page.SetActive(false);
                paused_page.SetActive(false);
                game_page.SetActive(false);
                game_over_page.SetActive(true);
                settings_page.SetActive(false);
                break;

            case PageState.Settings:
                start_page.SetActive(false);
                paused_page.SetActive(false);
                game_page.SetActive(false);
                game_over_page.SetActive(false);
                settings_page.SetActive(true);
                break;
        }
    }

   public void ConfirmGameOver(){
        // activated when replay button is hit
        OnGameOverConfirmed(); // event
        score = 0;
        score_text.text = "0";
        SetPageState(PageState.Game);
        OnGameStarted();
        game_over = false;
    }

    public void StartGame(){
        //when play button is hit
        SetPageState(PageState.Game);
        score = 0;
        OnGameStarted();
        game_over = false;
    }

    public void PauseGame(){
        //pause button hit
        SetPageState(PageState.Paused);
        game_over = true;
        OnGamePaused();
    }

    public void ContinueGame(){
        //continue game button hit
        SetPageState(PageState.Game);
        game_over = false;
        OnGameStarted();
    }

    public void Settings(){
        //activates setting page
        SetPageState(PageState.Settings);
    }

    public void ReturnHome(){
        //home button in settings hit
        SetPageState(PageState.Start);
    }
}
