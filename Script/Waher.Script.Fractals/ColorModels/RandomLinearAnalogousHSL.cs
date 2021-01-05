﻿using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using Waher.Script.Exceptions;
using Waher.Script.Functions;
using Waher.Script.Graphs;
using Waher.Script.Model;
using Waher.Script.Objects;
using Waher.Script.Objects.VectorSpaces;
using Waher.Script.Abstraction.Elements;

namespace Waher.Script.Fractals.ColorModels
{
	/// <summary>
	/// Calculates a palette of random color bands.
	/// 
	/// RandomLinearAnalogousHSL()                  N = 1024 by default
	/// RandomLinearAnalogousHSL(N)                 BandSize=16 by default
	/// RandomLinearAnalogousHSL(N,BandSize)        Seed=random by default.
	/// RandomLinearAnalogousHSL(N,BandSize,Seed)
	/// </summary>
	/// <example>
	/// TestColorModel(RandomLinearAnalogousHSL(1024,64))
	/// MandelbrotFractal(-1.261,0.0399575195312504,0.0035,randomlinearanalogoushsl(10240,16,3),640,480)
	/// MandelbrotSmoothFractal(-1.261,0.0399575195312504,0.0035,randomlinearanalogoushsl(10240,16,3),800,800)
	/// </example>
	public class RandomLinearAnalogousHSL : FunctionMultiVariate
    {
		public RandomLinearAnalogousHSL(ScriptNode N, int Start, int Length, Expression Expression)
			: base(new ScriptNode[] { N }, argumentTypes1Scalar, Start, Length, Expression)
		{
		}

		public RandomLinearAnalogousHSL(ScriptNode N, ScriptNode BandSize, int Start, int Length, Expression Expression)
			: base(new ScriptNode[] { N, BandSize }, argumentTypes2Scalar, Start, Length, Expression)
		{
		}

		public RandomLinearAnalogousHSL(ScriptNode N, ScriptNode BandSize, ScriptNode Seed, int Start, int Length, Expression Expression)
            : base(new ScriptNode[] { N, BandSize, Seed }, argumentTypes3Scalar, Start, Length, Expression)
        {
		}

		/// <summary>
		/// Evaluates the function.
		/// </summary>
		/// <param name="Arguments">Function arguments.</param>
		/// <param name="Variables">Variables collection.</param>
		/// <returns>Function result.</returns>
		public override IElement Evaluate(IElement[] Arguments, Variables Variables)
		{
			int i = 0;
            int c = Arguments.Length;
            int N;
            int BandSize;
            int Seed;

            if (i < c)
                N = (int)Expression.ToDouble(Arguments[i++].AssociatedObjectValue);
            else
            {
                N = 1024;
                Variables.ConsoleOut.WriteLine("N = " + N.ToString(), Variables);
            }

            if (i < c)
                BandSize = (int)Expression.ToDouble(Arguments[i++].AssociatedObjectValue);
            else
            {
                BandSize = 16;
                Variables.ConsoleOut.WriteLine("BandSize = " + BandSize.ToString(), Variables);
            }

            if (i < c)
                Seed = (int)Expression.ToDouble(Arguments[i++].AssociatedObjectValue);
            else
            {
                lock (gen)
                {
                    Seed = gen.Next();
                }

                Variables.ConsoleOut.WriteLine("Seed = " + Seed.ToString(), Variables);
            }

			return new ObjectVector(CreatePalette(N, BandSize, Seed, this));
        }

		/// <summary>
		/// Default Argument names
		/// </summary>
		public override string[] DefaultArgumentNames
		{
			get
			{
				return new string[] { "N", "BandSize", "Seed" };
			}
		}

		public static SKColor[] CreatePalette(int N, int BandSize, ScriptNode Node)
        {
            return CreatePalette(N, BandSize, null, Node);
        }

        public static SKColor[] CreatePalette(int N, int BandSize, out int Seed, ScriptNode Node, Variables Variables)
        {
            lock (gen)
            {
                Seed = gen.Next();
            }

			Variables?.ConsoleOut?.WriteLine("Seed = " + Seed.ToString(), Variables);

			return CreatePalette(N, BandSize, Seed, Node);
        }

        public static SKColor[] CreatePalette(int N, int BandSize, int? Seed, ScriptNode Node)
        {
            if (N <= 0)
                throw new ScriptRuntimeException("N in RandomLinearAnalogousHSL(N[,BandSize]) has to be positive.", Node);

            if (BandSize <= 0)
                throw new ScriptRuntimeException("BandSize in RandomLinearAnalogousHSL(N[,BandSize]) has to be positive.", Node);

            SKColor[] Result = new SKColor[N];
            double H, S, L;
            int R1, G1, B1;
            int R2, G2, B2;
            int R, G, B;
            int i, j, c, d;
            int BandSize2 = BandSize / 2;
            Random Generator;

            if (Seed.HasValue)
                Generator = new Random(Seed.Value);
            else
                Generator = gen;

            lock (Generator)
            {
                H = Generator.NextDouble() * 360;
                S = Generator.NextDouble();
                L = Generator.NextDouble();

				SKColor cl = Graph.ToColorHSL(H, S, L);
				R2 = cl.Red;
				G2 = cl.Green;
				B2 = cl.Blue;

                i = 0;
                while (i < N)
                {
                    R1 = R2;
                    G1 = G2;
                    B1 = B2;

                    H += Generator.NextDouble() * 120 - 60;
                    S = Generator.NextDouble();
                    L = Generator.NextDouble();

					cl = Graph.ToColorHSL(H, S, L);
					R2 = cl.Red;
					G2 = cl.Green;
					B2 = cl.Blue;

					c = BandSize;
                    j = N - i;
                    if (c > j)
                        c = j;

                    d = N - i;
                    if (d > c)
                        d = c;

                    for (j = 0; j < d; j++)
                    {
                        R = ((R2 * j) + (R1 * (BandSize - j)) + BandSize2) / BandSize;
                        G = ((G2 * j) + (G1 * (BandSize - j)) + BandSize2) / BandSize;
                        B = ((B2 * j) + (B1 * (BandSize - j)) + BandSize2) / BandSize;

                        if (R > 255)
                            R = 255;

                        if (G > 255)
                            G = 255;

                        if (B > 255)
                            B = 255;

                        Result[i++] = new SKColor((byte)R, (byte)G, (byte)B);
                    }
                }
            }

            return Result;
        }

        private static readonly Random gen = new Random();

        public override string FunctionName
        {
            get { return "RandomLinearAnalogousHSL"; }
        }
    }
}
