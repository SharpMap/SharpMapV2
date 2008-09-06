/*
 * Created by SharpDevelop.
 * User: Lars Brubaker
 * Date: 10/13/2007
 * Time: 12:07 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using AGG.Color;
using AGG.Rendering;
using AGG.Transform;
using AGG.UI;
using AGG.VertexSource;
using NPack;
using NPack.Interfaces;
using Reflexive.Audio;
using Reflexive.Game;

namespace RockBlaster
{
    public class PlayerStyleSheet<T> : GameObject<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        #region GameObjectStuff
        public PlayerStyleSheet()
        {
        }
        public static new GameObject<T> Load(String PathName)
        {
            return GameObject<T>.Load(PathName);
        }
        #endregion

        [GameDataNumberAttribute("TurnRate")]
        public T TurnRate = M.New<T>(4);
        [GameDataNumberAttribute("ShipThrust")]
        public T ThrustAcceleration = M.New<T>(400);
        [GameDataNumberAttribute("Friction")]
        public T Friction = M.New<T>(.99);
        [GameDataNumberAttribute("DistanceToFrontOfShip")]
        public T DistanceToFrontOfShip = M.New<T>(10);
        [GameDataNumberAttribute("DamageOnCollide")]
        public T DamageOnCollide = M.New<T>(100);

        [GameData("FireSound")]
        public AssetReference<T, SoundBuffer<T>> FireSoundReference = new AssetReference<T, SoundBuffer<T>>("PlayerSmallShot");
    }

    /// <summary>
    /// Description of Player.
    /// </summary>
    public class Player<T> : Entity<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        #region GameObjectStuff
        public Player()
        {
        }
        public static new GameObject<T> Load(String PathName)
        {
            return GameObject<T>.Load(PathName);
        }
        #endregion

        [GameData("StyleSheet")]
        public AssetReference<T, PlayerStyleSheet<T>> m_PlayerStyleSheetReference = new AssetReference<T, PlayerStyleSheet<T>>();

        [GameData("IntArrayTest")]
        public int[] m_IntArray = new int[] { 0, 1, 23, 234 };

        private static int[] s_DefaultList = new int[] { 0, 1, 23, 234 };
        [GameDataList("IntListTest")]
        public List<int> m_IntList = new List<int>(s_DefaultList);

        protected PathStorage<T> m_PlayerToDraw = new PathStorage<T>();

        [GameDataNumberAttribute("Rotation")] // This is for save game
        protected T m_Rotation = M.Zero<T>();

        IVector<T> m_Acceleration;

        bool m_TurningLeft;
        bool m_TurningRight;
        bool m_Thrusting;
        bool m_FireKeyDown;
        List<Entity<T>> m_BulletList;

        public Player(List<Entity<T>> bulletList)
            : base(26)
        {
            m_BulletList = bulletList;
            Position = MatrixFactory<T>.CreateVector2D(GameWidth / 2, GameHeight / 2);
            m_PlayerToDraw.MoveTo(18, 0);
            m_PlayerToDraw.LineTo(-8, +10);
            m_PlayerToDraw.LineTo(-8, -10);
            m_Velocity = MatrixFactory<T>.CreateVector2D(0, 0);
        }

        protected override void DoDraw(RendererBase<T> destRenderer)
        {
            IAffineTransformMatrix<T> Final = MatrixFactory<T>.NewIdentity(VectorDimension.Two);
            Final.RotateAlong(MatrixFactory<T>.CreateVector2D(0, 0), m_Rotation.ToDouble());
            Final.Translate(m_Position);
            ConvTransform<T> TransformedShip = new ConvTransform<T>(m_PlayerToDraw, Final);
            destRenderer.Render(TransformedShip, new RGBA_Bytes(.9, .4, .2, 1));
        }

        public void KeyDown(KeyEventArgs keyEvent)
        {
            if (keyEvent.KeyCode == Keys.Z)
            {
                m_TurningLeft = true;
            }
            if (keyEvent.KeyCode == Keys.X)
            {
                m_TurningRight = true;
            }
            if (keyEvent.KeyCode == Keys.OemPeriod)
            {
                m_Thrusting = true;
            }
            if (keyEvent.KeyCode == Keys.OemQuestion)
            {
                if (!m_FireKeyDown)
                {
                    double bulletVelocity = 220;
                    // WIP: have a weapon and tell it to fire.
                    // set something to fire down
                    IVector<T> DirectionVector = MatrixFactory<T>.CreateVector2D(m_Rotation.Cos(), m_Rotation.Sin());
                    m_BulletList.Add(new Bullet<T>(Position.Add(DirectionVector.Multiply(m_PlayerStyleSheetReference.Instance.DistanceToFrontOfShip.ToDouble())),
                        m_Velocity.Add(DirectionVector.Multiply(bulletVelocity))));
                    m_FireKeyDown = true;
                    m_PlayerStyleSheetReference.Instance.FireSoundReference.Instance.PlayAnAvailableCopy();
                }
            }
        }

        public void KeyUp(KeyEventArgs keyEvent)
        {
            if (keyEvent.KeyCode == Keys.Z)
            {
                m_TurningLeft = false;
            }
            if (keyEvent.KeyCode == Keys.X)
            {
                m_TurningRight = false;
            }
            if (keyEvent.KeyCode == Keys.OemPeriod)
            {
                m_Thrusting = false;
                m_Acceleration = MatrixFactory<T>.CreateVector2D(0, 0);
            }
            if (keyEvent.KeyCode == Keys.OemQuestion)
            {
                m_FireKeyDown = false;
            }
        }

        public override void Update(double numSecondsPassed)
        {
            if (m_Thrusting)
            {
                m_Acceleration.Set(
                   m_Rotation.Cos().Multiply(m_PlayerStyleSheetReference.Instance.ThrustAcceleration),
                   m_Rotation.Sin().Multiply(m_PlayerStyleSheetReference.Instance.ThrustAcceleration));
            }
            if (m_TurningLeft)
            {
                m_Rotation.AddEquals(m_PlayerStyleSheetReference.Instance.TurnRate.MultiplyEquals(numSecondsPassed));
            }
            if (m_TurningRight)
            {
                m_Rotation.SubtractEquals(m_PlayerStyleSheetReference.Instance.TurnRate.Multiply(numSecondsPassed));
            }

            m_Velocity.Add(m_Acceleration.Multiply(numSecondsPassed));
            m_Velocity.Multiply(m_PlayerStyleSheetReference.Instance.Friction.ToDouble());
            base.Update(numSecondsPassed);
        }

        public void Respawn()
        {
            Random rand = new Random();
            Position = MatrixFactory<T>.CreateVector2D(rand.NextDouble() * GameWidth, rand.NextDouble() * GameHeight);

        }

        public override T GiveDamage()
        {
            // The player just hit something.
            Respawn();
            return m_PlayerStyleSheetReference.Instance.DamageOnCollide;
        }
    }
}
