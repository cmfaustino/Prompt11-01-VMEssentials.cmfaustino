using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sessao2
{
    public class SessionRecorder
    {
        private Form form;
        private event EventHandler eventos;

        // Recebe na construção o Form de que se pretende gravar os
        // eventos gerados durante um período de utilização
        public SessionRecorder(Form form)
        {
            this.form = form;
        }

        // Inicia o período de gravação de eventos
        public void StartRecorder()
        {
            ;
        }

        // Termina o período de gravação de eventos
        public void StopRecorder()
        {
            ;
        }

        // Guarda a informação sobre os eventos no ficheiro fileName
        public void SaveEvents(string fileName)
        {
            ;
        }
    }
}
