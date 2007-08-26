// Copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
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
using System.Collections.Generic;

namespace SharpMap.Utilities
{
	/// <summary>
	/// Represents a tolerance used in comparing <see cref="float"/> 
	/// and <see cref="double"/> floating point values.
	/// </summary>
	[Serializable]
	public class Tolerance
	{
		/// <summary>
		/// The default tolerance: 0.000000001 (1E-9).
		/// </summary>
		public static readonly double DefaultToleranceValue = 1E-9;

		#region Instance Fields

		private double _toleranceValue = DefaultToleranceValue;

		#endregion

		#region Static Fields

		private static readonly object _globalToleranceSetSync = new object();
		private static Tolerance _globalTolerance = new Tolerance();

		private static readonly Dictionary<RuntimeTypeHandle, Tolerance> _toleranceRegistry
			= new Dictionary<RuntimeTypeHandle, Tolerance>();

		#endregion

		#region Static Members

		/// <summary>
		/// The globally accessible tolerance. Used to change the tolerance computation
		/// for the entire AppDomain.
		/// </summary>
		public static Tolerance Global
		{
			get { return _globalTolerance; }
			set
			{
				lock (_globalToleranceSetSync)
				{
					_globalTolerance = value;
				}
			}
		}

		/// <summary>
		/// Registers a tolerance to be used for a specific type.
		/// </summary>
		/// <typeparam name="TValue">
		/// The type to register a tolerance for.
		/// </typeparam>
		/// <param name="tolerance">The tolerance to register.</param>
		public static void RegisterTolerance<TValue>(Tolerance tolerance)
		{
			RuntimeTypeHandle key = typeof (TValue).TypeHandle;

			if (tolerance == null)
			{
				_toleranceRegistry.Remove(key);
			}
			else
			{
				_toleranceRegistry[key] = tolerance;
			}
		}

		/// <summary>
		/// Unregisters the tolerance value for a specific type.
		/// </summary>
		/// <typeparam name="TValue">
		/// The type to unregister a tolerance for.
		/// </typeparam>
		public static void UnregisterTolerance<TValue>()
		{
			RuntimeTypeHandle key = typeof (TValue).TypeHandle;
			_toleranceRegistry.Remove(key);
		}

		/// <summary>
		/// Compares two values for equality, using <typeparamref name="TValue"/>
		/// to lookup the tolerance registered for that type.
		/// </summary>
		/// <typeparam name="TValue">The type which may have a <see cref="Tolerance"/> value registered.</typeparam>
		/// <param name="leftHand">The left hand value in the comparison.</param>
		/// <param name="rightHand">The right hand value in the comparison.</param>
		/// <returns>
		/// True if the values are equal within the tolerance registered for <typeparamref name="TValue"/>
		/// or the <see cref="Global"/> tolerance if the type isn't registered; false otherwise.
		/// </returns>
		public static bool Equal<TValue>(double leftHand, double rightHand)
		{
			Tolerance t;
			if (_toleranceRegistry.TryGetValue(typeof (TValue).TypeHandle, out t))
			{
				return t.Equal(leftHand, rightHand);
			}
			else
			{
				return Global.Equal(leftHand, rightHand);
			}
		}

		/// <summary>
		/// Compares if the <paramref name="leftHand">left hand</paramref> parameter is greater
		/// than the <paramref name="rightHand">right hand</paramref> parameter, using 
		/// <typeparamref name="TValue"/> to lookup the tolerance registered for that type.
		/// </summary>
		/// <typeparam name="TValue">The type which may have a <see cref="Tolerance"/> value registered.</typeparam>
		/// <param name="leftHand">The left hand value in the comparison.</param>
		/// <param name="rightHand">The right hand value in the comparison.</param>
		/// <returns>
		/// True if the left hand parameter is greater than the right hand parameter 
		/// within the tolerance registered for <typeparamref name="TValue"/>
		/// or the <see cref="Global"/> tolerance if the type isn't registered; false otherwise.
		/// </returns>
		public static bool Greater<TValue>(double leftHand, double rightHand)
		{
			Tolerance t;
			if (_toleranceRegistry.TryGetValue(typeof (TValue).TypeHandle, out t))
			{
				return t.Greater(leftHand, rightHand);
			}
			else
			{
				return Global.Greater(leftHand, rightHand);
			}
		}

		/// <summary>
		/// Compares if the <paramref name="leftHand">left hand</paramref> parameter is greater
		/// than or equal to the <paramref name="rightHand">right hand</paramref> parameter, using 
		/// <typeparamref name="TValue"/> to lookup the tolerance registered for that type.
		/// </summary>
		/// <typeparam name="TValue">The type which may have a <see cref="Tolerance"/> value registered.</typeparam>
		/// <param name="leftHand">The left hand value in the comparison.</param>
		/// <param name="rightHand">The right hand value in the comparison.</param>
		/// <returns>
		/// True if the left hand parameter is greater than or equal to the right hand parameter 
		/// within the tolerance registered for <typeparamref name="TValue"/>
		/// or the <see cref="Global"/> tolerance if the type isn't registered; false otherwise.
		/// </returns>
		public static bool GreaterOrEqual<TValue>(double leftHand, double rightHand)
		{
			Tolerance t;
			if (_toleranceRegistry.TryGetValue(typeof (TValue).TypeHandle, out t))
			{
				return t.GreaterOrEqual(leftHand, rightHand);
			}
			else
			{
				return Global.GreaterOrEqual(leftHand, rightHand);
			}
		}

		/// <summary>
		/// Compares if the <paramref name="leftHand">left hand</paramref> parameter is less
		/// than the <paramref name="rightHand">right hand</paramref> parameter, using 
		/// <typeparamref name="TValue"/> to lookup the tolerance registered for that type.
		/// </summary>
		/// <typeparam name="TValue">The type which may have a <see cref="Tolerance"/> value registered.</typeparam>
		/// <param name="leftHand">The left hand value in the comparison.</param>
		/// <param name="rightHand">The right hand value in the comparison.</param>
		/// <returns>
		/// True if the left hand parameter is less than the right hand parameter 
		/// within the tolerance registered for <typeparamref name="TValue"/>
		/// or the <see cref="Global"/> tolerance if the type isn't registered; false otherwise.
		/// </returns>
		public static bool Less<TValue>(double leftHand, double rightHand)
		{
			Tolerance t;
			if (_toleranceRegistry.TryGetValue(typeof (TValue).TypeHandle, out t))
			{
				return t.Less(leftHand, rightHand);
			}
			else
			{
				return Global.Less(leftHand, rightHand);
			}
		}

		/// <summary>
		/// Compares if the <paramref name="leftHand">left hand</paramref> parameter is less
		/// than or equal to the <paramref name="rightHand">right hand</paramref> parameter, using 
		/// <typeparamref name="TValue"/> to lookup the tolerance registered for that type.
		/// </summary>
		/// <typeparam name="TValue">The type which may have a <see cref="Tolerance"/> value registered.</typeparam>
		/// <param name="leftHand">The left hand value in the comparison.</param>
		/// <param name="rightHand">The right hand value in the comparison.</param>
		/// <returns>
		/// True if the left hand parameter is less than or equal to the right hand parameter 
		/// within the tolerance registered for <typeparamref name="TValue"/>
		/// or the <see cref="Global"/> tolerance if the type isn't registered; false otherwise.
		/// </returns>
		public static bool LessOrEqual<TValue>(double leftHand, double rightHand)
		{
			Tolerance t;
			if (_toleranceRegistry.TryGetValue(typeof (TValue).TypeHandle, out t))
			{
				return t.LessOrEqual(leftHand, rightHand);
			}
			else
			{
				return Global.LessOrEqual(leftHand, rightHand);
			}
		}

		/// <summary>
		/// Compares the <paramref name="leftHand">left hand</paramref> parameter 
		/// to the <paramref name="rightHand">right hand</paramref> parameter, using 
		/// <typeparamref name="TValue"/> to lookup the tolerance registered for that type.
		/// </summary>
		/// <typeparam name="TValue">The type which may have a <see cref="Tolerance"/> value registered.</typeparam>
		/// <param name="leftHand">The left hand value in the comparison.</param>
		/// <param name="rightHand">The right hand value in the comparison.</param>
		/// <returns>
		/// 0 if the parameters differ within the tolerance registered for <typeparamref name="TValue"/>
		/// or the <see cref="Global"/> tolerance if the type isn't registered;
		/// 1 if <paramref name="leftHand"/> is greater (right hand is less) within the tolerance;
		/// -1 if <paramref name="rightHand"/> is greater (left hand is less) within the tolerance.
		/// </returns>
		public static int Compare<TValue>(double leftHand, double rightHand)
		{
			Tolerance t;
			if (_toleranceRegistry.TryGetValue(typeof (TValue).TypeHandle, out t))
			{
				return t.Compare(leftHand, rightHand);
			}
			else
			{
				return Global.Compare(leftHand, rightHand);
			}
		}

		#endregion

		#region Instance Members

		/// <summary>
		/// Initializes a new <see cref="Tolerance"/> with the value of 
		/// <see cref="DefaultToleranceValue"/>.
		/// </summary>
		public Tolerance()
		{
		}

		/// <summary>
		/// Initializes a new <see cref="Tolerance"/> with the value <paramref name="value"/>.
		/// </summary>
		/// <param name="value">The value of the tolerance.</param>
		public Tolerance(double value)
		{
			_toleranceValue = Math.Abs(value);
		}

		/// <summary>
		/// Gets the value of the tolerance.
		/// </summary>
		public double Value
		{
			get { return _toleranceValue; }
		}

		/// <summary>
		/// Compares if the left value and right value are equal using the 
		/// <see cref="Value">tolerance value</see>.
		/// </summary>
		/// <param name="leftHand">Left hand side of comparison.</param>
		/// <param name="rightHand">Right hand side of comparison.</param>
		/// <returns>
		/// True if <paramref name="leftHand"/> is equal to <see cref="rightHand"/> 
		/// (within the tolerance), false otherwise.
		/// </returns>
		public bool Equal(double leftHand, double rightHand)
		{
			return Compare(leftHand, rightHand) == 0;
		}

		/// <summary>
		/// Compares if the left value is greater than using the 
		/// <see cref="Value">tolerance value</see>.
		/// </summary>
		/// <param name="leftHand">Left hand side of comparison.</param>
		/// <param name="rightHand">Right hand side of comparison.</param>
		/// <returns>
		/// True if <paramref name="leftHand"/> is greater than <see cref="rightHand"/> 
		/// (within the tolerance), false otherwise.
		/// </returns>
		public bool Greater(double leftHand, double rightHand)
		{
			return Compare(leftHand, rightHand) == 1;
		}

		/// <summary>
		/// Compares if the left value is greater than or equal using the 
		/// <see cref="Value">tolerance value</see>.
		/// </summary>
		/// <param name="leftHand">Left hand side of comparison.</param>
		/// <param name="rightHand">Right hand side of comparison.</param>
		/// <returns>
		/// True if <paramref name="leftHand"/> is greater than or equal to 
		/// <see cref="rightHand"/> (within the tolerance), false otherwise.
		/// </returns>
		public bool GreaterOrEqual(double leftHand, double rightHand)
		{
			int comparison = Compare(leftHand, rightHand);
			return comparison == 0 || comparison == 1;
		}

		/// <summary>
		/// Compares if the left value is less than using the 
		/// <see cref="Value">tolerance value</see>.
		/// </summary>
		/// <param name="leftHand">Left hand side of comparison.</param>
		/// <param name="rightHand">Right hand side of comparison.</param>
		/// <returns>
		/// True if <paramref name="leftHand"/> is less than <see cref="rightHand"/> 
		/// (within the tolerance), false otherwise.
		/// </returns>
		public bool Less(double leftHand, double rightHand)
		{
			return Compare(leftHand, rightHand) == -1;
		}

		/// <summary>
		/// Compares if the left value is less than or equal using the 
		/// <see cref="Value">tolerance value</see>.
		/// </summary>
		/// <param name="leftHand">Left hand side of comparison.</param>
		/// <param name="rightHand">Right hand side of comparison.</param>
		/// <returns>
		/// True if <paramref name="leftHand"/> is less than or equal to 
		/// <see cref="rightHand"/> (within the tolerance), false otherwise.
		/// </returns>
		public bool LessOrEqual(double leftHand, double rightHand)
		{
			int comparison = Compare(leftHand, rightHand);
			return comparison == 0 || comparison == -1;
		}

		/// <summary>
		/// Compares two values using the <see cref="Value">tolerance value</see>.
		/// </summary>
		/// <param name="leftHand">Left hand side of comparison.</param>
		/// <param name="rightHand">Right hand side of comparison.</param>
		/// <returns>
		/// 0 if the parameters differ by the tolerance value or less, 
		/// 1 if <paramref name="leftHand"/> is greater,
		/// -1 if <paramref name="rightHand"/> is greater.
		/// </returns>
		public int Compare(double leftHand, double rightHand)
		{
			double difference = leftHand - rightHand;

			if (Value >= Math.Abs(difference))
			{
				return 0;
			}
			else
			{
				return difference > 0 ? 1 : -1;
			}
		}

		#endregion
	}
}