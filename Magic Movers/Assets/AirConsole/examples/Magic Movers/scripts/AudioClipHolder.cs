using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioClipHolder : MonoBehaviour
{
    public AudioClip[] clips;

    public AudioClip RandomAudio()
    {
        return clips[Random.Range(0, clips.Length - 1)];
    }
}
