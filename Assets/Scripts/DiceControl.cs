using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceControl : MonoBehaviour
{
    private Texture2D[] textures;
    public Sprite[] DiceSprites { get; private set; }
    private GameManager gameManager;
    private SoundManager soundManager;
    [SerializeField] private ComputerPlayer computerPlayer;

    // Start is called before the first frame update
    void Awake()
    {
        textures = Resources.LoadAll<Texture2D>("Images/Dice");
        gameManager = GetComponent<GameManager>();
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        CreateSprites();
    }

    public void CreateSprites()
    {
        DiceSprites = new Sprite[textures.Length];
        var i = 0;
        foreach (var texture in textures)
        {
            DiceSprites[i] = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            i++;
        }
    }

    public IEnumerator RollAnimation(bool isComputer)
    {
        soundManager.PlayRollSound();
        gameManager.ChangeToDefaultButton();
        float timer = 0.5f;
        float interval = 0.15f;
        float elapsed = 0f;
        while (elapsed < timer)
        {
            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }

        if (isComputer)
        {
            gameManager.ChangeButton(computerPlayer.getComputerRoll());
        }
        else
        {
            gameManager.TurnPiecesOn();
            gameManager.ChangeButton(gameManager.getCurrentRoll());
        }
        gameManager.ShouldRoll = true;
        gameManager.HandleRollButton();
    }
}
