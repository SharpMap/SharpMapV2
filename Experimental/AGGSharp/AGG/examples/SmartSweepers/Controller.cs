using System;
using System.Collections.Generic;
using AGG.Buffer;
using AGG.Color;
using AGG.Rendering;
using AGG.Transform;
using AGG.UI;
using AGG.VertexSource;
using NeuralNet;
using NPack.Interfaces;
using NPack;

namespace SmartSweeper
{
    public class CController<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        PathStorage<T> m_LinesToDraw = new PathStorage<T>();
        PathStorage<T> m_BestPathToDraw = new PathStorage<T>();
        PathStorage<T> m_AveragePathToDraw = new PathStorage<T>();
        T m_SweeperScale;
        IVector<T>[] sweeper = {            
                                MatrixFactory<T>.CreateVector3D(M.New<T>(  -1),M.New<T>( -1), M.Zero<T>() ),
                                MatrixFactory<T>.CreateVector3D(M.New<T>(-1), M.New<T>(1), M.Zero<T>()),
                                MatrixFactory<T>.CreateVector3D(M.New<T>(-0.5),M.One<T>(),M.Zero<T>()),
                                MatrixFactory<T>.CreateVector3D(M.New<T>(-0.5),M.New<T>( -1), M.Zero<T>()),

                                MatrixFactory<T>.CreateVector3D(M.New<T>(0.5), M.New<T>(-1), M.Zero<T>()),
                                MatrixFactory<T>.CreateVector3D(M.One<T>(), M.New<T>(-1), M.Zero<T>()),
                                MatrixFactory<T>.CreateVector3D(M.One<T>(), M.One<T>(), M.Zero<T>()),
                                MatrixFactory<T>.CreateVector3D(M.New<T>(0.5), M.One<T>(), M.Zero<T>()),

                                MatrixFactory<T>.CreateVector3D(M.New<T>(-0.5), M.New<T>(-0.5),M.Zero<T>()),
                                MatrixFactory<T>.CreateVector3D(M.New<T>(0.5), M.New<T>(-0.5), M.Zero<T>()),

                                MatrixFactory<T>.CreateVector3D(M.New<T>(-0.5), M.New<T>(0.5), M.Zero<T>()),
                                MatrixFactory<T>.CreateVector3D(M.New<T>(-0.25), M.New<T>(0.5), M.Zero<T>()),
                                MatrixFactory<T>.CreateVector3D(M.New<T>(-0.25), M.New<T>(1.75), M.Zero<T>()),
                                MatrixFactory<T>.CreateVector3D(M.New<T>(0.25), M.New<T>(1.75), M.Zero<T>()),
                                MatrixFactory<T>.CreateVector3D(M.New<T>(0.25), M.New<T>(0.5),M.Zero<T>()),
                                MatrixFactory<T>.CreateVector3D(M.New<T>(0.5), M.New<T>(0.5), M.Zero<T>())};



        T m_MineScale;
        IVector<T>[] mine = {
                                MatrixFactory<T>.CreateVector3D(M.New<T>(-1), M.New<T>(-1), M.Zero<T>()),
                                MatrixFactory<T>.CreateVector3D(M.New<T>(-1), M.One<T>(), M.Zero<T>()),
                                MatrixFactory<T>.CreateVector3D(M.One<T>(),M.One<T>(), M.Zero<T>()),
                                MatrixFactory<T>.CreateVector3D(M.One<T>(), M.New<T>(-1),M.Zero<T>())};

        //storage for the population of genomes
        List<SGenome> m_vecThePopulation;

        //and the minesweepers
        List<CMinesweeper<T>> m_vecSweepers = new List<CMinesweeper<T>>();

        //and the mines
        List<IVector<T>> m_vecMines = new List<IVector<T>>();

        //pointer to the GA
        CGenAlg m_pGA;

        int m_NumWeightsInNN;

        //vertex buffer for the sweeper shape's vertices
        List<IVector<T>> m_SweeperVB = new List<IVector<T>>();

        //vertex buffer for the mine shape's vertices
        List<IVector<T>> m_MineVB = new List<IVector<T>>();

        //stores the average fitness per generation for use 
        //in graphing.
        List<double> m_vecAvFitness = new List<double>();

        //stores the best fitness per generation
        List<double> m_vecBestFitness = new List<double>();

        double dMutationRate;
        double dCrossoverRate;

        //pens we use for the stats
        RGBA_Bytes m_BlackPen;
        RGBA_Bytes m_RedPen;
        RGBA_Bytes m_BluePen;
        RGBA_Bytes m_GreenPen;

        //handle to the application window
        RasterBuffer m_hwndMain;

        //toggles the speed at which the simulation runs
        bool m_bFastRender;

        //cycles per generation
        int m_NumTicksPerGeneration;
        int m_TicksThisGeneration;

        //generation counter
        int m_iGenerations;

        double m_BestFitnessYet;
        int LastFitnessCount;

        //window dimensions
        int cxClient, cyClient;

        ConvStroke<T> m_AverageLinesToDraw;
        ConvStroke<T> m_BestLinesToDraw;

        //this function plots a graph of the average and best fitnesses
        //over the course of a run
        void PlotStats(RendererBase<T> renderer)
        {
            if (m_vecBestFitness.Count == 0)
            {
                return;
            }
            string s = "Best Fitness:       " + m_pGA.BestFitness.ToString();
            TextWidget<T> InfoText = new TextWidget<T>(s, M.New<T>(5), M.New<T>(30), M.New<T>(9));
            //InfoText.Render(renderer);

            s = "Average Fitness: " + m_pGA.AverageFitness.ToString();
            InfoText = new TextWidget<T>(s, M.New<T>(5), M.New<T>(45), M.New<T>(9));
            //InfoText.Render(renderer);

            //render the graph
            T HSlice = M.New<T>((double)cxClient / (m_iGenerations + 1));
            T VSlice = M.New<T>((double)(cyClient / ((m_BestFitnessYet + 1) * 2)));

            bool foundNewBest = false;
            if (m_vecBestFitness[m_vecBestFitness.Count - 1] > m_BestFitnessYet
                || m_BestLinesToDraw == null
                || LastFitnessCount != m_vecBestFitness.Count)
            {
                LastFitnessCount = m_vecBestFitness.Count;
                m_BestFitnessYet = m_vecBestFitness[m_vecBestFitness.Count - 1];

                foundNewBest = true;
            }

            if (foundNewBest)
            {
                //plot the graph for the best fitness
                T x = M.Zero<T>();
                m_BestPathToDraw.RemoveAll();
                m_BestPathToDraw.MoveTo(M.Zero<T>(), M.Zero<T>());
                for (int i = 0; i < m_vecBestFitness.Count; ++i)
                {
                    m_BestPathToDraw.LineTo(x, VSlice.Multiply(m_vecBestFitness[i]));
                    x.AddEquals(HSlice);
                }

                m_BestLinesToDraw = new ConvStroke<T>(m_BestPathToDraw);

                //plot the graph for the average fitness
                x = M.Zero<T>();

                m_AveragePathToDraw.RemoveAll();
                m_AveragePathToDraw.MoveTo(M.Zero<T>(), M.Zero<T>());
                for (int i = 0; i < m_vecAvFitness.Count; ++i)
                {
                    m_AveragePathToDraw.LineTo(x, VSlice.Multiply(m_vecAvFitness[i]));
                    x.AddEquals(HSlice);
                }

                m_AverageLinesToDraw = new ConvStroke<T>(m_AveragePathToDraw);
            }
            else
            {
                renderer.Render(m_BestLinesToDraw, m_BluePen);
                renderer.Render(m_AverageLinesToDraw, m_RedPen);
            }
        }



        public CController(RasterBuffer hwndMain, int iNumSweepers, int iNumMines, double _dMutationRate, double _dCrossoverRate, double dMaxPerturbation,
            int NumElite, int NumCopiesElite, int NumTicksPerGeneration)
        {
            Random Rand = new Random();

            m_MineScale = M.One<T>();
            m_SweeperScale = M.New<T>(3);

            m_pGA = null;
            m_bFastRender = (false);
            m_TicksThisGeneration = 0;
            m_NumTicksPerGeneration = NumTicksPerGeneration;
            m_hwndMain = hwndMain;
            m_iGenerations = (0);
            cxClient = (int)hwndMain.Width;
            cyClient = (int)hwndMain.Height;
            dMutationRate = _dMutationRate;
            dCrossoverRate = _dCrossoverRate;
            //let's create the mine sweepers
            for (int i = 0; i < iNumSweepers; ++i)
            {
                m_vecSweepers.Add(new CMinesweeper<T>((int)hwndMain.Width, (int)hwndMain.Height, m_SweeperScale, .3));
            }

            //get the total number of weights used in the sweepers
            //NN so we can initialise the GA
            m_NumWeightsInNN = m_vecSweepers[0].GetNumberOfWeights();

            //initialize the Genetic Algorithm class
            m_pGA = new CGenAlg(iNumSweepers, dMutationRate, dCrossoverRate, m_NumWeightsInNN, dMaxPerturbation,
                NumElite, NumCopiesElite);

            //Get the weights from the GA and insert into the sweepers brains
            m_vecThePopulation = m_pGA.GetChromos();

            for (int i = 0; i < iNumSweepers; i++)
            {
                m_vecSweepers[i].PutWeights(m_vecThePopulation[i].Weights);
            }

            //initialize mines in random positions within the application window
            for (int i = 0; i < iNumMines; ++i)
            {
                m_vecMines.Add(MatrixFactory<T>.CreateVector3D(
                    M.New<T>(Rand.NextDouble() * cxClient),
                    M.New<T>(Rand.NextDouble() * cyClient),
                    M.Zero<T>()));
            }

            //create a pen for the graph drawing
            m_BlackPen = new RGBA_Bytes(0, 0, 0, 255);
            m_BluePen = new RGBA_Bytes(0, 0, 255);
            m_RedPen = new RGBA_Bytes(255, 0, 0);
            m_GreenPen = new RGBA_Bytes(0, 150, 0);

            //fill the vertex buffers
            for (int i = 0; i < sweeper.Length; ++i)
            {
                m_SweeperVB.Add(sweeper[i]);
            }

            for (int i = 0; i < mine.Length; ++i)
            {
                m_MineVB.Add(mine[i]);
            }

        }

        public void Render(RendererBase<T> renderer)
        {
            //render the stats
            string s = "Generation:          " + m_iGenerations.ToString();
            AGG.UI.TextWidget<T> GenerationText = new AGG.UI.TextWidget<T>(s, M.New<T>(150), M.New<T>(10), M.New<T>(9));
            //GenerationText.Render(renderer);

            //do not render if running at accelerated speed
            if (!m_bFastRender)
            {
                //render the mines
                for (int i = 0; i < m_vecMines.Count; ++i)
                {
                    //grab the vertices for the mine shape
                    List<IVector<T>> mineVB = new List<IVector<T>>();
                    foreach (IVector<T> vector in m_MineVB)
                    {
                        mineVB.Add(vector);
                    }

                    WorldTransform(mineVB, m_vecMines[i]);

                    //draw the mines
                    m_LinesToDraw.RemoveAll();
                    m_LinesToDraw.MoveTo(mineVB[0][0], mineVB[0][1]);
                    for (int vert = 1; vert < mineVB.Count; ++vert)
                    {
                        m_LinesToDraw.LineTo(mineVB[vert][0], mineVB[vert][1]);
                    }

                    renderer.Render(m_LinesToDraw, m_BlackPen);
                }

                RGBA_Bytes currentColor = m_RedPen;
                //render the sweepers
                for (int i = 0; i < m_vecSweepers.Count; i++)
                {
                    //grab the sweeper vertices
                    List<IVector<T>> sweeperVB = new List<IVector<T>>();
                    foreach (IVector<T> vector in m_SweeperVB)
                    {
                        sweeperVB.Add(vector);
                    }

                    //Transform the vertex buffer
                    m_vecSweepers[i].WorldTransform(sweeperVB);

                    //draw the sweeper left track
                    m_LinesToDraw.RemoveAll();
                    m_LinesToDraw.MoveTo(sweeperVB[0][0], sweeperVB[0][1]);
                    for (int vert = 1; vert < 4; ++vert)
                    {
                        m_LinesToDraw.LineTo(sweeperVB[vert][0], sweeperVB[vert][1]);
                    }

                    if (i == m_pGA.NumElite)
                    {
                        currentColor = m_BlackPen;
                    }

                    renderer.Render(m_LinesToDraw, currentColor);

                    //draw the sweeper right track
                    m_LinesToDraw.RemoveAll();
                    m_LinesToDraw.MoveTo(sweeperVB[4][0], sweeperVB[4][1]);
                    for (int vert = 5; vert < 8; ++vert)
                    {
                        m_LinesToDraw.LineTo(sweeperVB[vert][0], sweeperVB[vert][1]);
                    }
                    renderer.Render(m_LinesToDraw, currentColor);

                    // draw the body
                    m_LinesToDraw.RemoveAll();
                    m_LinesToDraw.MoveTo(sweeperVB[8][0], sweeperVB[8][1]);
                    m_LinesToDraw.LineTo(sweeperVB[9][0], sweeperVB[9][1]);
                    m_LinesToDraw.MoveTo(sweeperVB[10][0], sweeperVB[10][1]);
                    for (int vert = 11; vert < 16; ++vert)
                    {
                        m_LinesToDraw.LineTo(sweeperVB[vert][0], sweeperVB[vert][1]);
                    }
                    renderer.Render(m_LinesToDraw, currentColor);
                }
            }
            else
            {
                PlotStats(renderer);
            }
        }


        public void WorldTransform(List<IVector<T>> VBuffer, IVector<T> vPos)
        {
            //create the world transformation matrix
            //Matrix4X4 matTransform = new Matrix4X4();

            IAffineTransformMatrix<T> matTransform = MatrixFactory<T>.NewIdentity(VectorDimension.Three);

            //scale
            matTransform.Scale(MatrixFactory<T>.CreateVector2D(m_MineScale, m_MineScale));

            //translate
            matTransform.Translate(MatrixFactory<T>.CreateVector2D(vPos[0], vPos[1]));

            //Transform the ships vertices
            for (int i = 0; i < VBuffer.Count; i++)
            {
                IVector<T> Temp = VBuffer[i];
                Temp = matTransform.TransformVector(Temp);
                VBuffer[i] = Temp;
            }
        }


        public bool Update()
        {
            //run the sweepers through NumTicks amount of cycles. During
            //this loop each sweepers NN is constantly updated with the appropriate
            //information from its surroundings. The output from the NN is obtained
            //and the sweeper is moved. If it encounters a mine its fitness is
            //updated appropriately,
            int NumSweepers = m_vecSweepers.Count;
            if (m_TicksThisGeneration++ < m_NumTicksPerGeneration)
            {
                for (int i = 0; i < NumSweepers; ++i)
                {
                    //update the NN and position
                    if (!m_vecSweepers[i].Update(m_vecMines))
                    {
                        //error in processing the neural net
                        //MessageBox(m_hwndMain, "Wrong amount of NN inputs!", "Error", MB_OK);

                        return false;
                    }

                    //see if it's found a mine
                    int GrabHit = m_vecSweepers[i].CheckForMine(m_vecMines, m_MineScale);

                    if (GrabHit >= 0)
                    {
                        Random Rand = new Random();
                        //we have discovered a mine so increase fitness
                        m_vecSweepers[i].IncrementFitness();

                        //mine found so replace the mine with another at a random 
                        //position
                        m_vecMines[GrabHit] = MatrixFactory<T>.CreateVector3D(
                            M.New<T>(Rand.NextDouble() * cxClient),
                              M.New<T>(Rand.NextDouble() * cyClient),
                              M.Zero<T>());
                    }

                    //update the chromos fitness score
                    m_vecThePopulation[i].Fitness = m_vecSweepers[i].Fitness();

                }
            }
            //Another generation has been completed.
            //Time to run the GA and update the sweepers with their new NNs
            else
            {
                //update the stats to be used in our stat window
                m_vecAvFitness.Add(m_pGA.AverageFitness);
                m_vecBestFitness.Add(m_pGA.BestFitness);

                //increment the generation counter
                ++m_iGenerations;

                //reset cycles
                m_TicksThisGeneration = 0;

                //run the GA to create a new population
                m_vecThePopulation = m_pGA.Epoch(m_vecThePopulation);

                //insert the new (hopefully)improved brains back into the sweepers
                //and reset their positions etc
                for (int i = 0; i < NumSweepers; ++i)
                {
                    m_vecSweepers[i].PutWeights(m_vecThePopulation[i].Weights);

                    m_vecSweepers[i].Reset();
                }
            }

            return true;
        }



        //accessor methods
        public bool FastRender() { return m_bFastRender; }
        public void FastRender(bool arg) { m_bFastRender = arg; }
        public void FastRenderToggle() { m_bFastRender = !m_bFastRender; }
    };
}
