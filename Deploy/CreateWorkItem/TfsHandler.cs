using System;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System.Collections.Generic;
using System.Linq;

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
                    tfs.Authenticate();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.InnerException);
                    return;
                }

                WorkItemStore wis = tfs.GetService<WorkItemStore>();
                Project teamProject = wis.Projects["TIA"];
                WorkItemType workItemType = teamProject.WorkItemTypes[wiType];

                foreach (WorkItemType temp in teamProject.WorkItemTypes)
                {
                    if (temp.Name.ToLower().Equals(wiType.ToLower()))
                    {
                        wiType = temp.Name;
                    }
                }

                WorkItem srcWorkItemwi = wis.GetWorkItem(srcWorkItemId);
                string srcTitle = srcWorkItemwi.Title;
                string srcType = srcWorkItemwi.Type.Name;

                if (srcType != "Request")
                {
                    Console.WriteLine("CreateChildWI works only for Requests!");
                    return;
                }
                WorkItemLinkTypeEnd wiTypeEnd;
                wis.WorkItemLinkTypes.LinkTypeEnds.TryGetByName("Duplicates", out wiTypeEnd);

                WorkItem newWorkItem = new WorkItem(workItemType);
                newWorkItem.Title = $"{titleprefix}{externalidsuffix}{srcTitle}";
                newWorkItem.AreaPath = area;
                newWorkItem.Fields["Assigned To"].Value = assignedtoname;
                newWorkItem.Fields["Feature Type"].Value = "Internal";
                newWorkItem.IterationPath = iterationPath;
                var wiLink = new WorkItemLink(wiTypeEnd, srcWorkItemId);
                newWorkItem.WorkItemLinks.Add(wiLink);
                bool linkedfeaturesalready = false;
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
                var issues = newWorkItem.Validate();
                foreach (var issue in issues)
                {

                }
                Console.WriteLine($"Error: Found {issues.Count} issues during validation of Feature Workitem!");
                newWorkItem.Save();
                Console.WriteLine($"Created New Feature with id: {newWorkItem.Id}");

            }
        }
    }
}
