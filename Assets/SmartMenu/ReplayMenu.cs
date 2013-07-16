using System;
using System.Collections.Generic;
using Assets.VirtualProfiler;
using UnityEngine;

namespace Assets.SmartMenu
{
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
        }

        public ISmartMenu CreateMenu()
        {
            GUI.BeginGroup(new Rect(5, 5, 150, 150));

            GUI.Box(new Rect(0, 0, 150, 150), "");

            if (GUI.Button(new Rect(5, 80, 80, 20), "Done"))
            {
                Global.Launcher.DisableStreamAdapter();
                return new MainMenuView();
            }

            if (GUI.Button(new Rect(5, 105, 80, 20), "Render Path"))
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
                    Global.Launcher.RenderReplay(_replayFolder);
                }
                catch (Exception e)
                {
                    return new ConfirmationDialogMenu("Profiler", string.Format("There was a problem while loading replay information: {0}", e.Message), this);
                }
            }

            foreach (var boundField in _boundTextFields)
                boundField.Update();

            GUI.EndGroup();

            return this;
        }
    }
}