using System.IO;

namespace TestTaskAlt.Sortet
{
    internal sealed class TargetFile
    {
        public TargetFile(string targetFilePath)
        {
            TargetFilePath = targetFilePath;
        }

        public int RunsCount { get; set; }
        public string TargetFilePath { get; private set; }

        public void MakeFinal()
        {
            const string outputFileName = "output.txt";
            if (File.Exists(outputFileName))
            {
                File.Delete(outputFileName);
            }

            File.Move(TargetFilePath, outputFileName);
        }
    }
}