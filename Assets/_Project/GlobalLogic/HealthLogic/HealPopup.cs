using UnityEngine;
using TMPro;

public class HealPopup : MonoBehaviour
{
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float fadeSpeed = 2f;

    private Camera cam;

    private TextMeshProUGUI text;
    private Color startColor;

    void Start()
    {
        cam = Camera.main;
    }

    void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        startColor = text.color;
    }

    public void Setup(float healAmount)
    {
        text.text = $"+{healAmount:0.##}";
    }

    void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // Fade out
        Color c = text.color;
        c.a -= fadeSpeed * Time.deltaTime;
        text.color = c;

        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
            Destroy(gameObject);
    }

    void LateUpdate()
    {
        if (cam != null)
            transform.LookAt(transform.position + cam.transform.forward);
    }

}
