using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour {
    [SerializeField] private MainBuilding mainBuilding;
    [SerializeField] private float restartDelay = 2f;
    [SerializeField] private GameObject gameOverUI;

    void OnEnable() {
        mainBuilding.OnCoreDestroyed += HandleGameOver;
    }

    void OnDisable() {
        mainBuilding.OnCoreDestroyed -= HandleGameOver;
    }

    void HandleGameOver() {
        if (gameOverUI) gameOverUI.SetActive(true);
        StartCoroutine(RestartGameRoutine());
    }

    IEnumerator RestartGameRoutine() {
        yield return new WaitForSeconds(restartDelay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}