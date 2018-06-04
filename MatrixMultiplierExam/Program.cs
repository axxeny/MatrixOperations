using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AxxenyUtilities.Helpers;

namespace MatrixMultiplierExam
{
    class Program
    {
        private static int _filesProcessed;
        private static long _bytesProcessed;
        private static long _fileSizesSum;
        private static int _fileCount;

        static void Main(string[] args)
        {
            var isArgCountValid = args.Length == 2;
            if (!isArgCountValid)
            {
                Console.Error.WriteLine("Invalid argument count. A folder name is expected.");
                ConsoleHelper.WaitForEnterPressed();
                return;
            }

            var folderName = args[1];
            var folderExists = Directory.Exists(folderName);
            if (!folderExists)
            {
                Console.Error.WriteLine("Folder does not exist");
                ConsoleHelper.WaitForEnterPressed();
                return;
            }

            var fileNames = Directory
                .EnumerateFiles(folderName)
                .Select(Path.GetFileName)
                .AsICollectionOrToList();
            if (!fileNames.Any())
            {
                Console.Error.WriteLine("Folder does not contain any files.");
                ConsoleHelper.WaitForEnterPressed();
                return;
            }

            if (FileNamesContainResults(fileNames))
            {
                var userPermittedOverwrite = AskOverwrite();
                if (!userPermittedOverwrite)
                {
                    Console.Error.WriteLine("You chose not to overwrite");
                    ConsoleHelper.WaitForEnterPressed();
                    return;
                }
            }

            var fileSizes = fileNames
                .ToDictionary(e => e, e => new FileInfo(Path.Combine(folderName, e)).Length);
            _fileCount = fileNames.Count;
            _fileSizesSum = fileSizes.Values.Sum();
            _filesProcessed = 0;
            _bytesProcessed = 0L;
            WriteProgress();
            var consoleSemaphore = new SemaphoreSlim(1);
            Parallel.ForEach(fileNames, fileName =>
            {
                var filePath = Path.Combine(folderName, fileName);
                var resultPath = Path.Combine(folderName, GetResultFileName(fileName));
                TryProcessFileAndWriteErrorToConsole(filePath, resultPath);
                Interlocked.Increment(ref _filesProcessed);
                Interlocked.Add(ref _bytesProcessed, fileSizes[fileName]);
                consoleSemaphore.Lock(TimeSpan.FromSeconds(2), WriteProgress);
            });
        }

        private static bool AskOverwrite()
        {
            Console.WriteLine("Folder contains one or more result files. Do you wish to overwrite them? [y]/n");
            while (true)
            {
                var result = Console.ReadKey();
                if (result.Key == ConsoleKey.Y || result.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine("Yes");
                    return true;
                }
                if (result.Key == ConsoleKey.N)
                {
                    Console.WriteLine("No");
                    return false;
                }
            }
        }

        private static bool FileNamesContainResults(IEnumerable<string> fileNames)
        {
            var orderedFileNames = fileNames
                .OrderBy(e=>e)
                .AsOrToList();
            return orderedFileNames
                .Select(e => orderedFileNames.BinarySearch(GetResultFileName(e)))
                .Any(resultFileBinarySearch => resultFileBinarySearch >= 0);
        }

        private static string GetResultFileName(string sourceFileName)
        {
            return $"{Path.GetFileNameWithoutExtension(sourceFileName)}_result.txt";
        }

        private static void WriteProgress()
        {
            ConsoleHelper.CleanCurrentLine();
            Console.Write($"Processed {_filesProcessed} of {_fileCount} files ({(_filesProcessed/_fileCount):P1}), {_bytesProcessed} of {_fileSizesSum} ({(_bytesProcessed/_fileSizesSum):P1}");
        }

        private static void TryProcessFileAndWriteErrorToConsole(string filePath, string resultPath)
        {
            try
            {
                FileProcessor.ProcessFile(filePath, resultPath);
            }
            catch (Exception ex)
            {
                ConsoleHelper.CleanCurrentLine();
                Console.Error.WriteLine(ex.Message);
            }
        }
    }
}
