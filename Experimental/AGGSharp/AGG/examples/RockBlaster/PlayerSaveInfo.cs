using System;
using AGG.Rendering;
using NPack.Interfaces;
using Reflexive.Game;


namespace RockBlaster
{
    public class PlayerSaveInfo<T> : GameObject<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        [GameData("PlayerScore")]
        public double Score;

        static PlayerSaveInfo<T> s_TestInfo = new PlayerSaveInfo<T>();

        public static PlayerSaveInfo<T> GetPlayerInfo()
        {
            return s_TestInfo;
        }

        #region GameObjectStuff
        public PlayerSaveInfo()
        {
        }
        public static new GameObject<T> Load(String PathName)
        {
            return GameObject<T>.Load(PathName);
        }
        #endregion

        public void Draw(RendererBase<T> currentRenderer)
        {
        }

        public void Update(double NumSecondsPassed)
        {
        }
    }
}
