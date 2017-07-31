using System;
using System.IO;
using System.Text;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace CreateFeatures
{
    internal class TfsHandler : IDisposable
    {
        private string titleprefix = "#RQ";
        private TfsTeamProjectCollection m_TfsConnection;
        private WorkItemStore m_WorkItemStore;
        private Project m_TeamProject;

        public TfsHandler(string tfsuri)
        {
            m_TfsConnection = new TfsTeamProjectCollection(new Uri(tfsuri));

            try
            {
                Connect();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
            }
        }

        private void Connect()
        {
            Console.WriteLine("Authenticating...");
            m_TfsConnection.Authenticate();
            Console.WriteLine("Authentication Succeeded.");
            m_WorkItemStore = m_TfsConnection.GetService<WorkItemStore>();
            m_TeamProject = m_WorkItemStore.Projects["TIA"];
            Console.WriteLine($"Getting WorkItemTypes = {"Feature"}");
        }

        public void Dispose()
        {
            m_TfsConnection?.Dispose();
        }

        public bool UpdateWorkItems(int id, string areaPath, string incPath, string assignedtoname, int parentFeature, bool createTask, int originalEstimateEffort, out string comment)
        {
            var externalidsuffix = $"{id} - ";
            comment = "Test";
            WorkItemType workItemType = m_TeamProject.WorkItemTypes["Feature"];
            WorkItem srcWorkItemwi = m_WorkItemStore.GetWorkItem(id);

            var r = GetFields(srcWorkItemwi);
            File.WriteAllText("Request.txt", r);


            string srcTitle = srcWorkItemwi.Title;
            string srcType = srcWorkItemwi.Type.Name;
            bool linkedfeaturesalready = false;
            foreach (WorkItemLink link in srcWorkItemwi.WorkItemLinks)
            {
                int target = link.TargetId;
                WorkItem targetWorkItem = m_WorkItemStore.GetWorkItem(target);
                if (targetWorkItem.Type.Name == "Feature")
                {
                    Console.WriteLine($"The Request {id} has already linked Feature = {target}");
                    comment = $"-->{target}(F) ";
                    linkedfeaturesalready = true;
                }
            }
            if (linkedfeaturesalready)
            {
                comment = "Already linked Feature..." + comment;
                return false;
            }
            if (srcWorkItemwi.Type.Name != "Request")
            {
                comment = "Id should be a workitem of type = 'Request'";
                return false;
            }

            WorkItemLinkTypeEnd duplicatesLink;
            m_WorkItemStore.WorkItemLinkTypes.LinkTypeEnds.TryGetByName("Duplicates", out duplicatesLink);
            WorkItemLinkTypeEnd parentTypeTask;
            m_WorkItemStore.WorkItemLinkTypes.LinkTypeEnds.TryGetByName("Parent", out parentTypeTask);


            Console.WriteLine($"Creating New Linked WorkItem for {id}");
            WorkItem newWorkItem = new WorkItem(workItemType)
            {
                Title = $"{titleprefix}{externalidsuffix}{srcTitle}",
                AreaPath = areaPath,
                IterationPath = incPath
            };

            var f = GetFields(newWorkItem);
            File.WriteAllText("Feature.txt", f);

            newWorkItem.Fields["Assigned To"].Value = assignedtoname;
            newWorkItem.Fields["Feature Type"].Value = "Internal";
            newWorkItem.Fields["Fix Vote"].Value = "Fix 2";
            newWorkItem.Fields["Original Estimate"].Value = originalEstimateEffort;
            var wiLink = new WorkItemLink(duplicatesLink, id);
            newWorkItem.WorkItemLinks.Add(wiLink);
            if (parentFeature > 0)
            {
                var parentLink = new WorkItemLink(parentTypeTask, parentFeature);
                newWorkItem.WorkItemLinks.Add(parentLink);
            }

            Console.WriteLine($@"Validating New Linked WorkItem.");
            var issues = newWorkItem.Validate();
            Console.WriteLine($@"Validation Completed with {issues.Count} issues.");
            foreach (var issue in issues)
            {
                Console.WriteLine($@"# Validation Issue of type {issue.GetType().FullName} - {issue}");
            }
            Console.WriteLine($@"Error: Found {issues.Count} issues during validation of Feature Workitem!");
            Console.WriteLine($@"Saving New Linked WorkItem.");
            newWorkItem.Save();
            var fields = GetFields(newWorkItem);
            int taskId = CreateTask(createTask, newWorkItem, originalEstimateEffort, parentTypeTask);
            Console.WriteLine($@"Created New Feature with id: {newWorkItem.Id}");
            comment = $@"-->{newWorkItem.Id}(F)" + (createTask ? $"-->{taskId}(T)" : string.Empty);

            return false;
        }

        private static string GetFields(WorkItem workItem)
        {
            StringBuilder s = new StringBuilder();
            foreach (Field field in workItem.Fields)
            {
                s.AppendLine(
                    $"{field.Name}\t\t\t\t{field.ReferenceName}\t\t\t\tHasAllowedValues:{field.HasAllowedValuesList}\t\t\t\tRequired:{field.IsRequired}\t\t\t\tdefault:{field.ValueWithServerDefault?.ToString()}");
            }
            return s.ToString();
        }

        private int CreateTask(bool createTask, WorkItem newWorkItem, int originalEstimateEffort, WorkItemLinkTypeEnd parentTypeTask)
        {
            if (!createTask) return 0;
            try
            {
                WorkItem newTask = new WorkItem(m_TeamProject.WorkItemTypes["Task"])
                {
                    Title = $@"IMPL: {newWorkItem.Title}",
                    AreaPath = newWorkItem.AreaPath,
                    IterationPath = newWorkItem.IterationPath
                };
                newTask.Fields["Assigned To"].Value = newWorkItem.Fields["Assigned To"].Value;
                try
                {
                    var f = GetFields(newWorkItem);
                    File.WriteAllText("Task.txt", f);

                    newTask.Fields["Task Subtype"].Value = "Implementation";
                    newTask.Fields["Original Estimate"].Value = originalEstimateEffort;
                    newTask.Fields["Remaining Work"].Value = originalEstimateEffort;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                var wiLink = new WorkItemLink(parentTypeTask, newWorkItem.Id);
                newTask.WorkItemLinks.Add(wiLink);
                newTask.Save();
                return newTask.Id;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return 0;
        }

        public string GetWorkItemTitle(int id)
        {
            try
            {
                WorkItem workItem = m_WorkItemStore.GetWorkItem(id);
                return workItem.Title;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return e.Message;
            }
        }
    }
}