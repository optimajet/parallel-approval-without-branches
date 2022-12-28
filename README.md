# Parallel approval without branches

## Introduction

Sometimes we need to get simultaneous document approval from several people. The document should be approved simultaneously by all parties.
Use parallel branches for this. The people with the authority to approve the document, however, frequently change between each stage.
To make it possible, you will need to create the scheme with complex parallel branches or use scheme generation.
Your project will thereafter get more challenging.

To consistently describe this logic, there is a rather simple solution, though.
In this case you won't have to use parallel branches or the generation of the scheme.
This solution is based on the ability of the engine to save any objects in [process parameters](https://workflowengine.io/documentation/scheme/parameters),
and pass the parameters to code actions. You can modify and use it in your own solution.
There is also an [approval plugin](https://workflowengine.io/documentation/plugins/approvalplugin) that simplifies this task and provides a ready-made template.

**See the full article [here](https://workflowengine.io/documentation/parallel-approval-without-branches).**