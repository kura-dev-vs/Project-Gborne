using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// sfx関連の管理
    /// 共通で再生されやすいサウンドを保持も行う (武器やキャラクター等固有のものはキャラクター独自に所有する)
    /// </summary>
    public class WorldSoundFXManager : MonoBehaviour
    {
        public static WorldSoundFXManager instance;
        [Header("Boss Track")]
        [SerializeField] AudioSource bossIntroPlayer;
        [SerializeField] AudioSource bossLoopPlayer;
        [Header("Damage Sounds")]
        public AudioClip[] physicalDamageSFX;
        [Header("Action Sounds")]
        public AudioClip rollSFX, stepSFX, spawnSFX, pickUpItemSFX, stanceBreakSFX, criticalStrikeSFX;
        [Header("Heal Sound")]
        public AudioClip healSFX;
        [Header("Energy Sound")]
        public AudioClip energySFX;
        [SerializeField] float defaultVolume = 0.05f;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// ランダム性のあるsfx配列からランダムに選んだ一つを返す
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public AudioClip ChooseRandomSFXFromArray(AudioClip[] array)
        {
            int index = Random.Range(0, array.Length);
            return array[index];
        }
        public void PlayBossTrack(AudioClip introTrack, AudioClip loopTrack)
        {
            bossIntroPlayer.volume = defaultVolume;
            bossIntroPlayer.clip = introTrack;
            bossIntroPlayer.loop = false;
            bossIntroPlayer.Play();

            bossLoopPlayer.volume = defaultVolume;
            bossLoopPlayer.clip = loopTrack;
            bossLoopPlayer.loop = true;
            bossLoopPlayer.PlayDelayed(bossIntroPlayer.clip.length);
        }
        public void StopAudio()
        {
            bossIntroPlayer.Stop();
            bossLoopPlayer.Stop();
        }
        public void StopBossMusic()
        {
            StartCoroutine(FadeOutBossMusicThenStop());
        }
        private IEnumerator FadeOutBossMusicThenStop()
        {
            while (bossLoopPlayer.volume > 0)
            {
                bossLoopPlayer.volume -= Time.deltaTime;
                bossIntroPlayer.volume -= Time.deltaTime;
                yield return null;
            }

            bossIntroPlayer.Stop();
            bossLoopPlayer.Stop();
        }
    }
}
