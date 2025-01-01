using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using System;

public class UIManager : MonoBehaviour
{

    // UI Elements
    [SerializeField] private TextMeshProUGUI playerNameText;

    [SerializeField] private TextMeshProUGUI botNameText;
    [SerializeField] private GameObject botunrevealedCard;

    [SerializeField] private TextMeshProUGUI botChipsText;
    [SerializeField] private TextMeshProUGUI playerChipsText;

    [SerializeField] private TextMeshProUGUI roundStatusText;
    [SerializeField] private Button revealButton;
    [SerializeField] private Button leaveButton;

    [SerializeField] private TextMeshProUGUI botWinCountText;
    [SerializeField] private Image botWinCountBg;

    [SerializeField] private GameObject resultWin;
    [SerializeField] private GameObject resultLose;
    // Images for win/lose result
    [SerializeField] private GameObject winImage;
    [SerializeField] private GameObject loseImage;

    [SerializeField] private Transform cardDisplayParent;     // Parent object to display cards
    [SerializeField] private GameObject cardPrefab;           // Prefab for the card
    [SerializeField] private GameObject cardCompAnimator;

    [SerializeField] private GameObject botCard;

    private Animator animator;
    [SerializeField] private AnimationManager animationManager;

    private void Start()
    {
        resultWin.SetActive(false);
        resultLose.SetActive(false);

        if (animationManager == null)
        {
            Debug.LogError("AnimationManager is not assigned in UIManager.");
        }
    }

    public void PlayCardRevealAnimation()
    {
        // Play the "CardReveal" animation
        animationManager.PlayAnimation("CardReveal");
        Debug.Log("CardReveal animation started.");
    }

    /// <summary>
    /// Subscribes to the animation completion event.
    /// </summary>
    /// <param name="onAnimationComplete">Action to invoke when animation finishes.</param>
    public void SubscribeToAnimationCompleted(Action onAnimationComplete)
    {
        if (animationManager != null)
        {
            animationManager.AnimationCompleted += onAnimationComplete;
        }
    }

    /// <summary>
    /// Unsubscribes from the animation completion event.
    /// </summary>
    /// <param name="onAnimationComplete">Action to remove from the animation event.</param>
    public void UnsubscribeFromAnimationCompleted(Action onAnimationComplete)
    {
        if (animationManager != null)
        {
            animationManager.AnimationCompleted -= onAnimationComplete;
        }
    }

    public void UpdatePlayerName(string name){ playerNameText.text = name; }
    public void UpdateBotName(int name) { botNameText.text = $"Bot_{name}"; }

    public void UpdatePlayerCard(int value, string suit="Heart")
    {
        GameObject cardInstance = Instantiate(cardPrefab, cardDisplayParent);

        // Reset local position, rotation, and scale to match the parent
        cardInstance.transform.localRotation = cardDisplayParent.rotation;
        cardInstance.transform.localScale = cardDisplayParent.localScale;

        CardUI cardUI = cardInstance.GetComponent<CardUI>();
        cardUI.SetCard(value, suit);
    }

    public void UpdateBotCard(int value, string suit = "Heart")
    {
        botunrevealedCard.SetActive(false);

        CardUI cardUI = botCard.GetComponent<CardUI>();
        cardUI.SetCard(value, suit);
    }

    public void UpdateChips(int chips, bool isPlayer)
    {
        if (!isPlayer) { botChipsText.text = $"{chips}"; }
        else
        {
            playerChipsText.text = $"{chips}";
        }
    }

    public void UpdateRoundStatus(string status)
    {
        if(roundStatusText == null) { Debug.Log(status); return; }
        roundStatusText.text = status;
    }

    public void ShowWinImage()
    {
        winImage.SetActive(true);
        loseImage.SetActive(false);
    }

    public void ShowLoseImage()
    {
        winImage.SetActive(false);
        loseImage.SetActive(true);
    }

    public void ShowWinResult()
    {
        resultWin.SetActive(true);
    }

    public void ShowLoseResult()
    {
        resultLose.SetActive(true);
    }

    public void HideResultImages()
    {
        winImage.SetActive(false);
        loseImage.SetActive(false);
    }

    public void UpdateBotWinCount(int botId, int remainingBots)
    {
        botWinCountText.text = $"Wins {botId} / {remainingBots}";
    }

    public void ChangeBotCountStatus(bool isActive)
    {
        botWinCountBg.gameObject.SetActive(isActive);
    }

    public void SetRevealButtonInteractable(bool interactable)
    {
        revealButton.interactable = interactable;
    }

    public void AddRevealButtonListener(UnityEngine.Events.UnityAction action)
    {
        revealButton.onClick.RemoveAllListeners();
        revealButton.onClick.AddListener(action);
    }

    public void AddLeaveButtonListener(UnityEngine.Events.UnityAction action)
    {
        leaveButton.onClick.AddListener(action);
    }
}
