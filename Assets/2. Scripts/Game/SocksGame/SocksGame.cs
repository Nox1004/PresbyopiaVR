using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PrebyopiaVR
{
    /// <summary>
    /// 양말짝맞추기 (미니게임을 상속받아 구현)
    /// </summary>
    public class SocksGame : MiniGame
    {
        #region Fields
        [SerializeField, Header("양말 일치 프리팹")]
        private GameObject _matchedPrefab;

        [SerializeField, Header("경과 텍스처")]
        private Text _text;

        [SerializeField, Header("양말 텍스쳐")]
        private Texture[] _texture;

        private Dictionary<int, int> _randomSocks;          // 맵 컨테이너를 이용해 양말 셋팅

        private int[] _counting;                        

        private Socks[] _socksObjects;                      // 양말 오브젝트들

        private List<GameObject> _matchedObjects;           // 완성된 양말 짝

        private ViveController[] _controller;           

        public bool checkProcessing { get; private set; }   // 양쪽양말 짝이 맞는지 검사하는지를 나타내는 불 변수

        #endregion 

        #region 가상함수 구현

        public override void GameStart()
        {
            cntState = STATE.Playing;

            GameManager.Instance.SettingLayerMask("Socks");

            score = 0;
            _playTime = 90.0f;

            for (int i = 0; i < _socksObjects.Length; i++)
            {
                _socksObjects[i].GetComponent<Rigidbody>().isKinematic = false;
            }

            StartCoroutine(CheckMatch());
        }

        public override void GamePlaying()
        {
            // 텍스트 나타내기
            _text.text = "Time : " + string.Format("{0:0.0}", _playTime - _flowTime) + "\nScore : " + score;

            _flowTime += Time.deltaTime;
            
            if (_flowTime > _playTime || score == 10)
            {
                StartCoroutine(GameEnding());
            }
        }

        public override void EndGame()
        {
            cntState = STATE.End;

            for (int i = 0; i < _controller.Length; i++)
            {
                _controller[i].Put();
            }
            
            for (int i = 0; i < _socksObjects.Length; i++)
            {
                _socksObjects[i].gameObject.SetActive(true);
                _socksObjects[i].GetComponent<Rigidbody>().isKinematic = true;
                _socksObjects[i].transform.localPosition = new Vector3(-0.1038265f, -0.178f, -0.1400001f);
                _socksObjects[i].transform.localRotation = Quaternion.Euler(-90f, 0, 0);
            }

            while (_matchedObjects.Count != 0)
            {
                Destroy(_matchedObjects[0]);
                _matchedObjects.RemoveAt(0);
            }

        }

        #endregion

        /// <summary>
        /// 양말 셋팅 코루틴
        /// </summary>
        IEnumerator Setting()
        {
            _randomSocks.Clear();

            // Dictionary 재설정
            for(int i = 0; i < 10; i++)
            {
                while(true)
                {
                    yield return null;

                    int random = Random.Range(0, _texture.Length);

                    if (!_randomSocks.ContainsValue(random))
                    {
                        _randomSocks.Add(i, random);
                        break;
                    }
                }
            }

            // 카운팅 배열의 값들을 0으로 설정
            for (int i = 0; i < 10; i++)
                _counting[i] = 0;

            // 딕셔너리를 이용해 양말 셋팅
            for(int i = 0; i < _socksObjects.Length; i++)
            {
                while(true)
                {
                    yield return null;

                    int random = Random.Range(0, _counting.Length);

                    if (_counting[random] != 2)
                    {
                        var value = _randomSocks[random];

                        _counting[random]++;
                        _socksObjects[i].Setting(value, _texture[value]);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 양말 짝 맞추는 코루틴
        /// </summary>
        IEnumerator CheckMatch()
        {
            while (cntState == STATE.Playing)
            {
                yield return new WaitForSeconds(0.1f);

                if (_controller[0].objectinHand != null && _controller[1].objectinHand != null)
                {
                    checkProcessing = true;

                    var socksleft = _controller[0].objectinHand.GetComponent<Socks>();
                    var socksRight = _controller[1].objectinHand.GetComponent<Socks>();

                    if (socksleft.index == socksRight.index)
                    {
                        yield return new WaitForSeconds(0.3f);

                        GameObject go = Instantiate(_matchedPrefab, new Vector3(4 - score, 0.05f, 9.5f), Quaternion.Euler(90, 0, 0));
                        go.GetComponent<Socks>().Setting(socksleft.index, _texture[socksleft.index]);

                        _matchedObjects.Add(go);
                        score++;
                        socksleft.gameObject.SetActive(false);
                        socksRight.gameObject.SetActive(false);
                    }

                    checkProcessing = false;
                }
            }
        }

        /// <summary>
        /// 게임이 끝났을 때 호출되는 코루틴
        /// </summary>
        IEnumerator GameEnding()
        {
            EndGame();

            yield return new WaitForSeconds(2.0f);

            _text.text = "";

            var gameMgr = GameManager.Instance;

            _controllerMgr.transform.position = new Vector3(6.88f, 3.5f, 6.75f);
            _controllerMgr.transform.rotation = Quaternion.Euler(0, 90, 0);
            gameMgr.ChangeState(GameManager.STATE.Normal);
            gameMgr.SettingLayerMask("Warp", "Game");

            cntState = STATE.Ready;

            StartCoroutine(Setting());
        }

        private void Awake()
        {
            _randomSocks = new Dictionary<int, int>();
            _counting = new int[10];
            _matchedObjects = new List<GameObject>();

            _socksObjects = GetComponentsInChildren<Socks>();
            _controller = _controllerMgr.GetComponentsInChildren<ViveController>();

            StartCoroutine(Setting());
        }
    }
}
