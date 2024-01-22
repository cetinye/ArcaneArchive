using System.Collections.Generic;
using UnityEngine;

public class W94_GameManager : MonoBehaviour
{
    public static W94_GameManager instance;
    [SerializeField] private W94_LevelManager levelManager;
    [SerializeField] private W94_UIManager uiManager;

    public GameState state;
    public Camera mainCamera;

    public Dictionary<string, string> statDescriptions = new Dictionary<string, string>
        {
            { "0-Correct", "Number of books matched" },
            { "1-Wrong", "Number of moves" },
        };

    private void Awake()
    {
        instance = this;
        state = GameState.idle;
        uiManager.PlayPauseVideo();
        uiManager.StartIntro();
    }

    public void StartGame()
    {
        uiManager.ResetTime();
        uiManager.DisableBlackScreen();
        levelManager.StartLevel();
    }

    public void RemoveFromBooksList(GameObject bookToRemove)
    {
        levelManager.RemoveBook(bookToRemove);
    }

    public void IncreaseTotalMoveCounter()
    {
        levelManager.IncreaseTotalMovesCounter();
    }

    public void CheckEndLevel()
    {
        if (levelManager.GetActiveBookCount() == 0)
        {
            W94_AudioManager.instance.PlayOneShot("Success");
            levelManager.StartEndAnim();
        }
    }

    public void CheckStuck()
    {
        if (levelManager.isShelvesFull())
            Finish();
    }

    public void Finish()
    {
        Debug.LogError("Game Finished");
    }

    public void ArrangeFrames(Transform parent)
    {
        uiManager.ArrangeFrames(parent);
    }

    public enum GameState
    {
        intro,
        idle,
        playing
    }
}
