using System;
using System.Data;
using Xunit;


namespace SharpMap.Tests
{
    internal static class DataTableHelper
    {
        public static void AssertTableStructureIdentical(DataTable lhs, DataTable rhs)
        {
            Assert.NotSame(lhs, rhs);
            Assert.True(lhs.Columns.Count > 0);
            Assert.Equal(lhs.Columns.Count, rhs.Columns.Count);

            for (Int32 i = 0; i < lhs.Columns.Count; i++)
            {
                Assert.Equal(lhs.Columns[i].AllowDBNull, rhs.Columns[i].AllowDBNull);
                Assert.Equal(lhs.Columns[i].AutoIncrement, rhs.Columns[i].AutoIncrement);
                Assert.Equal(lhs.Columns[i].AutoIncrementSeed, rhs.Columns[i].AutoIncrementSeed);
                Assert.Equal(lhs.Columns[i].AutoIncrementStep, rhs.Columns[i].AutoIncrementStep);
                Assert.Equal(lhs.Columns[i].Caption, rhs.Columns[i].Caption);
                Assert.Equal(lhs.Columns[i].ColumnMapping, rhs.Columns[i].ColumnMapping);
                Assert.Equal(lhs.Columns[i].ColumnName, rhs.Columns[i].ColumnName);
                Assert.Equal(lhs.Columns[i].DataType, rhs.Columns[i].DataType);
                Assert.Equal(lhs.Columns[i].DateTimeMode, rhs.Columns[i].DateTimeMode);
                Assert.Equal(lhs.Columns[i].DefaultValue, rhs.Columns[i].DefaultValue);
                Assert.Equal(lhs.Columns[i].Expression, rhs.Columns[i].Expression);
                Assert.Equal(lhs.Columns[i].MaxLength, rhs.Columns[i].MaxLength);
                Assert.Equal(lhs.Columns[i].Namespace, rhs.Columns[i].Namespace);
                Assert.Equal(lhs.Columns[i].Ordinal, rhs.Columns[i].Ordinal);
                Assert.Equal(lhs.Columns[i].Prefix, rhs.Columns[i].Prefix);
                Assert.Equal(lhs.Columns[i].ReadOnly, rhs.Columns[i].ReadOnly);
                Assert.Equal(lhs.Columns[i].Unique, rhs.Columns[i].Unique);
                Assert.Equal(lhs.Columns[i].ExtendedProperties.Count, rhs.Columns[i].ExtendedProperties.Count);

                Object[] lhsProperties = new Object[lhs.Columns[i].ExtendedProperties.Count];
                lhs.Columns[i].ExtendedProperties.CopyTo(lhsProperties, 0);

                Object[] rhsProperties = new Object[rhs.Columns[i].ExtendedProperties.Count];
                rhs.Columns[i].ExtendedProperties.CopyTo(rhsProperties, 0);

                Assert.Equal(lhsProperties.Length, rhsProperties.Length);

                for (Int32 epIndex = 0; epIndex < lhsProperties.Length; epIndex++)
                {
                    Assert.Equal(lhsProperties[epIndex], rhsProperties[epIndex]);
                }
            }

            Assert.Equal(lhs.PrimaryKey.Length, rhs.PrimaryKey.Length);

            if (lhs.PrimaryKey.Length > 0)
            {
                for (Int32 i = 0; i < lhs.PrimaryKey.Length; i++)
                {
                    Assert.Equal(lhs.PrimaryKey[i].AllowDBNull, rhs.PrimaryKey[i].AllowDBNull);
                    Assert.Equal(lhs.PrimaryKey[i].AutoIncrement, rhs.PrimaryKey[i].AutoIncrement);
                    Assert.Equal(lhs.PrimaryKey[i].AutoIncrementSeed, rhs.PrimaryKey[i].AutoIncrementSeed);
                    Assert.Equal(lhs.PrimaryKey[i].AutoIncrementStep, rhs.PrimaryKey[i].AutoIncrementStep);
                    Assert.Equal(lhs.PrimaryKey[i].Caption, rhs.PrimaryKey[i].Caption);
                    Assert.Equal(lhs.PrimaryKey[i].ColumnMapping, rhs.PrimaryKey[i].ColumnMapping);
                    Assert.Equal(lhs.PrimaryKey[i].ColumnName, rhs.PrimaryKey[i].ColumnName);
                    Assert.Equal(lhs.PrimaryKey[i].DataType, rhs.PrimaryKey[i].DataType);
                    Assert.Equal(lhs.PrimaryKey[i].DateTimeMode, rhs.PrimaryKey[i].DateTimeMode);
                    Assert.Equal(lhs.PrimaryKey[i].DefaultValue, rhs.PrimaryKey[i].DefaultValue);
                    Assert.Equal(lhs.PrimaryKey[i].Expression, rhs.PrimaryKey[i].Expression);
                    Assert.Equal(lhs.PrimaryKey[i].MaxLength, rhs.PrimaryKey[i].MaxLength);
                    Assert.Equal(lhs.PrimaryKey[i].Namespace, rhs.PrimaryKey[i].Namespace);
                    Assert.Equal(lhs.PrimaryKey[i].Ordinal, rhs.PrimaryKey[i].Ordinal);
                    Assert.Equal(lhs.PrimaryKey[i].Prefix, rhs.PrimaryKey[i].Prefix);
                    Assert.Equal(lhs.PrimaryKey[i].ReadOnly, rhs.PrimaryKey[i].ReadOnly);
                    Assert.Equal(lhs.PrimaryKey[i].Unique, rhs.PrimaryKey[i].Unique);
                    Assert.Equal(lhs.PrimaryKey[i].ExtendedProperties.Count,
                                    rhs.PrimaryKey[i].ExtendedProperties.Count);

                    Object[] lhsProperties = new Object[lhs.PrimaryKey[i].ExtendedProperties.Count];
                    lhs.PrimaryKey[i].ExtendedProperties.CopyTo(lhsProperties, 0);

                    Object[] rhsProperties = new Object[rhs.PrimaryKey[i].ExtendedProperties.Count];
                    rhs.PrimaryKey[i].ExtendedProperties.CopyTo(rhsProperties, 0);

                    for (Int32 epIndex = 0; epIndex < lhsProperties.Length; epIndex++)
                    {
                        Assert.Equal(lhsProperties[epIndex], rhsProperties[epIndex]);
                    }
                }
            }

            for (Int32 i = 0; i < lhs.Constraints.Count; i++)
            {
                Assert.Equal(lhs.Constraints[i].ConstraintName, rhs.Constraints[i].ConstraintName);

                Object[] lhsProperties = new Object[lhs.Constraints[i].ExtendedProperties.Count];
                lhs.Columns[i].ExtendedProperties.CopyTo(lhsProperties, 0);

                Object[] rhsProperties = new Object[rhs.Constraints[i].ExtendedProperties.Count];
                rhs.Columns[i].ExtendedProperties.CopyTo(rhsProperties, 0);

                for (Int32 epIndex = 0; epIndex < lhsProperties.Length; epIndex++)
                {
                    Assert.Equal(lhsProperties[epIndex], rhsProperties[epIndex]);
                }
            }
        }
    }
}