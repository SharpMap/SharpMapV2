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

namespace SharpMap.Indexing.RTree
{
	/// <summary>
	/// Encapsulates a heuristic to determine when an index is restructured.
	/// </summary>
	public struct RestructuringHuristic
	{
		private readonly RestructureOpportunity _whenToRestructure;
		private readonly double _period;
		private readonly double _executionPercentage;

		/// <summary>
		/// Creates a new restructuring heuristic with no period.
		/// </summary>
		/// <param name="whenToRestructure">A RestructureOpportunity enumeration value.</param>
		/// <param name="executionPercentage">
		/// A percentage value (0 to 100) which specifies the percent 
		/// of the time that a restructure could run that it actually is run.
		/// </param>
		public RestructuringHuristic(RestructureOpportunity whenToRestructure, double executionPercentage)
			: this(whenToRestructure, executionPercentage, -1)
		{
		}

		/// <summary>
		/// Creates a new restructuring heuristic with no period.
		/// </summary>
		/// <param name="whenToRestructure">A RestructureOpportunity enumeration value.</param>
		/// <param name="executionPercentage">
		/// A percentage value (0 to 100) which specifies the percent 
		/// of the time that a restructure could run that it actually is run.
		/// </param>
		/// <param name="period">A value in milliseconds after which a restructure is triggered.</param>
		public RestructuringHuristic(RestructureOpportunity whenToRestructure, double executionPercentage, double period)
		{
			_whenToRestructure = whenToRestructure;
			_executionPercentage = executionPercentage;
			_period = period;
		}

		/// <summary>
		/// Gets the heuristic's RestructureOpportunity enumeration value.
		/// </summary>
		public RestructureOpportunity WhenToRestructure
		{
			get { return _whenToRestructure; }
		}

		/// <summary>
		/// Gets a percentage value (0 to 100) which specifies the percent 
		/// of the time that a restructure could run that it actually is run.
		/// </summary>
		public double ExecutionPercentage
		{
			get { return _executionPercentage; }
		}

		/// <summary>
		/// Gets the period in milliseconds after which a restructure is triggered.
		/// </summary>
		public double Period
		{
			get { return _period; }
		}
	}
}
