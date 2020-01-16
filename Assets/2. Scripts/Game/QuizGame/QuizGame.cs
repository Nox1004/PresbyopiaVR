using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PrebyopiaVR
{
    /// <summary>
    /// 퀴즈게임 (미니게임을 상속받아 구현)
    /// </summary>
    public class QuizGame : MiniGame
    {
        #region Fields
        [SerializeField, Header("신문 이미지 컴포넌트")]
        private Image _newsPaper;

        [SerializeField, Header("신문 이미지 리스트")]
        private Sprite[] _newsPaperSprites;

        [SerializeField, Header("선택지 오브젝트들")]
        private GameObject[] _answerObjs;

        [SerializeField, Header("선택지 색상")]
        private Color _orginColor;

        [SerializeField, Header("질문 텍스트 컴포넌트")]
        private Text _questionText;

        [SerializeField, Header("선택지 텍스트 컴포넌트들")]
        private Text[] _answers;

        [SerializeField, Header("점수 텍스트")]
        private Text _scoreText;

        [SerializeField, Header("문제 번호 텍스트")]
        private Text _QNumText;

        [SerializeField, Header("문제 리스트")]
        private List<Quiz> _quizList;

        [System.Serializable]
        public struct Quiz
        {
            [Header("질문")]
            public string question;

            [Header("선택답안"), Tooltip("4개만 설정해주세요")]
            public string[] answers;

            [Header("정답 설정")]
            public int rightAnswer;
        }

        public bool answerProcessing { get; private set; }  // 답변 처리를 나타내는 불 변수

        public int QNum { get; private set; }          // 문제 번호

        private int _rightAnswer;  // 퀴즈문제에 대한 정답

        private bool[] _checkQuiz;  // 해당 문제가 나왔는지 나타내는 배열

        #endregion

        #region 가상함수 구현

        public override void GameStart()
        {
            cntState = STATE.Playing;

            GameManager.Instance.SettingLayerMask("Answer");

            // 신문지 객체 셋팅
            transform.SetParent(_controllerMgr.transform);
            transform.localPosition = new Vector3(1, 1.5f, -5f);
            transform.localRotation = Quaternion.Euler(15, 180, 0);

            // 선택지 활성화
            SetAnswerObjs(true);

            // 문제 체크를 모두 false로 변경
            for (int i = 0; i < _checkQuiz.Length; i++)
            {
                _checkQuiz[i] = false;
            }

            _newsPaper.gameObject.SetActive(true);
            QNum = 0;
            _rightAnswer = -1;
            _scoreText.text = "Score : " + score;
            _QNumText.text = "No." + QNum;
           
            SettingQuiz();
        }

        public override void EndGame()
        {
            cntState = STATE.End;

            _newsPaper.gameObject.SetActive(false);

            _QNumText.text = "";
            _scoreText.text = "";
            _questionText.text = "당신의 총 점수는 : " + score + "입니다.\n 수고하셨습니다.";

            SetAnswerObjs(false);
        }

        public override void GamePlaying() { }
        
        #endregion

        /// <summary>
        /// 답이 맞는지 확인 (컨트롤러에서 호출해준다.)
        /// </summary>
        public void CheckAnswer(GameObject gameObj)
        {
            if (int.Parse(gameObj.name) == _rightAnswer)
                StartCoroutine(AnswerResult(true, gameObj));
            else
                StartCoroutine(AnswerResult(false, gameObj));
        }

        /// <summary>
        /// 게임에서 벗어나기 (컨트롤러에서 호출해준다.) 
        /// </summary>
        [ContextMenu("Escape")]
        public void EscapeGame()
        {
            if (cntState == STATE.End)
            {
                _questionText.text = "";
                score = 0;

                // 신문지 객체 셋팅
                transform.SetParent(null);
                transform.position = new Vector3(-9.0f, -3.0f, -5.0f);
                transform.rotation = Quaternion.Euler(90.0f, 0, 0);

                var gameMgr = GameManager.Instance;

                cntState = STATE.Ready;
                gameMgr.ChangeState(GameManager.STATE.Normal);
                gameMgr.SettingLayerMask("Warp", "Game");
            }
        }

        private IEnumerator AnswerResult(bool correct, GameObject obj)
        {
            answerProcessing = true;

            if (correct)
            {
                score++;
                _scoreText.text = "Score : " + score;

                obj.GetComponent<Renderer>().material.color = new Color(0, 255, 0);
            }
            else
            {
                obj.GetComponent<Renderer>().material.color = new Color(255, 0, 0);
            }

            yield return new WaitForSeconds(0.5f);

            answerProcessing = false;

            if (QNum >= 12)
            {
                EndGame();
            }
            else
            {
                SettingQuiz();
            }
        }

        private void SettingQuiz()
        {
            ResetColor();

            ChoiceQuiz(UnityEngine.Random.Range(0, _quizList.Count));
        }

        /// <summary>
        /// 재귀를 이용해 퀴즈를 낸다
        /// </summary>
        private void ChoiceQuiz(int idx)
        {
            if (!_checkQuiz[idx])
            {
                _checkQuiz[idx] = true;

                QNum++;
                _QNumText.text = "No." + QNum;

                _questionText.text = _quizList[idx].question;
                
                for (int i = 0; i < 4; i++)
                {
                    _answers[i].text = _quizList[idx].answers[i];
                }

                _rightAnswer = _quizList[idx].rightAnswer;
            }
            else
            {
                ChoiceQuiz(UnityEngine.Random.Range(0, _quizList.Count));
            }
        }

        /// <summary>
        /// 선택지 오브젝트들 색상 리셋
        /// </summary>
        private void ResetColor()
        {
            for (int i = 0; i < _answerObjs.Length; i++)
            {
                _answerObjs[i].GetComponent<Renderer>().material.color = _orginColor;
            }
        }

        /// <summary>
        /// 선택지 오브젝트들 한번에 활성 및 비활성
        /// </summary>
        private void SetAnswerObjs(bool enabled)
        {
            for(int i = 0; i < _answerObjs.Length; i++)
            {
                _answerObjs[i].gameObject.SetActive(enabled);
            }
        }

        private void Awake()
        {
            _checkQuiz = new bool[_quizList.Count];
        }

        [ContextMenu("1번선택")]
        private void TestOne()
        {
            CheckAnswer(_answerObjs[0]);
        }

        [ContextMenu("2번선택")]
        private void TestTwo()
        {
            CheckAnswer(_answerObjs[1]);
        }

        [ContextMenu("3번선택")]
        private void TestThree()
        {
            CheckAnswer(_answerObjs[2]);
        }

        [ContextMenu("4번선택")]
        private void TestFour()
        {
            CheckAnswer(_answerObjs[3]);
        }
    }
}