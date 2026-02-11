using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [Header("Caméras")]
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject deathCamera;
    [SerializeField] private GameObject transitionCamera;

    [Header("Paramètres de transition")]
    [SerializeField] private float transitionDuration = 1.5f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        mainCamera.SetActive(true);
    }

    public void ShowDeathCamera()
    {
        StopAllCoroutines();
        StartCoroutine(TransitionRoutine(mainCamera, deathCamera, true));
    }

    public Coroutine ShowMainCamera()
    {
        StopAllCoroutines();
        return StartCoroutine(TransitionRoutine(deathCamera, mainCamera, true));
    }

    IEnumerator TransitionRoutine(GameObject from, GameObject to, bool activateFinalAtEnd)
    {
        Camera fromCam = from.GetComponent<Camera>();
        Camera toCam = to.GetComponent<Camera>();
        Camera transCam = transitionCamera.GetComponent<Camera>();

        if (to == mainCamera)
        {
            to.SetActive(true);
            yield return new WaitForEndOfFrame();
            to.SetActive(false);
        }

        // On enregistre la position, rotation et FOV exacts à atteindre
        Vector3 targetPos = to.transform.position;
        Quaternion targetRot = to.transform.rotation;
        float targetFOV = toCam.fieldOfView;

        transitionCamera.transform.position = from.transform.position;
        transitionCamera.transform.rotation = from.transform.rotation;
        float startFOV = fromCam.fieldOfView;
        transCam.fieldOfView = startFOV;

        from.SetActive(false);
        transitionCamera.SetActive(true);

        float elapsed = 0;
        Vector3 startPos = transitionCamera.transform.position;
        Quaternion startRot = transitionCamera.transform.rotation;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);
            float curve = t * t * (3f - 2f * t);

            transitionCamera.transform.position = Vector3.Lerp(startPos, targetPos, curve);
            transitionCamera.transform.rotation = Quaternion.Slerp(startRot, targetRot, curve);
            transCam.fieldOfView = Mathf.Lerp(startFOV, targetFOV, curve);

            yield return null;
        }

        if (activateFinalAtEnd)
        {
            to.SetActive(true);
            toCam.fieldOfView = targetFOV;
            transitionCamera.SetActive(false);
        }
    }
}