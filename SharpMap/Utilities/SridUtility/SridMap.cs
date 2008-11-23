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

namespace SharpMap.Utilities.SridUtility
{
    /// <summary>
    /// Utility class which attempts to map between integer and string srids and associated coordinate systems
    /// </summary>
    public class SridMap : ISridMap
    {
        private ICollection<SridMapStrategyBase> _strategies;
        public ICollection<SridMapStrategyBase> Strategies
        {
            get
            {
                return _strategies;
            }
        }

        public SridMap(IEnumerable<SridMapStrategyBase> links)
        {
            _strategies = new List<SridMapStrategyBase>(links);
        }

        #region IStrategyProcessor<ICoordinateSystem,int?> Members

        public int? Process(ICoordinateSystem input, int? defaultOutput)
        {
            return ProcessInternal(input, defaultOutput);
        }

        #endregion

        #region IStrategyProcessor<ICoordinateSystem,string> Members

        public string Process(ICoordinateSystem input, string defaultOutput)
        {
            return ProcessInternal(input, defaultOutput);
        }

        #endregion

        #region IStrategyProcessor<string,int?> Members

        public int? Process(string input, int? defaultOutput)
        {
            return ProcessInternal(input, defaultOutput);
        }

        #endregion

        #region IStrategyProcessor<int?,string> Members

        public string Process(int? input, string defaultOutput)
        {
            return ProcessInternal(input, defaultOutput);
        }

        #endregion

        private TOutput ProcessInternal<TInput, TOutput>(TInput input, TOutput defaultOutput)
        {
            return new Chain.StrategyProcessor<TInput, TOutput>(
                Processor.Select(_strategies, o => (Chain.IStrategy<TInput, TOutput>)o)).Process(input, defaultOutput);

        }

        #region IStrategyProcessor<int?,ICoordinateSystem> Members

        public ICoordinateSystem Process(int? input, ICoordinateSystem defaultOutput)
        {
            return ProcessInternal(input, defaultOutput);
        }

        #endregion

        #region IStrategyProcessor<string,ICoordinateSystem> Members

        public ICoordinateSystem Process(string input, ICoordinateSystem defaultOutput)
        {
            return ProcessInternal(input, defaultOutput);
        }

        #endregion
    }

    public interface ISridMap :
        Chain.IStrategyProcessor<int?, ICoordinateSystem>,
        Chain.IStrategyProcessor<string, ICoordinateSystem>,
        Chain.IStrategyProcessor<ICoordinateSystem, int?>,
        Chain.IStrategyProcessor<ICoordinateSystem, string>,
        Chain.IStrategyProcessor<string, int?>,
        Chain.IStrategyProcessor<int?, string>
    {
    }
}