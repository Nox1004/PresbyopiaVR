using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrebyopiaVR
{
    /// <summary>
    /// 양말 컴포넌트
    /// </summary>
    [DisallowMultipleComponent]
    public class Socks : MonoBehaviour
    {
        public int index { get; private set; }

        /// <summary>
        /// 양말 셋팅
        /// </summary>
        /// <param name="idx">인덱스</param>
        /// <param name="texture">텍스쳐</param>
        public void Setting(int idx, Texture texture)
        {
            index = idx;

            GetComponent<MeshRenderer>().material.mainTexture = texture;
        }
    }
}
