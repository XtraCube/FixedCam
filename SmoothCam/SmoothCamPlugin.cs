using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using HarmonyLib;
using Reactor;
using System.Collections;
using UnityEngine;

namespace Example
{
    [BepInPlugin(Id)]
    [BepInProcess("Among Us.exe")]
    [BepInDependency(ReactorPlugin.Id)]
    public class SmoothCamPlugin : BasePlugin
    {
        public const string Id = "com.xtracube.smoothcam";
        public static bool smoothEnabled = true;
        public static bool showPing = true;
        public Harmony Harmony { get; } = new Harmony(Id);

        public override void Load()
        {
            Harmony.PatchAll();
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        public static class PlayerControlFixedUpdatePatch
        {
            public static void Postfix()
            {
                if (smoothEnabled)
                {
                    if (Camera.main.transform.parent != PlayerControl.LocalPlayer.transform)
                        Camera.main.transform.SetParent(PlayerControl.LocalPlayer.transform);
                }
                else
                {
                    if (Camera.main.transform.parent != null)
                        Camera.main.transform.SetParent(null);
                }
            }
        }
        
        [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
        public static class KeyboardJoystickUpdatePatchPatch
        {
            public static void Postfix()
            {
                if (Input.GetKeyDown(KeyCode.F5))
                    showPing = !showPing;

                if (Input.GetKeyDown(KeyCode.F6))
                    smoothEnabled = !smoothEnabled;
            }
        }
                
        [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
        public static class PingTrackerUpdatePatch
        {
            public static void Postfix(PingTracker __instance)
            {
                __instance.text.Centered = true;
                if (showPing)
                {
                    if (smoothEnabled)
                        __instance.text.Text += "\nSmoothCam: [00FF00FF]Enabled[]";
                    else
                        __instance.text.Text += "\nSmoothCam: [FF0000FF]Disabled[]";
                }
            }
        }


    }
}
