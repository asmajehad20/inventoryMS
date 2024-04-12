using System;
using System.Diagnostics;
using System.Threading.Tasks;
using inventoryMSCli.CLI;

namespace inventoryMSCli
{
    class Program
    {
        /// <summary>
        /// Entry point of the application.
        /// </summary>
        static async Task Main(string[] args)
        { 
            Task cliTask = Task.Run(() => MainPage.Run());
            StartProcess();
            await cliTask;
        }

        /// <summary>
        /// Starts the API process.
        /// </summary>
        static void StartProcess()
        {
            string apiProjectPath = Path.GetFullPath(@"..\..\..\..\..\inventoryMSApi\inventoryMSApi.csproj");
            ProcessStartInfo startInfo = new()
            {
                FileName = "cmd",
                Arguments = $"/k dotnet run --project \"{apiProjectPath}\"",
                UseShellExecute = true,
                WorkingDirectory = Path.GetDirectoryName(apiProjectPath)
            };

            Process.Start(startInfo);
        }
    }
}
