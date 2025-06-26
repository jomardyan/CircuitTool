#nullable enable
using System;
using System.Numerics;

namespace CircuitTool.Mathematics
{
    /// <summary>
    /// Provides matrix operations for circuit analysis, particularly for nodal analysis
    /// </summary>
    public static class MatrixOperations
    {
        /// <summary>
        /// Represents a complex matrix for AC circuit analysis
        /// </summary>
        public class ComplexMatrix
        {
            private readonly Complex[,] _matrix;
            
            public ComplexMatrix(int rows, int columns)
            {
                if (rows <= 0 || columns <= 0)
                    throw new ArgumentException("Matrix dimensions must be positive");
                
                Rows = rows;
                Columns = columns;
                _matrix = new Complex[rows, columns];
            }
            
            public ComplexMatrix(Complex[,] matrix)
            {
                _matrix = matrix ?? throw new ArgumentNullException(nameof(matrix));
                Rows = matrix.GetLength(0);
                Columns = matrix.GetLength(1);
            }
            
            public int Rows { get; }
            public int Columns { get; }
            
            public Complex this[int row, int col]
            {
                get => _matrix[row, col];
                set => _matrix[row, col] = value;
            }
            
            /// <summary>
            /// Creates an identity matrix
            /// </summary>
            public static ComplexMatrix Identity(int size)
            {
                var matrix = new ComplexMatrix(size, size);
                for (int i = 0; i < size; i++)
                {
                    matrix[i, i] = Complex.One;
                }
                return matrix;
            }
            
            /// <summary>
            /// Matrix multiplication
            /// </summary>
            public static ComplexMatrix operator *(ComplexMatrix a, ComplexMatrix b)
            {
                if (a.Columns != b.Rows)
                    throw new ArgumentException("Matrix dimensions incompatible for multiplication");
                
                var result = new ComplexMatrix(a.Rows, b.Columns);
                
                for (int i = 0; i < a.Rows; i++)
                {
                    for (int j = 0; j < b.Columns; j++)
                    {
                        Complex sum = Complex.Zero;
                        for (int k = 0; k < a.Columns; k++)
                        {
                            sum += a[i, k] * b[k, j];
                        }
                        result[i, j] = sum;
                    }
                }
                
                return result;
            }
            
            /// <summary>
            /// Matrix addition
            /// </summary>
            public static ComplexMatrix operator +(ComplexMatrix a, ComplexMatrix b)
            {
                if (a.Rows != b.Rows || a.Columns != b.Columns)
                    throw new ArgumentException("Matrix dimensions must match for addition");
                
                var result = new ComplexMatrix(a.Rows, a.Columns);
                
                for (int i = 0; i < a.Rows; i++)
                {
                    for (int j = 0; j < a.Columns; j++)
                    {
                        result[i, j] = a[i, j] + b[i, j];
                    }
                }
                
                return result;
            }
            
            /// <summary>
            /// Matrix subtraction
            /// </summary>
            public static ComplexMatrix operator -(ComplexMatrix a, ComplexMatrix b)
            {
                if (a.Rows != b.Rows || a.Columns != b.Columns)
                    throw new ArgumentException("Matrix dimensions must match for subtraction");
                
                var result = new ComplexMatrix(a.Rows, a.Columns);
                
                for (int i = 0; i < a.Rows; i++)
                {
                    for (int j = 0; j < a.Columns; j++)
                    {
                        result[i, j] = a[i, j] - b[i, j];
                    }
                }
                
                return result;
            }
            
            /// <summary>
            /// Scalar multiplication
            /// </summary>
            public static ComplexMatrix operator *(Complex scalar, ComplexMatrix matrix)
            {
                var result = new ComplexMatrix(matrix.Rows, matrix.Columns);
                
                for (int i = 0; i < matrix.Rows; i++)
                {
                    for (int j = 0; j < matrix.Columns; j++)
                    {
                        result[i, j] = scalar * matrix[i, j];
                    }
                }
                
                return result;
            }
            
            /// <summary>
            /// Scalar multiplication
            /// </summary>
            public static ComplexMatrix operator *(ComplexMatrix matrix, Complex scalar)
            {
                return scalar * matrix;
            }
            
            /// <summary>
            /// Transposes the matrix
            /// </summary>
            public ComplexMatrix Transpose()
            {
                var result = new ComplexMatrix(Columns, Rows);
                
                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < Columns; j++)
                    {
                        result[j, i] = this[i, j];
                    }
                }
                
                return result;
            }
            
            /// <summary>
            /// Calculates the determinant (for square matrices only)
            /// </summary>
            public Complex Determinant()
            {
                if (Rows != Columns)
                    throw new InvalidOperationException("Determinant can only be calculated for square matrices");
                
                return CalculateDeterminant(_matrix, Rows);
            }
            
            private static Complex CalculateDeterminant(Complex[,] matrix, int n)
            {
                if (n == 1)
                    return matrix[0, 0];
                
                if (n == 2)
                    return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];
                
                Complex det = Complex.Zero;
                var sign = 1;
                
                for (int j = 0; j < n; j++)
                {
                    var subMatrix = GetSubMatrix(matrix, 0, j, n);
                    det += sign * matrix[0, j] * CalculateDeterminant(subMatrix, n - 1);
                    sign *= -1;
                }
                
                return det;
            }
            
            private static Complex[,] GetSubMatrix(Complex[,] matrix, int excludeRow, int excludeCol, int n)
            {
                var subMatrix = new Complex[n - 1, n - 1];
                int subRow = 0;
                
                for (int i = 0; i < n; i++)
                {
                    if (i == excludeRow) continue;
                    
                    int subCol = 0;
                    for (int j = 0; j < n; j++)
                    {
                        if (j == excludeCol) continue;
                        
                        subMatrix[subRow, subCol] = matrix[i, j];
                        subCol++;
                    }
                    subRow++;
                }
                
                return subMatrix;
            }
            
            /// <summary>
            /// Solves the linear system Ax = b using Gaussian elimination
            /// </summary>
            public static Complex[] SolveLinearSystem(ComplexMatrix a, Complex[] b)
            {
                if (a.Rows != a.Columns)
                    throw new ArgumentException("Matrix must be square for linear system solving");
                
                if (a.Rows != b.Length)
                    throw new ArgumentException("Matrix rows must equal vector length");
                
                int n = a.Rows;
                var augmented = new Complex[n, n + 1];
                
                // Create augmented matrix [A|b]
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        augmented[i, j] = a[i, j];
                    }
                    augmented[i, n] = b[i];
                }
                
                // Forward elimination
                for (int i = 0; i < n; i++)
                {
                    // Find pivot
                    int maxRow = i;
                    for (int k = i + 1; k < n; k++)
                    {
                        if (augmented[k, i].Magnitude > augmented[maxRow, i].Magnitude)
                            maxRow = k;
                    }
                    
                    // Swap rows
                    if (maxRow != i)
                    {
                        for (int j = 0; j <= n; j++)
                        {
                            (augmented[i, j], augmented[maxRow, j]) = (augmented[maxRow, j], augmented[i, j]);
                        }
                    }
                    
                    // Make all rows below this one 0 in current column
                    for (int k = i + 1; k < n; k++)
                    {
                        if (augmented[i, i].Magnitude < 1e-10)
                            throw new InvalidOperationException("Matrix is singular or near-singular");
                        
                        var factor = augmented[k, i] / augmented[i, i];
                        for (int j = i; j <= n; j++)
                        {
                            augmented[k, j] -= factor * augmented[i, j];
                        }
                    }
                }
                
                // Back substitution
                var solution = new Complex[n];
                for (int i = n - 1; i >= 0; i--)
                {
                    solution[i] = augmented[i, n];
                    for (int j = i + 1; j < n; j++)
                    {
                        solution[i] -= augmented[i, j] * solution[j];
                    }
                    solution[i] /= augmented[i, i];
                }
                
                return solution;
            }
        }
        
        /// <summary>
        /// Creates a nodal admittance matrix for circuit analysis
        /// </summary>
        /// <param name="impedances">Array of impedance values</param>
        /// <param name="connections">Array of node connections (from, to)</param>
        /// <param name="numNodes">Number of nodes in the circuit</param>
        /// <returns>Nodal admittance matrix</returns>
        public static ComplexMatrix CreateNodalAdmittanceMatrix(
            Complex[] impedances, 
            (int from, int to)[] connections, 
            int numNodes)
        {
            if (impedances.Length != connections.Length)
                throw new ArgumentException("Impedances and connections arrays must have the same length");
            
            var admittanceMatrix = new ComplexMatrix(numNodes, numNodes);
            
            for (int i = 0; i < impedances.Length; i++)
            {
                var admittance = 1.0 / impedances[i];
                var (from, to) = connections[i];
                
                // Self-admittances (diagonal elements)
                admittanceMatrix[from, from] += admittance;
                admittanceMatrix[to, to] += admittance;
                
                // Mutual admittances (off-diagonal elements)
                admittanceMatrix[from, to] -= admittance;
                admittanceMatrix[to, from] -= admittance;
            }
            
            return admittanceMatrix;
        }
        
        /// <summary>
        /// Solves a nodal analysis problem
        /// </summary>
        /// <param name="admittanceMatrix">Nodal admittance matrix</param>
        /// <param name="currentVector">Current injection vector</param>
        /// <returns>Node voltages</returns>
        public static Complex[] SolveNodalAnalysis(ComplexMatrix admittanceMatrix, Complex[] currentVector)
        {
            return ComplexMatrix.SolveLinearSystem(admittanceMatrix, currentVector);
        }
    }
}
