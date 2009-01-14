using SharpMap.Data;

namespace SharpMap.Demo.FormatConverter
{
    public interface IConvertData
    {
        FeatureDataRow ConvertRecord(IFeatureDataRecord source);
    }

    public interface IConvertData<TSource, TTarget> : IConvertData
    {
        new FeatureDataRow<TTarget> ConvertRow(IFeatureDataRecord source);
    }
}