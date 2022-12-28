using Newtonsoft.Json;

namespace WorkflowLib;

public class Approvers
{
    // Must be а public property to serialize to JSON
    public Dictionary<string, bool> ApproversDictionary { get; set; }
  
    public Approvers(List<string> ids)
    {
        if (ids == null)
        {
            // Required for correct deserialization from JSON
            ApproversDictionary = new Dictionary<string, bool>();
        }
        else
        {
            ApproversDictionary = ids.ToDictionary(id => id, id => false);
        }
    }
  
    [JsonIgnore]
    public bool IsApproved
    {
        get { return ApproversDictionary.Values.All(v => v); }
    }
  
    public void Approve(string id)
    {
        ApproversDictionary[id] = true;
    }
  
    public void Reset()
    {
        foreach (var k in ApproversDictionary.Keys)
        {
            ApproversDictionary[k] = false;
        }
    }
  
    public List<string> GetAvailiableApprovers()
    {
        return ApproversDictionary.Where(s => !s.Value).Select(s => s.Key).ToList();
    }
}