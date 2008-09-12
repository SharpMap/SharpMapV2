using System;
using System.Collections.Generic;
using AGG.Color;
using AGG.Rendering;
using AGG.VertexSource;
using NPack;
using NPack.Interfaces;
using Reflexive.Audio;
using Reflexive.Game;

namespace RockBlaster
{
    public class Rock<T> : Entity<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        protected Ellipse<T> m_RockToDraw;

        public static double MinSplitRadius = 15;

        List<Entity<T>> m_RockList;

        public Rock(double radius, List<Entity<T>> rockList)
            : this(M.New<T>(radius), rockList)
        {
        }

        public Rock(T radius, List<Entity<T>> rockList)
            : base(radius)
        {
            m_RockList = rockList;
            m_RockToDraw = new Ellipse<T>(m_Position[0], m_Position[1], m_Radius, m_Radius);
            m_MaxDamage = M.New<T>(10);
        }

        protected override void DoDraw(RendererBase<T> destRenderer)
        {
            m_RockToDraw.X = m_Position[0];
            m_RockToDraw.Y = m_Position[1];
            destRenderer.Render(m_RockToDraw, new RGBA_Bytes(.9, .4, .2, 1));
        }

        public override void Update(double numSecondsPassed)
        {
            base.Update(numSecondsPassed);
        }

        public override void TakeDamage(T DamageToTake)
        {
            ((SoundBuffer<T>)DataAssetCache<T>.Instance.GetAsset(typeof(SoundBuffer<T>), "AsteroidHit")).PlayAnAvailableCopy();
            base.TakeDamage(DamageToTake);
        }

        public override void Destroying()
        {
            ((SoundBuffer<T>)DataAssetCache<T>.Instance.GetAsset(typeof(SoundBuffer<T>), "AsteroidExplosion")).PlayAnAvailableCopy();
            if (Radius.GreaterThan(MinSplitRadius))
            {
                PlayerSaveInfo<T>.GetPlayerInfo().Score += this.Radius.Multiply(20).ToDouble();

                Random rand = new Random();
                Rock<T> newRock = new Rock<T>(this.Radius.Divide(2), m_RockList);
                newRock.Position = this.Position;
                newRock.m_MaxDamage = this.MaxDamage.Divide(2);
                m_RockList.Add(newRock);
                newRock = new Rock<T>(this.Radius.Divide(2), m_RockList);
                newRock.Position = this.Position;
                newRock.m_MaxDamage = this.MaxDamage.Divide(2);
                newRock.m_Velocity.Set(M.New<T>(rand.NextDouble() * 200), M.New<T>(rand.NextDouble() * 200));
                m_RockList.Add(newRock);
            }
        }
    }
}
