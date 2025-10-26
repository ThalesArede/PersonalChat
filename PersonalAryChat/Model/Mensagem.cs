using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalAryChat.Model
{
    public class Mensagem
    {
        public string Texto { get; set; }
        public bool EhUsuario { get; set; }
        public DateTime Horario { get; set; } = DateTime.Now;
    }
}
