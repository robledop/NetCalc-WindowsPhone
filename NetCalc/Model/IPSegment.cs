
using System;
using System.Collections.Generic;


namespace NetCalc.Model
{
    public class IPSegment : IComparable<IPSegment>
    {
        // IP e máscara em formato uint
        private uint ip;
        private uint mask;

        public IPSegment(string _ip, string _mask)
        {
            ip = _ip.ParseIp();
            mask = _mask.ParseIp();
        }

        public IPSegment(string _ip, byte _cidr)
        {
            ip = _ip.ParseIp();
            mask = CidrToMask(_cidr);
        }

        public bool Rfc3021
        {
            get 
            {
                if (this.Cidr == 31)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private byte MaskToCidr(UInt32 mask)
        {
            UInt32 __mask = mask;
            __mask = __mask - ((__mask >> 1) & 0x55555555);
            __mask = (__mask & 0x33333333) + ((__mask >> 2) & 0x33333333);
            return Convert.ToByte((((__mask + (__mask >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24);
        }

        private uint CidrToMask(byte cidr)
        {
            uint mask = 0xFFFFFFFF;
            mask = mask << (32 - cidr);
            return mask;
        }

        public byte Cidr
        {
            get 
            {
                return MaskToCidr(mask);
            }
        }

        public string SubnetMask
        {
            get
            {
                return mask.ToIpString();
            }
        }

        public uint NumberOfHosts
        {
            get 
            {
                uint allIPs = ~mask + 1;

                uint hosts = 0;

                if (allIPs > 2)
                {
                    hosts = allIPs - 2;
                }
                else if (allIPs == 2 && Cidr == 31)
                {
                    hosts = 2;
                }
                else if (allIPs == 1)
                {
                    hosts = 1;
                }

                return hosts;
            }
        }

        public uint NetworkAddress
        {
            get { return this.ip & this.mask; }
        }

        public uint BroadcastAddress
        {
            get { return NetworkAddress + ~mask; }
        }

        public IEnumerable<uint> Hosts()
        {
            for (var host = NetworkAddress + 1; host < BroadcastAddress; host++)
            {
                yield return host;
            }
        }

        /// <summary>
        /// First usable IP adress in Network
        /// </summary>
        public uint FirstUsable
        {
            get
            {
                if (this.Cidr == 31 || this.Cidr == 32)
                {
                    return this.NetworkAddress;
                }
                else
                {
                    return (this.Usable <= 0) ? this.NetworkAddress : this.NetworkAddress + 1;
                }
            }
        }

        /// <summary>
        /// Last usable IP adress in Network
        /// </summary>
        public uint LastUsable
        {
            get
            {
                if (this.Cidr == 31 || this.Cidr == 32)
                {
                    return this.BroadcastAddress;
                }
                else
                {
                    return (this.Usable <= 0) ? this.NetworkAddress : this.BroadcastAddress - 1;
                }
            }
        }

        public string WildCardSubnetMask
        {
            get
            {
                return (~mask).ToIpString();
            }
        }
        /// <summary>
        /// Number of usable IP adress in Network
        /// </summary>
        public uint Usable
        {
            get
            {
                //return (this.CIDR > 32) ? 0 : ((0xffffffff >> this.CIDR) - 1);
                return ((0xffffffff >> this.Cidr) - 1);
            }
        }

        public int CompareTo(IPSegment other)
        {
            int network = this.NetworkAddress.CompareTo(other.NetworkAddress);
            if (network != 0)
            {
                return network;
            }

            int cidr = this.Cidr.CompareTo(other.Cidr);
            return cidr;
        }
    }
}