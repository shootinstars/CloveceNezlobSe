using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource Source;
    public AudioSource menuSource;
    [SerializeField] private AudioClip rollSound;
    [SerializeField] private AudioClip screamSound;
    [SerializeField] private AudioClip moveSound;
    [SerializeField] private AudioClip clipSound;
     
    public void PlayRollSound()
    {
        Source.volume = 1f;
        Source.PlayOneShot(rollSound);
    }

    public void PlayScreamSound()
    {
        Source.volume = 0.5f;
        Source.PlayOneShot(screamSound);
    }

    public void PlayMoveSound()
    {
        Source.volume = 1f;
        Source.PlayOneShot(moveSound);
    }

    public void PlayClickSound()
    {
        menuSource.PlayOneShot(clipSound);
    }
}
