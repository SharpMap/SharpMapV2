using System.Collections.Generic;

namespace SharpMap.Rendering.Symbolize
{
    public interface ISymbolizer<TArtifact>
    {

    }

    public interface ISymbolizer<TArtifact, TOutput> : ISymbolizer<TArtifact>
    {
        ICollection<ISymbolizerRule<TArtifact, TOutput>> Rules { get; }
        IEnumerable<TOutput> Symbolize(IEnumerable<TArtifact> artifacts);
    }
}