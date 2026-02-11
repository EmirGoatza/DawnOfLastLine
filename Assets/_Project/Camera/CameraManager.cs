using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject deathCamera;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        ShowMainCamera(); // État par défaut
    }

    public void ShowMainCamera()
    {
        mainCamera.SetActive(true);
        deathCamera.SetActive(false);
    }

    public void ShowDeathCamera()
    {
        mainCamera.SetActive(false);
        deathCamera.SetActive(true);
    }
}