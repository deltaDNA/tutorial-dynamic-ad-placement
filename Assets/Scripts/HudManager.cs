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
        // Clear Hud
        txtCoins.text = "";
        txtLevel.text = "";
        txtFoodRemaining.text = "";
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
