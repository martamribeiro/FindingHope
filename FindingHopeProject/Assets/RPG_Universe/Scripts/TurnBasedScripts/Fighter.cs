using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    [SerializeField] public string fighterName;

    // Character base stats
    [SerializeField]public int hp;
    [SerializeField]public int damage;
    [SerializeField]public double defense; // attack reduction
    [SerializeField]public double missRate;
    [SerializeField]public double critRate;
    [SerializeField]public int speed;

    [HideInInspector] public int currentHP;
    [HideInInspector] public int currentDamage;
    [HideInInspector] public double currentDefense;
    [HideInInspector] public int currentSpeed;

    public void Start()
    {
        currentHP = hp;
        currentDamage = damage;
        currentDefense = defense;
        currentSpeed = speed;
    }

    public bool TakeDamage(int damage)
    {
        int finalDmg = damage - (int)Math.Round(damage * currentDefense);
        if (finalDmg < 0) finalDmg = 0;

        currentHP -= finalDmg;
        if (currentHP < 0)
        {
            currentHP = 0;
            return true; // dead
        }
        return false;
    }

    public void Heal(string potion)
    {
        switch (potion)
        {
            case "Normal":
                currentHP += 20;
                break;
            case "Super":
                currentHP += 50;
                break;
            case "Max":
                currentHP = hp;
                break;
        }

        if (currentHP > hp)
        {
            currentHP = hp;
        }
    }

    public void AttackBooster()
    {
        currentDamage = (int)Math.Round(currentDamage * 1.1);
    }

    public void DefenseBooster()
    {
        currentDefense *= 1.1;
    }
}
