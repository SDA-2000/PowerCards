using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : MonoBehaviour, IPointerClickHandler, IPointerExitHandler, IPointerEnterHandler
{
    private static Sprite[] monstersImages;
    public TextMeshProUGUI powerText;
    public TextMeshProUGUI armorText;
    public Image background;
    public GameObject dialoguePrefab;
    public GameObject currentDialogue;

    public Card card;
    private GameManager gameManager;
    private bool isPlayer;

    public void Setup(Card card, bool player, GameManager gameManager)
    {
        this.card = card;
        card.ui = this;
        this.gameManager = gameManager;
        isPlayer = player;
        powerText.text = card.power.ToString();
        armorText.text = card.armor.ToString();
        if (monstersImages == null || monstersImages.Length == 0)
        {
            monstersImages = Resources.LoadAll<Sprite>("Monsters");
        }
        if(monstersImages.Length > 0)
        {
            Sprite RandomMonster = monstersImages[Random.Range(0, monstersImages.Length)];
            background.sprite = RandomMonster;
        }
    }

    public void UpdateUI()
    {
        powerText.text = card.power.ToString();
        armorText.text = card.armor.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        gameManager.SelectCard(card, isPlayer);
    }

    private void ShowInfo()
    {
        Canvas parentCanvas = GetComponentInParent<Canvas>();
        if (dialoguePrefab == null || currentDialogue != null) return;
        currentDialogue = Instantiate(dialoguePrefab, parentCanvas.transform);
        currentDialogue.transform.SetAsLastSibling();
        var dialogueUI = currentDialogue.GetComponent<CardDialogueUI>();
        if (dialogueUI != null)
        {
            dialogueUI.Setup(card);
        }
    }

    private void HideInfo()
    {
        if (currentDialogue != null)
        {
            Destroy(currentDialogue);
            currentDialogue = null;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CancelInvoke(nameof(HideInfo));
        Invoke(nameof(ShowInfo), 1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CancelInvoke(nameof(ShowInfo));
        Invoke(nameof(HideInfo), 2f);
    }

    public void Select()
    {
        background.color = Color.green;
    }

    public void Deselect()
    {
        background.color = Color.white;
    }
}
