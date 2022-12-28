using System.Xml.Linq;
using OptimaJet.Workflow.Core.Builder;
using OptimaJet.Workflow.Core.Runtime;
using OptimaJet.Workflow.DbPersistence;
using OptimaJet.Workflow.Plugins.ApproversProvider;
using Approvers = WorkflowLib.Approvers;

public static class WorkflowInit
{
    private static readonly Lazy<WorkflowRuntime> LazyRuntime = new Lazy<WorkflowRuntime>(InitWorkflowRuntime);

    public static WorkflowRuntime Runtime
    {
        get { return LazyRuntime.Value; }
    }

    public static string ConnectionString { get; set; }

    private static WorkflowRuntime InitWorkflowRuntime()
    {
        if (string.IsNullOrEmpty(ConnectionString))
        {
            throw new Exception("Please init ConnectionString before calling the Runtime!");
        }
        // TODO If you have a license key, you have to register it here
        //WorkflowRuntime.RegisterLicense("your license key text");
        
        var dbProvider = new MSSQLProvider(ConnectionString);

        var builder = new WorkflowBuilder<XElement>(
            dbProvider,
            new OptimaJet.Workflow.Core.Parser.XmlWorkflowParser(),
            dbProvider
        ).WithDefaultCache();

        var runtime = new WorkflowRuntime()
            .WithBuilder(builder)
            .WithPersistenceProvider(dbProvider)
            .EnableCodeActions()
            .RegisterAssemblyForCodeActions(typeof(Approvers).Assembly)
            .SwitchAutoUpdateSchemeBeforeGetAvailableCommandsOn()
            .AsSingleServer();
        
        runtime.Start();

        return runtime;
    }
}