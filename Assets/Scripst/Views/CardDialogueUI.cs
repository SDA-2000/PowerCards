using NUnit.Framework.Constraints;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDialogueUI : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    private static Sprite[] damageTypeImages;
    public TextMeshProUGUI powerText;
    public TextMeshProUGUI armorText;
    public Image damageTypeImage;

    public TextMeshProUGUI HealthSlashResText;
    public TextMeshProUGUI HealthPierceResText;
    public TextMeshProUGUI HealthBluntResText;

    public TextMeshProUGUI ArmorSlashResText;
    public TextMeshProUGUI ArmorPierceResText;
    public TextMeshProUGUI ArmorBluntResText;

    private bool pointerInside = false;
    private float timeSinceExit = 0f;
    private float hideDelay = 2f;

    private void Update()
    {
        if (!pointerInside)
        {
            timeSinceExit += Time.deltaTime;
            if (timeSinceExit >= hideDelay)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Setup(Card card)
    {
        if (damageTypeImages == null || damageTypeImages.Length == 0)
        {
            damageTypeImages = Resources.LoadAll<Sprite>("DamageIcons");
        }
        powerText.text = $"Здоровье: {card.power}";
        armorText.text = $"Броня: {card.armor}";
        switch (card.damageType)
        {
            case(DamageType.SLASH):
                damageTypeImage.sprite = damageTypeImages[2];
                break;

            case (DamageType.PIERCE):
                damageTypeImage.sprite = damageTypeImages[1];
                break;

            case (DamageType.BLUNT):
                damageTypeImage.sprite = damageTypeImages[0];
                break;
        }

        HealthSlashResText.text = $"{card.healthResistanses[DamageType.SLASH]:f1}x";
        HealthPierceResText.text = $"{card.healthResistanses[DamageType.PIERCE]:f1}x";
        HealthBluntResText.text = $"{card.healthResistanses[DamageType.BLUNT]:f1}x";

        ArmorSlashResText.text = $"{card.armorResistanses[DamageType.SLASH]:f1}x";
        ArmorPierceResText.text = $"{card.armorResistanses[DamageType.PIERCE]:f1}x";
        ArmorBluntResText.text = $"{card.armorResistanses[DamageType.BLUNT]:f1}x";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerInside = true;
        timeSinceExit = 0f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerInside = false;
        timeSinceExit = 0f;
    }
}

