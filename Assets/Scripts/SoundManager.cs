using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource Source;
    [SerializeField] private AudioClip rollSound;
    [SerializeField] private AudioClip screamSound;
    [SerializeField] private AudioClip moveSound;

    public void PlayRollSound()
    {
        Source.clip = rollSound;
        Source.Play();
    }

    public void PlayScreamSound()
    {
        Source.clip = screamSound;
        Source.Play();
    }

    public void PlayMoveSound()
    {
        Source.clip = moveSound;
        Source.Play();
    }
}
