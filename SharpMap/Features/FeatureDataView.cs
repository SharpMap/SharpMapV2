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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using SharpMap.Geometries;

namespace SharpMap.Features
{
    public class FeatureDataView : DataView, IEnumerable<FeatureDataRow>
    {
        #region Nested Types

        private delegate void SetDataViewManagerDelegate(FeatureDataView view, DataViewManager dataViewManager);

        private delegate void SetLockedDelegate(DataView view, bool locked);

        private delegate void SetIndex2Delegate(
            FeatureDataView view, string newSort, DataViewRowState dataViewRowState, object expression, bool fireEvent);

        #endregion

        #region Type Fields

        private static readonly SetDataViewManagerDelegate _setDataViewManager;
        private static readonly SetLockedDelegate _setLocked;
        private static readonly SetIndex2Delegate _setIndex2;

        #endregion

        #region Static Constructor

        static FeatureDataView()
        {
            // Create method to set DataViewManager
            _setDataViewManager = GenerateSetDataViewManagerDelegate();

            // Create method to set locked status
            _setLocked = GenerateSetLockedDelegate();

            // Create a delegate to the SetIndex2 method
            _setIndex2 = GenerateSetIndex2Delegate();
        }

        #endregion

        #region Object Fields

        private BoundingBox _visibleRegion;

        #endregion

        #region Object Constructors

        public FeatureDataView(FeatureDataTable table)
            : base(table)
        {
        }

        internal FeatureDataView(FeatureDataTable table, bool locked)
            : this(table)
        {
            _setLocked(this, locked);
        }

        #endregion

        /// <summary>
        /// Gets the DataViewManager which is managing this view's settings.
        /// </summary>
        public new FeatureDataViewManager DataViewManager
        {
            get { return base.DataViewManager as FeatureDataViewManager; }
        }

        /// <summary>
        /// Gets or sets the visible region encompassed by this view.
        /// </summary>
        public BoundingBox VisibleRegion
        {
            get { return _visibleRegion; }
            set { _visibleRegion = value; }
        }

        #region IEnumerable<FeatureDataRow> Members

        public new IEnumerator<FeatureDataRow> GetEnumerator()
        {
            IEnumerator e = base.GetEnumerator();

            while (e.MoveNext())
            {
                FeatureDataRow feature = e.Current as FeatureDataRow;
                yield return feature;
            }
        }

        #endregion

        internal void SetDataViewManager(FeatureDataViewManager featureDataViewManager)
        {
            // Call the delegate we wired up to bypass the normally inaccessible base class method
            _setDataViewManager(this, featureDataViewManager);
        }

        internal void SetIndex2(string newSort, DataViewRowState dataViewRowState, object dataExpression, bool fireEvent)
        {
            // Call the delegate we wired up to bypass the normally inaccessible base class method
            _setIndex2(this, newSort, dataViewRowState, dataExpression, fireEvent);
        }

        #region Private static helper methods

        private static SetIndex2Delegate GenerateSetIndex2Delegate()
        {
            // We need to generate a delegate based on the function pointer, 
            // since the SetIndex2 method requires a parameter of type DataExpression,
            // which is internal to System.Data, so we can't do LCG with DynamicMethod
            MethodInfo setIndex2Info = typeof (DataView).GetMethod(
                "SetIndex2", BindingFlags.NonPublic | BindingFlags.Instance);
            ConstructorInfo setIndexDelegateCtor = typeof (SetIndex2Delegate)
                .GetConstructor(new Type[] {typeof (object), typeof (IntPtr)});
            IntPtr setIndex2Pointer = setIndex2Info.MethodHandle.GetFunctionPointer();
            return (SetIndex2Delegate) setIndexDelegateCtor.Invoke(new Object[] {null, setIndex2Pointer});
        }

        private static SetLockedDelegate GenerateSetLockedDelegate()
        {
            // Use LCG to create a set accessor to the DataView.locked field
            DynamicMethod setLockedMethod = new DynamicMethod("set_locked_DynamicMethod",
                                                              null, new Type[] {typeof (DataView), typeof (bool)},
                                                              typeof (DataView));

            ILGenerator il = setLockedMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            FieldInfo lockedField = typeof (DataView).GetField("locked", BindingFlags.Instance | BindingFlags.NonPublic);
            il.Emit(OpCodes.Stfld, lockedField);
            il.Emit(OpCodes.Ret);

            return setLockedMethod.CreateDelegate(typeof (SetLockedDelegate)) as SetLockedDelegate;
        }

        private static SetDataViewManagerDelegate GenerateSetDataViewManagerDelegate()
        {
            // Use LCG to create a delegate to the internal DataView.SetDataViewManager method
            DynamicMethod setDataViewManagerMethod = new DynamicMethod("set_DataViewManager_DynamicMethod",
                                                                       null,
                                                                       new Type[]
                                                                           {
                                                                               typeof (FeatureDataView),
                                                                               typeof (DataViewManager)
                                                                           }, typeof (DataView));

            ILGenerator il = setDataViewManagerMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            MethodInfo setDataViewManagerInfo = typeof (DataView).GetMethod("SetDataViewManager",
                                                                            BindingFlags.Instance |
                                                                            BindingFlags.NonPublic, null,
                                                                            new Type[] {typeof (DataViewManager)}, null);
            il.Emit(OpCodes.Call, setDataViewManagerInfo);

            return setDataViewManagerMethod.CreateDelegate(typeof (SetDataViewManagerDelegate))
                   as SetDataViewManagerDelegate;
        }

        #endregion
    }
}