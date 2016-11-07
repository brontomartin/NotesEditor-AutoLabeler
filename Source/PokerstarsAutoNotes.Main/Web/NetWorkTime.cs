using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PokerstarsAutoNotes.Web
{
    /// <summary>
    /// 
    /// </summary>
    public class NetWorkTime
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DateTime GetNetworkTime()
        {
            return GetNetworkTime("pool.ntp.org");
        }

        /// <summary>
        /// Gets the current DateTime from <paramref name="ntpServer"/>.
        /// </summary>
        /// <param name="ntpServer">The hostname of the NTP server.</param>
        /// <returns>A DateTime containing the current time.</returns>
        private DateTime GetNetworkTime(string ntpServer)
        {
            var address = Dns.GetHostEntry(ntpServer).AddressList;

            if (address == null || address.Length == 0)
                throw new ArgumentException(@"Could not resolve ip address from '" + ntpServer + @"'.", "ntpServer");

            var ep = new IPEndPoint(address[0], 123);

            return GetNetworkTime(ep);
        }

        /// <summary>
        /// Gets the current DateTime form <paramref name="ep"/> IPEndPoint.
        /// </summary>
        /// <param name="ep">The IPEndPoint to connect to.</param>
        /// <returns>A DateTime containing the current time.</returns>
        private DateTime GetNetworkTime(IPEndPoint ep)
        {
            var s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            s.Connect(ep);

            var ntpData = new byte[48]; // RFC 2030
            ntpData[0] = 0x1B;
            for (var i = 1; i < 48; i++)
                ntpData[i] = 0;

            s.Send(ntpData);
            s.Receive(ntpData);

            const byte offsetTransmitTime = 40;
            ulong intpart = 0;
            ulong fractpart = 0;

            for (var i = 0; i <= 3; i++)
                intpart = 256 * intpart + ntpData[offsetTransmitTime + i];

            for (var i = 4; i <= 7; i++)
                fractpart = 256 * fractpart + ntpData[offsetTransmitTime + i];

            var milliseconds = (intpart * 1000 + (fractpart * 1000) / 0x100000000L);
            s.Close();

            var timeSpan = TimeSpan.FromTicks((long)milliseconds * TimeSpan.TicksPerMillisecond);

            var dateTime = new DateTime(1900, 1, 1);
            dateTime += timeSpan;

            var offsetAmount = TimeZone.CurrentTimeZone.GetUtcOffset(dateTime);
            var networkDateTime = (dateTime + offsetAmount);

            return networkDateTime;
        }
    }
}
