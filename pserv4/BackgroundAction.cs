using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace pserv4
{
    public abstract class BackgroundAction
    {
        public delegate void SetOutputTextDelegate(string message);
        protected SetOutputTextDelegate SetOutputText;
        protected BackgroundWorker Worker;

        public BackgroundAction()
        {
        }

        public void Bind(BackgroundWorker worker)
        {
            Worker = worker;
        }

        public void Setup(SetOutputTextDelegate setOutputText)
        {
            SetOutputText = setOutputText;
        }

        public abstract void DoWork();
    }
}
