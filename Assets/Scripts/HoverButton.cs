using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverButton : MonoBehaviour
{
    [SerializeField] private SoundManager soundManager;
    void OnMouseEnter()
    {
        Debug.Log("Hi");
        soundManager.PlayClickSound();
    }
}
