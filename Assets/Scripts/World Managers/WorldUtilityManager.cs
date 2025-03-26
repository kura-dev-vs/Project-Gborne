using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RK
{
    /// <summary>
    /// ワールドにおけるキャラクターレイヤーやステージのレイヤー等管理し参照しやすくする
    /// </summary>
    public class WorldUtilityManager : MonoBehaviour
    {
        public static WorldUtilityManager instance;
        [Header("Layers")]
        [SerializeField] LayerMask characterLayers;
        [SerializeField] LayerMask enviroLayers;
        [SerializeField] LayerMask uiLayers;
        [SerializeField] LayerMask burstCameraRemovedLayers;
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
        public LayerMask GetCharacterLayers()
        {
            return characterLayers;
        }
        public LayerMask GetEnviroLayers()
        {
            return enviroLayers;
        }
        public LayerMask GetUILayers()
        {
            return uiLayers;
        }
        public LayerMask GetBurstCameraRemovedLayers()
        {
            return burstCameraRemovedLayers;
        }
        public bool CanIDamageThisTarget(CharacterGroup attackingCharacter, CharacterGroup targetCharacter)
        {
            if (attackingCharacter == CharacterGroup.Team01)
            {
                switch (targetCharacter)
                {
                    case CharacterGroup.Team01:
                        return false;
                    case CharacterGroup.Team02:
                        return true;
                    default:
                        break;
                }
            }
            else if (attackingCharacter == CharacterGroup.Team02)
            {
                switch (targetCharacter)
                {
                    case CharacterGroup.Team01:
                        return true;
                    case CharacterGroup.Team02:
                        return false;
                    default:
                        break;
                }
            }

            return false;
        }
        public float GetAngleOfTarget(Transform characterTransform, Vector3 targetDirection)
        {
            targetDirection.y = 0;
            float viewableAngle = Vector3.Angle(characterTransform.forward, targetDirection);
            Vector3 cross = Vector3.Cross(characterTransform.forward, targetDirection);

            if (cross.y < 0) viewableAngle = -viewableAngle;

            return viewableAngle;
        }
        public DamageIntensity GetDamageIntensityBasedOnPoiseDamage(float poiseDamage)
        {
            // throwing daggers, small items
            DamageIntensity damageIntensity = DamageIntensity.Ping;

            // daggers / light attacks
            if (poiseDamage >= 10)
                damageIntensity = DamageIntensity.Light;

            // standard weapons / medium attacks
            if (poiseDamage >= 30)
                damageIntensity = DamageIntensity.Medium;

            // great weapons / heavy attacks
            if (poiseDamage >= 70)
                damageIntensity = DamageIntensity.Heavy;

            // ultra weapons / colossal attacks
            if (poiseDamage >= 120)
                damageIntensity = DamageIntensity.Colossal;

            return damageIntensity;
        }
        public Vector3 GetRipostingPositionBasedOnPC(PlayableCharacter pc)
        {
            Vector3 position = new Vector3(0.1f, 0, 0.7f);

            switch (pc)
            {

            }
            return position;
        }
        public Vector3 GetBackstabPositionBasedOnPC(PlayableCharacter pc)
        {
            Vector3 position = new Vector3(0.1f, 0, 0.7f);

            switch (pc)
            {

            }
            return position;
        }
    }
}
