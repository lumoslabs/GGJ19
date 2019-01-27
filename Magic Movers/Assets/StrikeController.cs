using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikeController : MonoBehaviour
{
    public GameObject[] spriteList;
    private int currentStrikes;
    // Start is called before the first frame update
    void Start()
    {
        currentStrikes = 0;
        
    }

    public bool GiveStrike()
    {

        currentStrikes++;
        spriteList[currentStrikes - 1].SetActive(true);
        if (currentStrikes == 3)
        {
            return true;
        }
        return false;

    }

    public void RestartGame()
    {
        Start();
        for (int i = 0; i < spriteList.Length; i++)
        {
            spriteList[i].SetActive(false);
        }
    }
}
