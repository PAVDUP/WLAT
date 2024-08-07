using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RandomImages : MonoBehaviour
{
    public Sprite[] sprites;
    public TextMeshProUGUI text;

    Image thisImage;

    void Start()
    {
        thisImage = GetComponent<Image>();

        int ranNum = Random.Range(0, sprites.Length);
        thisImage.sprite = sprites[ranNum];

        switch (ranNum)
        {
            case 0:
                text.text = "저장을 생활화하자!";
                break;
            case 1:
                text.text = "잠잘 때도 일을 하는 건... 싫어...";
                break;
            case 2:
                text.text = "미라클 모닝! 햇살도 날 이길 수 없지!";
                break;
            case 3:
                text.text = "카페인과 함께라면 두려울게 없지!...";
                break;
        }
    }
}
