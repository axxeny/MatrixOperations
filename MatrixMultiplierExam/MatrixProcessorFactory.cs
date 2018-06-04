using System.Collections.Generic;
using MatrixMultiplierExam.MatrixProcessors;

namespace MatrixMultiplierExam
{
    internal static class MatrixProcessorFactory
    {
        public static IMatrixProcessor GetProcessor(string name)
        {
            return Processors.ContainsKey(name) ? Processors[name] : null;
        }

        private static readonly IDictionary<string, IMatrixProcessor> Processors = new Dictionary<string, IMatrixProcessor>
        {
            ["multiply"] = new MultiplicationProcessor(),
            ["add"] = new AdditionProcessor(),
            ["subtract"] = new SubtractionProcessor(),
            ["transpose"] = new TransposeProcessor()
        };
    }
}