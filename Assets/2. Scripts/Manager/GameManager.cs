using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

namespace PrebyopiaVR
{
    public class GameManager : PersistentSingleton<GameManager> 
    {
        #region Fields
        public enum ROOM { LivingRoom, SubRoom }
        public enum STATE { Normal, Menu, QuizGame, SocksGame, Sewing }
        
        public ROOM room { get { return _room; } private set { _room = value; } }
        public STATE state { get { return _state; } private set { _state = value; } }
        public STATE preState { get { return _preState; } private set { _preState = value; } }

        public LayerMask layerMask { get; private set; }

        public MiniGame miniGame { get { return _miniGame; } private set { _miniGame = value; } }

        [SerializeField, Header("현재 미니게임")]
        private MiniGame _miniGame; // 현재 미니게임을 나타내는 변수

        [SerializeField, Header("방 (에디터확인용)")]
        private ROOM _room;

        [SerializeField, Header("상태 (에디터확인용)")]
        private STATE _state;

        [SerializeField, Header("이전 상태(에디터 확인용)")]
        private STATE _preState;
        #endregion

        /// <summary>
        /// 방 변경
        /// </summary>
        public void ChangeROOM(ROOM newRoom)
        {
            room = newRoom;
        }

        /// <summary>
        /// 게임상태 변경
        /// </summary>
        public void ChangeState(STATE newState)
        {
            preState = state;

            state = newState;
        }

        public void ChangeState(STATE newState, MiniGame game)
        {
            preState = state;

            state = newState;

            miniGame = game;

            switch (state)
            {
                case STATE.QuizGame:
                    miniGame.GameStart();
                    break;

                case STATE.SocksGame:
                    miniGame.GameStart();
                    break;
            }
        }

        public void SettingLayerMask(params string[] layerNames)
        {
            layerMask = LayerMask.GetMask(layerNames);
        }

        private void Start()
        {
            SettingLayerMask("Warp", "Game");
        }

        private void Update()
        {
            if (miniGame != null && miniGame.cntState == MiniGame.STATE.Playing)
            {
                miniGame.GamePlaying();
            }
        }

        [ContextMenu("퀴즈게임 강제 시작")]
        private void QuizGameStart()
        {
            if(room == ROOM.LivingRoom)
                ChangeState(STATE.QuizGame, miniGame);
        }

        [ContextMenu("양말게임 강제 시작")]
        private void SocksGameStart()
        {
            if (room == ROOM.SubRoom)
                ChangeState(STATE.SocksGame, miniGame);
        }
    }
}
