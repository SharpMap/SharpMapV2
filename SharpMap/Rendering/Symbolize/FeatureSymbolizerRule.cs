using SharpMap.Data;

namespace SharpMap.Rendering.Symbolize
{
    public abstract class FeatureSymbolizerRule<TOutput> : ISymbolizerRule<IFeatureDataRecord, TOutput>
    {
        #region ISymbolizerRule<IFeatureDataRecord,TOutput> Members

        public abstract bool Symbolize(IFeatureDataRecord obj, out TOutput output);

        public abstract bool Enabled { get; }

        public bool Symbolize(IFeatureDataRecord obj, out object output)
        {
            TOutput toutput;
            if (Symbolize(obj, out toutput))
            {
                output = toutput;
                return true;
            }
            output = null;
            return false;
        }

        #endregion
    }
}