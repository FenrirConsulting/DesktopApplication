using IAMHeimdall.Core;
using System;
using System.DirectoryServices.AccountManagement;

namespace IAMHeimdall.MVVM.Model
{
    public class FactoolHistoryBuilder : ObservableObject
    {
        #region Methods
        public FactoolHistoryBuilder()
        {
            //Constructor
        }
        #endregion

        #region Functions
        public FactoolRequestHistory HistoryBuilder(FacToolRequest OriginalRequest, FacToolRequest UpdatedRequest, string RequestState)
        {
            FactoolRequestHistory returnedHistory = new();
            try
            {
                string ChangesBuilder = "";

                // History Record for Request Creation
                if (RequestState == "New")
                {
                    ChangesBuilder += "";
                    returnedHistory.ReferenceNumber = UpdatedRequest.ReferenceNumber;
                    returnedHistory.DisplayName = App.GlobalUserInfo.DisplayName;
                    returnedHistory.DateModified = UpdatedRequest.ModifiedDate;
                    returnedHistory.ModifiedTick = UpdatedRequest.ModifiedTick;
                    returnedHistory.State = "Created";
                    string joinedUpdatedDefects = string.Join(",", UpdatedRequest.DefectReason);
                    string joinedUpdatedSystems = string.Join(",", UpdatedRequest.Systems);
                    string joinedUpdatedReplys = string.Join(",", UpdatedRequest.ReplyTypes);
                    string joinedUpdatedRequests = string.Join(",", UpdatedRequest.RequestType);
                    ChangesBuilder += "Request # " + UpdatedRequest.ReferenceNumber + " Created On " + UpdatedRequest.CreateDate + Environment.NewLine;
                    ChangesBuilder += "Created by " + UpdatedRequest.DisplayName + " as a " + UpdatedRequest.RequestStatus + " request." + Environment.NewLine;
                    ChangesBuilder += "Form Type - " + UpdatedRequest.FormType + " | RequestType(s): " + joinedUpdatedRequests + " | Defect Reason(s): " + joinedUpdatedDefects + Environment.NewLine;
                    ChangesBuilder += "System(s): " + joinedUpdatedSystems + " | Reply Type(s): " + joinedUpdatedReplys + Environment.NewLine;
                    ChangesBuilder += "Additional Comments : " + UpdatedRequest.Comments + " | " + UpdatedRequest.XREF1 + " | " + UpdatedRequest.XREF2;
                    returnedHistory.Changes = ChangesBuilder;
                }

                // History Record for Update Creation
                if (RequestState == "Existing")
                {
                    ChangesBuilder += "";
                    returnedHistory.ReferenceNumber = UpdatedRequest.ReferenceNumber;
                    returnedHistory.DisplayName = App.GlobalUserInfo.DisplayName;
                    returnedHistory.DateModified = UpdatedRequest.ModifiedDate;
                    returnedHistory.ModifiedTick = UpdatedRequest.ModifiedTick;
                    returnedHistory.State = "Updated";
                    ChangesBuilder += "Request # " + UpdatedRequest.ReferenceNumber + " Modified On " + UpdatedRequest.ModifiedDate + Environment.NewLine;
                    ChangesBuilder += "Modified by " + returnedHistory.DisplayName + " as a " + UpdatedRequest.RequestStatus + " request." + Environment.NewLine;
                    string joinedOriginalDefects = string.Join(",", OriginalRequest.DefectReason);
                    string joinedOriginalSystems = string.Join(",", OriginalRequest.Systems);
                    string joinedOriginalReplys = string.Join(",", OriginalRequest.ReplyTypes);
                    string joinedOriginalRequests = string.Join(",", OriginalRequest.RequestType);
                    string joinedUpdatedDefects = string.Join(",", UpdatedRequest.DefectReason);
                    string joinedUpdatedSystems = string.Join(",", UpdatedRequest.Systems);
                    string joinedUpdatedReplys = string.Join(",", UpdatedRequest.ReplyTypes);
                    string joinedUpdatedRequests = string.Join(",", UpdatedRequest.RequestType);

                    // Checking for Changes Between the Original and Updated Request, noting Changes where Found.
                    if (OriginalRequest.RequestStatus != UpdatedRequest.RequestStatus) { ChangesBuilder += "Request Status Changed from [" + OriginalRequest.RequestStatus + "] to [" + UpdatedRequest.RequestStatus + "]." + Environment.NewLine; }
                    if (OriginalRequest.TotalUsers != UpdatedRequest.TotalUsers) { ChangesBuilder += "Total Users Changed from [" + OriginalRequest.TotalUsers + "] to [" + UpdatedRequest.TotalUsers + "]." + Environment.NewLine; }
                    if (OriginalRequest.FormType != UpdatedRequest.FormType) { ChangesBuilder += "FormType Type Changed from [" + OriginalRequest.FormType + "] to [" + UpdatedRequest.FormType + "]." + Environment.NewLine; }
                    if (joinedOriginalRequests != joinedUpdatedRequests) { ChangesBuilder += "Request Types Changed from [" + joinedOriginalRequests + "] to [" + joinedUpdatedRequests + "]." + Environment.NewLine; }
                    if (joinedOriginalDefects != joinedUpdatedDefects) { ChangesBuilder += "Defect Reasons Changed from [" + joinedOriginalDefects + "] to [" + joinedUpdatedDefects + "]." + Environment.NewLine; }
                    if (joinedOriginalSystems != joinedUpdatedSystems) { ChangesBuilder += "Systems Changed from [" + joinedOriginalSystems + "] to [" + joinedUpdatedSystems + "]." + Environment.NewLine; }
                    if (joinedOriginalReplys != joinedUpdatedReplys) { ChangesBuilder += "Reply Types Changed from [" + joinedOriginalReplys + "] to [" + joinedUpdatedReplys + "]." + Environment.NewLine; }
                    if (OriginalRequest.Comments != UpdatedRequest.Comments) { ChangesBuilder += "Comments Changed from [" + OriginalRequest.Comments + "] to [" + UpdatedRequest.Comments + "]." + Environment.NewLine; }
                    if (OriginalRequest.XREF1 != UpdatedRequest.XREF1) { ChangesBuilder += "XREF1 Changed from [" + OriginalRequest.XREF1 + "] to [" + UpdatedRequest.XREF1 + "]." + Environment.NewLine; }
                    if (OriginalRequest.XREF2 != UpdatedRequest.XREF2) { ChangesBuilder += "XREF2 Changed from [" + OriginalRequest.XREF2 + "] to [" + UpdatedRequest.XREF2 + "]." + Environment.NewLine; }
                    if (OriginalRequest.RequestState != UpdatedRequest.RequestState) { ChangesBuilder += "Request State Changed from [" + OriginalRequest.RequestState + "] to [" + UpdatedRequest.RequestState + "]." + Environment.NewLine; }
                    if (OriginalRequest.SLAStart != UpdatedRequest.SLAStart) { ChangesBuilder += "SLA Start Date Changed from [" + OriginalRequest.SLAStart + "] to [" + UpdatedRequest.SLAStart + "]." + Environment.NewLine; }
                    if (OriginalRequest.TimesReturned != UpdatedRequest.TimesReturned) { ChangesBuilder += "Times Returned Changed from [" + OriginalRequest.TimesReturned + "] to [" + UpdatedRequest.TimesReturned + "]." + Environment.NewLine; }
                    if (OriginalRequest.SLACompletionTime != UpdatedRequest.SLACompletionTime) { ChangesBuilder += "Completion Time Changed from [" + OriginalRequest.SLACompletionTime + "] to [" + UpdatedRequest.SLACompletionTime + "]." + Environment.NewLine; }
                    if (OriginalRequest.AgentComments != UpdatedRequest.AgentComments) { ChangesBuilder += "Agent Comments Changed from [" + OriginalRequest.AgentComments + "] to [" + UpdatedRequest.AgentComments + "]." + Environment.NewLine; }
                    if (OriginalRequest.AgentsWorked != UpdatedRequest.AgentsWorked) { ChangesBuilder += "Agents Worked Changed from [" + OriginalRequest.AgentsWorked + "] to [" + UpdatedRequest.AgentsWorked + "]." + Environment.NewLine; }

                    returnedHistory.Changes = ChangesBuilder;
                }

                // History Record for Request Deletion
                if (RequestState == "Deleted")
                {
                    DateTime createDate = DateTime.UtcNow;
                    returnedHistory.ReferenceNumber = UpdatedRequest.ReferenceNumber;
                    returnedHistory.DisplayName = App.GlobalUserInfo.DisplayName;
                    returnedHistory.DateModified = createDate.ToString("yyyy/MM/dd h:mm:ss.fff");
                    returnedHistory.ModifiedTick = createDate.Ticks.ToString();
                    returnedHistory.State = "Deleted";
                    ChangesBuilder = "Request # " + UpdatedRequest.ReferenceNumber + " Deleted by : " + returnedHistory.DisplayName + " On " + returnedHistory.DateModified;
                    returnedHistory.Changes = ChangesBuilder;
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }

            return returnedHistory;
        }
        #endregion
    }
}
