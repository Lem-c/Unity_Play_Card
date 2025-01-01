using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardValueText; // Card number (e.g., 5, 6, 7)
    [SerializeField] private Image suitSprite;     // SpriteRenderer for the suit icon
    [SerializeField] private Sprite heartSprite;
    [SerializeField] private Sprite clubSprite;
    [SerializeField] private Sprite diamondSprite;
    [SerializeField] private Sprite spadeSprite;

    public void SetCard(int cardValue, string suit)
    {
        // Assign the card value
        cardValueText.text = cardValue.ToString();

        // Assign a suit
        switch (suit)
        {
            case "Heart":
                suitSprite.sprite = heartSprite;
                break;
            case "Club":
                suitSprite.sprite = clubSprite;
                break;
            case "Diamond":
                suitSprite.sprite = diamondSprite;
                break;
            case "Spade":
                suitSprite.sprite = spadeSprite;
                break;
        }
    }
}
