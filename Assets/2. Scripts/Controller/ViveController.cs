using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrebyopiaVR
{
    /// <summary>
    /// 바이브 컨트롤러
    /// </summary>
    /// http://ko.mr_project.wikidok.net/wp-d/59672883772ffd9c53e681f2/View 를 참고해 제작
    [DisallowMultipleComponent]
    public class ViveController : MonoBehaviour
    {
        public enum TouchPad { Up, Down, Right, Left }

        public enum Trigger { None, PressDown, Pressing, PressUp }

        public SteamVR_TrackedObject trackedObj { get; private set; }

        public SteamVR_Controller.Device Controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }

        public Collider contactCollider { get; set; }           // 레이저가 접촉한 컬라이더
        
        public GameObject objectinHand { get; private set; }    // 손에 잡고 있는 오브젝트

        public Trigger triggerState { get; private set; }       // 트리거의 상태를 나타낸다.

        private bool isGrapping;

        private IEnumerator getCoroutine = null;
        private IEnumerator grapCoroutine = null;
        
        public void Put()
        {
            if (getCoroutine != null)
            {
                StopCoroutine(getCoroutine);
                getCoroutine = null;
            }

            if (grapCoroutine != null)
            {
                StopCoroutine(grapCoroutine);
                grapCoroutine = null;
            }

            if (objectinHand)
            {
                objectinHand.GetComponent<Rigidbody>().isKinematic = false;
                objectinHand.transform.parent
                            = GameManager.Instance.miniGame.gameObject.transform;

                objectinHand.layer = LayerMask.NameToLayer("Socks");
            }

            objectinHand = null;
            isGrapping = false;
        }
        
        private void Awake()
        {
            trackedObj = GetComponent<SteamVR_TrackedObject>();
        }

        private void Update()
        {
            ExamineTriggerState();

            #region 질병 메뉴창 불러오기

            // ApplicationMenu를 눌렀을 경우
            if ( !DiseaseManager.Instance.isAutomatic && 
                Controller.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
            {
                CallUpMenu();
            }

            #endregion

            var gameMgr = GameManager.Instance;

            switch (gameMgr.state)
            {
                case GameManager.STATE.Normal:
                    ActionNormalState();
                    break;

                case GameManager.STATE.Menu:
                    ActionMenuState();
                    break;

                case GameManager.STATE.QuizGame:
                    ActionQuizGameState();
                    break;

                case GameManager.STATE.SocksGame:
                    ActionSocksGameState();
                    break;

                case GameManager.STATE.Sewing:
                    ActionSewingGameState();
                    break;
            }
        }

        [ContextMenu("질병창 불러오기")]
        private void CallUpMenu()
        {
            var diseaseMgr = DiseaseManager.Instance;

            if (diseaseMgr.isAutomatic)
                return;

            if (diseaseMgr.isOpened)
            {
                diseaseMgr.CloseMenu();
            }
            else
            {
                diseaseMgr.OpenMenu();
            }
        }

        private void ExamineTriggerState()
        {
            if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                triggerState = Trigger.PressDown;
            }
            else if (Controller.GetPress(SteamVR_Controller.ButtonMask.Trigger))
            {
                triggerState = Trigger.Pressing;
            }
            else if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                triggerState = Trigger.PressUp;
            }
            else
                triggerState = Trigger.None;
        }

        private void ActionNormalState()
        {
            if (contactCollider != null && triggerState == Trigger.PressDown)
            {
                var gameMgr = GameManager.Instance;

                // 방 이동
                if (contactCollider.CompareTag("Warp"))
                {
                    if (gameMgr.room == GameManager.ROOM.LivingRoom)
                    {
                        gameMgr.ChangeROOM(GameManager.ROOM.SubRoom);
                        UnityEngine.SceneManagement.SceneManager.LoadScene("SubRoomScene");
                    }
                    else
                    {
                        gameMgr.ChangeROOM(GameManager.ROOM.LivingRoom);
                        UnityEngine.SceneManagement.SceneManager.LoadScene("LivingRoomScene");
                    }

                    return;
                }

                if (contactCollider.CompareTag("QuizGame"))
                {
                    gameMgr.ChangeState(GameManager.STATE.QuizGame, 
                                        contactCollider.GetComponent<MiniGame>());

                    return;
                }

                if (contactCollider.CompareTag("SocksGame"))
                {
                    gameMgr.ChangeState(GameManager.STATE.SocksGame,
                                        contactCollider.GetComponent<MiniGame>());

                    return;
                }

                if (contactCollider.CompareTag("SewingGame"))
                {
                    gameMgr.ChangeState(GameManager.STATE.Sewing);
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Sewing");
                    return;
                }
            }
        }

        private void ActionMenuState()
        {
            DiseaseManager diseaseMgr = DiseaseManager.Instance;

            if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
            {
                Vector2 touchpad = Controller.GetAxis();

                if (touchpad.y > 0.5f)
                {
                    diseaseMgr.ChooseDisease(TouchPad.Up);
                }
                else if (touchpad.y < -0.5f)
                {
                    diseaseMgr.ChooseDisease(TouchPad.Down);
                }
                else if (touchpad.x > 0.5f)
                {
                    diseaseMgr.ChooseDisease(TouchPad.Right);
                }
                else if (touchpad.x < -0.5f)
                {
                    diseaseMgr.ChooseDisease(TouchPad.Left);
                }
            }

            if (triggerState == Trigger.PressDown)
            {
                diseaseMgr.ChoiceDisease();
            }
        }

        private void ActionQuizGameState()
        {
            var gameMgr = GameManager.Instance;
            var quizGame = gameMgr.miniGame as QuizGame;         // 다운캐스팅

            if (!quizGame.answerProcessing)
            {
                if (contactCollider != null && triggerState == Trigger.PressDown)
                {
                    if (quizGame.cntState == MiniGame.STATE.Playing)
                    {
                        quizGame.CheckAnswer(contactCollider.gameObject);
                    }
                    else if (quizGame.cntState == MiniGame.STATE.End)
                    {
                        quizGame.EscapeGame();
                    }
                }
            }
        }

        private void ActionSocksGameState()
        {
            var gameMgr = GameManager.Instance;
            var socksGame = gameMgr.miniGame as SocksGame;       

            if (!socksGame.checkProcessing)
            {
                if (triggerState == Trigger.PressUp)
                {
                    Put();
                }

                if (triggerState == Trigger.PressDown)
                {
                    if (objectinHand == null && !isGrapping)
                    {
                        getCoroutine = GetObj(contactCollider.gameObject);

                        StartCoroutine(getCoroutine);
                    }
                }
            }
        }

        private void ActionSewingGameState()
        {
            var gameMgr = GameManager.Instance;
            var sewingGame = gameMgr.miniGame as SewingGame;

            if (sewingGame.cntState == MiniGame.STATE.Ready)
            {
                if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
                {
                    Vector2 touchpad = Controller.GetAxis();

                    if (touchpad.y > 0.5f)
                    {
                        sewingGame.explanation.Choose(TouchPad.Up);
                    }
                    else if (touchpad.y < -0.5f)
                    {
                        sewingGame.explanation.Choose(TouchPad.Down);
                    }
                }

                if (triggerState == Trigger.PressDown)
                {
                    sewingGame.explanation.Choice();
                }
            }
        }

        private IEnumerator GetObj(GameObject obj)
        {
            float i = 0;
            isGrapping = true;

            while (true)
            {
                yield return null;
                obj.transform.position = Vector3.Lerp(obj.transform.position, this.transform.position, i);

                i += 0.1f;
                if (i > 0.2f)
                {
                    grapCoroutine = GrabObject();
                    StartCoroutine(grapCoroutine);
                    yield break;
                }
            }
        }

        private IEnumerator GrabObject()
        {
            yield return new WaitForSeconds(0.1f);

            objectinHand = contactCollider.gameObject;
            objectinHand.layer = LayerMask.NameToLayer("None");

            objectinHand.GetComponent<Rigidbody>().isKinematic = true;
            objectinHand.transform.parent = gameObject.transform;
            objectinHand.transform.localPosition = new Vector3(0, 0, 2.7f);
        }
    }
}
