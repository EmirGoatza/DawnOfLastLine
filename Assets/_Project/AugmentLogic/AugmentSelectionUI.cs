using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class AugmentSelectionUI : MonoBehaviour
{
    [SerializeField] private List<AugmentCardUI> cards;
    [SerializeField] private float inputCooldown = 0.2f;

    private int currentIndex = 0;
    private float lastInputTime;
    private bool isActive = false;

    [SerializeField] private GameObject player;

    void Update()
    {
        if (!isActive) return;

        HandleNavigation();
        HandleSelection();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        isActive = true;
        currentIndex = 0;
        HighlightCurrent();
        Time.timeScale = 0f; // Pause le jeu
    }

    public void Hide()
    {
        isActive = false;
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    void HandleNavigation()
    {
        if (Time.unscaledTime - lastInputTime < inputCooldown)
            return;

        float horizontal = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
                horizontal = 1;
            if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
                horizontal = -1;
        }

        if (Gamepad.current != null)
        {
            float stick = Gamepad.current.leftStick.x.ReadValue();
            if (stick > 0.5f) horizontal = 1;
            if (stick < -0.5f) horizontal = -1;
        }

        if (horizontal != 0)
        {
            currentIndex += (int)horizontal;
            currentIndex = Mathf.Clamp(currentIndex, 0, cards.Count - 1);

            HighlightCurrent();
            lastInputTime = Time.unscaledTime;
        }
    }

    void HandleSelection()
    {
        bool validate =
            (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame) ||
            (Gamepad.current != null && Gamepad.current.buttonNorth.wasPressedThisFrame);

        if (validate)
        {
            Debug.Log("Augment sélectionné : " + currentIndex);

            AugmentData selectedAugment = cards[currentIndex].augmentData;

            if (selectedAugment != null && selectedAugment.effect != null)
            {
                selectedAugment.effect.Apply(player);
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
