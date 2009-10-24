using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SharpMap.Rendering.Symbolize
{
    public class Symbolizer<TArtifact, TOutput> : ISymbolizer<TArtifact, TOutput>
    {
        private readonly Collection<ISymbolizerRule<TArtifact, TOutput>> _rules = new Collection<ISymbolizerRule<TArtifact, TOutput>>();

        #region ISymbolizer<T> Members

        public ICollection<ISymbolizerRule<TArtifact, TOutput>> Rules
        {
            get { return _rules; }
        }

        public IEnumerable<TOutput> Symbolize(IEnumerable<TArtifact> artifacts)
        {
            List<TArtifact> working = new List<TArtifact>(artifacts);
            foreach (ISymbolizerRule<TArtifact, TOutput> rule in Rules)
            {
                if (rule.Enabled)
                {
                    foreach (TArtifact artifact in working)
                    {
                        TOutput output;
                        if (rule.Symbolize(artifact, out output))
                            yield return output;
                    }
                }
            }
        }

        #endregion
    }
}