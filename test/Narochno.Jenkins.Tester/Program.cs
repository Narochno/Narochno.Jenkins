using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Narochno.Jenkins.Tester
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                DoWork().Wait();
            }
            catch (AggregateException e)
            {
                throw e.Flatten();
            }
        }

        public static async Task DoWork()
        {
            var provider = new ServiceCollection()
                .AddJenkins(new JenkinsConfig { JenkinsUrl = "<your jenkins instance>" })
                .BuildServiceProvider();

            var jenkinsClient = provider.GetService<IJenkinsClient>();

            Console.WriteLine("Press any key to loop through all builds and all jobs on your jenkins instance");
            Console.ReadLine();

            var master = await jenkinsClient.GetMaster();

            foreach (var job in master.Jobs.Where(j => j.Name.Contains("BuildTriggeringOtherBuilds")))
            {
                var jobInfo = await jenkinsClient.GetJob(job.Name);

                var build = jobInfo.Builds.Single(b => b.Number == jobInfo.Builds.Max(x => x.Number));
                var buildInfo = await jenkinsClient.GetBuild(job.Name, build.Number.ToString());
                var buildGraphInfo = await jenkinsClient.GetBuildGraph(job.Name, build.Number.ToString());

                foreach (var node in buildGraphInfo.Nodes)
                {
                    var subBuildInfo = await jenkinsClient.GetBuild(node.JobName, node.BuildNumber.ToString());
                    Console.WriteLine($"Got build {subBuildInfo}. Result {subBuildInfo.Result}");

                    if (subBuildInfo.ChangeSet.Items.Count > 0)
                    {
                        Console.WriteLine($"Got build {subBuildInfo} from {subBuildInfo.ChangeSet.Kind} revision {subBuildInfo.ChangeSet.Items.FirstOrDefault()}");
                    }
                }
            }

            foreach (var job in master.Jobs)
            {
                var jobInfo = await jenkinsClient.GetJob(job.Name);

                foreach (var build in jobInfo.Builds)
                {
                    var buildInfo = await jenkinsClient.GetBuild(job.Name, build.Number.ToString());

                    if (buildInfo.ChangeSet.Items.Count > 0)
                    {
                        Console.WriteLine($"Got build {buildInfo} from {buildInfo.ChangeSet.Kind} revision {buildInfo.ChangeSet.Items.FirstOrDefault()}");
                    }
                }
            }

            foreach (var view in master.Views)
            {
                var viewInfo = await jenkinsClient.GetView(view.Name);

                Console.WriteLine($"Got view {viewInfo.Name} with {viewInfo.Jobs.Count} jobs");
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }
    }
}
