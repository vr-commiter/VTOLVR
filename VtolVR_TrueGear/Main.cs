global using static VtolVR_TrueGear.Logger;
using HarmonyLib;
using ModLoader.Framework;
using ModLoader.Framework.Attributes;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using static HPEquippable;
using System.Threading;
using UnityEngine;
using MyTrueGear;
using System;

namespace VtolVR_TrueGear
{
    [ItemId("vtolvr.truegear.mod")] // Harmony ID for your mod, make sure this is unique
    public class Main : VtolMod
    {
        public string ModFolder;
        private static TrueGearMod _TrueGear = null;

        private void Awake()
        {            
            Debug.Log("---------------------------------------");
            Debug.Log("Load VoltVR_TrueGear");
            Harmony.CreateAndPatchAll(typeof(Main));
            _TrueGear = new TrueGearMod();
        }

        public override void UnLoad()
        {
            // Destroy any objects
        }

        public static bool isSurfaceShock = false;
        public static bool isEngineShock = false;
        public static bool isHeartBeat = false;
        public static bool isFire = false;
        public static bool isStopListen = false;
        public static Transform playerTransform = null;
        public static WeaponTypes weaponType = WeaponTypes.Unknown;
        private static bool canStarted = true;
        //private static bool canLeftHaptic = true;
        //private static bool canRightHaptic = true;


        public static KeyValuePair<float, float> GetAngle(Transform transform, Vector3 hitPoint)
        {
            Vector3 hitPos = hitPoint - transform.position;
            float hitAngle = Mathf.Atan2(hitPos.x, hitPos.z) * Mathf.Rad2Deg;
            if (hitAngle < 0f)
            {
                hitAngle += 360f;
            }
            float verticalDifference = hitPoint.y - transform.position.y;
            return new KeyValuePair<float, float>(hitAngle, verticalDifference);
        }

        [HarmonyPatch(typeof(VehicleMaster), "Update", new Type[] { })]
        public class VehicleMaster_Update_Postfix
        {
            [HarmonyPostfix]
            public static void Postfix(VehicleMaster __instance)
            {

                playerTransform = __instance.actor.transform;
                if (__instance != null && __instance.actor != null && __instance.actor.weaponManager != null && __instance.actor.weaponManager.currentEquip != null)
                {
                    weaponType = __instance.actor.weaponManager.currentEquip.weaponType;
                }


                if (!isStopListen)
                {
                    if (__instance.flightInfo.isLanded && __instance.flightInfo.surfaceSpeed > 5)
                    {
                        if (!isSurfaceShock)
                        {
                            isSurfaceShock = true;
                            isEngineShock = false;
                            Debug.Log("---------------------------------------");
                            Debug.Log("StopEngineShock");
                            Debug.Log("StartSurfaceShock");
                            _TrueGear.StopEngineShock();
                            _TrueGear.StartSurfaceShock();
                        }
                    }
                    else if (isSurfaceShock)
                    {
                        isSurfaceShock = false;
                        Debug.Log("---------------------------------------");
                        Debug.Log("StopSurfaceShock");
                        _TrueGear.StopSurfaceShock();
                    }
                    if (__instance.engines[0].startedUp && !isSurfaceShock)
                    {
                        if (!isEngineShock)
                        {
                            isEngineShock = true;
                            Debug.Log("---------------------------------------");
                            Debug.Log("StartEngineShock");
                            _TrueGear.StartEngineShock();
                        }
                    }
                    else if (__instance.engines.Length > 1 && !isSurfaceShock)
                    {
                        if (__instance.engines[1].startedUp)
                        {
                            if (!isEngineShock)
                            {
                                isEngineShock = true;
                                Debug.Log("---------------------------------------");
                                Debug.Log("StartEngineShock");
                                _TrueGear.StartEngineShock();
                            }
                        }
                        else if (isEngineShock)
                        {
                            isEngineShock = false;
                            Debug.Log("---------------------------------------");
                            Debug.Log("StopEngineShock");
                            _TrueGear.StopEngineShock();
                        }
                    }
                    if (__instance.pilotIsDead)
                    {
                        isStopListen = true;
                        Debug.Log("---------------------------------------");
                        Debug.Log("PlayerDeath");
                        _TrueGear.Play("PlayerDeath");
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Bullet), "Fire", new Type[] { typeof(Vector3), typeof(Vector3), typeof(float), typeof(float), typeof(float), typeof(float), typeof(float), typeof(float), typeof(Vector3), typeof(Color), typeof(float), typeof(Actor), typeof(float), typeof(Bullet.BulletFiredDelegate), typeof(bool) })]
        public class Bullet_Fire_Postfix
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                if (!isFire)
                {
                    isFire = true;
                    switch (weaponType)
                    {
                        case HPEquippable.WeaponTypes.Gun:
                            Debug.Log("---------------------------------------");
                            Debug.Log("GunFire");
                            _TrueGear.Play("RifleShoot");
                            break;
                        case HPEquippable.WeaponTypes.AntiShip:
                            Debug.Log("---------------------------------------");
                            Debug.Log("AntiShipFire");
                            _TrueGear.Play("RifleShoot");
                            break;
                        case HPEquippable.WeaponTypes.AAM:
                            Debug.Log("---------------------------------------");
                            Debug.Log("AAMFire");
                            _TrueGear.Play("RifleShoot");
                            break;
                        case HPEquippable.WeaponTypes.AGM:
                            Debug.Log("---------------------------------------");
                            Debug.Log("AGMFire");
                            _TrueGear.Play("RifleShoot");
                            break;
                        case HPEquippable.WeaponTypes.Rocket:
                            Debug.Log("---------------------------------------");
                            Debug.Log("RocketFire");
                            _TrueGear.Play("ShotgunShoot");
                            break;
                        case HPEquippable.WeaponTypes.Bomb:
                            Debug.Log("---------------------------------------");
                            Debug.Log("BombFire");
                            _TrueGear.Play("ShotgunShoot");
                            break;
                        case HPEquippable.WeaponTypes.AntiRadMissile:
                            Debug.Log("---------------------------------------");
                            Debug.Log("AntiRadMissileFire");
                            _TrueGear.Play("ShotgunShoot");
                            break;
                        case HPEquippable.WeaponTypes.AGMCruise:
                            Debug.Log("---------------------------------------");
                            Debug.Log("AGMCruiseFire");
                            _TrueGear.Play("ShotgunShoot");
                            break;
                        case HPEquippable.WeaponTypes.Unknown:
                            Debug.Log("---------------------------------------");
                            Debug.Log("GunFire");
                            _TrueGear.Play("ShotgunShoot");
                            break;
                        default:
                            Debug.Log("---------------------------------------");
                            Debug.Log("GunFire");
                            _TrueGear.Play("PistolShoot");
                            break;
                    }
                    Timer t = new Timer(TimerCallback, null, 60, Timeout.Infinite);
                }

            }
        }
        private static void TimerCallback(System.Object o)
        {
            isFire = false;
        }


        [HarmonyPatch(typeof(EjectionSeat), "Eject", new Type[] { })]
        public class EjectionSeat_Eject_Postfix
        {
            [HarmonyPostfix]
            public static void Postfix(EjectionSeat __instance)
            {
                isStopListen = true;
                Debug.Log("---------------------------------------");
                Debug.Log("Eject");
                _TrueGear.Play("Eject");
                Debug.Log("StopEngineShock");
                _TrueGear.StopEngineShock();
                Debug.Log("StopSurfaceShock");
                _TrueGear.StopSurfaceShock();
                isSurfaceShock = false;
                isEngineShock = false;
            }
        }

        [HarmonyPatch(typeof(FlightSceneManager), "ReturnToBriefingOrExitScene", new Type[] { })]
        public class FlightSceneManager_ReturnToBriefingOrExitScene_Prefix
        {
            [HarmonyPrefix]
            public static bool Prefix(FlightSceneManager __instance)
            {
                isStopListen = true;
                Debug.Log("---------------------------------------");
                Debug.Log("LevelFinished");
                _TrueGear.Play("LevelFinished");
                Debug.Log("StopEngineShock");
                _TrueGear.StopEngineShock();
                Debug.Log("StopSurfaceShock");
                _TrueGear.StopSurfaceShock();
                isSurfaceShock = false;
                isEngineShock = false;
                return true;
            }
        }

        //[HarmonyPatch(typeof(FlightSceneManager), "ReloadScene", new Type[]{})]
        //public class FlightSceneManager_ReloadScene_Prefix
        //{
        //    [HarmonyPrefix]
        //    public static bool Prefix(FlightSceneManager __instance)
        //    {
        //        isStopListen = false;
        //        Debug.Log("---------------------------------------");
        //        Debug.Log("LevelStarted");
        //        _TrueGear.Play("LevelStarted");
        //        Debug.Log("StopEngineShock");
        //        _TrueGear.StopEngineShock();
        //        Debug.Log("StopSurfaceShock");
        //        _TrueGear.StopSurfaceShock();
        //        isSurfaceShock = false;
        //        isEngineShock = false;
        //        return true;
        //    }
        //}

        //[HarmonyPatch(typeof(VTOLMPSceneManager), "Awake", new Type[]{})]
        //public class VTOLMPSceneManager_Awake_Postfix
        //{
        //    [HarmonyPostfix]
        //    public static void Postfix(VTOLMPSceneManager __instance)
        //    {
        //        __instance.OnEnterVehicle += EnterVehicleAddition;
        //    }

        //    public static void EnterVehicleAddition()
        //    {
        //        isStopListen = false;
        //    }
        //}

        [HarmonyPatch(typeof(Actor), "H_OnDeath", new Type[] { })]
        public class Actor_H_OnDeath_Prefix
        {
            [HarmonyPrefix]
            public static bool Prefix(Actor __instance)
            {
                if (isStopListen)
                {
                    return true;
                }
                if (__instance.isPlayer)
                {
                    isStopListen = true;
                    Debug.Log("---------------------------------------");
                    Debug.Log("PlayerDeath");
                    _TrueGear.Play("PlayerDeath");
                    Debug.Log("StopEngineShock");
                    _TrueGear.StopEngineShock();
                    Debug.Log("StopSurfaceShock");
                    _TrueGear.StopSurfaceShock();
                    isSurfaceShock = false;
                    isEngineShock = false;
                }
                return true;
            }
        }


        [HarmonyPatch(typeof(VTOLCollisionEffects), "OnCollisionEnter", new Type[] { typeof(Collision) })]
        public class VTOLCollisionEffects_OnCollisionEnter_Postfix
        {
            [HarmonyPostfix]
            public static void Postfix(Collision col)
            {
                if (!isStopListen)
                {
                    KeyValuePair<float, float> angle = GetAngle(playerTransform, col.collider.transform.position);
                    Debug.Log("---------------------------------------");
                    Debug.Log($"DefaultDamage,{angle.Key},{angle.Value}");
                    _TrueGear.PlayAngle("DefaultDamage", angle.Key, angle.Value);
                }
            }
        }

        [HarmonyPatch(typeof(OverGWarning), "Update", new Type[] { })]
        public class OverGWarning_Update_Postfix
        {
            [HarmonyPostfix]
            public static void Postfix(OverGWarning __instance)
            {
                if (__instance.flightInfo.playerGs > __instance.maxG && !isStopListen)
                {
                    var Gs = __instance.flightInfo.playerGs - __instance.maxG;
                    if (!isHeartBeat)
                    {
                        if (Gs > 0f && Gs < 10f)
                        {
                            Debug.Log("---------------------------------------");
                            Debug.Log("LowHreatBeat");
                            _TrueGear.StartLowHeartBeat();
                            _TrueGear.StopMidHeartBeat();
                            _TrueGear.StopFastHeartBeat();
                        }
                        else if (Gs < 25f)
                        {
                            Debug.Log("---------------------------------------");
                            Debug.Log("MidHreatBeat");
                            _TrueGear.StartMidHeartBeat();
                            _TrueGear.StopLowHeartBeat();
                            _TrueGear.StopFastHeartBeat();
                        }
                        else
                        {
                            Debug.Log("---------------------------------------");
                            Debug.Log("FastHreatBeat");
                            _TrueGear.StartFastHeartBeat();
                            _TrueGear.StopLowHeartBeat();
                            _TrueGear.StopMidHeartBeat();
                        }
                        isHeartBeat = true;
                    }
                }
                else if (isHeartBeat)
                {
                    isHeartBeat = false;
                    Debug.Log("---------------------------------------");
                    Debug.Log("StopHeartBeat");
                    _TrueGear.StopLowHeartBeat();
                    _TrueGear.StopMidHeartBeat();
                    _TrueGear.StopFastHeartBeat();
                }
            }
        }

        [HarmonyPatch(typeof(VTOLCollisionEffects), "Health_OnDamage", new Type[] { typeof(float), typeof(Vector3), typeof(Health.DamageTypes) })]
        public class VTOLCollisionEffects_Health_OnDamage_Postfix
        {
            [HarmonyPostfix]
            public static void Postfix(VTOLCollisionEffects __instance, Vector3 position, Health.DamageTypes damageType)
            {
                if (!isStopListen)
                {
                    KeyValuePair<float, float> angle = GetAngle(playerTransform, position);
                    if (damageType == Health.DamageTypes.Impact)
                    {
                        Debug.Log("---------------------------------------");
                        Debug.Log($"DefaultDamage,{angle.Key},{angle.Value}");
                        _TrueGear.PlayAngle("DefaultDamage", angle.Key, angle.Value);
                    }
                    else if (damageType == Health.DamageTypes.Scrape)
                    {
                        Debug.Log("---------------------------------------");
                        Debug.Log($"PlayerBulletDamage,{angle.Key},{angle.Value}");
                        _TrueGear.PlayAngle("PlayerBulletDamage", angle.Key, angle.Value);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(CarrierCatapult), "Hook", new Type[] { typeof(CatapultHook) })]
        public class CarrierCatapult_Hook_Postfix
        {
            [HarmonyPostfix]
            public static void Postfix(CatapultHook hook)
            {
                if (!isStopListen)
                {
                    Debug.Log("---------------------------------------");
                    Debug.Log("CarrierCatapultHook");
                    _TrueGear.Play("CarrierCatapultHook");
                }
            }
        }

        [HarmonyPatch(typeof(FlightSceneManager), "SceneLoadFinished", new Type[] { })]
        public class FlightSceneManager_SceneLoadFinished_Prefix
        {
            [HarmonyPostfix]
            public static void Postfix(FlightSceneManager __instance, bool __result)
            {
                if (!__result)
                {
                    return;
                }
                Debug.Log("---------------------------------------");
                isStopListen = false;
                Debug.Log("StopEngineShock");
                _TrueGear.StopEngineShock();
                Debug.Log("StopSurfaceShock");
                _TrueGear.StopSurfaceShock();
                isSurfaceShock = false;
                isEngineShock = false;
                if (!canStarted)
                {
                    return;
                }
                canStarted = false;
                Timer StartTimer = new Timer(StartTimerCallBack, null, 500, Timeout.Infinite);
                Debug.Log("LevelStarted11111111");
                _TrueGear.Play("LevelStarted");
                Debug.Log(__result);
            }
        }

        private static void StartTimerCallBack(object o)
        {
            canStarted = true;
        }

        [HarmonyPatch(typeof(Rocket), "Fire", new Type[] { typeof(Actor) })]
        public class Rocket_Fire_Postfix
        {
            [HarmonyPostfix]
            public static void Postfix(Rocket __instance)
            {
                Debug.Log("---------------------------------------");
                Debug.Log("RocketFire");
                _TrueGear.Play("ShotgunShoot");
            }
        }

        [HarmonyPatch(typeof(Missile), "Fire", new Type[] { })]
        public class Missile_Fire_Postfix
        {
            [HarmonyPostfix]
            public static void Postfix(Rocket __instance)
            {
                Debug.Log("---------------------------------------");
                Debug.Log("MissileFire");
                _TrueGear.Play("ShotgunShoot");
            }
        }

        [HarmonyPatch(typeof(VRHandController), "HapticPulse", new Type[] { typeof(float) })]
        public class VRHandController_HapticPulse_Postfix
        {
            [HarmonyPostfix]
            public static void Postfix(VRHandController __instance, float power)
            {
                if (power == 0.6f || power == 1.2f || isStopListen)
                {
                    return;
                }
                if (power > 0.6f)
                {
                    if (__instance.isLeft)
                    {
                        Debug.Log("---------------------------------------");
                        Debug.Log("LeftHandPickupItem");
                        _TrueGear.Play("LeftHandPickupItem");
                    }
                    else
                    {
                        Debug.Log("---------------------------------------");
                        Debug.Log("RightHandPickupItem");
                        _TrueGear.Play("RightHandPickupItem");
                    }
                }
                else
                {
                    //if (__instance.isLeft)
                    //{
                    //    if (!canLeftHaptic)
                    //    {
                    //        return;
                    //    }
                    //    canLeftHaptic = false;
                    //    Timer leftHapticTimer = new Timer(LeftHapticTimerCallBack, null, 120, Timeout.Infinite);
                    //    Debug.Log("---------------------------------------");
                    //    Debug.Log("LeftHandShock");
                    //    _TrueGear.Play("LeftHandShock");
                    //}
                    //else
                    //{
                    //    if (!canRightHaptic)
                    //    {
                    //        return;
                    //    }
                    //    canRightHaptic = false;
                    //    Timer rightHapticTimer = new Timer(RightHapticTimerCallBack, null, 120, Timeout.Infinite);
                    //    Debug.Log("---------------------------------------");
                    //    Debug.Log("RightHandShock");
                    //    _TrueGear.Play("RightHandShock");
                    //}
                }

            }
        }

        //private static void LeftHapticTimerCallBack(object o)
        //{
        //    canLeftHaptic = true;
        //}

        //private static void RightHapticTimerCallBack(object o)
        //{
        //    canRightHaptic = true;
        //}

        [HarmonyPatch(typeof(LoadingSceneHelmet), "Update", new Type[] { })]
        public class LoadingSceneHelmet_Update_Postfix
        {
            [HarmonyPostfix]
            public static void Postfix(LoadingSceneHelmet __instance)
            {
                if ((__instance.grabbed || __instance.rb.velocity.sqrMagnitude > 0.01f) && Vector3.Distance(__instance.transform.position, __instance.headTransform.position) < __instance.radius)
                {
                    Debug.Log("---------------------------------------");
                    Debug.Log("Helmeted");
                    _TrueGear.Play("Helmeted");
                }
            }
        }



    }
}