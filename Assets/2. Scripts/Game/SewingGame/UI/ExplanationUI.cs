using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PrebyopiaVR
{
    [DisallowMultipleComponent]
    public class ExplanationUI : MonoBehaviour
    {
        [SerializeField]
        private Image _yesImage;

        [SerializeField]
        private Image _noImage;

        [SerializeField, Header("활성화 색상")]
        private Color _activeColor;

        [SerializeField, Header("비활성된 색상")]
        private Color _inactiveColor;

        protected int idx { get; private set; }

        /// <summary>
        /// 이 컴포넌트의 게임오브젝트 Set Active
        /// </summary>
        public void SetActive(bool enabled)
        {
            this.gameObject.SetActive(enabled);

            if  (enabled)
            {
                idx = 0;
                _yesImage.color = _activeColor;
                _noImage.color = _inactiveColor;
            }
        }

        public void Choose(ViveController.TouchPad dir)
        {
            switch (dir)
            {
                case ViveController.TouchPad.Up:
                    if (idx == 0)
                    {
                        idx = 1;
                        _yesImage.color = _inactiveColor;
                        _noImage.color = _activeColor;
                    }
                    else
                    {
                        idx = 0;
                        _yesImage.color = _activeColor;
                        _noImage.color = _inactiveColor;
                    }
                    break;

                case ViveController.TouchPad.Down:
                    if (idx == 1)
                    {
                        idx = 0;
                        _yesImage.color = _activeColor;
                        _noImage.color = _inactiveColor;
                    }
                    else
                    {
                        idx = 1;
                        _yesImage.color = _inactiveColor;
                        _noImage.color = _activeColor;
                    }
                    break;
            }
        }

        public virtual void Choice() { }
    }
}
