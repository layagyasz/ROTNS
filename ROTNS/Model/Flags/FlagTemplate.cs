using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Graphing;
using Cardamom.Serialization;
using Cardamom.Utilities;

using SFML.Graphics;

namespace ROTNS.Model.Flags
{
    public class FlagTemplate : Graph<string>
    {
        public FlagTemplate(ParseBlock Block, TextureSheet Textures)
        {
            int i = 0;
            foreach (ParseBlock B in Block.Break())
            {
                switch (B.Name.Trim().ToLower())
                {
                    case "parts":
                        string[] Keys = Cardamom.Serialization.Parse.Array<string>(B.String, delegate(string Input) { return Input; });
                        foreach (string Key in Keys) AddNode(new FlagTemplateZone(Key, i++));
                        break;
                    default:
                        ((FlagTemplateZone)GetNode(B.Name.Trim().ToLower())).Initialize(B, this, Textures);
                        break;
                }
            }

        }

        private Pair<double,int> ColoringScore(Culture Culture, FlagColor[] Colors)
        {
            double Score = 0;
            double WorstPartScore = double.MaxValue;
            int WorstPart = -1;
            foreach (KeyValuePair<string, Node<string>> N in _Nodes)
            {
                FlagTemplateZone G = (FlagTemplateZone)N.Value;
                FlagColor Color = Colors[G.ID];
                double Distance = 1 - Color.DistanceTo(Culture) / 2.2360679775;
                Distance *= Distance * Distance;
                double PartScore = Distance;
                IEnumerator<Pair<float, GraphNode<string>>> I = G.Neighbors;
                while (I.MoveNext())
                {
                    FlagTemplateZone Z = (FlagTemplateZone)I.Current.Second;
                    double M = 1- Color.DistanceTo(Colors[Z.ID]) / 1.73205080757;
                    M = M * M * M * M;
                    //double M = Color == Colors[Z.ID] ? 1 : 0;
                    PartScore -= Distance * M * I.Current.First;
                }
                Score += PartScore;
                if (PartScore < WorstPartScore)
                {
                    WorstPart = G.ID;
                    WorstPartScore = PartScore;
                }
            }
            return new Pair<double, int>(Score, WorstPart);
        }

        public Vertex[] MakeFlag(FlagColor[] Colors)
        {
            Vertex[] V = new Vertex[_Nodes.Count * 4];

            foreach (KeyValuePair<string, Node<string>> N in _Nodes)
            {
                FlagTemplateZone Z = (FlagTemplateZone)N.Value;
                for (int i = 0; i < Z.Vertices.Length; ++i)
                {
                    V[Z.ID * 4 + i] = new Vertex(Z.Vertices[i], Colors[Z.ID].Color);
                }
            }

            return V;
        }

        public FlagColor[] SelectColors(Culture Culture, FlagColorMap Colors, Random Random, int Iterations = 30)
        {
            // Initialize to random state
            FlagColor[] Chosen = Colors.Closest(Culture, _Nodes.Count);

            Pair<double, int> P = ColoringScore(Culture, Chosen);

            for (int j = 0; j < Iterations; ++j)
            {
                // Select random field to change
                P = ColoringScore(Culture, Chosen);
                int C = Random.NextDouble() > .5 ? P.Second : Random.Next(0, _Nodes.Count);
                double Score = P.First;

                FlagColor Original = Chosen[C];
                // Find best new assignment
                FlagColor Best = Chosen[C];
                for (int i = 0; i < Colors.Colors.Length; ++i)
                {
                    Chosen[C] = Colors.Colors[i];
                    double thisScore = ColoringScore(Culture, Chosen).First;
                    if (thisScore > Score)
                    {
                        Score = thisScore;
                        Best = Colors.Colors[i];
                    }
                }
                /*
                // Reinitialize if no change
                if (Original == Best && Restarts > 0)
                {
                    Restarts--;
                    Iterations = 0;
                    FlagColor[] c = new FlagColor[_Nodes.Count];
                    for (int i = 0; i < c.Length; ++i) c[i] = Colors[Random.Next(0, Colors.Length)];
                }
                 */
                Chosen[C] = Best;
            }

            return Chosen;
        }
    }
}
