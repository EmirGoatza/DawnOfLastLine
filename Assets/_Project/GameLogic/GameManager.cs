using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private MainBuilding mainBuilding;
    [SerializeField] private float restartDelay = 2f;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private TMP_Text gameOverText;

    void OnEnable()
    {
        mainBuilding.OnCoreDestroyed += HandleGameOver;
    }

    void OnDisable()
    {
        mainBuilding.OnCoreDestroyed -= HandleGameOver;
    }

    void HandleGameOver()
    {
        if (gameOverUI) gameOverUI.SetActive(true);
        StartCoroutine(RestartGameRoutine());
    }

    IEnumerator RestartGameRoutine()
    {
        float remainingTime = restartDelay;

        while (remainingTime > 0f)
        {
            if (gameOverText)
            {
                gameOverText.text =
                    $"Game Over\nLa partie se relancera dans {remainingTime:F1} secondes";
            }

            remainingTime -= Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}