using UnityEngine;

namespace Common
{
    /// <summary>
    /// 싱글톤 객체
    /// </summary>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; protected set; }

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
            }
        }
    }
}