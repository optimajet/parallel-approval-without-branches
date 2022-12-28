using OptimaJet.Workflow.Core.Model;

namespace WorkflowLib;

public class ApproversProvider
{
    public static Approvers GetApprovers(ProcessInstance processInstance, string name)
    {
        switch (name)
        {
            case "Stage1": 
                return new Approvers(new List<string>{"user1", "user2"});
            case "Stage2":
                return new Approvers(new List<string> {"user3", "user4", "user5"});
            default:
                return new Approvers(new List<string>());
        }
    }
}