using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class StrikeController : MonoBehaviour
{
    public GameObject[] spriteList;
    private int currentStrikes;

    private AudioSource audio;
    private AudioClipHolder clipHolder;

    void Start()
    {
        currentStrikes = 0;
        audio = GetComponent<AudioSource>();
        clipHolder = GetComponent<AudioClipHolder>();
    }


    public bool GiveStrike()
    {
        audio.clip = clipHolder.RandomAudio();
        audio.Play();
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
