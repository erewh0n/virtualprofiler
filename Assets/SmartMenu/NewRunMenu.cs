using System;
using System.Collections.Generic;
using Assets.VirtualProfiler;
using UnityEngine;

namespace Assets.SmartMenu
{
    public class NewRunMenu : ISmartMenu
    {
        private readonly List<IMenuTextField> _boundTextFields;
        private string _runFolder = "";
        private string _notes = "";
        private readonly Texture2D _greenTexture = new Texture2D(1, 1);
        private readonly Texture2D _redTexture = new Texture2D(1, 1);
        private DateTime _lastStreamReadTime = DateTime.UtcNow;
        private bool _isStreamValidAtLeastOnce = false;

        public NewRunMenu()
        {
            _greenTexture.SetPixel(0, 0, new Color(0.2f, 0.7f, 0.2f));
            _redTexture.SetPixel(0, 0, new Color(0.7f, 0.2f, 0.2f));
            _greenTexture.Apply();
            _redTexture.Apply();
            _boundTextFields = new List<IMenuTextField>
                {
                    new MenuTextFieldBinder<string>(_runFolder)
                        {
                            Name = "Folder name",
                            Description = "Name for this run",
                            FieldUpdater = x =>
                                {
                                    GUI.Label(new Rect(5, 50, 150, 25), "Name for this run: ");
                                    return GUI.TextField(new Rect(160, 50, 220, 25), x, 512);
                                },
                            Validator = x =>
                                {
                                    if (string.IsNullOrEmpty(x))
                                        throw new ArgumentException("A valid folder name must be specified.");
                                    _runFolder = x;
                                },
                        },
                    new MenuTextFieldBinder<string>(_notes)
                        {
                            Name = "Notes",
                            Description = "Any notes that should be included for this run.",
                            FieldUpdater = x =>
                                {
                                    GUI.Label(new Rect(5, 80, 150, 25), "Notes: ");
                                    return GUI.TextArea(new Rect(160, 80, 220, 125), x, 3000);
                                },
                            Validator = x => _notes = x
                        },
                };
            Global.Launcher.EnableStreamAdapter();
        }

        public ISmartMenu CreateMenu()
        {
            GUI.BeginGroup(new Rect(Screen.width/2 - 200, Screen.height/2 - 160, 400, 600));

            GUI.Box(new Rect(0, 0, 400, 300), "");

            // TODO KPH: this is pretty ugly ... make pretty? :)
            if (Global.Launcher.IsStreamAdapterReceivingData())
            {
                _lastStreamReadTime = DateTime.UtcNow;
                _isStreamValidAtLeastOnce = true;
                var style = new GUIStyle {normal = {background = _greenTexture}, alignment = TextAnchor.MiddleCenter};
                GUI.Box(new Rect(5, 10, 390, 25), new GUIContent("Movement stream is active."), style);
            }
            else if (!_isStreamValidAtLeastOnce || DateTime.UtcNow - _lastStreamReadTime > TimeSpan.FromSeconds(2))
            {
                var style = new GUIStyle { normal = { background = _redTexture }, alignment = TextAnchor.MiddleCenter };
                GUI.Box(new Rect(5, 10, 390, 25), new GUIContent("Movement stream is inactive."), style);
            }
            else
            {
                var style = new GUIStyle { normal = { background = _greenTexture }, alignment = TextAnchor.MiddleCenter };
                GUI.Box(new Rect(5, 10, 390, 25), new GUIContent("Movement stream is active."), style);
            }

            if (GUI.Button(new Rect(60, 270, 110, 25), "Run!"))
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
                    Global.Launcher.Start(_runFolder, _notes);
                }
                catch (Exception e)
                {
                    try
                    {
                        Global.Launcher.Stop();
                    }
                    catch
                    {
                    }
                    return new ConfirmationDialogMenu("Profiler", string.Format("There was a problem while starting the profiler: {0}", e.Message), this);
                }
                return new RunningMenu();
            }

            if (GUI.Button(new Rect(230, 270, 110, 25), "Cancel"))
            {
                Global.Launcher.DisableStreamAdapter();
                return new MainMenuView();
            }

            foreach (var boundField in _boundTextFields)
                boundField.Update();

            GUI.EndGroup();

            return this;
        }
    }
}