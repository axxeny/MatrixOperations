﻿using System.Collections.Generic;
using System.Linq;

namespace MatrixMultiplierExam.MatrixProcessors
{
    class AdditionProcessor : IMatrixProcessor
    {
        public IEnumerable<Matrix> Process(IEnumerable<Matrix> input)
        {
            return new[] {input.Aggregate((acc, @new) => acc + @new)};
        }
    }
}