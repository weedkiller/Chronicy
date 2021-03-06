﻿using System.Collections.Generic;
using System.ServiceModel;
using Chronicy.Communication;
using Chronicy.Data;
using Chronicy.Excel.Information;
using Chronicy.Information;

namespace Chronicy.Excel.Communication
{
    /// <summary>
    /// Represents this client and the operations that can be called by the service.
    /// </summary>
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class TrackedClient : IClientCallback
    {
        private IInformationContext context = new MessageBoxContext();

        public delegate void NotebooksRecievedHandler(List<Notebook> notebooks);
        public delegate void MessageRecievedHandler(string message);

        public event NotebooksRecievedHandler NotebooksRecieved;
        public event MessageRecievedHandler DebugMessageRecieved;
        public event MessageRecievedHandler ErrorMessageRecieved;

        public void SendAvailableNotebooks(List<Notebook> notebooks)
        {
            NotebooksRecieved?.Invoke(notebooks);
        }

        public void SendDebugMessage(string message)
        {
            DebugMessageRecieved?.Invoke(message);
            InformationDispatcher.Default.Dispatch(message, DebugLogContext.Current);
        }

        public void SendErrorMessage(string message)
        {
            ErrorMessageRecieved?.Invoke(message);
            InformationDispatcher.Default.Dispatch("The service encountered an error!\n" + message, context, InformationKind.Error);
        }
    }
}
