using System;
using System.Collections;
using System.Collections.Generic;
using NetCalc.Model;

namespace NetCalc.Model
{
    public class IPSegmentCollection : IEnumerable<IPSegment>, IEnumerator<IPSegment>
    {
        private double enumerator;
        private byte cidrSubnet;
        private IPSegment ipnetwork;

        private byte _cidr
        {
            get { return this.ipnetwork.Cidr; }
        }

        private string _mask
        {
            get { return this.ipnetwork.SubnetMask; }
        }
        private uint _broadcast
        {
            get { return this.ipnetwork.BroadcastAddress; }
        }
        private uint _network
        {
            get { return this.ipnetwork.NetworkAddress; }
        }

        public IPSegmentCollection(IPSegment ipnetwork, byte cidrSubnet)
        {

            if (cidrSubnet > 32)
            {
                throw new ArgumentOutOfRangeException("cidrSubnet");
            }

            if (cidrSubnet < ipnetwork.Cidr)
            {
                throw new ArgumentException("cidr");
            }

            this.cidrSubnet = cidrSubnet;
            this.ipnetwork = ipnetwork;
            this.enumerator = -1;
        }

        #region Count, Array, Enumerator

        public double Count
        {
            get
            {
                double count = Math.Pow(2, this.cidrSubnet - this._cidr);
                return count;
            }
        }

        public IPSegment this[double i ]
        {
            get
            {
                if (i - 1 >= this.Count)
                {
                    throw new ArgumentOutOfRangeException("i");
                }
                double size = this.Count;
                int increment = (int)((this._broadcast - this._network) / size);
                uint uintNetwork = (uint)(this._network + ((increment + 1) * i));
                IPSegment ipn = new IPSegment(uintNetwork.ToIpString(), this.cidrSubnet);
                return ipn;
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator<IPSegment> IEnumerable<IPSegment>.GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        #region IEnumerator<IPNetwork> Members

        public IPSegment Current
        {
            get { return this[this.enumerator]; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            // nothing to dispose
            return;
        }

        #endregion

        #region IEnumerator Members

        object IEnumerator.Current
        {
            get { return this.Current; }
        }

        public bool MoveNext()
        {
            // Por questões de performance só os primeiros 65536 itens são retornados
            this.enumerator++;
            if (this.enumerator >= this.Count || this.enumerator >= 65536)
            {
                return false;
            }
            return true;

        }

        public void Reset()
        {
            this.enumerator = -1;
        }

        #endregion

        #endregion

    }
}
