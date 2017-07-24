using System;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System.Collections.Generic;

namespace CreateWorkItem
{
    public class TfsHandler
    {

        void Query(WorkItemStore workItemStore, string selectedProject, string folder, List<string> listQueries)
        {
            var project = workItemStore.Projects[selectedProject];
            QueryHierarchy queryHierarchy = project.QueryHierarchy;
            var queryFolder = queryHierarchy as QueryFolder;
            QueryItem queryItem = queryFolder[folder];
            queryFolder = queryItem as QueryFolder;
            foreach (var item in queryFolder)
            {
                listQueries.Add(item.Name);
            }
        }

        internal static void CreateWorkItem(int srcWorkItemId, string tfsuri, string assignedtoname, string wiType,
            string titleprefix, string externalidsuffix, string area, string iterationPath)
        {
            //var tfsCreds = new TfsClientCredentials(new WindowsCredential(), false);
            using (TfsTeamProjectCollection tfs = new TfsTeamProjectCollection(new Uri(tfsuri)))
            {
                ;
                //TfsTeamProjectCollection targettfs = new TfsTeamProjectCollection(new Uri(tfsuri), tfsCreds);
                try
                {
                    Console.WriteLine("Authenticating...");
                    tfs.Authenticate();
                    Console.WriteLine("Authentication Succeeded.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.InnerException);
                    return;
                }

                Console.WriteLine("Getting WorkItemStore...");
                WorkItemStore wis = tfs.GetService<WorkItemStore>();
                Console.WriteLine("Getting TIA...");
                Project teamProject = wis.Projects["TIA"];
                Console.WriteLine($"Getting WorkItemTypes = {wiType}");
                WorkItemType workItemType = teamProject.WorkItemTypes[wiType];

                /*
                foreach (WorkItemType temp in teamProject.WorkItemTypes)
                {
                    if (temp.Name.ToLower().Equals(wiType.ToLower()))
                    {
                        wiType = temp.Name;
                    }
                }
                */
                Console.WriteLine($"Getting WorkItem = {srcWorkItemId}");
                WorkItem srcWorkItemwi = wis.GetWorkItem(srcWorkItemId);

                string srcTitle = srcWorkItemwi.Title;
                string srcType = srcWorkItemwi.Type.Name;
                bool linkedfeaturesalready = false;

                Console.WriteLine($"Checking Links({srcWorkItemwi.WorkItemLinks.Count}) of WorkItem ({srcWorkItemId})");
                foreach (WorkItemLink link in srcWorkItemwi.WorkItemLinks)
                {
                    int target = link.TargetId;
                    WorkItem targetWorkItem = wis.GetWorkItem(target);
                    if (targetWorkItem.Type.Name == wiType)
                    {
                        Console.WriteLine($"The Request {srcWorkItemId} has already linked Feature = {target}");
                        linkedfeaturesalready = true;
                    }
                }
                if (linkedfeaturesalready)
                {
                    Console.WriteLine($"Error: Found linked Features!!! ");
                    return;
                }
                if (srcType != "Request")
                {
                    Console.WriteLine("CreateChildWI works only for Requests!");
                    return;
                }
                WorkItemLinkTypeEnd wiTypeEnd;
                wis.WorkItemLinkTypes.LinkTypeEnds.TryGetByName("Duplicates", out wiTypeEnd);

                Console.WriteLine($"Creating New Linked WorkItem for {srcWorkItemId}");
                WorkItem newWorkItem = new WorkItem(workItemType);
                newWorkItem.Title = $"{titleprefix}{externalidsuffix}{srcTitle}";
                newWorkItem.AreaPath = area;
                newWorkItem.Fields["Assigned To"].Value = assignedtoname;
                newWorkItem.Fields["Feature Type"].Value = "Internal";
                newWorkItem.IterationPath = iterationPath;
                var wiLink = new WorkItemLink(wiTypeEnd, srcWorkItemId);
                newWorkItem.WorkItemLinks.Add(wiLink);

                Console.WriteLine($"Validating New Linked WorkItem.");
                var issues = newWorkItem.Validate();
                Console.WriteLine($"Validation Completed with {issues.Count} issues.");
                foreach (var issue in issues)
                {
                    Console.WriteLine($"# Validation Issue of type {issue.GetType().FullName} - {issue}");
                }
                Console.WriteLine($"Error: Found {issues.Count} issues during validation of Feature Workitem!");
                Console.WriteLine($"Saving New Linked WorkItem.");
                newWorkItem.Save();
                Console.WriteLine($"Created New Feature with id: {newWorkItem.Id}");

            }
        }
    }
}
