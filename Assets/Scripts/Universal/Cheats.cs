using UnityEngine;

public class Cheats : MonoBehaviour
{
    private ClearAndResetGame _clearAndResetGame;

    void Start()
    {
        _clearAndResetGame = FindFirstObjectByType<ClearAndResetGame>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Home))
        {
            _clearAndResetGame.GameOver();
        }
        if (Input.GetKeyDown(KeyCode.End))
        {
            _clearAndResetGame.GameCompleted();
        }
    }
}