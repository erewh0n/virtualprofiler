using System;
using System.Collections.Generic;
using Assets.VirtualProfiler;
using UnityEngine;

namespace Assets.SmartMenu
{
    public class ReplayRunningMenu : ISmartMenu
    {
        public ISmartMenu CreateMenu()
        {
            GUI.BeginGroup(new Rect(5, 5, 150, 80));

            GUI.Box(new Rect(0, 0, 150, 80), "");

            if (GUI.Button(new Rect(5, 30, 100, 20), "Stop Replay"))
            {
                Global.Launcher.StopReplay();
                return new ReplayMenu();
            }

            var statusTexture = new Texture2D(1, 1);
            statusTexture.SetPixel(0, 0, new Color(0.2f, 0.2f, 0.2f));
            statusTexture.Apply();
            var style = new GUIStyle(GUI.skin.textArea) { normal = { background = statusTexture }, alignment = TextAnchor.MiddleCenter };
            GUI.Box(new Rect(5, 5, 140, 15), new GUIContent("% complete: " + (int) Global.Launcher.ReplayStatusPercentDone()), style);

            GUI.EndGroup();

            return this;
        }
    }

    public class ReplayMenu : ISmartMenu
    {
        private readonly List<IMenuTextField> _boundTextFields;
        private string _replayFolder = "";

        public ReplayMenu()
        {
            _boundTextFields = new List<IMenuTextField>
                {
                    new MenuTextFieldBinder<string>(_replayFolder)
                        {
                            Name = "Movement log file",
                            Description = "Path to movement log file that should be replayed.",
                            FieldUpdater = x =>
                                {
                                    GUI.Label(new Rect(5, 10, 100, 20), "Movement log file:");
                                    return GUI.TextField(new Rect(5, 35, 140, 20), x, 512);
                                },
                            Validator = x =>
                                {
                                    if (string.IsNullOrEmpty(x))
                                        throw new ArgumentException("A valid movement log file name must be specified.");
                                    _replayFolder = x;
                                },
                        },
                };

            Global.Launcher.StartReplay();
        }

        public ISmartMenu CreateMenu()
        {
            GUI.BeginGroup(new Rect(5, 5, 150, 155));

            GUI.Box(new Rect(0, 0, 150, 155), "");

            if (GUI.Button(new Rect(5, 80, 100, 20), "Exit To Main Menu"))
            {
                Global.Launcher.StopReplay();
                return new MainMenuView();
            }
            if (GUI.Button(new Rect(5, 105, 100, 20), "Instant Render"))
            {
                return LaunchReplay(false);
            }
            if (GUI.Button(new Rect(5, 130, 100, 20), "Real-time Render"))
            {
                return LaunchReplay(true);
            }

            foreach (var boundField in _boundTextFields)
                boundField.Update();

            GUI.EndGroup();

            return this;
        }

        public ISmartMenu LaunchReplay(bool isRealTime)
        {
            foreach (var boundField in _boundTextFields)
            {
                if (!boundField.UpdateAndValidate())
                {
                    return new ConfirmationDialogMenu(boundField.Name, boundField.Description, this);
                }
            }
            try
            {
                Global.Launcher.RenderReplay(_replayFolder, isRealTime);
            }
            catch (Exception e)
            {
                Logger.Warning("Problem during replay.", e);
                return new ConfirmationDialogMenu("Profiler", string.Format("There was a problem during replay: {0}", e.Message), this);
            }

            foreach (var boundField in _boundTextFields)
                boundField.Update();

            return new ReplayRunningMenu();
        }
    }
}