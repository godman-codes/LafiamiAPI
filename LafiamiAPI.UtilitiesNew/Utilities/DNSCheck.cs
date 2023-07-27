using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace LafiamiAPI.Utilities.Utilities
{
    public class DNSCheck
    {
        public async Task<bool> IsEmailValid(string emailAddress)
        {
            try
            {
                IPHostEntry ipEntry = await Dns.GetHostEntryAsync(emailAddress.Substring(emailAddress.IndexOf("@") + 1));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> AreEmailsValid(List<string> emailAddresses)
        {
            foreach (string emailAddress in emailAddresses)
            {
                if (!await IsEmailValid(emailAddress.Trim()))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
