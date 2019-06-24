﻿using System;

namespace Chronicy.Excel.Tracking.Events
{
    public class TrackingEvent
    {
        public Type ValueType { get; set; }
        public object Value { get; }

        public DateTime TriggerDate { get; }

        public bool Handled { get; private set; }

        public TrackingEvent(object value)
        {
            Value = value;
            ValueType = value.GetType();
            TriggerDate = DateTime.Now;
        }

        public void Handle()
        {
            Handled = true;
        }
    }
}
