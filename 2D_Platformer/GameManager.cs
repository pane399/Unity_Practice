using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int lifePoint;
    public PlayerMove player;
    public GameObject[] Stages;

    public Image[] UILifePoint;
    public Text UIPoint;
    public Text UIStage;
    public GameObject UIRestartBtn;

    void Update()
    {
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }

    public void NextStage()
    {
        //Change Stage
        if (stageIndex < Stages.Length - 1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();

            UIStage.text = "STAGE " + (stageIndex + 1);
        }
        else //Game Clear
        {
            //Player Control Lock
            Time.timeScale = 0;

            //Result UI
            Debug.Log("게임 클리어!");
            //Restart Button UI
            Text btnText = UIRestartBtn.GetComponentInChildren<Text>();
            btnText.text = "Game Clear!";
            UIRestartBtn.SetActive(true);
        }

        //Calculate Point
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    public void lifePointDown()
    {
        if (lifePoint > 1)
        {
            lifePoint--;
            UILifePoint[lifePoint].color = new Color(1, 0, 0, 0.2f);
        }
        else
        {
            //All Health UI Off
            UILifePoint[0].color = new Color(1, 0, 0, 0.2f);

            //Player Die Effect
            player.OnDie();
            //Result UI
            Debug.Log("사망하였습니다");
            //Retry Button UI
            UIRestartBtn.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            // Player Reposition
            if (lifePoint > 1)
            {
                PlayerReposition();
            }

            //Life Point Down
            lifePointDown();
        }
    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(0, 3, 0);
        player.VelocityZero();
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
