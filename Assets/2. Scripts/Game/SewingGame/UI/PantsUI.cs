using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrebyopiaVR
{
    public class PantsUI : ExplanationUI
    {
        public override void Choice()
        {
            if (idx == 0)
            {
                // 본 게임 시작
                PantsSewingStart();
            }
            else
            {
                // 게임나가기
                EscapeSewingGame();
            }

            this.gameObject.SetActive(false);
        }

        [ContextMenu("본 게임 시작")]
        private void PantsSewingStart()
        {
            var sewingGame = GameManager.Instance.miniGame as SewingGame;

            sewingGame.Play(false, true);
        }

        [ContextMenu("게임 나가기")]
        private void EscapeSewingGame()
        {
            var sewingGame = GameManager.Instance.miniGame as SewingGame;

            sewingGame.EscapeGame();
        }
    }
}