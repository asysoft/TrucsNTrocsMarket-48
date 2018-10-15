
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TnTTickets.Lib
{

    public class TicketNumerosModel
    {
        public TicketNumerosModel()
        { }

        public int ID { get; set; }
        public string Numero { get; set; }
        public System.DateTime DateGeneration { get; set; }
        public bool IsValid { get; set; }
        public bool IsActif { get; set; }
    }

}