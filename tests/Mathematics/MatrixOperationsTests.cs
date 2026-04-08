using System;
using System.Numerics;
using NUnit.Framework;
using CircuitTool.Mathematics;

namespace CircuitTool.Tests.Mathematics
{
    [TestFixture]
    public class MatrixOperationsTests
    {
        private const double Tolerance = 1e-10;

        [Test]
        public void ComplexMatrix_Constructor_ValidDimensions_CreatesMatrix()
        {
            // Arrange & Act
            var matrix = new MatrixOperations.ComplexMatrix(3, 4);
            
            // Assert
            Assert.That(matrix.Rows, Is.EqualTo(3));
            Assert.That(matrix.Columns, Is.EqualTo(4));
        }

        [Test]
        public void ComplexMatrix_Constructor_InvalidDimensions_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new MatrixOperations.ComplexMatrix(0, 5));
            Assert.Throws<ArgumentException>(() => new MatrixOperations.ComplexMatrix(5, 0));
            Assert.Throws<ArgumentException>(() => new MatrixOperations.ComplexMatrix(-1, 5));
        }

        [Test]
        public void ComplexMatrix_Constructor_WithArray_CreatesCorrectMatrix()
        {
            // Arrange
            var array = new Complex[,] { { new Complex(1, 2), new Complex(3, 4) }, { new Complex(5, 6), new Complex(7, 8) } };
            
            // Act
            var matrix = new MatrixOperations.ComplexMatrix(array);
            
            // Assert
            Assert.That(matrix.Rows, Is.EqualTo(2));
            Assert.That(matrix.Columns, Is.EqualTo(2));
            Assert.That(matrix[0, 0], Is.EqualTo(new Complex(1, 2)));
            Assert.That(matrix[1, 1], Is.EqualTo(new Complex(7, 8)));
        }

        [Test]
        public void ComplexMatrix_Constructor_NullArray_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() => new MatrixOperations.ComplexMatrix(null));
        }

        [Test]
        public void Identity_CreatesCorrectIdentityMatrix()
        {
            // Arrange & Act
            var identity = MatrixOperations.ComplexMatrix.Identity(3);
            
            // Assert
            Assert.That(identity.Rows, Is.EqualTo(3));
            Assert.That(identity.Columns, Is.EqualTo(3));
            
            // Check diagonal elements are 1
            for (int i = 0; i < 3; i++)
            {
                Assert.That(identity[i, i], Is.EqualTo(Complex.One));
            }
            
            // Check off-diagonal elements are 0
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (i != j)
                    {
                        Assert.That(identity[i, j], Is.EqualTo(Complex.Zero));
                    }
                }
            }
        }

        [Test]
        public void MatrixMultiplication_ValidMatrices_ReturnsCorrectResult()
        {
            // Arrange
            var a = new MatrixOperations.ComplexMatrix(2, 3);
            a[0, 0] = new Complex(1, 0); a[0, 1] = new Complex(2, 0); a[0, 2] = new Complex(3, 0);
            a[1, 0] = new Complex(4, 0); a[1, 1] = new Complex(5, 0); a[1, 2] = new Complex(6, 0);
            
            var b = new MatrixOperations.ComplexMatrix(3, 2);
            b[0, 0] = new Complex(7, 0); b[0, 1] = new Complex(8, 0);
            b[1, 0] = new Complex(9, 0); b[1, 1] = new Complex(10, 0);
            b[2, 0] = new Complex(11, 0); b[2, 1] = new Complex(12, 0);
            
            // Act
            var result = a * b;
            
            // Assert
            Assert.That(result.Rows, Is.EqualTo(2));
            Assert.That(result.Columns, Is.EqualTo(2));
            
            // Expected: [58, 64; 139, 154]
            Assert.That(result[0, 0].Real, Is.EqualTo(58).Within(Tolerance));
            Assert.That(result[0, 1].Real, Is.EqualTo(64).Within(Tolerance));
            Assert.That(result[1, 0].Real, Is.EqualTo(139).Within(Tolerance));
            Assert.That(result[1, 1].Real, Is.EqualTo(154).Within(Tolerance));
        }

        [Test]
        public void MatrixMultiplication_IncompatibleDimensions_ThrowsException()
        {
            // Arrange
            var a = new MatrixOperations.ComplexMatrix(2, 3);
            var b = new MatrixOperations.ComplexMatrix(2, 2); // Should be 3x2 to be compatible
            
            // Act & Assert
            Assert.Throws<ArgumentException>(() => { var result = a * b; });
        }

        [Test]
        public void MatrixAddition_ValidMatrices_ReturnsCorrectResult()
        {
            // Arrange
            var a = new MatrixOperations.ComplexMatrix(2, 2);
            a[0, 0] = new Complex(1, 2); a[0, 1] = new Complex(3, 4);
            a[1, 0] = new Complex(5, 6); a[1, 1] = new Complex(7, 8);
            
            var b = new MatrixOperations.ComplexMatrix(2, 2);
            b[0, 0] = new Complex(2, 1); b[0, 1] = new Complex(4, 3);
            b[1, 0] = new Complex(6, 5); b[1, 1] = new Complex(8, 7);
            
            // Act
            var result = a + b;
            
            // Assert
            Assert.That(result[0, 0], Is.EqualTo(new Complex(3, 3)));
            Assert.That(result[0, 1], Is.EqualTo(new Complex(7, 7)));
            Assert.That(result[1, 0], Is.EqualTo(new Complex(11, 11)));
            Assert.That(result[1, 1], Is.EqualTo(new Complex(15, 15)));
        }

        [Test]
        public void MatrixAddition_IncompatibleDimensions_ThrowsException()
        {
            // Arrange
            var a = new MatrixOperations.ComplexMatrix(2, 2);
            var b = new MatrixOperations.ComplexMatrix(3, 2);
            
            // Act & Assert
            Assert.Throws<ArgumentException>(() => { var result = a + b; });
        }

        [Test]
        public void MatrixSubtraction_ValidMatrices_ReturnsCorrectResult()
        {
            // Arrange
            var a = new MatrixOperations.ComplexMatrix(2, 2);
            a[0, 0] = new Complex(5, 6); a[0, 1] = new Complex(7, 8);
            a[1, 0] = new Complex(9, 10); a[1, 1] = new Complex(11, 12);
            
            var b = new MatrixOperations.ComplexMatrix(2, 2);
            b[0, 0] = new Complex(1, 2); b[0, 1] = new Complex(3, 4);
            b[1, 0] = new Complex(5, 6); b[1, 1] = new Complex(7, 8);
            
            // Act
            var result = a - b;
            
            // Assert
            Assert.That(result[0, 0], Is.EqualTo(new Complex(4, 4)));
            Assert.That(result[0, 1], Is.EqualTo(new Complex(4, 4)));
            Assert.That(result[1, 0], Is.EqualTo(new Complex(4, 4)));
            Assert.That(result[1, 1], Is.EqualTo(new Complex(4, 4)));
        }

        [Test]
        public void ScalarMultiplication_ReturnsCorrectResult()
        {
            // Arrange
            var matrix = new MatrixOperations.ComplexMatrix(2, 2);
            matrix[0, 0] = new Complex(1, 2); matrix[0, 1] = new Complex(3, 4);
            matrix[1, 0] = new Complex(5, 6); matrix[1, 1] = new Complex(7, 8);
            
            var scalar = new Complex(2, 1);
            
            // Act
            var result1 = scalar * matrix;
            var result2 = matrix * scalar;
            
            // Assert
            var expected00 = new Complex(1, 2) * new Complex(2, 1); // (1+2i)(2+1i) = 2+i+4i-2 = 0+5i
            Assert.That(result1[0, 0], Is.EqualTo(expected00));
            Assert.That(result2[0, 0], Is.EqualTo(expected00));
            
            // Test commutativity
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    Assert.That(result1[i, j], Is.EqualTo(result2[i, j]));
                }
            }
        }

        [Test]
        public void Transpose_ReturnsCorrectResult()
        {
            // Arrange
            var matrix = new MatrixOperations.ComplexMatrix(2, 3);
            matrix[0, 0] = new Complex(1, 2); matrix[0, 1] = new Complex(3, 4); matrix[0, 2] = new Complex(5, 6);
            matrix[1, 0] = new Complex(7, 8); matrix[1, 1] = new Complex(9, 10); matrix[1, 2] = new Complex(11, 12);
            
            // Act
            var transposed = matrix.Transpose();
            
            // Assert
            Assert.That(transposed.Rows, Is.EqualTo(3));
            Assert.That(transposed.Columns, Is.EqualTo(2));
            
            Assert.That(transposed[0, 0], Is.EqualTo(new Complex(1, 2)));
            Assert.That(transposed[1, 0], Is.EqualTo(new Complex(3, 4)));
            Assert.That(transposed[2, 0], Is.EqualTo(new Complex(5, 6)));
            Assert.That(transposed[0, 1], Is.EqualTo(new Complex(7, 8)));
            Assert.That(transposed[1, 1], Is.EqualTo(new Complex(9, 10)));
            Assert.That(transposed[2, 1], Is.EqualTo(new Complex(11, 12)));
        }

        [Test]
        public void Determinant_2x2Matrix_ReturnsCorrectValue()
        {
            // Arrange
            var matrix = new MatrixOperations.ComplexMatrix(2, 2);
            matrix[0, 0] = new Complex(3, 0); matrix[0, 1] = new Complex(8, 0);
            matrix[1, 0] = new Complex(4, 0); matrix[1, 1] = new Complex(6, 0);
            
            // Act
            var det = matrix.Determinant();
            
            // Assert
            // det = 3*6 - 8*4 = 18 - 32 = -14
            Assert.That(det.Real, Is.EqualTo(-14).Within(Tolerance));
            Assert.That(det.Imaginary, Is.EqualTo(0).Within(Tolerance));
        }

        [Test]
        public void Determinant_3x3Matrix_ReturnsCorrectValue()
        {
            // Arrange
            var matrix = new MatrixOperations.ComplexMatrix(3, 3);
            matrix[0, 0] = new Complex(2, 0); matrix[0, 1] = new Complex(-3, 0); matrix[0, 2] = new Complex(1, 0);
            matrix[1, 0] = new Complex(2, 0); matrix[1, 1] = new Complex(0, 0); matrix[1, 2] = new Complex(-1, 0);
            matrix[2, 0] = new Complex(1, 0); matrix[2, 1] = new Complex(4, 0); matrix[2, 2] = new Complex(5, 0);
            
            // Act
            var det = matrix.Determinant();
            
            // Assert
            // Manual calculation: 2*(0*5 - (-1)*4) - (-3)*(2*5 - (-1)*1) + 1*(2*4 - 0*1)
            // = 2*(0 + 4) + 3*(10 + 1) + 1*(8 - 0) = 8 + 33 + 8 = 49
            Assert.That(det.Real, Is.EqualTo(49).Within(Tolerance));
            Assert.That(det.Imaginary, Is.EqualTo(0).Within(Tolerance));
        }

        [Test]
        public void Determinant_NonSquareMatrix_ThrowsException()
        {
            // Arrange
            var matrix = new MatrixOperations.ComplexMatrix(2, 3);
            
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => matrix.Determinant());
        }

        [Test]
        public void SolveLinearSystem_SimpleSystem_ReturnsCorrectSolution()
        {
            // Arrange
            // System: 2x + 3y = 7, x - y = 1
            // Solution: x = 2, y = 1
            var a = new MatrixOperations.ComplexMatrix(2, 2);
            a[0, 0] = new Complex(2, 0); a[0, 1] = new Complex(3, 0);
            a[1, 0] = new Complex(1, 0); a[1, 1] = new Complex(-1, 0);
            
            var b = new Complex[] { new Complex(7, 0), new Complex(1, 0) };
            
            // Act
            var solution = MatrixOperations.ComplexMatrix.SolveLinearSystem(a, b);
            
            // Assert
            Assert.That(solution.Length, Is.EqualTo(2));
            Assert.That(solution[0].Real, Is.EqualTo(2).Within(Tolerance));
            Assert.That(solution[1].Real, Is.EqualTo(1).Within(Tolerance));
        }

        [Test]
        public void SolveLinearSystem_ComplexSystem_ReturnsCorrectSolution()
        {
            // Arrange
            // System with complex coefficients
            var a = new MatrixOperations.ComplexMatrix(2, 2);
            a[0, 0] = new Complex(1, 1); a[0, 1] = new Complex(2, 0);
            a[1, 0] = new Complex(0, 1); a[1, 1] = new Complex(1, -1);
            
            var b = new Complex[] { new Complex(3, 1), new Complex(1, 1) };
            
            // Act
            var solution = MatrixOperations.ComplexMatrix.SolveLinearSystem(a, b);
            
            // Assert
            Assert.That(solution.Length, Is.EqualTo(2));
            // Verify solution by substitution
            var verification1 = a[0, 0] * solution[0] + a[0, 1] * solution[1];
            var verification2 = a[1, 0] * solution[0] + a[1, 1] * solution[1];
            
            Assert.That((verification1 - b[0]).Magnitude, Is.LessThan(Tolerance));
            Assert.That((verification2 - b[1]).Magnitude, Is.LessThan(Tolerance));
        }

        [Test]
        public void SolveLinearSystem_NonSquareMatrix_ThrowsException()
        {
            // Arrange
            var a = new MatrixOperations.ComplexMatrix(2, 3);
            var b = new Complex[] { Complex.One, Complex.One };
            
            // Act & Assert
            Assert.Throws<ArgumentException>(() => MatrixOperations.ComplexMatrix.SolveLinearSystem(a, b));
        }

        [Test]
        public void SolveLinearSystem_MismatchedDimensions_ThrowsException()
        {
            // Arrange
            var a = new MatrixOperations.ComplexMatrix(2, 2);
            var b = new Complex[] { Complex.One }; // Should have 2 elements
            
            // Act & Assert
            Assert.Throws<ArgumentException>(() => MatrixOperations.ComplexMatrix.SolveLinearSystem(a, b));
        }

        [Test]
        public void CreateNodalAdmittanceMatrix_SimpleCircuit_ReturnsCorrectMatrix()
        {
            // Arrange
            // Simple circuit with 3 nodes and 2 resistors
            var impedances = new Complex[] { new Complex(2, 0), new Complex(4, 0) }; // 2Ω, 4Ω resistors
            var connections = new (int from, int to)[] { (0, 1), (1, 2) }; // R1 from node 0 to 1, R2 from node 1 to 2
            int numNodes = 3;
            
            // Act
            var admittanceMatrix = MatrixOperations.CreateNodalAdmittanceMatrix(impedances, connections, numNodes);
            
            // Assert
            Assert.That(admittanceMatrix.Rows, Is.EqualTo(3));
            Assert.That(admittanceMatrix.Columns, Is.EqualTo(3));
            
            // Check diagonal elements (self-admittances)
            Assert.That(admittanceMatrix[0, 0].Real, Is.EqualTo(0.5).Within(Tolerance)); // 1/2Ω
            Assert.That(admittanceMatrix[1, 1].Real, Is.EqualTo(0.75).Within(Tolerance)); // 1/2Ω + 1/4Ω = 0.5 + 0.25
            Assert.That(admittanceMatrix[2, 2].Real, Is.EqualTo(0.25).Within(Tolerance)); // 1/4Ω
            
            // Check off-diagonal elements (mutual admittances)
            Assert.That(admittanceMatrix[0, 1].Real, Is.EqualTo(-0.5).Within(Tolerance)); // -1/2Ω
            Assert.That(admittanceMatrix[1, 0].Real, Is.EqualTo(-0.5).Within(Tolerance)); // Symmetric
            Assert.That(admittanceMatrix[1, 2].Real, Is.EqualTo(-0.25).Within(Tolerance)); // -1/4Ω
            Assert.That(admittanceMatrix[2, 1].Real, Is.EqualTo(-0.25).Within(Tolerance)); // Symmetric
        }

        [Test]
        public void CreateNodalAdmittanceMatrix_MismatchedArrays_ThrowsException()
        {
            // Arrange
            var impedances = new Complex[] { new Complex(2, 0) };
            var connections = new (int from, int to)[] { (0, 1), (1, 2) }; // Different length
            
            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                MatrixOperations.CreateNodalAdmittanceMatrix(impedances, connections, 3));
        }

        [Test]
        public void SolveNodalAnalysis_ReturnsCorrectVoltages()
        {
            // Arrange
            // Create a simple circuit matrix and current vector
            var admittanceMatrix = new MatrixOperations.ComplexMatrix(2, 2);
            admittanceMatrix[0, 0] = new Complex(0.5, 0); // 1/2Ω
            admittanceMatrix[0, 1] = new Complex(-0.5, 0);
            admittanceMatrix[1, 0] = new Complex(-0.5, 0);
            admittanceMatrix[1, 1] = new Complex(0.5, 0);
            
            var currentVector = new Complex[] { new Complex(1, 0), new Complex(-1, 0) }; // 1A in, -1A out
            
            // Act
            var voltages = MatrixOperations.SolveNodalAnalysis(admittanceMatrix, currentVector);
            
            // Assert
            Assert.That(voltages.Length, Is.EqualTo(2));
            // For this symmetric case, both voltages should be equal in magnitude but opposite
            Assert.That(Math.Abs(voltages[0].Real + voltages[1].Real), Is.LessThan(Tolerance));
        }

        [Test]
        public void ComplexMatrix_IndexerAccess_WorksCorrectly()
        {
            // Arrange
            var matrix = new MatrixOperations.ComplexMatrix(2, 2);
            var testValue = new Complex(3.14, 2.71);
            
            // Act
            matrix[1, 0] = testValue;
            var retrievedValue = matrix[1, 0];
            
            // Assert
            Assert.That(retrievedValue, Is.EqualTo(testValue));
        }
    }
}
