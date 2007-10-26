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
using System.Threading;
using NPack;
using SharpMap.Utilities;

namespace SharpMap.Indexing.RTree
{
	/// <summary>
	/// A dynamic R-Tree which periodically restructures in order to provide more optimal indexing.
	/// </summary>
	/// <typeparam name="TValue">The type of the value used in the entries.</typeparam>
	public sealed class SelfOptimizingDynamicSpatialIndex<TValue> : DynamicRTree<TValue>
	{
		#region Instance fields
		private readonly MersenneTwister _executionProbability = new MersenneTwister();
		private readonly IIndexRestructureStrategy _restructureStrategy;
		private RestructuringHuristic _restructuringHeuristic;
		private readonly EventWaitHandle _userIdleEvent;
		private readonly EventWaitHandle _machineIdleEvent;
		private readonly EventWaitHandle _terminateEvent;
		private readonly Thread _restructureThread;
		private readonly Int32 _periodMilliseconds;
		private Int32 _terminating = 0;
		private Int32 _insertedEntriesSinceLastRestructure = 0;
		private readonly IdleMonitor _idleMonitor;
		#endregion
		
		#region Object construction and disposal

		#region Constructor
		/// <summary>
		/// Creates a new SelfOptimizingDynamicSpatialIndex.
		/// </summary>
		/// <param name="restructureStrategy">The strategy used to restructure the index.</param>
		/// <param name="restructureHeuristic">The heuristic to control when the index is restructured.</param>
		/// <param name="insertStrategy">The strategy used to insert entries into the index.</param>
		/// <param name="nodeSplitStrategy">The strategy used to split index nodes.</param>
		/// <param name="indexHeuristic">A heuristic used to balance the index for optimum efficiency.</param>
		/// <param name="idleMonitor">A monitor to determine idle conditions on the executing machine.</param>
		public SelfOptimizingDynamicSpatialIndex(IIndexRestructureStrategy restructureStrategy,
												 RestructuringHuristic restructureHeuristic,
												 IEntryInsertStrategy<RTreeIndexEntry<TValue>> insertStrategy,
												 INodeSplitStrategy nodeSplitStrategy,
												 DynamicRTreeBalanceHeuristic indexHeuristic, IdleMonitor idleMonitor)
			: base(insertStrategy, nodeSplitStrategy, indexHeuristic)
		{
			_periodMilliseconds = restructureHeuristic.WhenToRestructure == RestructureOpportunity.Periodic
									? (Int32)(restructureHeuristic.Period / 1000.0)
									: -1;

			_restructureStrategy = restructureStrategy;
			_restructuringHeuristic = restructureHeuristic;
			_idleMonitor = idleMonitor;

			_userIdleEvent = new AutoResetEvent(false);
			_machineIdleEvent = new AutoResetEvent(false);
			_terminateEvent = new ManualResetEvent(false);

			if (restructureHeuristic.WhenToRestructure != RestructureOpportunity.None)
			{
				_restructureThread = new Thread(doRestructure);
				_restructureThread.Start();
			}

			RestructureOpportunity idle = RestructureOpportunity.OnMachineIdle | RestructureOpportunity.OnUserIdle;

			if (((Int32)(restructureHeuristic.WhenToRestructure & idle)) > 0)
			{
				if(_idleMonitor == null) 
				{
					throw new ArgumentNullException("idleMonitor", 
						"If the restructuring heuristic has a value of anything but " +
						"None for WhenToRestructure, the idleMonitor cannot be null.");
				}

				_idleMonitor.UserIdle += _idleMonitor_UserIdle;
				_idleMonitor.UserBusy += _idleMonitor_UserBusy;
				_idleMonitor.MachineIdle += _idleMonitor_MachineIdle;
				_idleMonitor.MachineBusy += _idleMonitor_MachineBusy;
			}
		}

		void _idleMonitor_MachineBusy(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		void _idleMonitor_MachineIdle(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		void _idleMonitor_UserBusy(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		void _idleMonitor_UserIdle(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region Dispose pattern

		~SelfOptimizingDynamicSpatialIndex()
		{
			Dispose(false);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			}

			terminateThreads();
			_terminateEvent.Close();

			base.Dispose(disposing);
		}

		#endregion

		#endregion
		
		#region Public properties

		/// <summary>
		/// Gets the heuristic used to specify when the index is restructured.
		/// </summary>
		public RestructuringHuristic RestructuringHeuristic
		{
			get { return _restructuringHeuristic; }
		} 
		#endregion

		#region Overrides

		public override void Insert(RTreeIndexEntry<TValue> entry)
		{
			base.Insert(entry);
			Interlocked.Increment(ref _insertedEntriesSinceLastRestructure);
		} 
		#endregion

		#region Private helper methods

		private bool restructureOnUserIdle()
		{
			return (_restructuringHeuristic.WhenToRestructure & RestructureOpportunity.OnUserIdle) 
				!= RestructureOpportunity.None;
		}

		private bool restructureOnMachineIdle()
		{
			return (_restructuringHeuristic.WhenToRestructure & RestructureOpportunity.OnMachineIdle) 
				!= RestructureOpportunity.None;
		}

		private void doRestructure()
		{
			WaitHandle[] events = new WaitHandle[] { _userIdleEvent, _machineIdleEvent, _terminateEvent };

			while (_terminating == 0)
			{
				// Wait on restructure or terminate events
				WaitHandle.WaitAny(events, _periodMilliseconds, false);

				if (Interlocked.CompareExchange(ref _terminating, 0, 0) == 1)
				{
					return;
				}

				// If there have been no inserts since last restructure, we don't need another restructure
				if (Interlocked.CompareExchange(ref _insertedEntriesSinceLastRestructure, 0, 0) == 0)
				{
					continue;
				}

				// The restructure only takes place if it exceeds the probability for restructure
				// as specified in the restructuring heuristic
				if (_restructuringHeuristic.ExecutionPercentage >= _executionProbability.NextDouble() * 100.0)
				{
					_restructureStrategy.RestructureNode(Root);
					Interlocked.Exchange(ref _insertedEntriesSinceLastRestructure, 0);
				}
			}
		}

		private void terminateThreads()
		{
			if (_restructureThread == null)
			{
				return;
			}

#pragma warning disable 420
			Interlocked.Increment(ref _terminating);
#pragma warning restore 420

			_terminateEvent.Set();

			if (_restructureThread.IsAlive && !_restructureThread.Join(5000))
			{
				_restructureThread.Abort();
			}
		}
		#endregion
	}
}