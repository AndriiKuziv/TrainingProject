namespace TrainingProject.Infrastructure.Configuration;

public class CouchbaseSettings
{
    public string BucketName { get; set; }

    public string ScopeName { get; set; }

    public Dictionary<string, string> Collections { get; set; }
}
