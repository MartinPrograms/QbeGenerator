using System.Diagnostics;

namespace QbeGenerator;

public static class QbeCompiler
{
    /// <summary>
    /// Compiles the given QBE IR to assembly code.
    /// Which can be used by Clang or other assemblers to produce a binary.
    /// </summary>
    public static bool Compile(string qbeIr, out string assembly, out string? qbeError)
    {
        if (!File.Exists("qbe"))
        {
            throw new FileNotFoundException(
                "QBE compiler not found. Please ensure 'qbe' is in the current directory or in your PATH.");
        }
        
        var tempFile = Path.GetTempFileName();
        var asmOut = Path.Combine(Path.GetTempPath(), "output.s");
        try
        {
            File.WriteAllText(tempFile, qbeIr);
            // qbe -o output.s input.qbe
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "qbe",
                    Arguments = $"-o \"{asmOut}\" \"{tempFile}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            qbeError = process.StandardError.ReadToEnd();
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                assembly = null;
                return false; // Compilation failed
            }
            assembly = File.ReadAllText(asmOut);
            qbeError = string.IsNullOrEmpty(qbeError) ? null : qbeError.Trim();
            return true; // Compilation succeeded
        }
        finally
        {
            File.Delete(tempFile);
            if (File.Exists(asmOut))
            {
                File.Delete(asmOut); // Clean up the generated assembly file
            }
        }
    }
}