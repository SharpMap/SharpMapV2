using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SharpMap.Data;
using SharpMap.Rendering.Rasterize;

namespace SharpMap.Rendering.Symbolize
{
    public class TextSymbolizer : ITextSymbolizer
    {
        private readonly ICollection<ITextSymbolizerRule> _rules = new Collection<ITextSymbolizerRule>();

        private ITextRasterizer _rasterizer;


        protected TextSymbolizer(ITextRasterizer rasterizer)
        {
            Rasterizer = rasterizer;
        }

        #region ITextSymbolizer Members

        public ITextRasterizer Rasterizer
        {
            get { return _rasterizer; }
            protected set { _rasterizer = value; }
        }

        public ICollection<ITextSymbolizerRule> Rules
        {
            get { return _rules; }
        }

        IRasterizer ISymbolizer.Rasterizer
        {
            get { return Rasterizer; }
        }

        ICollection<ISymbolizerRule> ISymbolizer.Rules
        {
            get { throw new NotSupportedException(); }
        }

        public void Symbolize(IEnumerable<IFeatureDataRecord> features, TextSymbolizingDelegate textSymbolizingDelegate)
        {
            Rasterizer.BeginPass();
            List<IFeatureDataRecord> working = new List<IFeatureDataRecord>(features);
            ITextRasterizer rasterizer = Rasterizer;
            foreach (ITextSymbolizerRule rule in Rules)
            {
                if (rule.Enabled)
                    foreach (IFeatureDataRecord record in working)
                        rule.Symbolize(record, textSymbolizingDelegate(record), rasterizer);
            }
            Rasterizer.EndPass();
        }

        #endregion


        #region ISymbolizer Members


        public void Symbolize(IEnumerable<IFeatureDataRecord> features)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}