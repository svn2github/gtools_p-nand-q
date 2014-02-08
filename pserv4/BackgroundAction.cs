using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pserv4
{
    public abstract class BackgroundAction
    {
        public delegate void SetOutputTextDelegate(string message);
        protected SetOutputTextDelegate SetOutputText;
        public bool IsCancelled;

        public BackgroundAction()
        {
        }

        public void Setup(SetOutputTextDelegate setOutputText)
        {
            SetOutputText = setOutputText;
            IsCancelled = false;
        }

        public abstract void DoWork();
    }
}
