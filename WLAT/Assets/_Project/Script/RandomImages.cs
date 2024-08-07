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
                text.text = "������ ��Ȱȭ����!";
                break;
            case 1:
                text.text = "���� ���� ���� �ϴ� ��... �Ⱦ�...";
                break;
            case 2:
                text.text = "�̶�Ŭ ���! �޻쵵 �� �̱� �� ����!";
                break;
            case 3:
                text.text = "ī���ΰ� �Բ���� �η���� ����!...";
                break;
        }
    }
}
