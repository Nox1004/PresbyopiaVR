using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using Common;

namespace PrebyopiaVR
{
    [DisallowMultipleComponent]
    public class DiseaseManager : Singleton<DiseaseManager>
    {
        #region Fields
        [SerializeField, Header("DiseaseMenu")]
        private GameObject _menuObj;

        [SerializeField, Header("백내장")]
        private BlurOptimized _cataract;

        [SerializeField, Header("노안")]
        private DepthOfField _presbyopia;

        [SerializeField, Header("녹내장")]
        private GameObject _glaucoma;

        [SerializeField, Header("활성된 색상")]
        private Color _activeColor;

        [SerializeField, Header("비활성된 색상")]
        private Color _inactiveColor;

        private Image[] _diseaseMenu;

        public bool isOpened { get; private set; }

        public bool isAutomatic { get; set; }           // 안질환 자동변환모드를 나타내는 불변수

        public static int curDisease { get; private set; }      // 현재 질병 상태
    
        public static int indexDisease { get; private set; }    // 현재 메뉴창에서의 질병 인덱스

        public System.Action<int> actionSetDisease;

        #endregion

        /// <summary>
        /// 질병 메뉴창 열기
        /// </summary>
        [ContextMenu("질병창 열기")]
        public void OpenMenu()
        {
            isOpened = true;

            _menuObj.SetActive(true);
            SetColor(indexDisease);

            GameManager.Instance.ChangeState(GameManager.STATE.Menu);
        }

        /// <summary>
        /// 질병 메뉴창 닫기
        /// </summary>
        [ContextMenu("질병창 닫기")]
        public void CloseMenu()
        {
            _menuObj.SetActive(false);

            isOpened = false;

            GameManager.Instance.ChangeState(GameManager.Instance.preState);
        }

        /// <summary>
        /// 질병 선택(확정)
        /// </summary>
        public void ChoiceDisease()
        {
            SetDisease(indexDisease);

            CloseMenu();
        }

        /// <summary>
        /// 질병 선택하기(고르는 단계)
        /// </summary>
        /// <param name="dir">방향</param>
        public void ChooseDisease(ViveController.TouchPad dir)
        {
            switch (dir)
            {
                case ViveController.TouchPad.Up:
                    if (indexDisease == 0) {
                        indexDisease = 2;
                    }
                    else if (indexDisease == 1) {
                        indexDisease = 0;
                    }
                    else if (indexDisease % 2 == 0) {
                        indexDisease--;
                    }
                    else {
                        indexDisease++;
                    }
                    break;

                case ViveController.TouchPad.Down:
                    if (indexDisease == 0) {
                        indexDisease = 1;
                    }
                    else if (indexDisease == 2) {
                        indexDisease = 0;
                    }
                    else if (indexDisease % 2 == 1) {
                        indexDisease++;
                    }
                    else {
                        indexDisease--;
                    }
                    break;

                case ViveController.TouchPad.Right:
                    if (indexDisease == 0) {
                        indexDisease = 1;
                    }
                    else if (indexDisease == 5) {
                        indexDisease = 2;
                    }
                    else if (indexDisease == 6) {
                        indexDisease = 0;
                    }
                    else {
                        indexDisease += 2;
                    }
                    break;


                case ViveController.TouchPad.Left:
                    if (indexDisease == 0) {
                        indexDisease = 6;
                    }
                    else if (indexDisease == 1) {
                        indexDisease = 0;
                    }
                    else if (indexDisease == 2) {
                        indexDisease = 5;
                    }
                    else {
                        indexDisease -= 2;
                    }
                    break;
            }

            SetColor(indexDisease);
        }

        /// <summary>
        /// 질병 메뉴 색상 변경
        /// </summary>
        private void SetColor(int index)
        {
            for(int i = 0; i < _diseaseMenu.Length; i++)
            {
                _diseaseMenu[i].color = _inactiveColor;
            }

            _diseaseMenu[index].color = _activeColor;
        }

        private void SetDisease(int idx)
        {
            curDisease = idx;

            _cataract.enabled = false;
            _presbyopia.enabled = false;
            _glaucoma.SetActive(false);

            switch (curDisease)
            {
                case 1: // 노안 초기
                    _presbyopia.enabled = true;
                    _presbyopia.focalLength = 15;
                    break;

                case 2: // 노안 중기
                    _presbyopia.enabled = true;
                    _presbyopia.focalLength = 35;
                    break;

                case 3: // 백내장 초기
                    _cataract.enabled = true;
                    _cataract.downsample = 1;
                    _cataract.blurSize = 0.25f;
                    _cataract.blurIterations = 1;
                    break;

                case 4: // 백내장 중기
                    _cataract.enabled = true;
                    _cataract.downsample = 1;
                    _cataract.blurSize = 1.5f;
                    _cataract.blurIterations = 1;
                    break;

                case 5: // 녹내장 초기
                    _glaucoma.SetActive(true);
                    _glaucoma.transform.localPosition = new Vector3(-0.75f, 0.75f, 1.21f);
                    _glaucoma.transform.localScale = new Vector3(0.008f, 0.008f, 0);
                    break;

                case 6: // 녹내장 중기
                    _glaucoma.SetActive(true);
                    _glaucoma.transform.localPosition = new Vector3(-0.5f, 0.5f, 1.21f);
                    _glaucoma.transform.localScale = new Vector3(0.006f, 0.006f, 0);
                    break;
            }
        }

        protected override void Awake()
        {
            Instance = this;

#if UNITY_EDITOR
            if(!_menuObj.CompareTag("Menu"))
            {
                Debug.LogError("질병관리자의 질병메뉴오브젝트를 제대로 할당해주세요");
            }
            else
            {
                _diseaseMenu = _menuObj.GetComponentsInChildren<Image>();
            }

            if (_cataract == null)
                Debug.LogError("질병관리자의 백내장 컴포넌트를 할당해주세요");

            if (_presbyopia == null)
                Debug.LogError("질병관리자의 노안 컴포넌트를 할당해주세요");

            if (!_glaucoma.CompareTag("Glaucoma"))
            {
                Debug.LogError("질병관리자의 녹내장 컴포넌트를 확인해주세요");
            }
#endif
            actionSetDisease = SetDisease;
        }

        private void Start()
        {
            SetDisease(0);
            CloseMenu();
        }
    }
}