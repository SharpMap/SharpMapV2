using SharpMap.Data;

namespace SharpMap.Demo.FormatConverter
{
    public interface IConvertData
    {
        FeatureDataRow ConvertRecord(IFeatureDataRecord source);
        FeatureDataTable Model { get; }
    }

    public interface IConvertData<TSource, TTarget> : IConvertData
    {
        new FeatureDataRow<TTarget> ConvertRow(IFeatureDataRecord source);
        new FeatureDataTable<TTarget> Model { get; }
    }
}