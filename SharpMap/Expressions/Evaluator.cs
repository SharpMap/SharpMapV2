// Copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using GeoAPI.Coordinates;
using NPack.Interfaces;
using SharpMap.Data;
using SharpMap.Layers;

namespace SharpMap.Expressions
{
    public class Evaluator
    {
        private readonly ILayer _layer;
        private IFeatureDataRecord _currentRecord;

        public Evaluator(ILayer layer)
        {
            _layer = layer;
        }

        public IFeatureDataRecord CurrentRecord
        {
            get { return _currentRecord; }
            set { _currentRecord = value; }
        }

        public ILayer Layer
        {
            get { return _layer; }
        }

        public Object Evaluate(Expression value)
        {
            switch (value.ExpressionType)
            {
                case ExpressionType.Add:
                    return Add((value as BinaryOperationExpression).Left, (value as BinaryOperationExpression).Right);
                case ExpressionType.Div:
                    return Div((value as BinaryOperationExpression).Left, (value as BinaryOperationExpression).Right);
                case ExpressionType.Function:
                    break;
                case ExpressionType.Literal:
                    return (value as LiteralExpression).Value;
                case ExpressionType.Mul:
                    return Mul((value as BinaryOperationExpression).Left, (value as BinaryOperationExpression).Right);
                case ExpressionType.PropertyName:
                    return CurrentRecord[(value as PropertyNameExpression).PropertyName];
                case ExpressionType.Sub:
                    return Sub((value as BinaryOperationExpression).Left, (value as BinaryOperationExpression).Right);
                case ExpressionType.And:
                    return And((value as BinaryLogicExpression).Left, (value as BinaryLogicExpression).Right);
                case ExpressionType.Not:
                    return Not((value as NotExpression).Expression);
                case ExpressionType.Or:
                    return Or((value as BinaryLogicExpression).Left, (value as BinaryLogicExpression).Right);
                case ExpressionType.PropertyIsBetween:
                    PropertyIsBetweenExpression between = value as PropertyIsBetweenExpression;
                    return Between(Evaluate(between.PropertyName),
                                   Evaluate(between.LowerBoundary),
                                   Evaluate(between.UpperBoundary));
                case ExpressionType.PropertyIsEqualTo:
                    break;
                case ExpressionType.PropertyIsGreaterThan:
                    break;
                case ExpressionType.PropertyIsGreaterThanOrEqualTo:
                    break;
                case ExpressionType.PropertyIsLessThan:
                    break;
                case ExpressionType.PropertyIsLessThanOrEqualTo:
                    break;
                case ExpressionType.PropertyIsLike:
                    break;
                case ExpressionType.PropertyIsNotEqualTo:
                    break;
                case ExpressionType.PropertyIsNull:
                    break;
                case ExpressionType.FeatureId:
                    break;
                case ExpressionType.IdExpression:
                    break;
                case ExpressionType.ComparisonExpression:
                    break;
                case ExpressionType.LogicExpression:
                    break;
                case ExpressionType.SpatialExpression:
                    break;
                case ExpressionType.Other:
                default:
                    throw new NotSupportedException("Evaluation of expression type not supported");
            }
        }

        public Object Add(Expression left, Expression right)
        {
            throw new NotImplementedException();
        }
    }
}
