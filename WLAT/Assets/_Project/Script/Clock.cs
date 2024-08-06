using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public Transform target;

    public float maxTime = 5f; // 최대 태엽시간
    public float currentTime = 0f; // 현재 태엽시간
    public bool isDecrease = true; // 태엽시간이 감소하는 경우 (게임 플레이 시 기본 상태) 
    public bool isTimeUp = true; // 태엽시간 = 0
    public bool isFull = false; // 태엽시간 최대치 도달

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
