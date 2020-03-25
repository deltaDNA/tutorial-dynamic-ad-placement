using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudManager : MonoBehaviour
{

    public Text txtCoins;
    public Text txtLevel;
    public Text txtFoodRemaining;

    // Start is called before the first frame update
    void Start()
    {
       
    }
    public void UpdateHud(int coins, int level, int foodRemaining)
    {
        txtCoins.text = coins.ToString();
        txtLevel.text = level.ToString();
        txtFoodRemaining.text = foodRemaining.ToString();
    }

}
