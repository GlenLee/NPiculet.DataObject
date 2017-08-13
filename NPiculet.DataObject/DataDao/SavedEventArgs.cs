using System;

namespace NPiculet.DataObject
{
    public enum SaveAction
    {
        None = 0,
        Insert = 1,
        Update = 2,
        Delete = 3
    }

    public class SavedEventArgs : EventArgs
    {
        public SavedEventArgs(SaveAction action)
        {
            this.Action = action;
        }

        public SaveAction Action { get; set; }

    }
}