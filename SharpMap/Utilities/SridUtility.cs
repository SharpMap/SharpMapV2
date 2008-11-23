using System;
using System.Collections.Generic;
using GeoAPI.CoordinateSystems;
#if DOTNET35
using Processor = System.Linq.Enumerable;
using Enumerable = System.Linq.Enumerable;
using Caster = System.Linq.Enumerable;
#else
using Processor = GeoAPI.DataStructures.Processor;
using Enumerable = GeoAPI.DataStructures.Enumerable;
using Caster = GeoAPI.DataStructures.Caster;
#endif

using Chain = GeoAPI.DataStructures;
namespace SharpMap.Utilities
{
    public class SridUtility :
        Chain.IChainProcessor<ICoordinateSystem, int?>,
        Chain.IChainProcessor<ICoordinateSystem, string>,
        Chain.IChainProcessor<string, int?>,
        Chain.IChainProcessor<int?, string>
    {
        private IEnumerable<SridChainLink> _links;
        public SridUtility(IEnumerable<SridChainLink> links)
        {
            _links = links;
        }

        #region IChainProcessor<ICoordinateSystem,int?> Members

        int? Chain.IChainProcessor<ICoordinateSystem, int?>.Process(ICoordinateSystem input, int? defaultOutput)
        {
            return Process(input, defaultOutput);
        }

        #endregion

        #region IChainProcessor<ICoordinateSystem,string> Members

        string Chain.IChainProcessor<ICoordinateSystem, string>.Process(ICoordinateSystem input, string defaultOutput)
        {
            return Process(input, defaultOutput);
        }

        #endregion

        #region IChainProcessor<string,int?> Members

        int? Chain.IChainProcessor<string, int?>.Process(string input, int? defaultOutput)
        {
            return Process(input, defaultOutput);
        }

        #endregion

        #region IChainProcessor<int?,string> Members

        string Chain.IChainProcessor<int?, string>.Process(int? input, string defaultOutput)
        {
            return Process(input, defaultOutput);
        }

        #endregion

        private TOutput Process<TInput, TOutput>(TInput input, TOutput defaultOutput)
        {
            return new Chain.ChainProcessor<TInput, TOutput>(
                Processor.Select(_links, o => (Chain.IChainLink<TInput, TOutput>)o)).Process(input, defaultOutput);

        }
    }

    public abstract class SridChainLink :
        Chain.IChainLink<int?, ICoordinateSystem>,
        Chain.IChainLink<ICoordinateSystem, int?>,
        Chain.IChainLink<string, int?>,
        Chain.IChainLink<int?, string>,
        Chain.IChainLink<ICoordinateSystem, string>
    {
        #region IChainLink<ICoordinateSystem,int?> Members

        bool Chain.IChainLink<ICoordinateSystem, int?>.Process(ICoordinateSystem input, out int? output)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IChainLink<int?,ICoordinateSystem> Members

        bool Chain.IChainLink<int?, ICoordinateSystem>.Process(int? input, out ICoordinateSystem output)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IChainLink<int?,string> Members

        bool Chain.IChainLink<int?, string>.Process(int? input, out string output)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IChainLink<string,int?> Members

        bool Chain.IChainLink<string, int?>.Process(string input, out int? output)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IChainLink<ICoordinateSystem,string> Members

        bool Chain.IChainLink<ICoordinateSystem, string>.Process(ICoordinateSystem input, out string output)
        {
            output = input.Wkt;
            return true;
        }

        #endregion
    }
}