using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrebyopiaVR
{
    public class TutorialUI : ExplanationUI
    {
        public override void Choice()
        {

            if (idx == 0)
            {
                // 튜토리얼 게임 시작
                TutorialStart();
            }
            else
            {
                // 본 게임 시작
                PantsSewingStart();
            }

            this.gameObject.SetActive(false);
        }


        [ContextMenu("튜토리얼 게임 시작")]
        private void TutorialStart()
        {
            var sewingGame = GameManager.Instance.miniGame as SewingGame;

            sewingGame.Play(true, false);
        }

        [ContextMenu("본 게임 시작")]
        private void PantsSewingStart()
        {
            var sewingGame = GameManager.Instance.miniGame as SewingGame;

            sewingGame.Play(false, true);
        }
    }
}