using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UI;

public class AugmentSelectionUI : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private RectTransform augmentPanel;
    private List<GameObject> cardPrefabs = new List<GameObject>(); // remplie automatiquement à partir du dossier "Resources/AugmentCards"
    [SerializeField] private int numberOfCardsToSpawn = 3;
    [SerializeField] private GameObject player;
    [SerializeField] private float inputCooldown = 0f;

    private List<AugmentCardUI> cards = new List<AugmentCardUI>();
    private int currentIndex = 0;
    private float lastInputTime;
    private bool isActive = false;

    [Header("Debug")]
    [SerializeField] private bool forceFirstCard = false;
    [SerializeField] private string forcedCardName;

    private void LoadAllPrefabs()
    {
        if (cardPrefabs.Count > 0) return; // Déjà chargé

        GameObject[] loadedPrefabs = Resources.LoadAll<GameObject>("Augments");
        cardPrefabs.AddRange(loadedPrefabs);

        if (cardPrefabs.Count == 0)
        {
            Debug.LogError("Aucun prefab d'Augment trouvé dans Resources/Augments !");
        }
        else
        {
            Debug.Log($"Nombre d'Augments chargés : {cardPrefabs.Count}");
        }
    }

    void Update()
    {
        if (!isActive) return;

        HandleNavigation();
        HandleSelection();
    }

    public void Show()
    {
        Debug.Log("Affichage de l'UI de sélection d'Augment");
        if (cardPrefabs.Count == 0)
        {
            LoadAllPrefabs();
        }
        GenerateCards();

        gameObject.SetActive(true);
        isActive = true;
        currentIndex = 0;
        HighlightCurrent();
        Time.timeScale = 0f;
    }

    public void Hide()
    {
        isActive = false;
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    private void GenerateCards()
    {
        // Nettoyage
        foreach (Transform child in augmentPanel)
        {
            Destroy(child.gameObject);
        }
        cards.Clear();

        List<GameObject> pool = new List<GameObject>(cardPrefabs);

        // -----------------------------
        // Force une carte si spécifié
        // -----------------------------
        if (forceFirstCard && !string.IsNullOrEmpty(forcedCardName))
        {
            Debug.LogWarning("Attention, carte forcée utilisée dans AugmentSelectionUI !");
            GameObject forcedPrefab = pool.Find(p => p.name == forcedCardName);

            if (forcedPrefab != null)
            {
                GameObject forcedCardObj = Instantiate(forcedPrefab, augmentPanel);
                forcedCardObj.transform.localScale = Vector3.one;

                AugmentCardUI forcedUI = forcedCardObj.GetComponent<AugmentCardUI>();
                if (forcedUI != null)
                {
                    cards.Add(forcedUI);
                }

                pool.Remove(forcedPrefab);
            }
            else
            {
                Debug.LogWarning($"Carte forcée '{forcedCardName}' introuvable !");
            }
        }

        // -----------------------------
        // Génération aléatoire du reste
        // -----------------------------
        while (cards.Count < numberOfCardsToSpawn && pool.Count > 0)
        {
            int randomIndex = Random.Range(0, pool.Count);
            GameObject cardObj = Instantiate(pool[randomIndex], augmentPanel);
            cardObj.transform.localScale = Vector3.one;

            AugmentCardUI cardUI = cardObj.GetComponent<AugmentCardUI>();
            if (cardUI != null)
            {
                cards.Add(cardUI);
            }

            pool.RemoveAt(randomIndex);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(augmentPanel);
    }

    void HandleNavigation()
    {
        if (Time.unscaledTime - lastInputTime < inputCooldown) return;

        float move = 0f;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.rightArrowKey.wasPressedThisFrame) move = 1;
            if (Keyboard.current.leftArrowKey.wasPressedThisFrame) move = -1;
        }

        if (Gamepad.current != null)
        {
            float stick = Gamepad.current.leftStick.x.ReadValue();
            if (Mathf.Abs(stick) > 0.5f) move = Mathf.Sign(stick);
        }

        if (move != 0)
        {
            currentIndex = Mathf.Clamp(currentIndex + (int)move, 0, cards.Count - 1);
            HighlightCurrent();
            lastInputTime = Time.unscaledTime;
        }
    }

    void HandleSelection()
    {
        bool validate = (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame) ||
                        (Gamepad.current != null && Gamepad.current.buttonNorth.wasPressedThisFrame);

        if (validate && cards.Count > 0)
        {
            AugmentData data = cards[currentIndex].augmentData;
            if (data != null && data.effect != null)
            {
                data.effect.Apply(player);
            }
            Hide();
        }
    }

    void HighlightCurrent()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].SetSelected(i == currentIndex);
        }
    }
}