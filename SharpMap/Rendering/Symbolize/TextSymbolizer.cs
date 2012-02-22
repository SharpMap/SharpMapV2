using System;
using System.Collections.Generic;
using System.ComponentModel;
using SharpMap.Data;
using SharpMap.Presentation;
using SharpMap.Rendering.Rasterize;
using SharpMap.Rendering.Rendering2D;

namespace SharpMap.Rendering.Symbolize
{
    public class TextSymbolizer : Symbolizer, ITextSymbolizer
    {
        private readonly BindingList<ITextSymbolizerRule> _rules = new BindingList<ITextSymbolizerRule>();

        public TextSymbolizer()
        {
            _rules.ListChanged += delegate
                                      {
                                          OnPropertyChanged("Rules");
                                      };
        }


        public ICollection<ITextSymbolizerRule> Rules
        {
            get { return _rules; }
        }


        public void Symbolize(IEnumerable<IFeatureDataRecord> features, RenderPhase renderPhase, Matrix2D transform,
                              ITextRasterizer rasterizer, TextSymbolizingDelegate textSymbolizingDelegate)
        {
            if (!Enabled)
                return;

            foreach (ITextSymbolizerRule rule in Rules)
            {
                if (rule.Enabled)
                    foreach (IFeatureDataRecord record in features)
                        rule.Symbolize(record, textSymbolizingDelegate(record), renderPhase, transform, rasterizer);
            }
        }

        public void Symbolize(IEnumerable<IFeatureDataRecord> features, RenderPhase renderPhase, Matrix2D transform,
                              IRasterizer rasterizer)
        {
            throw new NotSupportedException();
        }

        [NonSerialized]
        private TextSymbolizingDelegate _textFunction;

        ///<summary>
        ///</summary>
        public TextSymbolizingDelegate LabelTextFunction
        {
            get
            {
                return this._textFunction;
            }
            private set
            {
                this._textFunction = value;
                this.OnPropertyChanged("TextFuncton");
            }
        }
    }
}