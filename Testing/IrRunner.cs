using System.Diagnostics;

namespace Testing;

public class IrRunner
{
    private string _ir;

    public IrRunner(string ir)
    {
        _ir = ir;
    }

    public void Run(string dir)
    {
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        
        string qbeFile = Path.Combine(dir, "output.qbe");
        File.WriteAllText(qbeFile, _ir);

        long totaltime = 0;
        
        Stopwatch sw = Stopwatch.StartNew();
        string asmFile = Path.Combine(dir, "output.s");
        RunProcess("qbe", $"-o {asmFile} {qbeFile}");
        sw.Stop();
        Console.WriteLine($"Qbe took {sw.Elapsed.Milliseconds}ms to compile into ASM.");
        totaltime += sw.Elapsed.Milliseconds;
        sw = Stopwatch.StartNew();
        string exeFile = Path.Combine(dir, "main");
        RunProcess("clang", $"-o {exeFile} {asmFile} -O2 -lm");
        sw.Stop();
        Console.WriteLine($"Clang took {sw.Elapsed.Milliseconds}ms to compile into a binary.");
        totaltime += sw.Elapsed.Milliseconds;
        
        Console.WriteLine($"Total compile time: {totaltime}");
        
        if (File.Exists(exeFile))
        {
            Console.WriteLine("Running executable...");
            sw = Stopwatch.StartNew();
            var exitCode = RunProcess(exeFile, "");
            sw.Stop();
            Console.WriteLine($"Program finished executing in {sw.Elapsed.Milliseconds}ms with exit code {exitCode}!");
        }
    }

    static int RunProcess(string filename, string args)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = filename,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();

        string stdout = process.StandardOutput.ReadToEnd();
        string stderr = process.StandardError.ReadToEnd();

        process.WaitForExit();

        if (!string.IsNullOrEmpty(stdout))
            Console.WriteLine(stdout);
        if (!string.IsNullOrEmpty(stderr))
            Console.Error.WriteLine(stderr);

        return process.ExitCode;
    }
}