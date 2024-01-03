using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusHUD : MonoBehaviour
{
    public Text nameText;
    public Text currentHpText;
    public Slider hpSlider;

    public void SetHUD(Fighter fighter) 
    {
        nameText.text = fighter.fighterName;
        currentHpText.text = fighter.currentHP + "/" + fighter.hp;
        hpSlider.maxValue = fighter.hp;
        hpSlider.value = fighter.currentHP;
    }

    public void SetHP(Fighter fighter)
    {
        hpSlider.value = fighter.currentHP;
        currentHpText.text = fighter.currentHP + "/" + fighter.hp;
    }
}
