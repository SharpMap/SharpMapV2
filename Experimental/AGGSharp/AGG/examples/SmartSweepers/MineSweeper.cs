using System;
using System.Collections.Generic;
using System.Linq;
using AGG.Transform;
using NeuralNet;
using NPack;
using NPack.Interfaces;

namespace SmartSweeper
{
    class CMinesweeper<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        double m_dMaxTurnRate;
        int m_WindowWidth;
        int m_WindowHeight;

        //the minesweeper's neural net
        CNeuralNet m_ItsBrain;

        //its position in the world
        IVector<T> m_vPosition;

        //direction sweeper is facing
        IVector<T> m_vLookAt;

        //its rotation (surprise surprise)
        double m_dRotation;

        double m_dSpeed;

        //to store output from the ANN
        double m_lTrack, m_rTrack;

        //the sweeper's fitness score 
        double m_dFitness;

        //the scale of the sweeper when drawn
        T m_dScale;

        //index position of closest mine
        int m_iClosestMine;


        public CMinesweeper(int WindowWidth, int WindowHeight, T SweeperScale, double dMaxTurnRate)
        {
            m_ItsBrain = new CNeuralNet(4, 2, 1, 6, -1, 1);
            //m_ItsBrain = new CNeuralNet(4, 2, 3, 8, -1, 1);
            Random Rand = new Random();
            m_WindowWidth = WindowWidth;
            m_WindowHeight = WindowHeight;
            m_dMaxTurnRate = dMaxTurnRate;
            m_dRotation = (Rand.NextDouble() * (System.Math.PI * 2));
            m_lTrack = (0.16);
            m_rTrack = (0.16);
            m_dFitness = (0);
            m_dScale = SweeperScale;
            m_iClosestMine = (0);
            //create a random start position
            m_vPosition = MatrixFactory<T>.CreateVector3D(M.New<T>(Rand.NextDouble() * WindowWidth), M.New<T>(Rand.NextDouble() * WindowHeight), M.One<T>());
            m_vLookAt = MatrixFactory<T>.CreateVector3D(M.New<T>(Rand.NextDouble() * WindowWidth), M.New<T>(Rand.NextDouble() * WindowHeight), M.One<T>());

        }

        //updates the ANN with information from the sweepers enviroment
        public bool Update(List<IVector<T>> mines)
        {

            //this will store all the inputs for the NN
            List<T> inputs = new List<T>();

            //get List to closest mine
            IVector<T> vClosestMine = GetClosestMine(mines);

            //normalise it
            vClosestMine = vClosestMine.Normalize();



#if false
            // get the angle to the closest mine
            Vector3D DeltaToMine = vClosestMine - m_vPosition;
            Vector2D DeltaToMine2D = new Vector2D(DeltaToMine.x, DeltaToMine.y);
            Vector2D LookAt2D = new Vector2D(m_vLookAt.x, m_vLookAt.y);
            double DeltaAngle = LookAt2D.GetDeltaAngle(DeltaToMine2D);

            inputs.Add(DeltaAngle);
            inputs.Add(DeltaAngle);
            inputs.Add(DeltaAngle);
            inputs.Add(DeltaAngle);
#else

            //add in List to closest mine
            inputs.Add(vClosestMine[0]);
            inputs.Add(vClosestMine[1]);

            //add in sweepers look at List
            inputs.Add(m_vLookAt[0]);
            inputs.Add(m_vLookAt[1]);
#endif

            //update the brain and get feedback
            ///watch the performance here
            List<double> output = m_ItsBrain.Update(inputs.Select(o => o.ToDouble()).ToList());

            //make sure there were no errors in calculating the 
            //output
            if (output.Count < m_ItsBrain.NumOutputs)
            {
                return false;
            }

            //assign the outputs to the sweepers left & right tracks
            m_lTrack = output[0];
            m_rTrack = output[1];

            //calculate steering forces
            double RotForce = m_lTrack - m_rTrack;

            //clamp rotation
            RotForce = System.Math.Min(System.Math.Max(RotForce, -m_dMaxTurnRate), m_dMaxTurnRate);

            m_dRotation += RotForce;

            m_dSpeed = (m_lTrack + m_rTrack);

            //update Look At 
            m_vLookAt[0] = M.New<T>((double)-System.Math.Sin(m_dRotation));
            m_vLookAt[1] = M.New<T>((double)System.Math.Cos(m_dRotation));

            //update position
            m_vPosition.Add(m_vLookAt.Multiply(m_dSpeed));

            //wrap around window limits
            if (m_vPosition[0].GreaterThan(m_WindowWidth)) m_vPosition[0] = M.Zero<T>();
            if (m_vPosition[0].LessThan(0)) m_vPosition[0] = M.New<T>(m_WindowWidth);
            if (m_vPosition[1].GreaterThan(m_WindowHeight)) m_vPosition[1] = M.Zero<T>();
            if (m_vPosition[1].LessThan(0)) m_vPosition[1] = M.New<T>(m_WindowHeight);

            return true;
        }


        //used to Transform the sweepers vertices prior to rendering
        public void WorldTransform(List<IVector<T>> sweeper)
        {
            ////create the world transformation matrix
            //Matrix4X4 matTransform = new Matrix4X4();

            ////scale
            //matTransform.Scale(m_dScale, m_dScale, 1);

            ////rotate
            //matTransform.AddRotate(2, (double)m_dRotation);

            ////and translate
            //matTransform.AddTranslate(m_vPosition.x, m_vPosition.y, 0);

            ////now Transform the ships vertices
            //for (int i = 0; i < sweeper.Count; i++)
            //{
            //    Vector3D Temp = sweeper[i];
            //    matTransform.TransformVector(ref Temp);
            //    sweeper[i] = Temp;
            //}

            //create the world transformation matrix
            IAffineTransformMatrix<T> matTransform = MatrixFactory<T>.NewIdentity(VectorDimension.Three);

            //scale
            matTransform.Scale(MatrixFactory<T>.CreateVector3D(m_dScale, m_dScale, M.One<T>()));

            //rotate
            //matTransform.AddRotate(2, (double)m_dRotation);
            IVector<T> axis = null;/// need to work out which axis this should be
            matTransform.RotateAlong(axis, m_dRotation);


            //and translate
            matTransform.Translate(MatrixFactory<T>.CreateVector3D(m_vPosition[0], m_vPosition[1], M.Zero<T>()));

            //now Transform the ships vertices
            for (int i = 0; i < sweeper.Count; i++)
            {
                IVector<T> Temp = sweeper[i];
                Temp = matTransform.TransformVector(Temp);
                sweeper[i] = Temp;
            }
        }


        //returns a List to the closest mine
        public IVector<T> GetClosestMine(List<IVector<T>> mines)
        {
            T closest_so_far = M.New<T>(99999);

            IVector<T> vClosestObject = MatrixFactory<T>.CreateZeroVector(VectorDimension.Three);

            //cycle through mines to find closest
            for (int i = 0; i < mines.Count; i++)
            {
                T len_to_object = mines[i].Subtract(m_vPosition).GetMagnitude();

                if (len_to_object.LessThan(closest_so_far))
                {
                    closest_so_far = len_to_object;

                    vClosestObject = m_vPosition.Subtract(mines[i]);

                    m_iClosestMine = i;
                }
            }

            return vClosestObject;
        }


        //checks to see if the minesweeper has 'collected' a mine
        public int CheckForMine(List<IVector<T>> mines, T size)
        {
            IVector<T> DistToObject = m_vPosition.Subtract(mines[m_iClosestMine]);

            if (DistToObject.GetMagnitude().LessThan(size.Add(5)))
            {
                return m_iClosestMine;
            }

            return -1;
        }


        public void Reset()
        {
            Random Rand = new Random();
            //reset the sweepers positions
            m_vPosition = MatrixFactory<T>.CreateVector3D(M.New<T>((Rand.NextDouble() * m_WindowWidth)),
                                           M.New<T>(Rand.NextDouble() * m_WindowHeight), M.Zero<T>());

            //and the fitness
            m_dFitness = 0;

            //and the rotation
            m_dRotation = Rand.NextDouble() * (System.Math.PI * 2);

            return;
        }

        //-------------------accessor functions
        public IVector<T> Position() { return m_vPosition; }

        public void IncrementFitness() { ++m_dFitness; }

        public double Fitness() { return m_dFitness; }

        public void PutWeights(List<double> w) { m_ItsBrain.PutWeights(w); }

        public int GetNumberOfWeights() { return m_ItsBrain.GetNumberOfWeights(); }
    };
}