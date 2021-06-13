using System;
using System.IO;
using System.Transactions;

namespace ConsoleApp30
{
    class Program
    {
        /// <summary>
        /// Show how to use programm.
        /// </summary>
        static void ShowInstruction()
        {
            Console.WriteLine("Hi, user!\n" +
                "I want to show you how to use my program!\n" +
                "Size of the matrix must be in this format:\n" +
                "       <lines> <columns>\n\n" +
                "The matrix must be in this format (example for 2x2):\n" +
                "       a11 a12\n" +
                "       a21 a22\n\n\n" +
                "Good luck!\n\n" +
                "Press any key to continue...");
            Console.ReadKey();
        }
        /// <summary>
        /// Show all commands user can choose.
        /// </summary>
        static void ShowCommands()
        {
            Console.WriteLine("Choose one of the available operations:\n" +
                "1) Get trace of the matrix\n" +
                "2) Transpose the matrix\n" +
                "3) Add two matrices\n" +
                "4) Subtract one matrix from another\n" +
                "5) Multiply one matrix by another matrix\n" +
                "6) Multiply matrix by a scalar\n" +
                "7) Get determinant of a matrix\n" +
                "8) Solve a system of linear algebraic equations (SLAE)\n" +
                "9) Exit");
        }
        /// <summary>
        /// Show ways how to read the matrix.
        /// </summary>
        static void ShowWaysToRead()
        {
            Console.WriteLine("1) From console\n" +
                "2) From file\n" +
                "3) Random \n\n" +
                "ATTENTION if you enter an incorrect option your matrix will be random");
        }
        /// <summary>
        /// Read the number of lines and the number of columns of matrix.
        /// </summary>
        /// <returns>Array consisting of the number of rows and columns.</returns>
        static int[] ReadSizeOfMatrix()
        {
            bool can = true;
            int[] size = new int[2];
            while (can)
            {
                string[] input = Console.ReadLine().Split();
                int n, m;
                if (input.Length == 2 && int.TryParse(input[0], out n) && int.TryParse(input[1], out m))
                {
                    if (n <= 10 && m <= 10 && n > 0 && m > 0)
                    {
                        size[0] = n;
                        size[1] = m;
                        can = false;
                    }
                    else
                    {
                        Console.WriteLine("Entered value is too big or not positive");
                    }
                }
                else
                {
                    Console.WriteLine("Incorrect input");
                    Console.WriteLine("Enter the size again.");
                }
            }
            return size;
        }
        /// <summary>
        /// Read the number of variables.
        /// </summary>
        /// <returns>Array consisting of the number of variables and equation.</returns>
        static int[] ReadSizeOfSLAU()
        {
            bool can = true;
            int[] size = new int[2];
            while (can)
            {
                string input = Console.ReadLine();
                int n;
                if (int.TryParse(input, out n))
                {
                    if (n <= 10 && n > 0)
                    {
                        size[0] = n;
                        size[1] = n;
                        can = false;
                    }
                    else
                    {
                        Console.WriteLine("Entered value is too big or not positive");
                    }
                }
                else
                {
                    Console.WriteLine("Incorrect value");
                }
            }
            return size;
        }
        /// <summary>
        /// Check if the matrix is square.
        /// </summary>
        /// <param name="size">Size of the matrix.</param>
        /// <returns>True if the matrix is square, false if not.</returns>
        static bool CheckSq(int[] size)
        {
            return size[1] == size[0];
        }
        /// <summary>
        /// Read the matrix by the user's way.
        /// </summary>
        /// <param name="size">The size of the matrix.</param>
        /// <param name="way">The way how to read the matrix.</param>
        /// <param name="mis">Show if there's some incorrect input.</param>
        /// <returns>The matrix.</returns>
        static double[,] ReadMatrix(int[] size, string way, ref bool mis)
        {
            double[,] mat = new double[size[0], size[1]];
            string[] input;
            string[] input2;
            // If user want to read the matrix from file.
            if (way == "2")
            {
                Console.WriteLine("Enter the name of the file.txt /file should be near the <program>.exe/");
                string name = Console.ReadLine();
                try
                {
                    input = File.ReadAllLines(name);
                    for (int i = 0; i < size[0]; i++)
                    {
                        input2 = input[i].Split();
                        if (input2.Length == size[1])
                        {
                            for (int j = 0; j < size[1]; j++)
                            {
                                if (!double.TryParse(input2[j], out mat[i, j]))
                                {
                                    mis = true;
                                    Console.WriteLine("Entered value was not correct");
                                    return mat;
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("The amount of numbers is not correct");
                            mis = true;
                            return mat;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    mis = true;
                }
            }
            // If user want to read the matrix from console.
            else if (way == "1")
            {
                Console.WriteLine("Enter the elements of the matrix:");
                for (int i = 0; i < size[0]; i++)
                {
                    input = Console.ReadLine().Split();
                    if (input.Length == size[1])
                    {
                        for (int j = 0; j < size[1]; j++)
                        {
                            if (!double.TryParse(input[j], out mat[i, j]))
                            {
                                mis = true;
                                Console.WriteLine("Entered value was not correct");
                                return mat;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("The amount of numbers is not correct");
                        mis = true;
                        return mat;
                    }
                }
            }
            // Random matrix.
            else
            {
                Random rnd = new Random();
                for(int i = 0; i < size[0]; i++)
                {
                    for(int j = 0; j < size[1]; j++)
                    {
                        mat[i, j] = Math.Round(Math.Pow(-1,rnd.Next(20)) * rnd.NextDouble() * 100, 2);
                    }
                }
            }
            return mat;
        }
        /// <summary>
        /// Read the row without variables (b).
        /// </summary>
        /// <param name="size">The number of variables.</param>
        /// <param name="mis">Show if there's some incorrect input.</param>
        /// <returns>The row without variables (b).</returns>
        static double[] ReadRes(int[] size, ref bool mis)
        {
            double[] mat = new double[size[0]];
            string[] input = Console.ReadLine().Split();
            if (input.Length == size[0])
            {
                for (int j = 0; j < size[1]; j++)
                {
                    if (!double.TryParse(input[j], out mat[j]))
                    {
                        mis = true;
                        Console.WriteLine("Entered value was not correct");
                        return mat;
                    }
                }
            }
            else
            {
                Console.WriteLine("The amount of numbers is not correct");
                mis = true;
                return mat;
            }
            return mat;
        }
        /// <summary>
        /// Find the trace of the matrix.
        /// </summary>
        /// <param name="mat">The matrix.</param>
        /// <param name="size">The size of the matrix.</param>
        /// <returns>The trace of the matrix.</returns>
        static double FindTrace(double[,] mat, int[] size)
        {
            double trace = 0;
            for (int i = 0; i < Math.Min(size[0], size[1]); i++)
            {
                trace += mat[i, i];
            }
            return trace;
        }
        /// <summary>
        /// Transpose the matrix.
        /// </summary>
        /// <param name="mat">The matrix.</param>
        /// <param name="size">The size of the matrix.</param>
        /// <returns>The transposed matrix.</returns>
        static double[,] Transpose(double[,] mat, int[] size)
        {
            double[,] tmat = new double[size[1], size[0]];
            for (int i = 0; i < size[0]; i++)
            {
                for (int j = 0; j < size[1]; j++)
                {
                    tmat[j, i] = mat[i, j];
                }
            }
            return tmat;
        }
        /// <summary>
        /// Show the matrix.
        /// </summary>
        /// <param name="mat">The matrix.</param>
        /// <param name="size">The size of the matrix.</param>
        static void ShowMatrix(double[,] mat, int[] size)
        {
            for (int i = 0; i < size[0]; i++)
            {
                for (int j = 0; j < size[1]; j++)
                {
                    Console.Write(mat[i, j] + " ");
                }
                Console.WriteLine();
            }
        }
        /// <summary>
        /// Add two matrix.
        /// </summary>
        /// <param name="mat">The matrix.</param>
        /// <param name="mat2">The matrix2.</param>
        /// <param name="size">The size of two matrixes.</param>
        /// <returns>The matrix which is add of two matrixes.</returns>
        static double[,] SummaMatrix(double[,] mat, double[,] mat2, int[] size)
        {
            double[,] res = new double[size[0], size[1]];
            for (int i = 0; i < size[0]; i++)
            {
                for (int j = 0; j < size[1]; j++)
                {
                    res[i, j] = mat[i, j] + mat2[i, j];
                }
            }
            return res;
        }
        /// <summary>
        /// Subtract two matrix.
        /// </summary>
        /// <param name="mat">The matrix.</param>
        /// <param name="mat2">The matrix2.</param>
        /// <param name="size">The size of two matrixes.</param>
        /// <returns>The matrix which is subtract of two matrixes.</returns>
        static double[,] DifMatrix(double[,] mat, double[,] mat2, int[] size)
        {
            double[,] res = new double[size[0], size[1]];
            for (int i = 0; i < size[0]; i++)
            {
                for (int j = 0; j < size[1]; j++)
                {
                    res[i, j] = mat[i, j] - mat2[i, j];
                }
            }
            return res;
        }
        /// <summary>
        /// Multiply two matrix.
        /// </summary>
        /// <param name="mat">The first matrix.</param>
        /// <param name="mat2">The second matrix.</param>
        /// <param name="size">The size of the first matrix.</param>
        /// <param name="size2">The size of the second matrix.</param>
        /// <returns>The matrix which is multiply of two matrixes.</returns>
        static double[,] MultMatOnMat(double[,] mat, double[,] mat2, int[] size, int[] size2)
        {
            double[,] res = new double[size[0], size2[1]];
            for (int i = 0; i < size[0]; i++)
            {
                for (int j = 0; j < size2[1]; j++)
                {
                    for (int l = 0; l < size[1]; l++)
                    {
                        res[i, j] += mat[i, l] * mat2[l, j];
                    }
                }
            }
            return res;
        }
        /// <summary>
        /// Multiply matrix on the koef.
        /// </summary>
        /// <param name="mat">The matrix.</param>
        /// <param name="size">The size of the matrix.</param>
        /// <param name="k">The koef.</param>
        /// <returns>The matrix which is multiply matrix on the koef.</returns>
        static double[,] MultMatOnNum(double[,] mat, int[] size, double k)
        {
            double[,] res = new double[size[0], size[1]];
            for (int i = 0; i < size[0]; i++)
            {
                for (int j = 0; j < size[1]; j++)
                {
                    res[i, j] = mat[i, j] * k;
                }
            }
            return res;
        }
        /// <summary>
        /// Check can the user multiply two matrix.
        /// </summary>
        /// <param name="size1">The size of the first matrix.</param>
        /// <param name="size2">The size of the second matrix.</param>
        /// <returns>True if sizes is correct, false if not.</returns>
        static bool CheckSizesToMult(int[] size1, int[] size2)
        {
            return size1[1] == size2[0];
        }
        /// <summary>
        /// Find the determinate of the matrix.
        /// </summary>
        /// <param name="mat">The matrix.</param>
        /// <param name="size">The size of the matrix.</param>
        /// <returns>The determinate of the matrix.</returns>
        static double Det(double[,] mat, int[] size)
        {
            double det = 0;
            // If the matrix's size is 1x1, determinate of this matrix is this matrix.
            if (size[0] == 1)
            {
                det = mat[0, 0];
            }
            else
            {
                int[] nSize = { size[0] - 1, size[1] - 1 };
                // Decomposition on the column.
                for (int i = 0; i < size[0]; i++)
                {
                    det += Math.Pow(-1, i) * mat[i, 0] * Det(DeleteIJ(mat, i, 0, size), nSize);
                }
            }
            return det;
        }
        /// <summary>
        /// Delete the line i and the column j from the matrix.
        /// </summary>
        /// <param name="mat">The matrix.</param>
        /// <param name="i_">The number of line.</param>
        /// <param name="j_">The number of column.</param>
        /// <param name="size">The size of the matrix.</param>
        /// <returns>The matrix without the line i and the column j from the matrix.</returns>
        static double[,] DeleteIJ(double[,] mat, int i_, int j_, int[] size)
        {
            double[,] res = new double[size[0] - 1, size[1] - 1];
            int count = 0;
            for (int i = 0; i < size[0]; i++)
            {
                for (int j = 0; j < size[1]; j++)
                {
                    if (!(i == i_ || j == j_))
                    {
                        res[count / (size[0] - 1), count % (size[0] - 1)] = mat[i, j];
                        count++;
                    }
                }
            }
            return res;
        }
        /// <summary>
        /// Find solutions of matrix (method of Kramer).
        /// </summary>
        /// <param name="mat">The matrix.</param>
        /// <param name="res">The column without variables.</param>
        /// <param name="size">The size of the matrix.</param>
        /// <returns>The array of the solution of a SLAE.</returns>
        static double[] SLAU(double[,] mat, double[] res, int[] size)
        {
            double[] result = new double[size[0]];
            double det = Det(mat, size);
            double newDet;
            for (int i = 0; i < size[0]; i++)
            {
                newDet = Det(ChangeMatrix(mat, res, i, size), size);
                result[i] = newDet / det;
            }
            return result;
        }
        /// <summary>
        /// Change the column j on another column.
        /// </summary>
        /// <param name="mat">The matrix.</param>
        /// <param name="toChange">The column what must be on matrix.</param>
        /// <param name="j_">The number of column to change.</param>
        /// <param name="size">The size of the matrix.</param>
        /// <returns>The matrix with new j column.</returns>
        static double[,] ChangeMatrix(double[,] mat, double[] toChange, int j_, int[] size)
        {
            double[,] result = new double[size[0], size[1]];
            int count = 0;
            for (int i = 0; i < size[0]; i++)
            {
                for (int j = 0; j < size[1]; j++)
                {
                    if (j == j_)
                    {
                        result[i, j] = toChange[count];
                        count++;
                    }
                    else
                    {
                        result[i, j] = mat[i, j];
                    }
                }
            }
            return result;
        }
        static void Main(string[] args)
        {
            bool alive = true;
            ShowInstruction();
            while (alive)
            {
                Console.Clear();
                ShowCommands();
                string command = Console.ReadLine();
                bool mis, mis2;
                int[] size, size2;
                double[,] matrix, matrix2;
                string way;
                switch (command)
                {
                    case "1":
                        mis = false;
                        Console.WriteLine("Enter the size of the matrix (from 1 to 10):");
                        size = ReadSizeOfMatrix();
                        // We can find the trace of matrix if only matrix is square.
                        if (CheckSq(size))
                        {
                            Console.WriteLine("Choose a way to read the matrix");
                            ShowWaysToRead();
                            way = Console.ReadLine();
                            matrix = ReadMatrix(size, way, ref mis);
                            // If all input values is correct.
                            if (!mis)
                            {
                                Console.WriteLine("Your matrix:");
                                ShowMatrix(matrix, size);
                                Console.WriteLine();
                                Console.WriteLine($"Trace of the matrix = {FindTrace(matrix, size)}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Matrix is not square");
                        }
                        break;
                    case "2":
                        mis = false;
                        Console.WriteLine("Enter the size of the matrix (from 1 to 10):");
                        size = ReadSizeOfMatrix();
                        Console.WriteLine("Choose a way to read the matrix");
                        ShowWaysToRead();
                        way = Console.ReadLine();
                        matrix = ReadMatrix(size, way, ref mis);
                        // If all input values is correct.
                        if (!mis)
                        {
                            double[,] tMatrix = Transpose(matrix, size);
                            // Make the size of the transposed matrix.
                            int[] tSize = { size[1], size[0] };
                            Console.WriteLine("Your matrix:");
                            ShowMatrix(matrix, size);
                            Console.WriteLine();
                            Console.WriteLine("The transposed matrix is:");
                            ShowMatrix(tMatrix, tSize);
                        }
                        break;
                    case "3":
                        mis = false;
                        Console.WriteLine("Enter the sizes of both matrices (from 1 to 10):");
                        size = ReadSizeOfMatrix();
                        Console.WriteLine("Choose a way to read the matrix");
                        ShowWaysToRead();
                        way = Console.ReadLine();
                        matrix = ReadMatrix(size, way, ref mis);
                        // If all input values is correct.
                        if (!mis)
                        {
                            Console.WriteLine("Choose a way to read the matrix");
                            ShowWaysToRead();
                            way = Console.ReadLine();
                            matrix2 = ReadMatrix(size, way, ref mis);
                            // If all input values is correct.
                            if (!mis)
                            {
                                double[,] summMatrix = SummaMatrix(matrix, matrix2, size);
                                Console.WriteLine("Your the first matrix:");
                                ShowMatrix(matrix, size);
                                Console.WriteLine();
                                Console.WriteLine("Your the second matrix:");
                                ShowMatrix(matrix2, size);
                                Console.WriteLine();
                                Console.WriteLine("Resulting matrix:");
                                ShowMatrix(summMatrix, size);
                            }
                        }
                        break;
                    case "4":
                        mis = false;
                        Console.WriteLine("Enter the sizes of both matrices (from 1 to 10):");
                        size = ReadSizeOfMatrix();
                        Console.WriteLine("Choose a way to read the matrix");
                        ShowWaysToRead();
                        way = Console.ReadLine();
                        matrix = ReadMatrix(size, way, ref mis);
                        // If all input values is correct.
                        if (!mis)
                        {
                            Console.WriteLine("Choose a way to read the matrix");
                            ShowWaysToRead();
                            way = Console.ReadLine();
                            matrix2 = ReadMatrix(size, way, ref mis);
                            // If all input values is correct.
                            if (!mis)
                            {
                                double[,] difMatrix = DifMatrix(matrix, matrix2, size);
                                Console.WriteLine("Your the first matrix:");
                                ShowMatrix(matrix, size);
                                Console.WriteLine();
                                Console.WriteLine("Your the second matrix:");
                                ShowMatrix(matrix2, size);
                                Console.WriteLine();
                                Console.WriteLine("Resulting matrix:");
                                ShowMatrix(difMatrix, size);
                            }
                        }
                        break;
                    case "5":
                        mis = false;
                        Console.WriteLine("Enter the size of the first matrix (from 1 to 10):");
                        size = ReadSizeOfMatrix();
                        Console.WriteLine("Choose a way to read the matrix");
                        ShowWaysToRead();
                        way = Console.ReadLine();
                        matrix = ReadMatrix(size, way, ref mis);
                        // If all input values is correct.
                        if (!mis)
                        {
                            mis2 = false;
                            Console.WriteLine("Enter the size of the second matrix (from 1 to 10):");
                            size2 = ReadSizeOfMatrix();
                            // If we can multiply this two matrixes.
                            if (CheckSizesToMult(size, size2))
                            {
                                Console.WriteLine("Choose a way to read the matrix");
                                ShowWaysToRead();
                                way = Console.ReadLine();
                                matrix2 = ReadMatrix(size2, way, ref mis);
                                // If all input values is correct.
                                if (!mis2)
                                {
                                    double[,] multMatrix = MultMatOnMat(matrix, matrix2, size, size2);
                                    Console.WriteLine("Your the first matrix:");
                                    ShowMatrix(matrix, size);
                                    Console.WriteLine();
                                    Console.WriteLine("Your the second matrix:");
                                    ShowMatrix(matrix2, size2);
                                    Console.WriteLine();
                                    Console.WriteLine("Resulting matrix:");
                                    // Make the size of the new matrix.
                                    int[] multSize = { size[0], size2[1] };
                                    ShowMatrix(multMatrix, multSize);
                                }
                            }
                        }
                        break;
                    case "6":
                        mis = false;
                        Console.WriteLine("Enter the size of the matrix (from 1 to 10):");
                        size = ReadSizeOfMatrix();
                        Console.WriteLine("Choose a way to read the matrix");
                        ShowWaysToRead();
                        way = Console.ReadLine();
                        matrix = ReadMatrix(size, way, ref mis);
                        // If all input values is correct.
                        if (!mis)
                        {
                            Console.WriteLine("Enter the coefficient:");
                            string input = Console.ReadLine();
                            double k;
                            // If the koef is correct.
                            if (double.TryParse(input, out k))
                            {
                                double[,] multMatrixK = MultMatOnNum(matrix, size, k);
                                Console.WriteLine("Your matrix:");
                                ShowMatrix(matrix, size);
                                Console.WriteLine();
                                Console.WriteLine("Resulting matrix:");
                                ShowMatrix(multMatrixK, size);
                            }
                        }
                        break;
                    case "7":
                        mis = false;
                        Console.WriteLine("Enter the size of the matrix (from 1 to 10):");
                        size = ReadSizeOfMatrix();
                        Console.WriteLine("Choose a way to read the matrix");
                        ShowWaysToRead();
                        way = Console.ReadLine();
                        matrix = ReadMatrix(size, way, ref mis);
                        // If all input values is correct.
                        if (!mis)
                        {
                            Console.WriteLine("Your matrix:");
                            ShowMatrix(matrix, size);
                            Console.WriteLine();
                            Console.WriteLine($"The determinant is {Det(matrix, size)}");
                        }
                        break;
                    case "8":
                        mis = false;
                        mis2 = false;
                        Console.WriteLine("Enter the size of the matrix (number of variables) ONLY ONE NUMBER:\n" +
                            "Remember that the number of columns must be equal to the number of variables");
                        size = ReadSizeOfSLAU();
                        Console.WriteLine("Choose a way to read the matrix");
                        ShowWaysToRead();
                        way = Console.ReadLine();
                        matrix = ReadMatrix(size, way, ref mis);
                        Console.WriteLine("Enter the row without variables (b):");
                        double[] res = ReadRes(size, ref mis2);
                        // If all input values is correct.
                        if (!mis && !mis2)
                        {
                            Console.WriteLine("Your system:");
                            ShowMatrix(matrix, size);
                            Console.WriteLine();
                            // SLAE has one solution is only determinate of the matrix is not 0.
                            if (Det(matrix, size) != 0)
                            {
                                double[] result = SLAU(matrix, res, size);
                                Console.WriteLine("Solved SLAE:");
                                for (int i = 0; i < size[0]; i++)
                                {
                                    Console.WriteLine($"x{i+1} = {result[i]%19}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("SLAE has 0 or infinitely many solutions.");
                            }
                        }
                        break;
                    case "9":
                        alive = false;
                        break;
                    default:
                        Console.WriteLine("This command is not avaliable");
                        break;
                }
                Console.WriteLine("Press any key to continue\n\n");
                Console.ReadKey();
            }
        }
    }
}
