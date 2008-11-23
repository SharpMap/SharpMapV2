using System;
using System.Collections.Generic;
using GeoAPI.CoordinateSystems;
using Proj4Utility;

namespace SharpMap.Utilities.SridUtility
{
    public class SridMapStrategy : SridMapStrategyBase
    {
        protected readonly IDictionary<int, ICoordinateSystem> _map;
        protected readonly IDictionary<int, int> _hashMap = new Dictionary<int, int>();

        public SridMapStrategy(int priority, ICoordinateSystemFactory csFactory, IDictionary<int, ICoordinateSystem> map)
            : base(priority, csFactory)
        {
            _map = map;
            foreach (KeyValuePair<int, ICoordinateSystem> kvp in map)
            {
                _hashMap.Add(HashCoordSystem(kvp.Value), kvp.Key);
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
                output = _hashMap[hash];
                return true;
            }

            foreach (KeyValuePair<int, ICoordinateSystem> kvp in _map)
            {
                if (kvp.Value.EqualParams(input))
                {
                    output = kvp.Key;
                    return true;
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
            if (_map.ContainsKey(input.Value))
            {
                output = _map[input.Value];
                return true;
            }
            output = null;
            return false;
        }

    }


    public class SridProj4Strategy : SridMapStrategy
    {
        public SridProj4Strategy(int priority, ICoordinateSystemFactory csFactory)
            : base(priority, csFactory, GetDictionary(csFactory))
        {

        }

        private static IDictionary<int, ICoordinateSystem> GetDictionary(ICoordinateSystemFactory fact)
        {
            Dictionary<int, ICoordinateSystem> dict = new Dictionary<int, ICoordinateSystem>();
            foreach (Proj4Reader.Proj4SpatialRefSys sys in Proj4Reader.GetSRIDs())
            {
                dict.Add((int)sys.Srid, fact.CreateFromWkt(sys.SrText));
            }
            return dict;
        }
    }
}