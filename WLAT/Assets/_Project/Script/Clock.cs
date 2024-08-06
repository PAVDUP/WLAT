using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public Transform target;

    public float maxTime = 5f; // �ִ� �¿��ð�
    public float currentTime = 0f; // ���� �¿��ð�
    public bool isDecrease = true; // �¿��ð��� �����ϴ� ��� (���� �÷��� �� �⺻ ����) 
    public bool isTimeUp = true; // �¿��ð� = 0
    public bool isFull = false; // �¿��ð� �ִ�ġ ����

    void Awake()
    {
        target = GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        if (currentTime <= 0)
        {
            currentTime = 0;
            isDecrease = false;
            isTimeUp = true;
        }

        else isDecrease = true;

        if(Input.GetMouseButton(0))
        {
            
            isDecrease = false;

            if(!isFull)
            {
                currentTime += Time.fixedDeltaTime;
                transform.RotateAround(target.position, Vector3.forward, -(360 / maxTime) * Time.fixedDeltaTime);
            }

            if (currentTime >= maxTime)
            {
                isFull = true;
                currentTime = maxTime;
            }
            else
            {
                isFull = false;
            }

        }
        else if(isDecrease)
        {
            transform.RotateAround(target.position, Vector3.forward, (360 / maxTime) * Time.fixedDeltaTime);
            currentTime -= Time.fixedDeltaTime;
        }
    }
}
