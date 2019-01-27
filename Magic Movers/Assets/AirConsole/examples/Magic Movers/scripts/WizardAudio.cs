using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardAudio : MonoBehaviour
{
    public GameObject player1, player2;

    [HideInInspector]
    public AudioSource player1Audio, player2Audio;

    [HideInInspector]
    public AudioClipHolder clips1, clips2;

    void Start()
    {
        player1Audio = player1.GetComponent<AudioSource>();
        player2Audio = player2.GetComponent<AudioSource>();
        clips1 = player1.GetComponent<AudioClipHolder>();
        clips2 = player2.GetComponent<AudioClipHolder>();
    }
}
