namespace SharpMap.Rendering.Symbolize
{
    public interface ISymbolizerRule<TArtifact>
    {
        bool Enabled { get; }
        bool Symbolize(TArtifact obj, out object output);
    }

    public interface ISymbolizerRule<TArtifact, TOutput> : ISymbolizerRule<TArtifact>
    {
        bool Symbolize(TArtifact obj, out TOutput output);
    }
}