using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace Utils
{
    public class SpriteRendererPoolManager : MonoBehaviour
    {
        public int defaultCapacity = 10;
        public int maxPoolSize = 10000;
        public Image prefab;
        
        bool _isInitialized = false;
        
        public IObjectPool<Image> Pool { get; private set; }

        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            if (_isInitialized)
                return;
            
            _isInitialized = true;
            
            Pool = new ObjectPool<Image>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool,
                OnDestroyPoolObject, true, defaultCapacity, maxPoolSize);
            for (int i = 0; i < defaultCapacity; i++)
            {
                Pool.Release(CreatePooledItem());
            }
        }

        // 생성
        private Image CreatePooledItem()
        {
            Image poolGo = Instantiate(prefab.gameObject).GetComponent<Image>();
            return poolGo;
        }

        // 사용
        private void OnTakeFromPool(Image poolGo)
        {
            poolGo.gameObject.SetActive(true);
        }

        // 반환
        private void OnReturnedToPool(Image poolGo)
        {
            poolGo.transform.SetParent(transform);
            poolGo.gameObject.SetActive(false);
        }

        // 삭제
        private void OnDestroyPoolObject(Image poolGo)
        {
            Destroy(poolGo.gameObject);
        }
    }
}
