using OptimaJet.Workflow.Core.Runtime;
using OptimaJet.Workflow.Core.Subprocess;

WorkflowInit.ConnectionString = "Data Source=(local);Initial Catalog=wfe_sample;User ID=sa;Password=1";
string currentUser = string.Empty;
const string schemeCode = "SimpleWF";
Guid? processId;
Main();

void Main()
{
    Console.WriteLine("Operation:");
    Console.WriteLine("0 - Set current user");
    Console.WriteLine("1 - CreateInstance");
    Console.WriteLine("2 - GetAvailableCommands");
    Console.WriteLine("3 - ExecuteCommand");
    Console.WriteLine("4 - GetAvailableState");
    Console.WriteLine("5 - SetState");
    Console.WriteLine("6 - DeleteProcess");
    Console.WriteLine("9 - Exit");

    Console.WriteLine("The process is not created.");
    CreateInstance();

    do
    {
        if (processId.HasValue)
        {
            Console.WriteLine("ProcessId = '{0}'. CurrentState: {1}, CurrentActivity: {2}",
                processId,
                WorkflowInit.Runtime.GetCurrentStateName(processId.Value),
                WorkflowInit.Runtime.GetCurrentActivityName(processId.Value));

            var processTree = WorkflowInit.Runtime.GetProcessInstancesTree(processId.Value);
            if (processTree != null)
            {
                var current = processTree.Root;
                var level = "->";
                WriteSubprocesses(current, level);
            }
        }

        if (!string.IsNullOrEmpty(currentUser))
            Console.WriteLine("Current user = {0}.", currentUser);
        else
            Console.WriteLine("Current user is undefined.");

        Console.Write("Enter code of operation:");

        char operation = (Console.ReadLine() ?? string.Empty).FirstOrDefault();

        switch (operation)
        {
            case '0':
                SetUser();
                break;
            case '1':
                CreateInstance();
                break;
            case '2':
                GetAvailableCommands();
                break;
            case '3':
                ExecuteCommand();
                break;
            case '4':
                GetAvailableState();
                break;
            case '5':
                SetState();
                break;
            case '6':
                DeleteProcess();
                break;
            case '9':
                return;
            default:
                Console.WriteLine("Unknown code. Please, repeat.");
                break;
        }

        Console.WriteLine();
    } while (true);
}

void WriteSubprocesses(ProcessInstancesTree current, string level)
{
    foreach (var child in current.Children)
    {
        Console.WriteLine("{0}SubProcessId = '{1}'. CurrentState: {2}, CurrentActivity: {3}", level, child.Id,
            WorkflowInit.Runtime.GetCurrentStateName(child.Id), WorkflowInit.Runtime.GetCurrentActivityName(child.Id));

        WriteSubprocesses(child, level + "->");
    }
}

void SetUser()
{
    Console.Write("Enter user's id: ");
    var readLine = Console.ReadLine();
    if (readLine != null)
    {
        currentUser = readLine.Trim();
    }
}

void CreateInstance()
{
    processId = Guid.NewGuid();

    try
    {
        WorkflowInit.Runtime.CreateInstance(schemeCode, processId.Value);
        Console.WriteLine("CreateInstance - OK.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("CreateInstance - Exception: {0}, {1}", ex.Message,
            ex.InnerException != null ? ex.InnerException.Message : string.Empty);
        processId = null;
    }
}

void GetAvailableCommands()
{
    if (processId == null)
    {
        Console.WriteLine("The process isn't created. Please, create process instance.");
        return;
    }

    var commands = WorkflowInit.Runtime.GetAvailableCommands(processId.Value, currentUser).ToList();

    Console.WriteLine("Available commands:");
    if (!commands.Any())
    {
        Console.WriteLine("Not found!");
    }
    else
    {
        foreach (var command in commands)
        {
            Console.WriteLine("- {0} (LocalizedName:{1}, Classifier:{2})", command.CommandName, command.LocalizedName,
                command.Classifier);
        }
    }
}

void ExecuteCommand()
{
    if (processId == null)
    {
        Console.WriteLine("The process isn't created. Please, create process instance.");
        return;
    }

    WorkflowCommand? command = null;

    do
    {
        GetAvailableCommands();
        Console.Write("Enter command:");
        var readLine = Console.ReadLine();
        if (readLine != null)
        {
            var commandName = readLine.ToLower().Trim();
            if (commandName == string.Empty)
                return;

            command = WorkflowInit.Runtime.GetAvailableCommands(processId.Value, currentUser)
                .FirstOrDefault(c => c.CommandName.Trim().ToLower() == commandName);
        }

        if (command == null)
            Console.WriteLine("The command isn't found.");
    } while (command == null);

    WorkflowInit.Runtime.ExecuteCommand(command, currentUser, currentUser);
    Console.WriteLine("ExecuteCommand - OK.");
}

void GetAvailableState()
{
    if (processId == null)
    {
        Console.WriteLine("The process isn't created. Please, create process instance.");
        return;
    }

    var states = WorkflowInit.Runtime.GetAvailableStateToSet(processId.Value, Thread.CurrentThread.CurrentCulture);

    Console.WriteLine("Available state to set:");

    var workflowStates = states as WorkflowState[] ?? states.ToArray();

    if (!workflowStates.Any())
    {
        Console.WriteLine("Not found!");
    }
    else
    {
        foreach (var state in workflowStates)
        {
            Console.WriteLine("- {0}", state.Name);
        }
    }
}

void SetState()
{
    if (processId == null)
    {
        Console.WriteLine("The process isn't created. Please, create process instance.");
        return;
    }

    WorkflowState? state;
    do
    {
        GetAvailableState();
        Console.Write("Enter state:");
        var readLine = Console.ReadLine();
        var stateName = readLine?.ToLower().Trim();
        if (string.IsNullOrEmpty(stateName))
            return;

        state = WorkflowInit.Runtime.GetAvailableStateToSet(processId.Value, Thread.CurrentThread.CurrentCulture)
            .FirstOrDefault(c => c.Name.Trim().ToLower() == stateName);

        if (state == null)
            Console.WriteLine("The state isn't found.");
        else
            break;
    } while (true);

    WorkflowInit.Runtime.SetState(new SetStateParams(processId.Value, state.Name)
    {
        IdentityId = currentUser,
        ImpersonatedIdentityId = currentUser
    });
    Console.WriteLine("SetState - OK.");
}

void DeleteProcess()
{
    if (processId == null)
    {
        Console.WriteLine("The process isn't created. Please, create process instance.");
        return;
    }

    WorkflowInit.Runtime.DeleteInstance(processId.Value);
    Console.WriteLine("DeleteProcess - OK.");
    processId = null;
}