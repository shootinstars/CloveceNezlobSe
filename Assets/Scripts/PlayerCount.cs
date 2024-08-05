using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCount : MonoBehaviour
{
    public int Count = 4;
    [SerializeField] private TMP_Text counterText;
    [SerializeField] private GameObject playerCounterScreen;
    [SerializeField] private GameObject gameChooser;
    public static PlayerCount Instance;

    void Awake()
    {
        counterText.text = Count.ToString();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }


    public void IncreaseCount()
    {
        if (Count < 4)
        {
            Count++;
            counterText.text = Count.ToString();
        }
    }

    public void DecreaseCount()
    {
        if (Count > 2)
        {
            Count--;
            counterText.text = Count.ToString();
        }
    }

    public void ShowCounterText()
    {
        counterText = GameObject.Find("PlayerCounterText").GetComponent<TMP_Text>();
        counterText.text = Count.ToString();
    }

}


