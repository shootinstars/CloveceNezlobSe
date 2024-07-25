using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceControl : MonoBehaviour
{
    public Texture2D[] Textures;
    public Sprite[] DiceSprites;

    // Start is called before the first frame update
    void Start()
    {
        Textures = Resources.LoadAll<Texture2D>("Images/Dice");
        CreateSprites();
    }

    public void CreateSprites()
    {
        DiceSprites = new Sprite[Textures.Length];
        var i = 0;
        foreach (var texture in Textures)
        {
            Debug.Log(i);
            DiceSprites[i] = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            i++;
        }
    }
    public Sprite ChangeDiceNumber(int roll)
    {
        return DiceSprites[roll - 1];
    }

    public Sprite BackToDefaultDice()
    {
        return DiceSprites[6];
    }
}
