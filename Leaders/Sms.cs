using AoCLibrary;
using PrivateStash;
using System;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Finder2020Win
{
    public class Sms
    {
        /// <summary>
        /// Sends message with Twilio- costs $.0075 per message and spamming will get us blocked so use sparingly
        /// </summary>
        /// <param name="phone">10-digit US phone number</param>
        /// <param name="body"></param>
        /// <returns>returns false if phone number invalid</returns>
        static public bool SendMessage(string phone, string body)
        {
            ElfHelper.Log($"Sms.SendMessage({phone})");
            try
            {
                phone = StripPhoneNumber(phone);
                if (phone?.Length == 10)
                    phone = "1" + phone;
                if (phone?.Length == 11)
                    phone = "+" + phone;
                if (phone?.Length != 12)
                {
					ElfHelper.Log($"SMS.SendMessage() Invalid phone number {phone}");
                    return false;
                }

                ElfHelper.Log($"SMS.SendMessage() to {phone}");

                TwilioClient.Init(TwilioStash.AccountSid, TwilioStash.AuthToken);

                ElfHelper.Log("Sms.SendMessage() Really send");
                var message = MessageResource.Create(
                    to: new PhoneNumber(phone),
                    from: new PhoneNumber(TwilioStash.FromNumber),
                    body: body);

                return true;
            }
            catch (Exception ex)
            {
                ElfHelper.Log("SMS.SendMessage() " + ex);
                return false;
            }
        }
        public static string StripPhoneNumber(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            var rv = string.Empty;
            foreach (var c in str)
                if (char.IsNumber(c))
                    rv += c;
            return rv;
        }
    }
}
