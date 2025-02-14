using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject deathMenu;
    [SerializeField] private GameObject victoryMenu;
    [SerializeField] private GameObject timeoutMenu;

    // Update is called once per frame
    void Update()
    {
        if(NewPlayer.Instance.health <= 0 && NewPlayer.Instance.currentTime > 0 && NewPlayer.Instance.coins < 20)
        {
            NewPlayer.Instance.frozen = true;
            deathMenu.SetActive(true);
        }

        if(NewPlayer.Instance.coins == NewPlayer.Instance.max_coins && NewPlayer.Instance.currentTime > 0)
        {
            NewPlayer.Instance.runRightSpeed = 0;
            NewPlayer.Instance.stopTime = true;
            victoryMenu.SetActive(true);
            float newTime = NewPlayer.Instance.startTime - NewPlayer.Instance.currentTime;
            gameObject.GetComponent<Record>().LoadGame();
            if(newTime < gameObject.GetComponent<Record>().saveData.minTime)
            {
                gameObject.GetComponent<Record>().saveData.minTime = newTime;
                gameObject.GetComponent<Record>().SaveGame();
            }
            GameObject.Find("VictoryMenu/TimeShow").GetComponent<Text>().text = "本局用时：" + newTime.ToString("F2") + "s" + "\n"
            + "历史用时：" + gameObject.GetComponent<Record>().saveData.minTime.ToString("F2") + "s"; 
        }

        if(NewPlayer.Instance.currentTime <= 0 && NewPlayer.Instance.coins < 20)
        {
            NewPlayer.Instance.runRightSpeed = 0;
            timeoutMenu.SetActive(true);
        }
    }

}
