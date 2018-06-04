using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AxxenyUtilities.Helpers;

namespace MatrixMultiplierExam
{
    class FileProcessor
    {

        public static void ProcessFile(string filePath, string resultPath)
        {
            var fileContents = File.ReadAllLines(filePath);
            string[] result;
            try
            {
                result = ProcessFileContents(fileContents);
            }
            catch (Exception ex)
            {
                File.WriteAllText(resultPath, ex.Message);
                throw;
            }
            File.WriteAllLines(resultPath, result);
        }

        private static string[] ProcessFileContents(string[] fileContents)
        {
            var gapLineIndexes = fileContents
                .Select((e, i) => new {IsEmpty = !e.Any(), Index = i})
                .Where(e => e.IsEmpty)
                .Select(e => e.Index)
                .AsIListOrToList();
            if (!gapLineIndexes.Any() || gapLineIndexes[0] != 1)
            {
                throw new InvalidOperationException(GapError);
            }
            var operationString = fileContents[0];
            var operation = MatrixProcessorFactory.GetProcessor(operationString);
            if (operation == null)
            {
                throw new InvalidOperationException(UnknownOperationError);
            }
            var matrices = ParseMatrices(fileContents, gapLineIndexes);
            var results = operation.Process(matrices);
            return ToLines(results);
        }

        private static string[] ToLines(IEnumerable<Matrix> resultMatrices)
        {
            var matricesLines = resultMatrices
                .Select(result => Enumerable.Range(0, result.RowCount)
                    .Select(rowIndex => string
                        .Join(" ", result[rowIndex]
                            .Select(e => e.ToString()))));
            var emptyLineArray = new[] {""};
            var lines = matricesLines.Aggregate((acc, @new) => acc.Concat(emptyLineArray).Concat(@new));
            return lines.AsOrToArray();
        }

        private static IEnumerable<Matrix> ParseMatrices(IReadOnlyCollection<string> fileContents, IList<int> gapLineIndexes)
        {
            var matrices = new List<Matrix>(gapLineIndexes.Count);
            foreach (var matrixIndex in Enumerable.Range(0, gapLineIndexes.Count))
            {
                var nextGapOrEndfile = matrixIndex + 1 < gapLineIndexes.Count
                    ? gapLineIndexes[matrixIndex + 1]
                    : fileContents.Count;
                var prevGapLineIndex = gapLineIndexes[matrixIndex];
                var matrixRowCount = nextGapOrEndfile - 1 - prevGapLineIndex;
                var matrixLines = fileContents
                    .Skip(prevGapLineIndex + 1)
                    .Take(matrixRowCount);
                var matrixCells = ParseMatrix(matrixLines);
                if (!matrixCells.Any())
                {
                    throw new InvalidOperationException(MatrixDoesNotContainAnyLines);
                }
                if (matrixCells.Any(e => e.Count != matrixCells[0].Count))
                {
                    throw new InvalidOperationException(RowLengthsVary);
                }
                var matrix = new Matrix(matrixCells);
                matrices.Add(matrix);
            }
            return matrices;
        }

        private static IList<IList<long>> ParseMatrix(IEnumerable<string> matrixLines)
        {
            IList<IList<long>> matrixCells;
            try
            {
                matrixCells = matrixLines.Select(e => e.Split(' ').Select(long.Parse).AsIListOrToList()).AsIListOrToList();
            }
            catch (Exception)
            {
                throw new InvalidOperationException(CouldNotParseAMatrixError);
            }
            return matrixCells;
        }

        private const string GapError = "Could not parse file: the second line is not a gap.";
        private const string UnknownOperationError = "Unknown operation in the first line";
        private const string CouldNotParseAMatrixError = "Could not parse a matrix";
        private const string MatrixDoesNotContainAnyLines = "A matrix does not contain any lines";
        private const string RowLengthsVary = "Matrix has rows with various lengths";
    }
}