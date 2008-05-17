
using System.Collections;

namespace SharpMap.Expressions
{
    public class OidCollectionExpression : CollectionBinaryExpression
    {
        public OidCollectionExpression(IEnumerable collection)
            : base(new OidExpression(), CollectionOperator.In, new CollectionExpression(collection)) { }
    }
}