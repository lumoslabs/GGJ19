using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FurnitureClipHolder : MonoBehaviour
{
    public AudioClip[] spawnClips;
    public AudioClip[] placeClips1;
    public AudioClip[] placeClips2;
    public AudioClip[] missClips;

    public AudioClip RandomSpawnClip()
    {
        return spawnClips[Random.Range(0, spawnClips.Length - 1)];
    }

    public AudioClip RandomPlaceClip1()
    {
        return placeClips1[Random.Range(0, placeClips1.Length - 1)];
    }

    public AudioClip RandomPlaceClip2()
    {
        return placeClips2[Random.Range(0, placeClips2.Length - 1)];
    }

    public AudioClip RandomMissClip()
    {
        return missClips[Random.Range(0, missClips.Length - 1)];
    }
}
