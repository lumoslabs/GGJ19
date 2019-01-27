using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardAnimatorController : MonoBehaviour
{

    private Animator am;
    // Start is called before the first frame update
    void Start()
    {
        am = GetComponent<Animator>();
    }

    public void PlayWalk()
    {
        am.SetTrigger("player_walk");
    }

    public void PlayIdle()
    {
        am.SetTrigger("player_idle");
    }

    public void PlayFloat()
    {
        am.SetTrigger("player_game");
    }

    public void PlayPull()
    {
        am.SetTrigger("player_buttonpress");
    }

    public void PlayLose()
    { 
        am.SetTrigger("player_lose");
    }

    public void PlayWin()
    {
        am.SetTrigger("player_win");
    }







}
