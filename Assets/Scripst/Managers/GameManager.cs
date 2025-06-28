using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform playerHand;
    public Transform botHand;

    public Button endTurnButton;
    public Button restartButton;
    public Button backToMenuButton;
    public TextMeshProUGUI statusText;

    private List<Card> playerCards = new();
    private List<Card> botCards = new();

    private Card selectedPlayerCard = null;
    private Card selectedBotCard = null;

    private string currentTurn = "player";

    private void Start()
    {
        restartButton.onClick.AddListener(ResetGame);
        endTurnButton.onClick.AddListener(EndTurn);
        backToMenuButton.onClick.AddListener(BackToMenu);
        ResetGame();
    }

    private void Update()
    {
        CheckVictory();
    }

    void ResetGame()
    {
        currentTurn = "player";
        selectedPlayerCard = null;
        selectedBotCard = null;
        ClearHand(playerHand);
        ClearHand(botHand);
        GenerateDecks();
        SpawnCards();
        statusText.text = "Выберите своего и вражеского монстра";
        endTurnButton.interactable = false;
    }

    void GenerateDecks()
    {
        int minCards = 3, maxCards = 5;
        int totalCards = Random.Range(6, 11);
        int minPower = 10, maxPower = 50;
        Array damageTypes = DamageType.GetValues(typeof(DamageType));

        List<(int power, int armor, DamageType type, Dictionary<DamageType, float> healthResistanses, Dictionary<DamageType, float> armorResistanses)> stats = new();

        for (int i = 0; i < totalCards; i++)
        {
            int totalValue = Random.Range(minPower + 5, maxPower + 10);

            int power = Random.Range(minPower, totalValue);
            int armor = Mathf.Clamp((totalValue - power) / 2, 0, 20);
            Dictionary<DamageType, float> healthResistanses = new Dictionary<DamageType, float>();
            Dictionary<DamageType, float> armorResistanses = new Dictionary<DamageType, float>();
            DamageType type = (DamageType)damageTypes.GetValue(Random.Range(0, damageTypes.Length));
            foreach(DamageType dType in damageTypes)
            {
                healthResistanses[dType] = Random.Range(0.5f, 2.0f);
                armorResistanses[dType] = Random.Range(0.5f, 2.0f);
            }

            stats.Add((power, armor, type, healthResistanses, armorResistanses));
        }

        stats.Sort((a, b) => b.power.CompareTo(a.power));

        playerCards.Clear();
        botCards.Clear();
        int playerPowerSum = 0, botPowerSum = 0;

        foreach (var (power, armor, type, healthResistanses, armorResistanses) in stats)
        {
            if (playerCards.Count < maxCards && (playerPowerSum <= botPowerSum || botCards.Count >= maxCards) || botCards.Count >= maxCards)
            {
                playerCards.Add(new Card(power, armor, "Monster", type, healthResistanses, armorResistanses));
                playerPowerSum += power;
            }
            else
            {
                botCards.Add(new Card(power, armor, "Monster", type, healthResistanses, armorResistanses));
                botPowerSum += power;
            }
        }

        if (playerCards.Count < minCards || botCards.Count < minCards || Mathf.Abs(playerPowerSum - botPowerSum) > 20)
        {
            GenerateDecks();
        }
    }


    void SpawnCards()
    {
        foreach (Card c in playerCards)
        {
            CreateCardUI(c, playerHand, true);
        }
        foreach (Card c in botCards)
        {
            CreateCardUI(c, botHand, false);
        }
    }

    public void SelectCard(Card card, bool isPlayer)
    {
        if (currentTurn != "player") return;
        if (isPlayer)
        {
            if (selectedPlayerCard != null)
            {
                selectedPlayerCard.ui.Deselect();
            }
            selectedPlayerCard = card;
            selectedPlayerCard.ui.Select();
        }
        else
        {
            if (selectedBotCard != null)
            {
                selectedBotCard.ui.background.color = Color.white;
            }
            selectedBotCard = card;
            selectedBotCard.ui.background.color = Color.green;
        }
        if (selectedPlayerCard != null && selectedBotCard != null)
        {
            endTurnButton.interactable = true;
        }

    }


    void EndTurn()
    {
        selectedPlayerCard.Attack(selectedBotCard);
        selectedBotCard.Attack(selectedPlayerCard);

        if (selectedPlayerCard.power <= 0)
            Destroy(selectedPlayerCard.ui.gameObject);
        else
            selectedPlayerCard.ui.UpdateUI();

        if (selectedBotCard.power <= 0)
            Destroy(selectedBotCard.ui.gameObject);
        else
            selectedBotCard.ui.UpdateUI();

        selectedPlayerCard.ui.Deselect();
        selectedBotCard.ui.Deselect();

        selectedPlayerCard = null;
        selectedBotCard = null;
        endTurnButton.interactable = false;

        currentTurn = "bot";
        Invoke(nameof(BotTurn), 1f);
    }

    void BotTurn()
    {
        CardUI bot = botHand.GetChild(0).GetComponent<CardUI>();
        CardUI player = playerHand.GetChild(0).GetComponent<CardUI>();

        bot.card.Attack(player.card);
        player.card.Attack(bot.card);

        if (bot.card.power <= 0)
        {
            Destroy(bot.gameObject);
        }
        bot.card.ui.UpdateUI();

        if (player.card.power <= 0)
        {
            Destroy(player.gameObject);
        }
        player.card.ui.UpdateUI();

        currentTurn = "player";
    }

    void CreateCardUI(Card data, Transform parent, bool isPlayer)
    {
        GameObject obj = Instantiate(cardPrefab, parent);
        CardUI cardUI = obj.GetComponent<CardUI>();
        cardUI.Setup(data, isPlayer, this);
    }

    

    void CheckVictory()
    {
        if(botHand.childCount == 0 && playerHand.childCount == 0)
        {
            statusText.text = "Ничья";
        }
        else if(botHand.childCount == 0)
        {
            statusText.text = "Победа!";
        }
        else if(playerHand.childCount == 0)
        {
            statusText.text = "Поражение!";
        }
        else
        {
            if (currentTurn == "player")
            {
                statusText.text = "Выберите своего и вражеского монстра";
            }
            else
            {
                statusText.text = "Ход противника";
            }
        }
    }

    void ClearHand(Transform hand)
    {
        foreach(Transform child in hand)
        {
            Destroy(child.gameObject);
        }
    }

    void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
