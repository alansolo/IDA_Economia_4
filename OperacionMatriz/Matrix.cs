using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperacionMatriz
{
    public class Matrix
    {
        double[,] matriz;

        public Matrix()
        {
        }
        public Matrix(double[,] m)
        {
            matriz = m;
        }
        public double[,] Suma(double[,] m)
        {
            double[,] Sumada = new double[m.GetLength(0), m.GetLength(1)];

            if (matriz.GetLength(0) != m.GetLength(0) || matriz.GetLength(1) != m.GetLength(1))
                throw new Exception("Las matrices son impoisibles de sumar");

            for (int i = 0; i < m.GetLength(0); i++)
            {
                for (int j = 0; j < m.GetLength(1); j++)
                {
                    Sumada[i, j] = m[i, j] + matriz[i, j];
                }
            }

            return Sumada;
        }
        public double[,] EscalarMult(double escalar)
        {
            double[,] multiplicada = new double[matriz.GetLength(0), matriz.GetLength(1)];

            for (int i = 0; i < matriz.GetLength(0); i++)
            {
                for (int j = 0; j < matriz.GetLength(1); j++)
                {
                    multiplicada[i, j] = escalar * matriz[i, j];
                }
            }

            return multiplicada;
        }
        public double[,] Resta(double[,] m)
        {
            if (matriz.GetLength(0) != m.GetLength(0) || matriz.GetLength(1) != m.GetLength(1))
                throw new Exception("Las matrices son impoisibles de restar");

            Matrix matriz1 = new Matrix(m);

            return Suma(matriz1.EscalarMult(-1));
        }
        private double[,] ElimFilCol(double[,] a, int fila, int column)
        {
            double[,] result = new double[a.GetLength(0) - 1, a.GetLength(1) - 1];
            bool fil = false;
            bool col = false;
            for (int i = 0; i < result.GetLength(0); i++)
            {
                col = false;
                if (i == fila) { fil = true; }
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    if (j == column) { col = true; }
                    if (!fil && !col) { result[i, j] = a[i, j]; }
                    if (!fil && col) { result[i, j] = a[i, j + 1]; }
                    if (fil && !col) { result[i, j] = a[i + 1, j]; }
                    if (fil && col) { result[i, j] = a[i + 1, j + 1]; }

                }
            }
            return result;
        }
        public double Determinante()
        {
            if (matriz.GetLength(0) != matriz.GetLength(1))
                throw new Exception("Matriz no cuadrada");
            return Determinante(this.matriz);
        }
        private double Determinante(double[,] m)
        {
            double determinante = 0;


            if (m.Length == 1)
                return m[0, 0];

            else
            {
                for (int i = 0; i < m.GetLength(0); i++)
                {
                    determinante += (double)Math.Pow(-1, i) * m[i, 0] * Determinante(ElimFilCol(m, i, 0));
                }
            }

            return determinante;
        }
        private double[,] SustCol(double[,] m, double[] soluciones, int col)
        {
            double[,] sustituida = new double[m.GetLength(0), m.GetLength(1)];

            for (int i = 0; i < sustituida.GetLength(0); i++)
            {
                for (int j = 0; j < sustituida.GetLength(1); j++)
                {
                    if (j == col)
                        sustituida[i, j] = soluciones[i];

                    else
                        sustituida[i, j] = m[i, j];
                }
            }
            return sustituida;
        }
        public double[] Cramer(double[] terminos)
        {
            double[] soluciones = new double[terminos.Length];

            double determinante = Determinante(matriz);

            for (int j = 0; j < matriz.GetLength(1); j++)
            {
                soluciones[j] = Determinante(SustCol(matriz, terminos, j)) / determinante;
            }
            return soluciones;
        }
        public double[,] Inversa()
        {
            double determinante = Determinante();
            double[,] result = new double[matriz.GetLength(0), matriz.GetLength(1)];
            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    result[i, j] = (double)Math.Pow(-1, i + j) * Determinante(ElimFilCol(matriz, i, j));
                }
            }
            result = EscalarMult(Transpuesta(result), 1 / determinante);
            return result;
        }
        double[,] Transpuesta(double[,] m)
        {
            double[,] result = new double[m.GetLength(0), m.GetLength(1)];
            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    result[i, j] = m[j, i];
                }
            }
            return result;
        }
        public double[,] Transpuesta()
        {
            double[,] result = new double[matriz.GetLength(0), matriz.GetLength(1)];
            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    result[i, j] = matriz[j, i];
                }
            }
            return result;

        }
        public double[,] TranspuestaX()
        {
            double[,] result = new double[matriz.GetLength(1), matriz.GetLength(0)];
            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    result[i, j] = matriz[j, i];
                }
            }
            return result;

        }
        public double[,] EscalarMult(double[,] matriz, double escalar)
        {
            double[,] multiplicada = new double[matriz.GetLength(0), matriz.GetLength(1)];

            for (int i = 0; i < matriz.GetLength(0); i++)
            {
                for (int j = 0; j < matriz.GetLength(1); j++)
                {
                    multiplicada[i, j] = escalar * matriz[i, j];
                }
            }

            return multiplicada;
        }
        public double[,] ProductoMatrices(double[,] b)
        {
            if (matriz.GetLength(1) != b.GetLength(0))
                throw new Exception("No se puede multiplicar");
            double[,] result = new double[matriz.GetLength(0), b.GetLength(1)];
            for (int i = 0; i < result.GetLength(0); i++)
                for (int j = 0; j < result.GetLength(1); j++)
                    for (int k = 0; k < matriz.GetLength(1); k++)
                    {
                        result[i, j] += matriz[i, k] * b[k, j];
                    }
            return result;
        }
        public double[,] ProductoMatrices(double[,] a, double[,] b)
        {
            if (a.GetLength(1) != b.GetLength(0))
                throw new Exception("No se puede multiplicar");
            double[,] result = new double[a.GetLength(0), b.GetLength(1)];
            for (int i = 0; i < result.GetLength(0); i++)
                for (int j = 0; j < result.GetLength(1); j++)
                    for (int k = 0; k < a.GetLength(1); k++)
                    {
                        result[i, j] += a[i, k] * b[k, j];
                    }
            return result;
        }
        public double[,] Gauss()
        {
            bool sePuedeContinuar = true;
            double[,] result = new double[matriz.GetLength(0), matriz.GetLength(1)];
            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    result[i, j] = matriz[i, j];
                }

            }
            for (int i = 0; i < Math.Min(result.GetLength(0), result.GetLength(1)); i++)
            {
                if (result[i, i] == 0)
                {
                    for (int j = i + 1; j < result.GetLength(0); j++)
                    {
                        if (result[j, i] != 0)
                        {
                            IntercambiarFilas(result, i, j);
                            sePuedeContinuar = true;
                            break;
                        }
                        else
                        {
                            sePuedeContinuar = false;
                        }
                    }
                }
                if (sePuedeContinuar)
                {
                    AnulaColumna(result, i);
                }

            }
            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    result[i, j] = Math.Round(result[i, j], 2);
                }
            }
            return result;
        }
        private void AnulaColumna(double[,] matriz, int i)
        {
            double terminoAnulante = 0;
            for (int j = i + 1; j < matriz.GetLength(0); j++)
            {
                terminoAnulante = -1 * matriz[j, i] / matriz[i, i];
                for (int k = i; k < matriz.GetLength(1); k++)
                {
                    matriz[j, k] = matriz[i, k] * terminoAnulante + matriz[j, k];
                }
            }
        }
        private void IntercambiarFilas(double[,] result, int i, int j)
        {
            double[] fila1 = new double[result.GetLength(1)];
            for (int k = 0; k < result.GetLength(1); k++)
            {
                fila1[k] = result[i, k];
            }
            for (int k = 0; k < result.GetLength(1); k++)
            {
                result[i, k] = result[j, k];
                result[j, k] = fila1[k];

            }
        }
    }
}
