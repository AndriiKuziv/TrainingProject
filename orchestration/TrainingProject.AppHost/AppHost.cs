using Couchbase.Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);
var config = builder.Configuration;

var couchbasePassword = builder.AddParameter("couchbase-password", config["Couchbase:Password"], secret: true);

var couchbasedb = builder
    .AddCouchbase("couchbase", password: couchbasePassword)
    .WithManagementPort(8091);

var bucket = couchbasedb.AddBucket("training-bucket")
    .WithScope(scopeName: "training-scope", collections: ["users"]);

couchbasedb.ApplicationBuilder.Eventing.Subscribe<ConnectionStringAvailableEvent>(
    couchbasedb.Resource,
    async (@event, ct) =>
    {
        try
        {
            var connectionString = await couchbasedb
                        .Resource.ConnectionStringExpression.GetValueAsync(ct)
                        .ConfigureAwait(false);

            Console.WriteLine($"Connecting to Couchbase with connection string: {connectionString}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error obtaining Couchbase connection string: {ex.Message}");
        }
    }
);

var trainingApiProject = builder.AddProject<Projects.TrainingProject_Api>("training-project-api")
    .WithReference(couchbasedb)
    .WaitFor(couchbasedb);

trainingApiProject
    .WithEnvironment("Couchbase__UserName", couchbasedb.Resource.UserNameReference.ValueExpression)
    .WithEnvironment("Couchbase__Password", couchbasedb.Resource.PasswordParameter);

builder.Build().Run();