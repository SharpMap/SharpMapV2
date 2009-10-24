using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SharpMap.Data;
using SharpMap.Rendering.Rasterize;

namespace SharpMap.Rendering.Symbolize
{
    public class FeatureSymbolizer : IFeatureSymbolizer
    {
        private readonly Collection<IFeatureSymbolizerRule> _rules = new Collection<IFeatureSymbolizerRule>();
        private IFeatureRasterizer _rasterizer;

        public FeatureSymbolizer(IFeatureRasterizer rasterizer)
        {
            Rasterizer = rasterizer;
        }

        #region IFeatureSymbolizer Members

        public ICollection<IFeatureSymbolizerRule> Rules
        {
            get { return _rules; }
        }

        public void Symbolize(IEnumerable<IFeatureDataRecord> records)
        {
            Rasterizer.BeginPass();
            List<IFeatureDataRecord> working = new List<IFeatureDataRecord>(records);
            foreach (IFeatureSymbolizerRule rule in Rules)
            {
                if (rule.Enabled)
                {
                    foreach (IFeatureDataRecord artifact in working)
                    {
                        rule.Symbolize(artifact, Rasterizer);
                    }
                }
            }
            Rasterizer.EndPass();
        }

        public IFeatureRasterizer Rasterizer
        {
            get { return _rasterizer; }
            protected set { _rasterizer = value; }
        }

        ICollection<ISymbolizerRule> ISymbolizer.Rules
        {
            get { throw new NotSupportedException(); }
        }

        IRasterizer ISymbolizer.Rasterizer
        {
            get { return Rasterizer; }
        }

        #endregion
    }
}