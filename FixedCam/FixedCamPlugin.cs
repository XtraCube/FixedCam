﻿using BepInEx;
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
    [ReactorPluginSide(PluginSide.Client)]
    public class FixedCamPlugin : BasePlugin
    {
        // Define variables
        public const string Id = "com.xtracube.fixedcam";
        public static bool fixedOn = true;
        public static bool showPing = true;
        public Harmony Harmony { get; } = new Harmony(Id);

        // Patch all methods
        public override void Load()
        {
            Harmony.PatchAll();
        }

        // Set the camera's parent to enable or disable fixed mode
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        public static class PlayerControlFixedUpdatePatch
        {
            public static void Postfix()
            {
                if (fixedOn && PlayerControl.LocalPlayer.CanMove)
                {
                    Camera.main.transform.SetParent(PlayerControl.LocalPlayer.transform);
                }
                else
                {
                    Camera.main.transform.SetParent(null);
                }

            }
        }
        
        // Detect key presses for toggling variables
        [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
        public static class KeyboardJoystickUpdatePatchPatch
        {
            public static void Postfix()
            {
                if (Input.GetKeyDown(KeyCode.F5))// Enable/disable status of cam under ping tracker
                    showPing = !showPing;

                if (Input.GetKeyDown(KeyCode.F6))// Enable/disable fixed camera
                    fixedOn = !fixedOn;
            }
        }
        
        // Show status of camera under the ping tracker
        [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
        public static class PingTrackerUpdatePatch
        {
            public static void Postfix(PingTracker __instance)
            {
                __instance.text.Centered = true;
                if (showPing)
                {
                    if (fixedOn)
                        __instance.text.Text += "\nFixedCam: [00FF00FF]Enabled[]";
                    else
                        __instance.text.Text += "\nFixedCam: [FF0000FF]Disabled[]";
                }
            }
        }




    }
}
