// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA using System;

// This file Copyright Newgrove Consultants Ltd 2008
// Author: John Diss

using System;
using System.Collections.Generic;
using GeoAPI.CoordinateSystems;
using Proj4Utility;

namespace SharpMap.Utilities.SridUtility
{
    public class SridMapStrategy : SridMapStrategyBase
    {
        protected interface IKey
        {
            string Authority { get; set; }
            object Code { get; set; }
        }

        protected interface IKey<TCode> : IKey
        {
            new TCode Code { get; set; }
        }

        private struct StringKey : IKey<String>
        {

            #region IKey<string> Members

            public string Code
            {
                get;
                set;
            }

            #endregion

            #region IKey Members

            public string Authority
            {
                get;
                set;
            }

            object IKey.Code
            {
                get
                {
                    return Code;
                }
                set
                {
                    Code = (string)value;
                }
            }

            #endregion
        }

        private struct LongKey : IKey<long>
        {

            #region IKey<long> Members

            public long Code
            {
                get;
                set;
            }

            #endregion

            #region IKey Members

            public string Authority
            {
                get;
                set;
            }

            object IKey.Code
            {
                get
                {
                    return Code;
                }
                set
                {
                    Code = (long)value;
                }
            }

            #endregion
        }


        //protected readonly IEnumerable<ICoordinateSystem> _coordinateSystems;
        protected readonly IDictionary<IKey, ICoordinateSystem> _map = new Dictionary<IKey, ICoordinateSystem>();
        protected readonly IDictionary<int, IKey> _hashMap = new Dictionary<int, IKey>();

        public SridMapStrategy(int priority, ICoordinateSystemFactory csFactory, IEnumerable<ICoordinateSystem> systems)
            : base(priority, csFactory)
        {
            //_coordinateSystems = systems;
            foreach (ICoordinateSystem cs in systems)
            {
                IKey key;
                long code;
                if (long.TryParse(cs.AuthorityCode, out code))
                {
                    key = new LongKey() { Authority = cs.Authority, Code = code };
                }
                else
                {
                    key = new StringKey() { Authority = cs.Authority, Code = cs.AuthorityCode };
                }

                _map.Add(key, cs);

                _hashMap.Add(HashCoordSystem(cs), key);

            }
        }

        //TODO: improve hashing system
        private int HashCoordSystem(ICoordinateSystem sys)
        {
            if (sys is IProjectedCoordinateSystem)
                return HashProjectedCoordinateSystem(sys as IProjectedCoordinateSystem);
            if (sys is IGeographicCoordinateSystem)
                return HashGeographicCoordinateSystem(sys as IGeographicCoordinateSystem);
            if (sys is IGeocentricCoordinateSystem)
                return HashGeocentricCoordinateSystem(sys as IGeocentricCoordinateSystem);

            throw new ArgumentException(string.Format("No hash routine for coordinate sys '{0}'", sys.GetType()));
        }

        private int HashGeocentricCoordinateSystem(IGeocentricCoordinateSystem sys)
        {
            return sys.HorizontalDatum.GetHashCode() ^ sys.PrimeMeridian.GetHashCode() ^ sys.Dimension.GetHashCode() ^
              sys.LinearUnit.GetHashCode();
        }

        private int HashGeographicCoordinateSystem(IGeographicCoordinateSystem sys)
        {
            return sys.AngularUnit.GetHashCode() ^ sys.HorizontalDatum.GetHashCode() ^ sys.PrimeMeridian.GetHashCode() ^
                   sys.Dimension.GetHashCode();
        }

        private int HashProjectedCoordinateSystem(IProjectedCoordinateSystem sys)
        {
            return HashGeographicCoordinateSystem(sys.GeographicCoordinateSystem) ^ sys.HorizontalDatum.GetHashCode() ^
                   sys.LinearUnit.GetHashCode() ^ HashProjection(sys.Projection);
        }

        private int HashProjection(IProjection proj)
        {
            int h = proj.ProjectionClassName.GetHashCode();

            foreach (var v in proj)
                h ^= v.Value.GetHashCode();

            return h;
        }

        public override bool Process(ICoordinateSystem input, out int? output)
        {
            int hash = HashCoordSystem(input);
            if (_hashMap.ContainsKey(hash))
            {
                IKey key = _hashMap[hash];
                if (key is IKey<long>)
                {
                    output = (int)((IKey<long>)_hashMap[hash]).Code;
                    return true;
                }

                //we found a matching hash but it has a string key.. hmmm
                output = null;
                return false;

            }

            foreach (KeyValuePair<IKey, ICoordinateSystem> kvp in _map)
            {
                if (kvp.Value.EqualParams(input))
                {
                    IKey key = kvp.Key;
                    if (key is IKey<long>)
                    {
                        output = (int)((IKey<long>)key).Code;
                        return true;
                    }

                }
            }
            output = null;
            return false;
        }

        public override bool Process(int? input, out ICoordinateSystem output)
        {
            if (!input.HasValue)
            {
                output = null;
                return false;
            }

            foreach (IKey key in _map.Keys)
            {
                if (key is IKey<long>)
                {
                    IKey<long> longKey = key as IKey<long>;
                    if (longKey.Code == (long)input)
                    {
                        output = _map[longKey];
                        return true;
                    }
                }
            }
            output = null;
            return false;
        }

        //TODO: add cases for URN codes
        public override bool Process(string input, out ICoordinateSystem output)
        {
            string[] prts = input.Split(':');
            if (prts.Length == 2)
            {
                IKey key;
                long code;
                if (long.TryParse(prts[1], out code))
                {
                    key = new LongKey() { Authority = prts[0], Code = code };
                }
                else
                {
                    key = new StringKey() { Authority = prts[0], Code = prts[1] };
                }

                if (_map.ContainsKey(key))
                {
                    output = _map[key];
                    return true;
                }
            }
            try
            {
                if (string.IsNullOrEmpty(input))
                {
                    output = null;
                    return false;
                }

                output = CoordinateSystemFactory.CreateFromWkt(input);
                return true;
            }
            catch
            {
                output = null;
                return false;
            }

        }
    }


    public class SridProj4Strategy : SridMapStrategy
    {
        public SridProj4Strategy(int priority, ICoordinateSystemFactory csFactory)
            : base(priority, csFactory, GetList(csFactory))
        {

        }

        private static IEnumerable<ICoordinateSystem> GetList(ICoordinateSystemFactory fact)
        {
            List<ICoordinateSystem> lst = new List<ICoordinateSystem>();
            foreach (Proj4Reader.Proj4SpatialRefSys sys in Proj4Reader.GetSRIDs())
            {
                string wkt = sys.SrText;


                ICoordinateSystem cs = fact.CreateFromWkt(wkt);
                lst.Add(cs);
            }
            return lst;
        }
    }
}