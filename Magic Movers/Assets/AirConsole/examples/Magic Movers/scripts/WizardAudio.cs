using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardAudio : MonoBehaviour
{
    public GameObject player1, player2;

    [HideInInspector]
    public AudioSource player1Audio, player1magic, player2Audio, player2magic;

    [HideInInspector]
    public AudioClipHolder clips1, clips2;

    void Start()
    {
        player1Audio = player1.GetComponents<AudioSource>()[0];
        player2Audio = player2.GetComponents<AudioSource>()[0];
        player1magic = player1.GetComponents<AudioSource>()[1];
        player2magic = player2.GetComponents<AudioSource>()[1];
        clips1 = player1.GetComponent<AudioClipHolder>();
        clips2 = player2.GetComponent<AudioClipHolder>();
    }
}
