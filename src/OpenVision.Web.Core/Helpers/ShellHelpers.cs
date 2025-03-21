using System.Diagnostics;

namespace OpenVision.Web.Core.Helpers;

/// <summary>
/// Helper methods for executing shell commands.
/// </summary>
public static class ShellHelpers
{
    /// <summary>
    /// Executes a Bash shell command and returns the output.
    /// </summary>
    /// <param name="cmd">The Bash command to execute.</param>
    /// <returns>The output of the Bash command.</returns>
    public static string Bash(this string cmd)
    {
        // Escape double quotes in the command
        string escapedCmd = cmd.Replace("\"", "\\\"");

        // Check if Bash is available
        if (File.Exists("/bin/bash"))
        {
            using Process process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"{escapedCmd}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result;
        }
        else
        {
            throw new InvalidOperationException("Bash is not available on this system.");
        }
    }
}
