using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public int power { get; set; }
    public int armor { get; set; }
    public DamageType damageType { get; set; }

    public Dictionary<DamageType, float> healthResistanses { get; set; }
    public Dictionary<DamageType, float> armorResistanses { get; set; }
    public string name { get; set; }
    public CardUI ui;

    public Card(int power, int armor, string name, DamageType damageType, Dictionary<DamageType, float> healthResistanses, Dictionary<DamageType, float> armorResistanses)
    {
        this.power = power;
        this.armor = armor;
        this.name = name;
        this.damageType = damageType;
        this.healthResistanses = healthResistanses;
        this.armorResistanses = armorResistanses;

    }

    public void Attack(Card target)
    {
        int incomingDamage = this.power;

        if (target.armor > 0)
        {
            int reducedDamage = Mathf.FloorToInt((incomingDamage * 0.75f) * target.armorResistanses[this.damageType]);
            int damageToArmor = Mathf.Min(target.armor, reducedDamage);
            target.armor -= damageToArmor;

            int leftoverDamage = reducedDamage - damageToArmor; 
            target.power -= Mathf.FloorToInt(leftoverDamage * target.healthResistanses[this.damageType]);
        }
        else
        {
            target.power -= Mathf.FloorToInt(incomingDamage * target.healthResistanses[this.damageType]);
        }

        target.armor = Mathf.Max(0, target.armor);
        target.power = Mathf.Max(0, target.power);
    }

}
