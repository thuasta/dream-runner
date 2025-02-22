using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Leaderboards;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject deathMenu;
    [SerializeField] private GameObject victoryMenu;
    [SerializeField] private GameObject timeoutMenu;
    [SerializeField] private GameObject audioTrigger;

    // Update is called once per frame
    void Update()
    {
        if(NewPlayer.Instance.firstLanded)
        {
            audioTrigger.SetActive(true);
        }

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
            int newScore = CalculateScore(newTime);
            gameObject.GetComponent<Record>().LoadGame();
            if(newScore > gameObject.GetComponent<Record>().saveData.score)
            {
                if(!gameObject.GetComponent<Record>().saveData.hasScore)
                {
                    gameObject.GetComponent<Record>().saveData.hasScore = true;
                    AddScoreAsync(newScore);
                }
                else
                {
                    int scoreAdd = newScore - gameObject.GetComponent<Record>().saveData.score;
                    AddScoreAsync(scoreAdd);
                }
                gameObject.GetComponent<Record>().saveData.score = newScore;
                gameObject.GetComponent<Record>().SaveGame();
            }
            GameObject.Find("VictoryMenu/Victory").GetComponent<Text>().text = "任务完成, 耗时" + newTime.ToString("F2") + "s";
            GameObject.Find("VictoryMenu/ScoreShow").GetComponent<Text>().text = "本局得分：" + newScore + "\n"
            + "历史得分：" + gameObject.GetComponent<Record>().saveData.score; 
        }

        if(NewPlayer.Instance.currentTime <= 0 && NewPlayer.Instance.coins < 20)
        {
            NewPlayer.Instance.runRightSpeed = 0;
            timeoutMenu.SetActive(true);
        }
    }

    public async void AddScoreAsync(int score)
    {
        try
        {
            var playerEntry = await LeaderboardsService.Instance.AddPlayerScoreAsync("dreamrunner2025", score);
        }
        catch (Exception exception)
        {
            Debug.Log(exception.Message);
        }
    }

    public int CalculateScore(float time)
    {
        return 36000 - Mathf.FloorToInt(100 * time);
    }

}
