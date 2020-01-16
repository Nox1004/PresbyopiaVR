using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrebyopiaVR
{
    [DisallowMultipleComponent]
    public class MiniGame : MonoBehaviour
    {
        [SerializeField, Header("SteamVR 컨트롤러 관리자")]
        protected SteamVR_ControllerManager _controllerMgr;

        protected float _playTime;                                    // 총 플레이 타임

        protected float _flowTime;                                    // 경과 시간

        public float flowTime { get { return _flowTime; } }
        
        public float playTime { get { return _playTime; } }

        public enum STATE { Ready, Playing, End }

        public STATE cntState { get; protected set; }

        public int score { get; protected set; }

        public virtual void GameStart() { }
        public virtual void GamePlaying() { }
        public virtual void EndGame() { }
    }
}
