using System.Collections.Generic;
using System.Linq;

namespace MatrixMultiplierExam.MatrixProcessors
{
    class TransposeProcessor : IMatrixProcessor
    {
        public IEnumerable<Matrix> Process(IEnumerable<Matrix> input)
        {
            return input.Select(e=>e.GetTransposed());
        }
    }
}