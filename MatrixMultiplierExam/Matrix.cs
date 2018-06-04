using System;
using System.Collections.Generic;
using System.Linq;

namespace MatrixMultiplierExam
{
    public class Matrix
    {
        public Matrix(int[,] values)
        {
            _values = values;
        }

        public Matrix(IList<IList<int>> matrixCells)
        {
            if (!matrixCells.Any())
            {
                throw new ArgumentException(MatrixDoesNotContainAnyLines);
            }
            if (matrixCells.Any(e => e.Count != matrixCells[0].Count))
            {
                throw new ArgumentException(RowLengthsVary);
            }
            var rowCount = matrixCells.Count;
            var colCount = matrixCells[0].Count;
            _values = new int[rowCount, colCount];
            foreach (var rowIndex in Enumerable.Range(0, rowCount))
            {
                foreach (var colIndex in Enumerable.Range(0, colCount))
                {
                    _values[rowIndex, colIndex] = matrixCells[rowIndex][colIndex];
                }
            }
        }

        private readonly int[,] _values;

        public int RowCount => _values.GetLength(0);
        public int ColumnCount => _values.GetLength(1);

        public int this[int rowIndex, int columnIndex]
        {
            get
            {
                CheckRowIndex(rowIndex);
                CheckColumnIndex(columnIndex);
                return _values[rowIndex, columnIndex];
            }
        }

        public IEnumerable<int> this[int rowIndex]
        {
            get
            {
                CheckRowIndex(rowIndex);
                return _values.Cast<int>().Skip(rowIndex * ColumnCount).Take(ColumnCount);
            }
        }

        public static Matrix operator +(Matrix left, Matrix right)
        {
            CheckRowAndColumnCountsEqual(left, right);
            return PerformOneElementAtATimeOperation(left, right, (a, b) => a + b);
        }
        public static Matrix operator -(Matrix left, Matrix right)
        {
            CheckRowAndColumnCountsEqual(left, right);
            return PerformOneElementAtATimeOperation(left, right, (a, b) => a - b);
        }

        /// <summary>
        /// This algorithm is O(n³). Solutions exist with a complexity of O(n^2.3728639) and higher.
        /// </summary>
        public static Matrix operator *(Matrix left, Matrix right)
        {
            CheckRowAndColumnCountsForMatrixMultiplication(left, right);
            return PerformMatrixMultiplication(left, right);
        }

        public Matrix GetTransposed()
        {
            var result = new int[ColumnCount, RowCount];
            foreach (var sourceRowIndex in Enumerable.Range(0, RowCount))
            {
                foreach (var sourceColIndex in Enumerable.Range(0, ColumnCount))
                {
                    result[sourceColIndex, sourceRowIndex] = _values[sourceRowIndex, sourceColIndex];
                }
            }
            return new Matrix(result);
        }

        private void CheckColumnIndex(int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= ColumnCount)
            {
                throw new ArgumentOutOfRangeException(nameof(columnIndex),
                    string.Format(IndexMustBeInTheRangeOf, ColumnCount));
            }
        }

        private void CheckRowIndex(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= RowCount)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex),
                    string.Format(IndexMustBeInTheRangeOf, RowCount));
            }
        }

        private static void CheckRowAndColumnCountsEqual(Matrix left, Matrix right)
        {
            if (left.RowCount != right.RowCount || left.ColumnCount != right.ColumnCount)
            {
                throw new ArgumentOutOfRangeException(nameof(right), RowAndColumnCountsMustMatch);
            }
        }

        private static void CheckRowAndColumnCountsForMatrixMultiplication(Matrix left, Matrix right)
        {
            if (left.ColumnCount != right.RowCount)
            {
                throw new ArgumentOutOfRangeException(nameof(right), RowAndColumnCountsMustSatisfyMatrixMultiplicationConstraints);
            }
        }

        /// <summary>
        /// This algorithm is O(n³). Solutions exist with a complexity of O(n^2.3728639) and higher.
        /// </summary>
        private static Matrix PerformMatrixMultiplication(Matrix left, Matrix right)
        {
            var resultColumnCount = right.ColumnCount;
            var resultRowCount = left.RowCount;
            var addendCount = left.ColumnCount;
            var result = new int[resultRowCount, resultColumnCount];
            foreach (var resultRowIndex in Enumerable.Range(0, resultRowCount))
            {
                foreach (var resultColIndex in Enumerable.Range(0, resultColumnCount))
                {
                    foreach (var addendIndex in Enumerable.Range(0, addendCount))
                    {
                        result[resultRowIndex, resultColIndex] += left[resultRowIndex, addendIndex] * right[addendIndex, resultColIndex];
                    }
                }
            }
            return new Matrix(result);
        }

        private static Matrix PerformOneElementAtATimeOperation(Matrix left, Matrix right, Func<int, int, int> operation)
        {
            var rowCount = left.RowCount;
            var colCount = left.ColumnCount;
            var result = new int[rowCount, colCount];
            foreach (var rowIndex in Enumerable.Range(0, rowCount))
            {
                foreach (var colIndex in Enumerable.Range(0, colCount))
                {
                    result[rowIndex, colIndex] = operation(left[rowIndex, colIndex], right[rowIndex, colIndex]);
                }
            }
            return new Matrix(result);
        }

        private const string IndexMustBeInTheRangeOf = "Index must be in the range of [0, {0})";
        private const string MatrixDoesNotContainAnyLines = "A matrix does not contain any lines";
        private const string RowAndColumnCountsMustMatch = "left and right matrix must have the same number of rows and the same number of columns.";
        private const string RowAndColumnCountsMustSatisfyMatrixMultiplicationConstraints = "left matrix must have the same number of columns as right matrix has rows, for a successful multiplication";
        private const string RowLengthsVary = "Matrix has rows with various lengths";
    }
}