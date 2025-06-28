using UnityEngine;

public class CardDialogue
{
    public Card card { get; set;}
    public CardDialogueUI ui;
    public CardDialogue(Card card)
    {
        this.card = card;
    }
}
