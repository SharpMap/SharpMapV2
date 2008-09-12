using System;
using AGG.Color;
using AGG.Rendering;
using AGG.Transform;
using AGG.VertexSource;
using NPack.Interfaces;
using Reflexive.Game;
using NPack;

namespace RockBlaster
{
    public class BulletStyleSheet<T> : GameObject<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        [GameDataNumberAttribute("TurnRate")]
        public T TurnRate = M.New<T>(4);
        [GameDataNumberAttribute("Ship Thrust")]
        public T ThrustAcceleration = M.New<T>(10);
        [GameDataNumberAttribute("Friction")]
        public T Friction = M.New<T>(.99);
        [GameDataNumberAttribute("DamageOnCollide")]
        public T DamageOnCollide = M.One<T>();
        [GameDataNumberAttribute("NumSecondsToLive")]
        public T NumSecondsToLive = M.New<T>(2);
    }

    /// <summary>
    /// Description of Player.
    /// </summary>
    public class Bullet<T> : Entity<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        //[GameDataFromAssetTree("StyleSheet")];
        protected BulletStyleSheet<T> m_BulletStyleSheet;

        protected Ellipse<T> m_Shape;
        T m_NumSocendsUpdated;

        [GameDataNumberAttribute("Rotation")] // This is for save game
        protected T m_Rotation;

        public Bullet(IVector<T> position, IVector<T> Velocity)
            : base(3)
        {
            Position = position;
            m_Shape = new Ellipse<T>(0, 0, 3, 3);
            m_Velocity = Velocity;

            m_BulletStyleSheet = new BulletStyleSheet<T>();
        }

        protected override void DoDraw(RendererBase<T> destRenderer)
        {
            IAffineTransformMatrix<T> Final = MatrixFactory<T>.NewIdentity(VectorDimension.Two);
            Final.RotateAlong(MatrixFactory<T>.CreateVector2D(0, 0), m_Rotation.ToDouble());
            Final.Translate(m_Position);
            ConvTransform<T> TransformedShip = new ConvTransform<T>(m_Shape, Final);
            destRenderer.Render(TransformedShip, new RGBA_Bytes(.9, .4, .2, 1));
        }

        public override void Update(double numSecondsPassed)
        {
            base.Update(numSecondsPassed);
            m_NumSocendsUpdated.AddEquals(numSecondsPassed);
            if (m_NumSocendsUpdated.GreaterThan(m_BulletStyleSheet.NumSecondsToLive))
            {
                this.TakeDamage(MaxDamage);
            }
        }

        public override T GiveDamage()
        {
            this.TakeDamage(MaxDamage);
            return m_BulletStyleSheet.DamageOnCollide;
        }
    }
}
