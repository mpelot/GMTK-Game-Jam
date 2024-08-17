using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameMangager : MonoBehaviour
{
    private int _money;
    public int money
    {
        get
        {
            return _money;
        }
        set
        {
            _money = value;
            moneyText.text = "$" + _money;
        }
    }
    public TextMeshProUGUI moneyText;

    // Start is called before the first frame update
    void Start()
    {
        money = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
