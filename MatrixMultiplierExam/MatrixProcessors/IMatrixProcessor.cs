using System.Collections;
using System.Collections.Generic;

namespace MatrixMultiplierExam.MatrixProcessors
{
    interface IMatrixProcessor
    {
        IEnumerable<Matrix> Process(IEnumerable<Matrix> input);
    }
}