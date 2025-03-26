using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RK
{
    /// <summary>
    /// PopUPUIを管理するmanger
    /// </summary>
    public class PlayerUIPopUpManager : MonoBehaviour
    {
        [Header("YOU DIED Pop Up")]
        [SerializeField] GameObject youDiedPopUpGameObject;
        [SerializeField] TextMeshProUGUI youDiedPopUpBackgroundText;
        [SerializeField] TextMeshProUGUI youDiedPopUpText;
        [SerializeField] CanvasGroup youDiedPopUpCanvasGroup;

        [Header("BOSS DEFEATED Pop Up")]
        [SerializeField] GameObject bossDefeatedPopUpGameObject;
        [SerializeField] TextMeshProUGUI bossDefeatedPopUpBackgroundText;
        [SerializeField] TextMeshProUGUI bossDefeatedPopUpText;
        [SerializeField] CanvasGroup bossDefeatedPopUpCanvasGroup;

        [Header("Rest Point Pop Up")]
        [SerializeField] GameObject restPointPopUpGameObject;
        [SerializeField] TextMeshProUGUI restPointPopUpBackgroundText;
        [SerializeField] TextMeshProUGUI restPointPopUpText;
        [SerializeField] CanvasGroup restPointPopUpCanvasGroup;

        [Header("SCORE Pop Up")]
        [SerializeField] GameObject scorePopUpObject;
        [SerializeField] TextMeshProUGUI scoretext;

        [Header("EX Scene Buff List Pop Up")]
        [SerializeField] GameObject buffListPopUpObject;
        [SerializeField] GameObject buffListContent;
        [SerializeField] GameObject buffCardUI;
        [SerializeField] CanvasGroup buffListPopUpCanvasGroup;

        public void CloseAllPopUpWindow()
        {
            PlayerUIManager.instance.popUpWindowIsOpen = false;
        }
        public void SendYouDiedPopUp()
        {
            // ここでpost processingを起動できる？？ 

            youDiedPopUpGameObject.SetActive(true);
            youDiedPopUpBackgroundText.characterSpacing = 0;
            StartCoroutine(StretchPopUpTextOverTime(youDiedPopUpBackgroundText, 8, 19f));
            StartCoroutine(FadeInPopUpOverTime(youDiedPopUpCanvasGroup, 5));
            StartCoroutine(WaitThenFadeOutPopUpOverTime(youDiedPopUpCanvasGroup, 2, 5));
            // フェードアウト

            // extra sceneのみスコアを表示
            if (SceneManager.GetActiveScene().buildIndex == 2)
            {
                SendScorePopUp();
            }
        }
        public void SendScorePopUp()
        {
            scorePopUpObject.SetActive(true);
            scoretext.SetText(PlayerCamera.instance.player.playerNetworkManager.currentScore.Value.ToString());
        }

        public void SendBossDefeatedPopUp(string bossDefeatedMessage)
        {
            // ここでpost processingを起動できる？？ 
            bossDefeatedPopUpText.text = bossDefeatedMessage;
            bossDefeatedPopUpBackgroundText.text = bossDefeatedMessage;
            bossDefeatedPopUpGameObject.SetActive(true);
            bossDefeatedPopUpBackgroundText.characterSpacing = 0;
            StartCoroutine(StretchPopUpTextOverTime(bossDefeatedPopUpBackgroundText, 8, 19f));
            StartCoroutine(FadeInPopUpOverTime(bossDefeatedPopUpCanvasGroup, 5));
            StartCoroutine(WaitThenFadeOutPopUpOverTime(bossDefeatedPopUpCanvasGroup, 2, 5));
            // フェードアウト
        }

        public void SendRestPointPopUp(string restPointMessage)
        {
            // ここでpost processingを起動できる？？ 
            restPointPopUpText.text = restPointMessage;
            restPointPopUpBackgroundText.text = restPointMessage;
            restPointPopUpGameObject.SetActive(true);
            restPointPopUpBackgroundText.characterSpacing = 0;
            StartCoroutine(StretchPopUpTextOverTime(restPointPopUpBackgroundText, 8, 19f));
            StartCoroutine(FadeInPopUpOverTime(restPointPopUpCanvasGroup, 5));
            StartCoroutine(WaitThenFadeOutPopUpOverTime(restPointPopUpCanvasGroup, 2, 5));
            // フェードアウト
        }
        private IEnumerator StretchPopUpTextOverTime(TextMeshProUGUI text, float duration, float stretchAmount)
        {
            if (duration > 0f)
            {
                text.characterSpacing = 0;
                float timer = 0;

                yield return null;
                while (timer < duration)
                {
                    timer = timer + Time.deltaTime;
                    text.characterSpacing = Mathf.Lerp(text.characterSpacing, stretchAmount, duration * (Time.deltaTime / 20));
                    yield return null;
                }
            }
        }
        private IEnumerator FadeInPopUpOverTime(CanvasGroup canvas, float duration)
        {
            if (duration > 0)
            {
                canvas.alpha = 0;
                float timer = 0;
                yield return null;

                while (timer < duration)
                {
                    timer = timer + Time.deltaTime;
                    canvas.alpha = Mathf.Lerp(canvas.alpha, 1, duration * Time.deltaTime);
                    yield return null;
                }
            }
            canvas.alpha = 1;
            yield return null;
        }
        private IEnumerator WaitThenFadeOutPopUpOverTime(CanvasGroup canvas, float duration, float delay)
        {
            if (duration > 0)
            {
                while (delay > 0)
                {
                    delay = delay - Time.deltaTime;
                    yield return null;
                }
                canvas.alpha = 1;
                float timer = 0;
                yield return null;

                while (timer < duration)
                {
                    timer = timer + Time.deltaTime;
                    canvas.alpha = Mathf.Lerp(canvas.alpha, 0, duration * Time.deltaTime);
                    yield return null;
                }
            }
            canvas.alpha = 0;
            yield return null;

        }

        public void SendBuffListPopUp()
        {
            buffListPopUpObject.SetActive(true);
            WorldExtraManager.instance.timeCount = false;
            StartCoroutine(a());
        }
        IEnumerator a()
        {
            for (int i = 0; i < 3; i++)
            {
                yield return null;
                GameObject buffCard = Instantiate(buffCardUI, buffListContent.transform, false);
                buffCardUI.GetComponent<BuffCardUI>().SetbuffCardInfo(i + 1);

                if (i == 0)
                {
                    buffCard.GetComponent<Toggle>().isOn = true;
                }
            }
        }

        public void ConfirmButton()
        {
            Toggle tgl = buffListContent.GetComponent<ToggleGroup>().ActiveToggles().FirstOrDefault();
            tgl.GetComponent<BuffCardUI>().AffectBufferCard();
            StartCoroutine(WorldExtraManager.instance.SpawnedCharacter());
            CloseBuffCardUI();
        }

        public void CloseBuffCardUI()
        {
            DestroyChildAll(buffListContent.transform);
            WorldExtraManager.instance.timeCount = true;
            buffListPopUpObject.SetActive(false);
        }

        public void RestartButton()
        {
            Time.timeScale = 1f;
            scorePopUpObject.SetActive(false);
            PlayerUIManager.instance.playerUICurrentPTManager.Restart();
            CloseBuffCardUI();
        }

        private void DestroyChildAll(Transform parent)
        {
            foreach (Transform child in parent)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
