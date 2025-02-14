using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;

/*Manages and updates the HUD, which contains your health bar, coins, etc*/

public class HUD : MonoBehaviour
{
    [Header ("Reference")]
    public Animator animator;
    [SerializeField] private GameObject ammoBar;
    public Slider coins_slider;
    public TextMeshProUGUI percent;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private Image inventoryItemGraphic;
    [SerializeField] private GameObject startUp;

    private float ammoBarWidth;
    private float ammoBarWidthEased; //Easing variables slowly ease towards a number
    [System.NonSerialized] public Sprite blankUI; //The sprite that is shown in the UI when you don't have any items
    private float coins;
    private float coinsEased;
    private float healthBarWidth;
    private float healthBarWidthEased;
    [System.NonSerialized] public string loadSceneName;
    [System.NonSerialized] public bool resetPlayer;
    [SerializeField] public Text Times;

    void Start()
    {
        //Set all bar widths to 1, and also the smooth variables.
        healthBarWidth = 1;
        healthBarWidthEased = healthBarWidth;
        ammoBarWidth = 1;
        ammoBarWidthEased = ammoBarWidth;
        coins = (float)NewPlayer.Instance.coins;
        coinsEased = coins;
        blankUI = inventoryItemGraphic.GetComponent<Image>().sprite;
    }

    void Update()
    {
        //Update coins text mesh to reflect how many coins the player has! However, we want them to count up.
        coins_slider.value = coinsEased / NewPlayer.Instance.max_coins;
        percent.text = string.Format("{0}%", Mathf.RoundToInt(coins_slider.value * 100));
        coinsEased += ((float)NewPlayer.Instance.coins - coinsEased) * Time.deltaTime * 5f;

        if (coinsEased >= coins)
        {
            animator.SetTrigger("getGem");
            coins = coinsEased + 1;
        }

        //Controls the width of the health bar based on the player's total health
        healthBarWidth = (float)NewPlayer.Instance.health / (float)NewPlayer.Instance.maxHealth;
        if (healthBarWidth < 0) healthBarWidth = 0;
        healthBarWidthEased += (healthBarWidth - healthBarWidthEased) * Time.deltaTime * 10;
        healthBar.transform.localScale = new Vector2(healthBarWidthEased, 1);

        //Controls the width of the ammo bar based on the player's total ammo
        if (ammoBar)
        {
            ammoBarWidth = (float)NewPlayer.Instance.ammo / (float)NewPlayer.Instance.maxAmmo;
            ammoBarWidthEased += (ammoBarWidth - ammoBarWidthEased) * Time.deltaTime * ammoBarWidthEased;
            ammoBar.transform.localScale = new Vector2(ammoBarWidthEased, transform.localScale.y);
        }
        
        Times.text = FormatTime(NewPlayer.Instance.currentTime);
        if(!NewPlayer.Instance.frozen && NewPlayer.Instance.firstLanded && !NewPlayer.Instance.stopTime)
        {
            NewPlayer.Instance.currentTime -= Time.deltaTime;
        }
        
    }

    public void HealthBarHurt()
    {
        animator.SetTrigger("hurt");
    }

    public void SetInventoryImage(Sprite image)
    {
        inventoryItemGraphic.sprite = image;
    }

    public string FormatTime(float seconds)
    {
        if(NewPlayer.Instance.currentTime >= 0)
        {
            int minutes = Mathf.FloorToInt(seconds / 60);
            int secs = Mathf.FloorToInt(seconds % 60);
            int minsecs = Mathf.FloorToInt((seconds - minutes * 60 - secs) * 100); 
            // 返回格式化的时间字符串
            return string.Format("{0:D2}:{1:D2}.{2:D2}", minutes, secs, minsecs);
        }
        return "00:00.00";
    }

    void ResetScene()
    {
        if (GameManager.Instance.inventory.ContainsKey("reachedCheckpoint"))
        {
            //Send player back to the checkpoint if they reached one!
            NewPlayer.Instance.ResetLevel();
        }
        else
        {
            //Reload entire scene
            SceneManager.LoadScene(loadSceneName);
        }
    }

}
