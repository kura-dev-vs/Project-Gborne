using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// sfx関連
    /// </summary>
    public class CharacterSoundFXManager : MonoBehaviour
    {
        public AudioSource audioSource;
        [Header("Damage Grunts")]
        [SerializeField] protected AudioClip[] damageGrunts;    // 被ダメージ時のボイス
        [SerializeField] protected AudioClip[] attackGrunts;    // アタック時
        [SerializeField] protected AudioClip[] footSteps;

        protected virtual void Awake()
        {
            audioSource = GetComponentInParent<AudioSource>();
        }
        public void SetAudioSource()
        {
            audioSource = GetComponentInParent<AudioSource>();
        }
        public void PlaySoundFX(AudioClip soundFX, float volume = 1, bool randomizePitch = true, float pitchRandom = 0.1f)
        {
            audioSource.PlayOneShot(soundFX, volume);

            audioSource.pitch = 1;
            if (randomizePitch)
            {
                audioSource.pitch += Random.Range(-pitchRandom, pitchRandom);
            }
        }
        public void PlayRollSoundFX()
        {
            audioSource.PlayOneShot(WorldSoundFXManager.instance.rollSFX, 0.5f);
        }
        public void PlayStepSoundFX()
        {
            audioSource.PlayOneShot(WorldSoundFXManager.instance.stepSFX, 0.5f);
        }
        public void PlaySpawnSoundFX()
        {
            audioSource.PlayOneShot(WorldSoundFXManager.instance.spawnSFX, 0.1f);
        }
        public virtual void PlayDamageGruntSoundFX()
        {
            if (damageGrunts.Length > 0)
                PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(damageGrunts));
        }
        public virtual void PlayAttackGruntSoundFX()
        {
            if (attackGrunts.Length > 0)
                PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(attackGrunts));
        }
        public virtual void PlayFootStepSoundFX()
        {
            if (footSteps.Length > 0)
                PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(footSteps), 0.2f);
        }
        public virtual void PlayStanceBreakSoundFX()
        {
            audioSource.PlayOneShot(WorldSoundFXManager.instance.stanceBreakSFX);
        }
        public virtual void PlayCriticalStrikeSoundFX()
        {
            audioSource.PlayOneShot(WorldSoundFXManager.instance.criticalStrikeSFX);
        }
        public virtual void PlayBlockSoundFX()
        {

        }

    }
}
