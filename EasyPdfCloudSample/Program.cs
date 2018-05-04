/*
 * The MIT License
 *
 * Copyright 2015 BCL Technologies.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.IO;
using System.Threading.Tasks;

using Bcl.EasyPdfCloud;

namespace EasyPdfCloudApiSample
{
    class Program
    {
        //////////////////////////////////////////////////////////////////////
        //
        // VERY IMPORTANT!!!
        //
        // You must configure these variables before running this sample code.
        //
        //////////////////////////////////////////////////////////////////////

        // Client ID. You can get it from the developer page (you must be signed in)
        // https://www.easypdfcloud.com/developer
        static string clientId = "0ee8b4e9fb3c4b7fb69ec950856bdaec";

        // Client secret. You can get it from the developer page (you must be signed in)
        // https://www.easypdfcloud.com/developer
        static string clientSecret = "A1CD9D2B13BB6F5C36FDD10E9911245160AD8494CD1FAD12A545B00D85162CE3";

        // Workflow ID. You can get it by going to the workflow edit
        // view from a web browser (you must be signed in) and reading the URL
        // https://www.easypdfcloud.com/edit/<workflow_id>
        static string workflowId = "0000000005F7E110";

        //////////////////////////////////////////////////////////////////////

        static void CheckParameters(string workflow, string outDir, string inFile)
        {
            Console.WriteLine("--------------------------------------------------------");
            Console.WriteLine("App parameters:");
            Console.WriteLine();
            Console.WriteLine("clientId: " + Program.clientId);
            Console.WriteLine("clientSecret: " + (string.IsNullOrEmpty(Program.clientSecret) ? "" : "********"));
            Console.WriteLine("workflowId: " + workflow);
            Console.WriteLine("inputFilePath: " + inFile);
            Console.WriteLine("outputDir: " + outDir);
            Console.WriteLine("--------------------------------------------------------");
            Console.WriteLine();

            if (string.IsNullOrEmpty(Program.clientId))
            {
                throw new ArgumentException("clientId is not specified in the code!");
            }
            if (string.IsNullOrEmpty(Program.clientSecret))
            {
                throw new ArgumentException("clientSecret is not specified in the code!");
            }
            if (string.IsNullOrEmpty(workflow))
            {
                throw new ArgumentException("Workflow ID is not specified in the code!");
            }
            if (string.IsNullOrEmpty(inFile))
            {
                throw new ArgumentException("inputFilePath is not specified in the code!");
            }
            if (string.IsNullOrEmpty(outDir))
            {
                throw new ArgumentException("outputDir is not specified in the code!");
            }
            if (!File.Exists(inFile))
            {
                throw new ArgumentException("Input File does not Exist!");
            }
            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
            }
        }

        //////////////////////////////////////////////////////////////////////

        static async Task<string> ExecuteNewMergeJob(string clientId, string clientSecret, string workflowId, string inputFilePath, string outputDir)
        {
            // Create easyPDF Cloud client object
            using (var client = new Bcl.EasyPdfCloud.Client(clientId, clientSecret))
            {
                var inputFilePaths = new string[]
                {
                    inputFilePath,
                    inputFilePath,
                    inputFilePath
                };

                // Upload input file and start new job
                //using (var job = await client.StartNewJobAsync(workflowId, inputFilePath))
                using (var job = await client.StartNewJobForMergeTaskAsync(workflowId, inputFilePaths))
                {
                    Console.WriteLine("New job started (job ID: " + job.JobInfo.JobId + ")");
                    Console.WriteLine("Waiting for job execution completion...");

                    // Wait until job execution is completed
                    using (var outputFileData = await job.WaitForJobExecutionCompletionAsync())
                    {
                        var outputFileName = outputFileData.Name;
                        var outputFileSize = outputFileData.Stream.Length;
                        var outputFilePath = Path.Combine(outputDir, outputFileName);

                        Console.WriteLine("Job execution completed");
                        Console.WriteLine("Output file name: " + outputFileName);
                        Console.WriteLine("Output file size: " + outputFileSize.ToString("N0") + " bytes");

                        // Save output to file
                        Console.WriteLine("Saving to output directory...");
                        using (var outputFileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
                        {
                            await outputFileData.Stream.CopyToAsync(outputFileStream);
                        }

                        // Return output file path
                        return outputFilePath;
                    }
                }
            }
        }
        static async Task<string> ExecuteNewJob(string clientId, string clientSecret, string workflowId, string inputFilePath, string outputDir)
        {
            // Create easyPDF Cloud client object
            using (var client = new Bcl.EasyPdfCloud.Client(clientId, clientSecret))
            {
                // Upload input file and start new job
                using (var job = await client.StartNewJobAsync(workflowId, inputFilePath))
                {
                    Console.WriteLine("New job started (job ID: " + job.JobInfo.JobId +  ")");
                    Console.WriteLine("Waiting for job execution completion...");

                    // Wait until job execution is completed
                    using (var outputFileData = await job.WaitForJobExecutionCompletionAsync())
                    {
                        var outputFileName = outputFileData.Name;
                        var outputFileSize = outputFileData.Stream.Length;
                        var outputFilePath = Path.Combine(outputDir, outputFileName);

                        Console.WriteLine("Job execution completed");
                        Console.WriteLine("Output file name: " + outputFileName);
                        Console.WriteLine("Output file size: " + outputFileSize.ToString("N0") + " bytes");

                        // Save output to file
                        Console.WriteLine("Saving to output directory...");
                        using (var outputFileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
                        {
                            await outputFileData.Stream.CopyToAsync(outputFileStream);
                        }

                        // Return output file path
                        return outputFilePath;
                    }
                }
            }
        }

        //////////////////////////////////////////////////////////////////////

        static async Task StartAsync(string workflow, string outDir, string inFile)
        {
            try
            {
                Console.WriteLine("Executing new job...");

                // Start job execution
                var outputFilePath = await ExecuteNewJob(
                    Program.clientId,
                    Program.clientSecret,
                    workflow,
                    inFile,
                    outDir);

                Console.WriteLine("Output saved to: " + outputFilePath);
            }
            catch (EasyPdfCloudApiException e)
            {
                Console.WriteLine("\nAPI execution failed!\nError: " + e.Message);
                Console.WriteLine("\nStack trace: " + e.StackTrace);
            }
            catch (JobExecutionException e)
            {
                Console.WriteLine("\nJob execution failed!\nError: " + e.Message);
                Console.WriteLine("\nStack trace: " + e.StackTrace);
            }
            catch (ApiAuthorizationException e)
            {
                Console.WriteLine("\nAPI Authorization failed!\nError: " + e.Message);
                Console.WriteLine("\nStack trace: " + e.StackTrace);
            }
            catch (Exception e)
            {
                Console.WriteLine("\nUncaught exception!\nError: " + e);
            }

            Console.WriteLine();
        }

        //////////////////////////////////////////////////////////////////////

        static void Main(string[] args)
        {

            string appPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\";

            if (args.Length==3)
            {
                try
                {
                    CheckParameters(args[0], args[1], args[2]);

                    var task = StartAsync(args[0], args[1], args[2]);
                    task.Wait();
                    Console.WriteLine("Done");
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine();
                    return;
                }
            }
            else
            {
                try
                {
                    CheckParameters(Program.workflowId, appPath, appPath + "in.docx");

                    var task = StartAsync(Program.workflowId, appPath, appPath + "in.docx");
                    task.Wait();
                    Console.WriteLine("Done");
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine();
                    return;
                }
            }
            System.Threading.Thread.Sleep(60000);
        }

        //////////////////////////////////////////////////////////////////////
    }
}
