using GeoAPI.CoordinateSystems;
using GeoAPI.DataStructures;

namespace SharpMap.Utilities.SridUtility
{
    public abstract class SridMapStrategyBase :
        IStrategy<int?, ICoordinateSystem>,
        IStrategy<ICoordinateSystem, int?>,
        IStrategy<string, int?>,
        IStrategy<int?, string>,
        IStrategy<ICoordinateSystem, string>
    {
        protected SridMapStrategyBase(int priority, ICoordinateSystemFactory coordSysFactory)
        {
            Priority = priority;
            CoordinateSystemFactory = coordSysFactory;
        }

        public ICoordinateSystemFactory CoordinateSystemFactory { get; protected set; }

        #region IStrategy<ICoordinateSystem,int?> Members

        /// <summary>
        /// Attempts to locate the integer id from the Coordinate System
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public abstract bool Process(ICoordinateSystem input, out int? output);

        #endregion

        #region IStrategy<ICoordinateSystem,string> Members

        /// <summary>
        /// attempts to return the wkt representing the coordinate system
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public virtual bool Process(ICoordinateSystem input, out string output)
        {
            if (input == null)
            {
                output = string.Empty;
                return false;
            }
            output = input.Wkt;
            return true;
        }

        #endregion

        #region IStrategy<int?,ICoordinateSystem> Members

        /// <summary>
        /// attempts to locate the coordinate system from the integer id
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public abstract bool Process(int? input, out ICoordinateSystem output);

        #endregion

        #region IStrategy<int?,string> Members

        /// <summary>
        /// attempts to get the wkt representing the coordinate system from the integer id
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public virtual bool Process(int? input, out string output)
        {
            ICoordinateSystem cs;
            if (Process(input, out cs))
            {
                return Process(cs, out output);
            }
            output = string.Empty;
            return false;
        }

        #endregion

        #region IStrategy<string,int?> Members

        /// <summary>
        /// attempts to locate the integer id from the wkt representing the coordinate system
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public virtual bool Process(string input, out int? output)
        {
            return Process(CoordinateSystemFactory.CreateFromWkt(input), out output);
        }

        #endregion

        #region IStrategy<int?,ICoordinateSystem> Members

        public int Priority
        {
            get;
            protected set;
        }

        #endregion
    }
}