using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrebyopiaVR
{
    /// <summary>
    /// 질병 자동변환을 위한 클래스
    /// 질병관리자에 대해 의존성을 가지고 있다.
    /// </summary>
    [RequireComponent(typeof(DiseaseManager))]
    [DisallowMultipleComponent]
    public class DiseaseAutoConvert : MonoBehaviour
    {
        [SerializeField, Header("StateShow Object")]
        private GameObject _stateShowObj;
        
        [SerializeField, Header("StateShow stateText")]
        private UnityEngine.UI.Text _stateText;

        private DiseaseManager _diseaseManager;

        private int _cntNum;

        private void Awake()
        {
            _diseaseManager = GetComponent<DiseaseManager>();

            // 질병관리자의 isAutomatic변수를 true로 변경
            _diseaseManager.isAutomatic = true;

            _stateShowObj.SetActive(true);
        }

        private void Update()
        {
            var gameMgr = GameManager.Instance;

            switch (gameMgr.state)
            {
                case GameManager.STATE.QuizGame:
                    if (gameMgr.miniGame.cntState == MiniGame.STATE.Playing)
                        ActionQuizGame(gameMgr);
                    break;

                case GameManager.STATE.SocksGame:
                    if (gameMgr.miniGame.cntState == MiniGame.STATE.Playing)
                        ActionTimeGame(gameMgr);
                    break;

                case GameManager.STATE.Sewing:
                    var sewingGame = gameMgr.miniGame as SewingGame;

                    if (!sewingGame.isTutorial
                        && gameMgr.miniGame.cntState == MiniGame.STATE.Playing)
                        ActionTimeGame(gameMgr);
                    break;

                case GameManager.STATE.Normal:
                    if (_cntNum != 0)
                    {
                        StartCoroutine(FadeInOutRoutine(0));
                    }
                    break;
            }
        }

        /// <summary>
        /// 퀴즈게임에서 작동하는 자동전환
        /// </summary>
        private void ActionQuizGame(GameManager gameMgr)
        {
            var quizGame = gameMgr.miniGame as QuizGame;

            if (quizGame.QNum % 2 == 1)
            {
                StartCoroutine(FadeInOutRoutine(quizGame.QNum / 2 + 1));
            }
        }

        /// <summary>
        /// 타임이 있는 게임에서 작동하는 자동전환
        /// </summary>
        private void ActionTimeGame(GameManager gameMgr)
        {
            var miniGame = gameMgr.miniGame;

            if (miniGame.flowTime > miniGame.playTime / 6 * 5)
            {
                StartCoroutine(FadeInOutRoutine(6));
            }
            else if (miniGame.flowTime > miniGame.playTime / 6 * 4)
            {
                StartCoroutine(FadeInOutRoutine(5));
            }
            else if (miniGame.flowTime > miniGame.playTime / 6 * 3)
            {
                StartCoroutine(FadeInOutRoutine(4));
            }
            else if (miniGame.flowTime > miniGame.playTime / 6 * 2)
            {
                StartCoroutine(FadeInOutRoutine(3));
            }
            else if (miniGame.flowTime > miniGame.playTime / 6 * 1)
            {
                StartCoroutine(FadeInOutRoutine(2));
            }
            else
            {
                StartCoroutine(FadeInOutRoutine(1));
            }
        }

        private IEnumerator FadeInOutRoutine(int num)
        {
            if (_cntNum == num)
            {
                yield break;
            }

            Color clr = _stateText.color;
            bool fadeIn = false;

            _cntNum = num;
            DiseaseManager.Instance.actionSetDisease(num);

            while (true)
            {
                if (!fadeIn)
                {
                    clr.a -= Time.deltaTime * 2;
                    _stateText.color = clr;
                    if (clr.a <= 0)
                    {
                        SetText(num);
                        fadeIn = true;
                    }
                }
                else
                {
                    clr.a += Time.deltaTime * 2;
                    _stateText.color = clr;
                    if (clr.a >= 1)
                        break;
                }
                yield return null;
            }
        }

        private void SetText(int num)
        {
            switch (num)
            {
                case 1:
                    _stateText.text = "노안 (초)";
                    break;
                case 2:
                    _stateText.text = "노안 (중)";
                    break;
                case 3:
                    _stateText.text = "백내장 (초)";
                    break;
                case 4:
                    _stateText.text = "백내장 (중)";
                    break;
                case 5:
                    _stateText.text = "녹내장 (초)";
                    break;
                case 6:
                    _stateText.text = "녹내장 (중)";
                    break;
                default:
                    _stateText.text = "정상";
                    break;
            }
        }
    }
}