using System;
using System.Collections.Generic;
using System.Text;

namespace Lafiami.Models.Internals
{
    public class AppSettings
    {
        public string AiicoAPIAddress { get; set; }
        public string AiicoAPIKey { get; set; }
        public string HygeiaAPIAddress { get; set; }


        public string HygeiaLafimiUsername { get; set; }
        public string HygeiaLafimiPassword { get; set; }
        public string HygeiaLafimiGrant { get; set; }

        public string AdminEmails { get; set; }
    }
}
