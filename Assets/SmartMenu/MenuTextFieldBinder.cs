using System;
using Assets.VirtualProfiler;

namespace Assets.SmartMenu
{
    public class MenuTextFieldBinder<TControlType> : IMenuTextField
    {
        private TControlType _textField;

        public Func<TControlType, TControlType> FieldUpdater { get; set; }
        public Action<TControlType> Validator { get; set; }
        public Action ErrorHandler { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }

        public MenuTextFieldBinder(TControlType initialValue)
        {
            _textField = initialValue;
        }

        public void Update()
        {
            _textField = FieldUpdater.Invoke(_textField);
        }

        public bool UpdateAndValidate()
        {
            Update();

            try
            {
                Validator.Invoke(_textField);
            }
            catch (Exception e)
            {
                Logger.Debug(string.Format("Failed to update object {0} to: {1}", Description, _textField));
                if (ErrorHandler != null) ErrorHandler.Invoke();
                return false;
            }

            return true;
        }
    }
}