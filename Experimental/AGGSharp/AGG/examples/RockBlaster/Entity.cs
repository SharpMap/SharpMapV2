/*
 * Created by SharpDevelop.
 * User: Lars Brubaker
 * Date: 10/13/2007
 * Time: 12:08 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using AGG.Rendering;
using AGG.Transform;
using NPack.Interfaces;
using Reflexive.Game;
using NPack;

namespace RockBlaster
{
    /// <summary>
    /// Description of Entity.
    /// </summary>
    public abstract class Entity<T> : GameObject<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        #region GameObjectStuff
        public Entity()
        {
        }
        public static new GameObject<T> Load(String PathName)
        {
            return GameObject<T>.Load(PathName);
        }
        #endregion

        private static int s_GameHeight;
        private static int s_GameWidth;

        protected T m_Radius;
        protected T m_Damage = M.Zero<T>();
        protected T m_MaxDamage = M.One<T>();

        [GameDataVector2DAttributeDoubleComponent("Position")]
        protected IVector<T> m_Position;
        protected IVector<T> m_Velocity;

        public T MaxDamage
        {
            get
            {
                return m_MaxDamage;
            }
        }

        public T Damage
        {
            get
            {
                return m_Damage;
            }
            set
            {
                m_Damage = value;
            }
        }

        public static int GameWidth
        {
            get
            {
                return s_GameWidth;
            }
            set
            {
                s_GameWidth = value;
            }
        }

        public static int GameHeight
        {
            get
            {
                return s_GameHeight;
            }
            set
            {
                s_GameHeight = value;
            }
        }

        public Entity(double radius)
            : this(M.New<T>(radius))
        {

        }

        public Entity(T radius)
        {
            m_Radius = radius;
            m_Velocity = MatrixFactory<T>.CreateVector2D(60, 120);
        }

        public IVector<T> Position
        {
            get
            {
                return m_Position;
            }
            set
            {
                m_Position = value;
            }
        }

        public T Radius
        {
            get
            {
                return m_Radius;
            }
        }

        public virtual void Update(double numSecondsPassed)
        {
            m_Position.Add(m_Velocity.Multiply(numSecondsPassed));
            if (m_Position[0].GreaterThan(GameWidth))
            {
                m_Position[0].SubtractEquals(GameWidth);
            }
            if (m_Position[0].LessThan(0))
            {
                m_Position[0].AddEquals(GameWidth);
            }
            if (m_Position[1].GreaterThan(GameHeight))
            {
                m_Position[1].SubtractEquals(GameHeight);
            }
            if (m_Position[1].LessThan(0))
            {
                m_Position[1].AddEquals(GameHeight);
            }
        }

        public virtual void Draw(RendererBase<T> destRenderer)
        {
            DoDraw(destRenderer);

            MirrorAsNeeded(destRenderer);
        }

        protected abstract void DoDraw(RendererBase<T> destRenderer);

        private void MirrorOnY(RendererBase<T> destRenderer)
        {
            if (Position[1].LessThan(Radius))
            {
                IVector<T> oldPosition = Position;
                oldPosition[1].AddEquals(GameHeight);
                Position = oldPosition;
                this.DoDraw(destRenderer);
            }
            else if (Position[1].LessThan(M.New<T>(GameHeight).Subtract(Radius)))
            {
                IVector<T> oldPosition = Position;
                oldPosition[1].SubtractEquals(GameHeight);
                Position = oldPosition;
                this.DoDraw(destRenderer);
            }
        }

        public virtual T GiveDamage()
        {
            return M.Zero<T>();
        }

        private void MirrorOnX(RendererBase<T> destRenderer)
        {
            if (Position[0].LessThan(Radius))
            {
                IVector<T> oldPosition = Position;
                oldPosition[0].AddEquals(GameWidth);
                Position = oldPosition;
                DoDraw(destRenderer);
            }
            else if (Position[0].GreaterThan(M.New<T>(GameWidth).Subtract(Radius)))
            {
                IVector<T> oldPosition = Position;
                oldPosition[0].SubtractEquals(GameWidth);
                Position = oldPosition;
                DoDraw(destRenderer);
            }
        }

        public virtual void TakeDamage(T DamageToTake)
        {
            Damage = Damage.Add(DamageToTake);
        }

        public virtual void Destroying()
        {

        }

        public void MirrorAsNeeded(RendererBase<T> destRenderer)
        {
            IVector<T> oldPosition = Position;
            MirrorOnX(destRenderer);
            MirrorOnY(destRenderer);
            MirrorOnX(destRenderer);
            Position = oldPosition;
        }
    }
}
