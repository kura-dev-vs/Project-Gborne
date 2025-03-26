using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums : MonoBehaviour
{

}

public enum CharacterSlot
{
    CharacterSlot_01, CharacterSlot_02, CharacterSlot_03, NO_SLOT
}
public enum CharacterGroup
{
    Team01, Team02
}
public enum Skill_BurstSlot
{
    LeftSkill, RightSkill, Burst
}
public enum WeaponModelSlot
{
    RightHand, LeftHandWeaponSlot, LeftHandShieldSlot, backRightSlot, backLeftSlot, RightLeg, LeftLeg,
    //Right Hips
    //Left Hips
    //Back
}
public enum WeaponModelType
{
    Weapon, Shield
}
public enum WeaponClass
{
    StraightSword,
    Spear,
    Gun,
    MediumShield,
    Fist
}
public enum EquipmentType
{
    Null,
    RightWeapon01, RightWeapon02, RightWeapon03,
    LeftWeapon01, LeftWeapon02, LeftWeapon03,
    Hat, Jacket, Tops, Bottoms, Shoes


}
// this is used to calculate damage based on attack type
public enum AttackType
{
    LightAttack01, LightAttack02, LightAttack03, LightAttack04,
    HeavyAttack01, HeavyAttack02,
    ChargedAttack01, ChargedAttack02,
    RunningAttack01,
    RollingAttack01,
    BackstepAttack01,
    Skill,
    Burst
}
public enum DamageIntensity
{
    Ping, Light, Medium, Heavy, Colossal
}
public enum Outfit
{
    Hat, Jacket, Tops, Bottoms, Shoes
}

public enum ItemPickUpType
{
    worldSpawn,
    CharacterDrop
}

