using System;
using System.Collections.Generic;
using Assets.VirtualProfiler;
using UnityEngine;

namespace Assets.SmartMenu
{
    public class LabeledField
    {

        public LabeledField(int xOffset, int yOffset, int labelWidth, int fieldWidth)
        {
            
        }
    }

    public class ArduinoMenu : ISmartMenu
    {
        private readonly List<IMenuTextField> _boundTextFields;

        public ArduinoMenu()
        {
            _boundTextFields = new List<IMenuTextField>
                {
                    new MenuTextFieldBinder<string>(Global.Config.SerialPortMovementInput)
                        {
                            Name = "Movement COM port",
                            Description = "The COM port for reading movement data (e.g. 'COM1', 'COM2', etc)",
                            FieldUpdater = x =>
                                {
                                    GUI.Label(new Rect(5, 50, 150, 25), "Movement COM Port: ");
                                    return GUI.TextField(new Rect(160, 50, 160, 25), x, 4);
                                },
                            Validator = x =>
                                {
                                    if (string.IsNullOrEmpty(x))
                                        throw new ArgumentException("The movement COM port must be specified!");
                                    Global.Config.SerialPortMovementInput = x;
                                },
                        },
                    new MenuTextFieldBinder<string>(Global.Config.SerialPortBaud.ToString())
                        {
                            Name = "Baud rate",
                            Description = "The baud rate for the movement COM",
                            FieldUpdater = x =>
                                {
                                    GUI.Label(new Rect(5, 80, 150, 25), "Movement COM Baud: ");
                                    return GUI.TextField(new Rect(160, 80, 160, 25), x.ToString(), 6);
                                },
                            Validator = x => Global.Config.SerialPortBaud = int.Parse(x),
                        },
                    new MenuTextFieldBinder<string>(Global.Config.MovementLogDirectory)
                        {
                            Name = "Reporting folder",
                            Description = "A path to a folder where the individual profiler runs are saved.",
                            FieldUpdater = x =>
                                {
                                    GUI.Label(new Rect(5, 110, 150, 25), "Reporting Folder: ");
                                    return GUI.TextField(new Rect(160, 110, 160, 25), x, 500);
                                },
                            Validator = x =>
                                {
                                    if (string.IsNullOrEmpty(x))
                                        throw new ArgumentException("The reporting folder cannot be empty!");
                                    Global.Config.MovementLogDirectory = x;
                                },
                        },
                    new MenuTextFieldBinder<string>(Global.Config.ScaleX.ToString())
                        {
                            Name = "X Scaling",
                            Description = "The 'X' scaling value can be any decimal (e.g. 0.01, -42.5, 100, etc)",
                            FieldUpdater = x =>
                                {
                                    GUI.Label(new Rect(5, 140, 150, 25), "X Scaling: ");
                                    return GUI.TextField(new Rect(160, 140, 160, 25), x.ToString(), 6);
                                },
                            Validator = x => Global.Config.ScaleX = float.Parse(x),
                        },
                    new MenuTextFieldBinder<string>(Global.Config.ScaleY.ToString())
                        {
                            Name = "Y Scaling",
                            Description = "The 'Y' scaling value can be any decimal (e.g. 0.01, -42.5, 100, etc)",
                            FieldUpdater = x =>
                                {
                                    GUI.Label(new Rect(5, 170, 150, 25), "Y Scaling: ");
                                    return GUI.TextField(new Rect(160, 170, 160, 25), x.ToString(), 6);
                                },
                            Validator = x => Global.Config.ScaleY = float.Parse(x),
                        },
                    new MenuTextFieldBinder<string>(Global.Config.ScaleZ.ToString())
                        {
                            Name = "Z Scaling",
                            Description = "The 'Z' scaling value can be any decimal (e.g. 0.01, -42.5, 100, etc)",
                            FieldUpdater = x =>
                                {
                                    GUI.Label(new Rect(5, 200, 150, 25), "Z Scaling: ");
                                    return GUI.TextField(new Rect(160, 200, 160, 25), x.ToString(), 6);
                                },
                            Validator = x => Global.Config.ScaleZ = float.Parse(x),
                        },
                    new MenuTextFieldBinder<string>(Global.Config.Smoothing.ToString())
                        {
                            Name = "Smoothing",
                            Description = "The smoothing value can be any decimal (e.g. 0.01, -42.5, 100, etc)",
                            FieldUpdater = x =>
                                {
                                    GUI.Label(new Rect(5, 230, 150, 25), "Smoothing: ");
                                    return GUI.TextField(new Rect(160, 230, 160, 25), x.ToString(), 6);
                                },
                            Validator = x => Global.Config.Smoothing = float.Parse(x),
                        },
                };
        }

        public ISmartMenu CreateMenu()
        {
            GUI.BeginGroup(new Rect(Screen.width/2 - 200, Screen.height/2 - 160, 400, 600));

            GUI.Box(new Rect(0, 0, 400, 400), "");

            if (GUI.Button(new Rect(145, 370, 110, 25), "OK"))
            {
                foreach (var boundField in _boundTextFields)
                {
                    if (!boundField.UpdateAndValidate())
                    {
                        return new ConfirmationDialogMenu(boundField.Name, boundField.Description, this);
                    }
                }
                return new MainMenuView();
            }

            foreach (var boundField in _boundTextFields)
                boundField.Update();

            GUI.EndGroup();

            return this;
        }

    }
}