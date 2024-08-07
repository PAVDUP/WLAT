using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomImages : MonoBehaviour
{
    public Sprite[] sprites;
    Image thisImage;

    void Start()
    {
        thisImage = GetComponent<Image>();

        int ranNum = Random.Range(0, sprites.Length);
        thisImage.sprite = sprites[ranNum];
    }
}
