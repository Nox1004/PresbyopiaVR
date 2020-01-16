using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    /// <summary>
    /// 지속성있는 싱글톤 객체
    /// </summary>
    public class PersistentSingleton<T> : Singleton<T>  where T : MonoBehaviour
    {
        protected override void Awake()
        {
            base.Awake();

            if (Instance == this)
            {
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }
}
