using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrebyopiaVR
{
    /// <summary>
    /// 미싱게임 (미니게임을 상속받아 구현)
    /// </summary>
    public class SewingGame : MiniGame
    {
        #region Fields

        [SerializeField, Header("Pivot"), Tooltip("해당 컴포넌트 부모객체를 넣어준다")]
        private GameObject _pivot;

        [SerializeField, Header("Arrow Object")]
        private GameObject _arrowObject;

        [SerializeField, Header("Seal Prefab")]
        private GameObject _sealPrefab;

        [SerializeField, Header("튜토리얼 게임")]
        private GameObject _tutorialGame;

        [SerializeField, Header("본 게임")]
        private GameObject _pantsGame;

        [SerializeField, Header("진행 과정 텍스트")]
        private UnityEngine.UI.Text _progressingText;

        [SerializeField, Header("튜토리얼 설명 UI")]
        private ExplanationUI _tutorialUI;

        [SerializeField, Header("본 게임 설명 UI")]
        private ExplanationUI _pantsUI;
        
        private ViveController[] _controller;

        private bool _isRotating;

        private Vector3 _prePosition;

        private float _degree;
        private Quaternion _quaternion;

        public bool[] area { get; private set; }

        private List<GameObject> _sealList;

        public bool isTutorial { get; private set; }                // 튜토리얼 모드인가?

        public ExplanationUI explanation { get; private set; }

        #endregion

        #region 가상함수 구현

        public override void GameStart()
        {
            cntState = STATE.Ready;

            if (isTutorial)
            {
                _tutorialUI.SetActive(true);
                _pantsUI.SetActive(false);

                explanation = _tutorialUI;

                isTutorial = false;
            }
            else
            {
                _tutorialUI.SetActive(false);
                _pantsUI.SetActive(true);

                explanation = _pantsUI;
            }
        }

        public override void GamePlaying()
        {
            // 텍스트 나타내기
            _progressingText.text = "Time : " + string.Format("{0:0.0}", _playTime - _flowTime) + "\nScore : " + score;

            _flowTime += Time.deltaTime;

            if (_flowTime > _playTime)
            {
                StartCoroutine(GameEnding());
            }
            else
            {
                // 회전관련
                if (_controller[0].triggerState == ViveController.Trigger.Pressing
                 && _controller[1].triggerState == ViveController.Trigger.Pressing)
                {

                    if (!_isRotating)
                    {
                        _isRotating = true;

                        _quaternion = _pivot.transform.rotation;

                        _degree = Mathf.Atan2((_controller[1].transform.position.x - _controller[0].transform.position.x),
                                              (_controller[1].transform.position.z - _controller[0].transform.position.z)) * Mathf.Rad2Deg;
                    }
                    float degree = Mathf.Atan2((_controller[1].transform.position.x - _controller[0].transform.position.x), 
                                               (_controller[1].transform.position.z - _controller[0].transform.position.z)) * Mathf.Rad2Deg - _degree;

                    _pivot.transform.rotation = _quaternion;
                    _pivot.transform.Rotate(Vector3.up, degree);
                }

                // 전진관련
                if ((_controller[0].triggerState == ViveController.Trigger.Pressing 
                    && _controller[1].triggerState != ViveController.Trigger.PressDown 
                    && _controller[1].triggerState != ViveController.Trigger.Pressing) 
                   ||(_controller[1].triggerState == ViveController.Trigger.Pressing 
                    && _controller[1].triggerState != ViveController.Trigger.PressDown 
                    && _controller[1].triggerState != ViveController.Trigger.Pressing))
                {
                    if (_isRotating)
                        _isRotating = false;

                    transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - (Time.deltaTime * 0.2f));
                }


                // 0.3 이동시 seal 생성
                if (Mathf.Abs(Vector3.Distance(_prePosition, transform.localPosition)) > 0.3f)
                {
                    _prePosition = transform.localPosition;

                    GameObject obj = Instantiate(_sealPrefab, _arrowObject.transform.position, Quaternion.identity, this.transform);

                    _sealList.Add(obj);

                    StartCoroutine(ExamineScore(obj.GetComponent<Seal>()));
                }

                // 모든 구간 통과시 게임 종료
                for(int i = 0; i < area.Length; i++)
                {
                    if (area[i] == false)
                        break;

                    if (i == area.Length - 1)
                        StartCoroutine(GameEnding());
                }
            }
        }

        public override void EndGame()
        {
            cntState = STATE.End;

            transform.position = new Vector3(-2.5f, 0, 2.5f);
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }


        #endregion

        /// <summary>
        /// 시작하기
        /// </summary>
        /// <param name="tutorialMode">튜토리얼</param>
        /// <param name="pantsMode">본 게임</param>
        public void Play(bool tutorialMode, bool pantsMode)
        {
            if (tutorialMode == pantsMode)
            {
                Debug.LogWarning("잘못 호출하셨습니다.");
                return;
            }

            _tutorialUI.SetActive(false);
            _pantsUI.SetActive(false);

            // 튜토리얼은 30초 설정
            if (tutorialMode) 
                _playTime = 30.0f;

            // 본 게임은 90초 설정
            if (pantsMode) 
                _playTime = 90.0f;

            _flowTime = 0.0f;
            score = 100;

            // 웨이포인트를 나타내는 불변수 false로 초기화
            for (int i = 0; i < area.Length; i++)
                area[i] = false;

            this.transform.position = new Vector3(-2.5f, -5f, 2.5f);
            _prePosition = transform.position;

            _tutorialGame.SetActive(tutorialMode);
            _pantsGame.SetActive(pantsMode);

            explanation = null;

            cntState = STATE.Playing;
        }

        /// <summary>
        /// 게임나가기
        /// </summary>
        public void EscapeGame()
        {
            var gameMgr = GameManager.Instance;

            gameMgr.ChangeState(GameManager.STATE.Normal);
            UnityEngine.SceneManagement.SceneManager.LoadScene("SubRoomScene");
        }

        IEnumerator ExamineScore(Seal seal)
        {
            yield return new WaitForSeconds(0.2f);

            if (seal.area == Seal.Area.A) { }
            else if (seal.area == Seal.Area.B) { score -= 2; }
            else { score -= 5; }

            if (seal.area == Seal.Area.Start) { area[0] = true; }
            if (seal.area == Seal.Area.Finish) { area[5] = true; }
            if (seal.area == Seal.Area.PointA) { area[1] = true; }
            if (seal.area == Seal.Area.PointB) { area[2] = true; }
            if (seal.area == Seal.Area.PointC) { area[3] = true; }
            if (seal.area == Seal.Area.PointD) { area[4] = true; }
        }

        IEnumerator GameEnding()
        {
            EndGame();

            yield return new WaitForSeconds(5.0f);

            while (_sealList.Count != 0)
            {
                Destroy(_sealList[0]);
                _sealList.RemoveAt(0);
            }

            _tutorialGame.SetActive(false);
            _pantsGame.SetActive(false);

            _progressingText.text = "";

            GameStart();
        }

        private void Awake()
        {
            _controller = _controllerMgr.GetComponentsInChildren<ViveController>();

            _sealList = new List<GameObject>();

            area = new bool[6];
        }

        private void Start()
        {
            isTutorial = true;

            GameManager.Instance.ChangeState(GameManager.STATE.Sewing, this);

            // 게임 진입
            GameStart();
        }
    }
}
