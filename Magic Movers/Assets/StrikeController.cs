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

    // Update is called once per frame
    void Update()
    {
        
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
}
