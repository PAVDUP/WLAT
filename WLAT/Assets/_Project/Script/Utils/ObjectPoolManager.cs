using UnityEngine;
using UnityEngine.Pool;

namespace Utils
{
    public class ObjectPoolManager : MonoBehaviour
    {
        public int defaultCapacity = 10;
        public int maxPoolSize = 10000;
        public GameObject prefab;
        
        bool isInitialized = false;

        public IObjectPool<GameObject> Pool { get; private set; }

        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            if (isInitialized)
                return;
            
            isInitialized = true;
            
            Pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool,
                OnDestroyPoolObject, true, defaultCapacity, maxPoolSize);
            for (int i = 0; i < defaultCapacity; i++)
            {
                Pool.Release(CreatePooledItem());
            }
        }

        // 생성
        private GameObject CreatePooledItem()
        {
            GameObject poolGo = Instantiate(prefab);
            return poolGo;
        }

        // 사용
        private void OnTakeFromPool(GameObject poolGo)
        {
            poolGo.SetActive(true);
        }

        // 반환
        private void OnReturnedToPool(GameObject poolGo)
        {
            poolGo.transform.SetParent(transform);
            poolGo.SetActive(false);
        }

        // 삭제
        private void OnDestroyPoolObject(GameObject poolGo)
        {
            Destroy(poolGo);
        }
    }
}