using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// 各skillmanagerの雛型. ここを継承した左右のskillmanagerをmodelに付ける
    /// </summary>
    public class PlayerSkillManager : MonoBehaviour
    {
        PlayerManager player;
        EntryManager entry;
        PlayableCharacterManager playableCharacterManager;
        [HideInInspector] public PlayableCharacter playableCharacter;
        [HideInInspector] public PCSkill skill;
        protected float recastTime;
        [HideInInspector] public float currentRecast;
        [HideInInspector] public bool nowRecasting = false;
        [HideInInspector] public int skillMagazine;
        protected virtual void Start()
        {
            player = GetComponentInParent<PlayerManager>();
            entry = GetComponentInParent<EntryManager>();
            playableCharacterManager = GetComponent<PlayableCharacterManager>();
            playableCharacter = playableCharacterManager.playableCharacter;
        }
        private void FixedUpdate()
        {
            if (skill == null)
                return;

            if (nowRecasting)
            {
                currentRecast -= 1f * Time.deltaTime;
            }
            if (currentRecast <= 0)
            {
                nowRecasting = false;
                currentRecast = recastTime;
            }
        }
    }
}
