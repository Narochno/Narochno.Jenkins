# Narochno.Jenkins [![Build status](https://ci.appveyor.com/api/projects/status/imperehby0ri5rmn/branch/master?svg=true)](https://ci.appveyor.com/project/Narochno/narochno-jenkins/branch/master)
A simple Jenkins client, providing a C# wrapper around the default Jenkins API.

## Example Usage
```csharp
var config = new JenkinsConfig
{
    JenkinsUrl = "<your jenkins instance>"
};

# Optionally dispose
using (var jenkinsClient = new JenkinsClient(config))
{
    # Get the master so we can loop all jobs
    var master = await jenkinsClient.GetMaster();
    
    foreach (var job in master.Jobs)
    {
        # Grab the full job metadata, including builds
        var jobInfo = await jenkinsClient.GetJob(job.Name);

        foreach (var build in jobInfo.Builds)
        {
            # Get the full build information
            var buildInfo = await jenkinsClient.GetBuild(job.Name, build.Number.ToString());

            if (buildInfo.ChangeSet.Items.Count > 0)
            {
                # Write the first change set item to the console
                Console.WriteLine($"Got build {buildInfo} from {buildInfo.ChangeSet.Kind} revision {buildInfo.ChangeSet.Items.FirstOrDefault()}");
            }
        }
    }
}
```
