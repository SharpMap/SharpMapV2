using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SharpMap.Geometries;
using System.Reflection.Emit;
using System.Reflection;

namespace SharpMap
{
    public class FeatureDataView : DataView, IEnumerable<FeatureDataRow>
    {
        #region Nested Types
        private delegate void SetDataViewManagerDelegate(FeatureDataView view, DataViewManager dataViewManager);
        private delegate void SetLockedDelegate(FeatureDataView view, bool locked);
        #endregion 

        #region Type Fields
        private static readonly SetDataViewManagerDelegate _setDataViewManager;
        private static readonly SetLockedDelegate _setLocked;
        #endregion

        #region Static Constructor
        static FeatureDataView()
        {
            // Create method to set DataViewManager
            DynamicMethod setDataViewManagerMethod = new DynamicMethod("SetDataViewManager_DynamicMethod",
                null, new Type[] { typeof(FeatureDataView), typeof(FeatureDataViewManager) }, typeof(DataView));

            ILGenerator il = setDataViewManagerMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, typeof(DataViewManager).GetMethod("SetDataViewManager",
                BindingFlags.Instance | BindingFlags.NonPublic, null,
                new Type[] { typeof(DataViewManager) }, null));

            _setDataViewManager = setDataViewManagerMethod.CreateDelegate(typeof(SetDataViewManagerDelegate))
                as SetDataViewManagerDelegate;

            // Create method to set locked status
            DynamicMethod setLockedMethod = new DynamicMethod("set_locked_DynamicMethod",
                null, new Type[] { typeof(FeatureDataView), typeof(bool) }, typeof(DataView));

            il = setLockedMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldfld, typeof(DataViewManager).GetField("locked",
                BindingFlags.Instance | BindingFlags.NonPublic));

            _setLocked = setLockedMethod.CreateDelegate(typeof(SetLockedDelegate)) as SetLockedDelegate;
        }
        #endregion

        #region Object Fields
        private BoundingBox _visibleRegion;
        #endregion

        #region Object Constructors
        public FeatureDataView(FeatureDataTable table)
            : base(table) { }

        public FeatureDataView(FeatureDataTable table, bool locked)
            : this(table)
        {
            _setLocked(this, locked);
        }
        #endregion

        public new FeatureDataViewManager DataViewManager
        {
            get { return base.DataViewManager as FeatureDataViewManager; }
        }

        public BoundingBox VisibleRegion
        {
            get { return _visibleRegion; }
            set 
            {
                _visibleRegion = value; 
            }
        }

        #region IEnumerable<FeatureDataRow> Members

        public new IEnumerator<FeatureDataRow> GetEnumerator()
        {
            IEnumerator e = base.GetEnumerator();

            while(e.MoveNext())
            {
                FeatureDataRow feature = e.Current as FeatureDataRow;
                yield return feature;
            }
        }

        #endregion

        internal void SetDataViewManager(FeatureDataViewManager featureDataViewManager)
        {
            _setDataViewManager(this, featureDataViewManager);
        }
    }
}
