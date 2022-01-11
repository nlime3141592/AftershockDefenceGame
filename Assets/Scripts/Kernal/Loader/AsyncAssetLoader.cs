using System;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace DefenceGameSystem.OS.Kernel
{
    public class AsyncAssetLoader<T_Value>
    {
        public bool isLoaded
        {
            get
            {
                return m_isLoaded;
            }
        }

        public T_Value Result
        {
            get
            {
                try
                {
                    return m_result;
                }
                catch(Exception)
                {
                    throw new NullReferenceException("에셋을 불러오지 않았습니다.");
                }
            }
        }

        private bool m_isStarted;
        private bool m_isLoaded;
        private T_Value m_result;

        public event Action<T_Value> Completed;

        public AsyncAssetLoader()
        {
            m_isStarted = false;
            m_isLoaded = false;
            m_result = default(T_Value);

            Completed += m_OnLoadCompleted;
        }

        public void LoadAsset(string key)
        {
            if(!m_isStarted)
            {
                m_isStarted = true;

                AsyncOperationHandle<T_Value> m_assetLoadingOperation;
                m_assetLoadingOperation = Addressables.LoadAssetAsync<T_Value>(key);

                m_assetLoadingOperation.Completed += (assetLoadingOperation) =>
                {
                    T_Value result = assetLoadingOperation.Result;

                    Completed(result);
                };
            }
        }

        private void m_OnLoadCompleted(T_Value result)
        {
            this.m_result = result;
            this.m_isLoaded = true;
        }
    }
}