using Newtonsoft.Json;
using IAMHeimdall.Core;
using System.Collections.Generic;
using System.Data;

namespace IAMHeimdall.MVVM.Model
{
    public class AccessNowRequestModel : ObservableObject
    {
        // Define Operations Table and Columns
        public static DataTable OperationsTable ()
        {
            DataTable tempTable = new();

            tempTable.Columns.AddRange(new DataColumn[] {
              new DataColumn("request_id", typeof(long))
             ,new DataColumn("requested_on", typeof(string))
             ,new DataColumn("request_type", typeof(string))
             ,new DataColumn("status", typeof(string))
             ,new DataColumn("closed_on", typeof(string))
             ,new DataColumn("asset", typeof(string))
             ,new DataColumn("access", typeof(string))
             ,new DataColumn("sub_request_type", typeof(string))
             ,new DataColumn("account_name", typeof(string))
             ,new DataColumn("requestor_name", typeof(string))
             ,new DataColumn("requestor_employee_id", typeof(string))
             ,new DataColumn("ticket_id", typeof(string))
             ,new DataColumn("ticket_status", typeof(string))
             ,new DataColumn("mail_to", typeof(string))
             ,new DataColumn("mail_sent_on", typeof(string))
             ,new DataColumn("mail_sent_on_date", typeof(string))
             ,new DataColumn("TicketCount", typeof(string))
             ,new DataColumn("Category", typeof(string))
             ,new DataColumn("Type", typeof(string))
              ,new DataColumn("SLADays", typeof(string))
            });

            return tempTable;
        }

        // Class Object Sent to SQL Databaase
        public class SQLObject
        {
            public double request_id { get; set; }
            public string requested_on { get; set; } // status
            public string request_type { get; set; }
            public string status { get; set; }
            public string closed_on { get; set; } // request_type
            public string asset { get; set; }
            public string access { get; set; } // request_type
            public string sub_request_type { get; set; }
            public string account_name { get; set; } // request_type
            public string requestor_name { get; set; }
            public string requestor_employee_id { get; set; } // request_type
            public string ticket_id { get; set; }
            public string ticket_status { get; set; } // request_type
            public string mail_to { get; set; }
            public string mail_sent_on { get; set; } // request_type
            public string mail_sent_on_date { get; set; }
            public string TicketCount { get; set; } // request_type
            public string Category { get; set; }
            public string Type { get; set; } // request_type
            public string SLADays { get; set; }
        }

        // Define Operations JSON Request Root Object
        public class RootObject
        {
            public int totalResults { get; set; }
            public int startIndex { get; set; }
            public int itemsPerPage { get; set; }   
            public List<Resources> resources { get; set; }
        }

        //Need to Define closed_on

        // Request Object Class 
        public class Resources
        {

            public long id { get; set; }
            public long externalId { get; set; }
            public string createdOn { get; set; }
            public string status { get; set; } // status
            public string isOpen { get; set; }
            public string requestDesc { get; set; }
            public string requestType { get; set; } // request_type
            public string requestSubmissionId { get; set; }
            public string entityId { get; set; }
            public string entityName { get; set; }
            public string entityTypeId { get; set; }
            public string entityPropType { get; set; }
            public string relatedEntityName { get; set; }
            public TicketData ticketData { get; set; }  // ticket_id
            public string commentTaskData { get; set; }
            public string closedOn { get; set; } // Closed On Date
            public string fulfilledOn { get; set; } // Fulfilled On Date
            public AdditionalData additionalData { get; set; }
            public string requestedOn { get; set; } // requested_on
            public Requestor requestor { get; set; }
            public List<ActionSummary> actionSummary { get; set; }
            public Approvers approvers { get; set;}
            public Asset asset { get; set; }
            public Account account { get; set; }
            public User user { get; set; }
            public RequestData requestData { get; set; }
            public long requestid { get; set; } // request_id
            public string additionalInfo { get; set; }  
        }

        public class TicketData
        {
            public string status { get; set; }
            public string ticketId { get; set; }
            public string externalTicketId { get; set; }
        }

        public class AdditionalData
        {
            public string type { get; set; }
            public FulfillmentData fulfillmentData { get; set; }
        }

        public class FulfillmentData
        {
            public string Fulfilled_For_Account { get; set; }
            public string Mail_Sent_On { get; set; }
            public string Mail_Sent_On_Date { get; set; }
            public string Mail_Sent_To { get; set; }
        }

        public class Requestor
        {
            public string Employee_ID { get; set; }  // requestor_employee_id
            public string Email_Address { get; set; }   // mail_to
            public string Company_AD_Account_Name { get; set; } // requestor_name

            public long id { get; set; }
            public string entityTypeId { get; set; }
            public string entityTypeName { get; set; }
            public string assetId { get; set; }
            public string Asset { get; set; }
            public string type { get; set; }
            public string Tag_High_risk { get; set; }
            public string Tag_Administrator { get; set; }
            public string Tag_Terminated_User { get; set; }
            public string Tag_ToBeTerminated_user { get; set; }
            public string Tag_Privileged_User { get; set; }
            public string Tag_Asset_Administrator { get; set; }
            public string Tag_Admin_Password_Aware { get; set; }
            public string Tag_Derived_Resource_Owner { get; set; }
            public string Tag_Resource_Owner { get; set; }
            public string[] tags {get; set;}
            public string Short_Name { get; set; }  
            public string First_Name { get; set; }
            public string Last_Name { get; set; }
            public string Hire_Date { get; set; }
            public string Termination_Date { get; set; }
            public string Start_Date { get; set; }
            public string Job_Code { get; set; }    
            public string Job_Title { get; set; }
            public string Location { get; set; }
            public string Department { get; set; }
            public RBAC_Model RBAC_Model { get; set; }
            public string Company_Employee_ID { get; set; } 
            public string PBM_ID { get; set; }  
            public string Is_Terminated { get; set; }   
            public string Site_OU { get; set; } 
            public Supervisor Supervisor { get; set; }  
            public string Level4_Department_Code { get; set; }  
            public string Mobile { get; set; }
            public string Alternate_Email_Address { get; set; }
            public string Middle_Name { get; set; }
            public string Status { get; set; }
            public string Country { get; set; }
            public string Company { get; set; }
            public string Application_Name { get; set; }
            public string Account_Targets { get; set; }
            public string MGRUSER { get; set; }
            public string Vendor_ID { get; set; } 
            public string Location_Name { get; set; }
            public string Company_Location_Name { get; set; }
            public string Company_Location { get; set; }
            public string Company_Department { get; set; }
            public string Registration_Mode { get; set; }
            public string Department_Entry_Date { get; set; }
            public string Benefit_Program { get; set; }
            public string Charge_Sys { get; set; }
            public string Compensation_Frequency { get; set; }
            public string Company_Department_ID { get; set; }
            public string Company_Job_Code { get; set; }
            public string EE_SVCS_USER { get; set; }
            public string Is_AuthoriaUser { get; set; }
            public string EECHGSYS { get; set; }
            public string Is_Employee { get; set; }
            public string Level5_Department_Code { get; set; }
            public string Emerge_DM { get; set; }
            public string Is_HRMS_User { get; set; }
            public string Emerge_RM { get; set; }
            public string IsEmployeeWithAID { get; set; }
            public string Emerge_RxSup { get; set; }
            public string UserHasANID { get; set; }
            public string FLSA_Status { get; set; }
            public string GL_Pay_Type { get; set; }
            public string Global_ID { get; set; }
            public string Home_City { get; set; }
            public string Home_Postal_Code { get; set; }
            public string Home_Street { get; set; }
            public string Job_Category_Code { get; set; }
            public string Job_Family_Code { get; set; }
            public string Job_Function { get; set; }
            public string Job_Grade { get; set; }
            public string Learnet_Compliance { get; set; }
            public string Manager_Level { get; set; }
            public string OPR_Class { get; set; }
            public string Pay_Group { get; set; }   
            public string PBM_SWO_COntractor { get; set; }  
            public string Personal_Phone { get; set; }  
            public string PharmTech_License_ID { get; set; }
            public string PharmTech_License_State { get; set; }
            public string Reg_Temp { get; set; }
            public string RX_Zone { get; set; }
            public string RxNavigator_URL { get; set; }
            public string Standard_Hours { get; set; }
            public string Union_Code { get; set; }
            public string User_Category { get; set; }
            public string Vendor_DSD { get; set; }
            public string Vendor_DSD_Access { get; set; }
            public string Vendor_DSD_Invoice { get; set; }
            public string Vendor_Sup_Mail_1 { get; set; }
            public string Vendor_Sup_Mail_2 { get; set; }
            public string Work_Area { get; set; }
            public string Work_District { get; set; }
            public string Work_Region { get; set;  }
            public string Reference_ID { get; set; }
            public string Work_Time { get; set; }
            public string Company_Business_Unit { get; set; }
            public string SSN { get; set; }
            public string phone { get; set; }
            public string Date_Of_Birth { get; set; }
            public string Organization_ID { get; set; }
            public string User_Type { get; set; }
            public string City { get; set; }
            public string Empl_Pay_Type { get; set; }
            public string State { get; set; }
            public string Emp_Status { get; set; }
            public string Postal_Code { get; set; }
            public string Country_Code { get; set; }
            public string Code_Center { get; set; }
            public string Department_ID { get; set; }
            public string Mail_Code { get; set; }
            public string Office_phone_Country_Prefix { get; set; }
            public string Identity_Type { get; set; }
            public string Uesr_Name { get; set; }
            public string Pre_Conversion_ID { get; set; }
            public string Job_Description { get; set; }
            public string Preferred_Name { get; set; }
            public string Cost_Center_Name { get; set; }
            public string Description { get; set; }
            public string Address_Line { get; set; }
            public string Office_Name { get; set; }
            public string Support_Location { get; set; }
            public string Street { get; set; }
            public string Added_Date { get; set; }
            public string Modified_Date { get; set; }
            public string Owner1 { get; set; }
            public string Owner2 { get; set; }
            public string Owner3 { get; set; }
            public string Is_Executive { get; set; }
            public string Employee_Type { get; set; }
            public string[] owners { get; set; }
            public string entityOwners{ get; set; }
        }

        public class Supervisor
        {
            public string Short_Name { get; set; }    
            public string Email_Address { get; set; }
            public long id { get; set; }
        }

        public class ActionSummary
        {
            [JsonProperty(PropertyName = "1")]
            public List<Action1> action1  {get; set;}

            [JsonProperty(PropertyName = "2")]
            public List<Action2> action2 { get; set; }

            [JsonProperty(PropertyName = "3")]
            public List<Action3> action3 { get; set; }
        }

        public class Action1
        {
            public string status { get; set; }  
            public string actedby { get; set; }
            public string actedById { get; set; }
            public string actedon { get; set; }
            public string comments { get; set; }
        }

        public class Action2
        {
            public string status { get; set; }
            public string actedby { get; set; }
            public string actedById { get; set; }
            public string actedon { get; set; }
            public string comments { get; set; }
        }

        public class Action3
        {
            public string status { get; set; }
            public string actedby { get; set; }
            public string actedById { get; set; }
            public string actedon { get; set; }
            public string comments { get; set; }
        }

        public class Approvers
        {
            [JsonProperty(PropertyName = "1")]
            public List<ApproversList1> approversList1 {get; set;}

            [JsonProperty(PropertyName = "2")]
            public List<ApproversList2> approversList2 { get; set; }
        }

        public class ApproversList1
        {
            public string approverId { get; set; }
            public string approverName { get; set; }
        }

        public class ApproversList2
        {
            public string approverId { get; set; }
            public string approverName { get; set; }
        }

        public class Asset
        {
            public long id { get; set; }
            public string managerAssetType { get; set; }
            public string isTicketingSystem { get; set; }
            public string sanitizedName { get; set; }
            public AssetAdditionalData additionalDate { get; set; }
            public string[] tags { get; set; }
            public int entityTypeId { get; set; }
            public string entityTypeName { get; set; }
            public string Short_Name { get; set; }
            public string Logo { get; set; }
            public string URL { get; set; }
            public string Resource_Type { get; set; }
            public string Name { get; set; }
            public string ITPM_Number { get; set; }
            public string Description { get; set; }
            public AssetOwner Owner { get; set; }
            public string Alternate_Name { get; set; }
            public string Asset_Identifier { get; set; }
            public string Non_Integrated { get; set; }
            public string Manage_Entitlement { get; set; }
            public string Is_Enabled { get; set; }
            public string Reference_ID {get; set; }
            public string[] owners { get; set; }
        }

        public class AssetAdditionalData
        {
            public string assetConnectorName { get; set; }
            public string eserviceConnectorName { get; set; }
            public string customGroupResourceURLName { get; set; }
            public string groupBaseDN { get; set; }
        }

        public class AssetOwner
        {
            public string Short_Name { get; set; }  
            public string Email_Address { get; set; }   
            public int id { get; set; }
        }

        public class Account
        {
            public long id { get; set; }
            public string entityTypeId { get; set; }
            public string entityTypeName { get; set; }
            public string assetId { get; set; }
            public Asset Asset { get; set; }
            public string Tag_Medium_Risk_Access { get; set; }
            public string Tag_User_Account { get; set; }
            public string Tag_Derived_Owner_Access { get; set; }
            public string Tag_UnApprovedAccess { get; set; }
            public string Tag_Reviewed_Medium_Risk_Access { get; set; }
            public string Tag_Cloud_Access { get; set; }
            public string Tag_Temporary_Account { get; set; }
            public string Tag_Low_Risk_Access { get; set; }
            public string Tag_Regular_Access { get; set; }
            public string Tag_Anomaly_Access { get; set; }
            public string Tag_Access_Groups { get; set; }
            public string Tag_Test_Account { get; set; }
            public string Tag_Risk_Not_Reviewed_Access { get; set; }
            public string Tag_System_Account { get; set; }
            public string Tag_Derived_Administrator_Access { get; set; }
            public string Tag_High_Risk_Access { get; set; }
            public string Tag_Indefinite_Access { get; set; }
            public string Tag_Administrator_Access { get; set; }
            public string Tag_L1_Exclusion_PreAuthorized { get; set; }
            public string Tag_No_Risk_Access { get; set; }
            public string Tag_Unauthorized_Access { get; set; }
            public string Tag_Orphaned_Account { get; set; }
            public string Tag_Shared_Account { get; set; }
            public string Tag_Service_Account { get; set; }
            public string Tag_L2_Exclusion_PreAuthorized { get; set; }
            public string Tag_Expired_Access { get; set; }
            public string Tag_Deleted_Account { get; set; }
            public string Tag_Authorization_Groups { get; set; }
            public string Tag_Temporary_Access { get; set; }
            public string Tag_Locked_Account { get; set; }
            public string Tag_Privileged_Access { get; set; }
            public string Tag_Disabled_Account { get; set; }
            public string Tag_RejectedAccess { get; set; }
            public string Tag_Exceptional_Access { get; set; }
            public string Tag_Derived_Privileged_Access { get; set; }
            public string Tag_Unprotected_Resource { get; set; }
            public string Tag_Derived_Administrative_Access { get; set; }
            public string Tag_Privileged_Resource { get; set; }
            public string Tag_Auto_Approval { get; set; }
            public string Tag_Reviewed_High_Risk_Access { get; set; }
            public string Tag_Owner_Access { get; set; }
            public string Tag_Administrative_Access { get; set; }
            public string Tag_Expired_Account { get; set; }
            public string Tag_Owner_Approval { get; set; }
            public string Tag_ViolatingAccess { get; set; }
            public string Tag_Manager_Approval { get; set; }
            public string Tag_Reviewed_Low_Risk_Access { get; set; }
            public string[] tags { get; set; }
            public string Short_Name { get; set; }
            public string Company { get; set; }
            public string Name { get; set; }
            public string Present_In { get; set; }
            public string UNique_Id { get; set; }
            public string Creation_Date { get; set; }
            public string Expiration_Date { get; set; }
            public string Last_Login_date { get; set; }
            public string User_Account_Control { get; set; }
            public string First_Name { get; set; }
            public string Is_Disabled { get; set; }
            public string Last_Name { get; set; }
            public string Is_Locked { get; set; }
            public string Location { get; set; }
            public string Department { get; set; }
            public string Job_Title { get; set; }
            public string State { get; set; }
            public string Password_Last_Set { get; set; }
            public string Healthcare_Role { get; set; }
            public string Email_Address { get; set; }
            public string Employee_ID { get; set; }
            public string Phone_Number { get; set; }
            public AccountManager Manager { get; set; }
            public string Department_Id { get; set; }
            public string Server_Workstation { get; set; }
            public string Application_Internal_Id { get; set; }
            public string Common_Name { get; set; }
            public string Lockout_Time { get; set; }
            public string Reference_ID { get; set; }
            public string Gecos { get; set; }
            public string Middle_Name { get; set; }
            public string Mobile { get; set; }
            public string User_Principal_Name { get; set; }
            public string UID_Number { get; set; }
            public string Unix_Home_Directory { get; set; }
            public string Login_Shell { get; set; }
            public string Alternate_Email { get; set; }
            public string Country_Code { get; set; }
            public string Company_Mail_Code { get; set; }
            public string Company_Location { get; set; }
            public string Address_Line_1 { get; set; }
            public string Company_IAM_Global_Identifier { get; set; }
            public string Postal_Code { get; set; }
            public string Company_IAM_Control_Data { get; set; }
            public string GID_Number { get; set; }
            public string Display_Name { get; set; }
            public string[] owners { get; set; }
        }

        public class AccountManager
        {
            public string Short_Name { get; set; }
            public string id { get; set; }
            public string entityTypeId { get; set; }
        }

        public class User
        {
            public long id { get; set; }
            public string Company_AD_Account_Name { get; set; } //account_name
            public string entityTypeId { get; set; }
            public string entityTypeName { get; set; }
            public string assetId { get; set; }
            public string Asset { get; set; }
            public string type { get; set; }
            public string Tag_High_Risk { get; set; }
            public string Tag_Administrator { get; set; }
            public string Tag_Terminated_User { get; set; }
            public string Tag_ToBeTerminated_User { get; set; }
            public string Tag_Privileged_User { get; set; }
            public string Tag_Asset_Administrator { get; set; }
            public string Tag_Admin_Password_Aware { get; set; }
            public string Tag_Derived_Resource_Owner { get; set; }
            public string Tag_Resource_Owner { get; set; }
            public string Short_Name { get; set; }
            public string First_Name { get; set; }
            public string Last_Name { get; set; }
            public string Employee_ID { get; set; }
            public string Hire_Date { get; set; }
            public string Termination_Date { get; set; }
            public string Start_Date { get; set; }
            public string Job_Code { get; set; }
            public string Job_Title { get; set; }
            public string Location { get; set; }
            public string Email_Address { get; set; }
            public string Department { get; set; }
            public RBAC_Model RBAC_Model { get; set; }
            public string Company_Employee_ID { get; set; }
            public string PBM_ID { get; set; }
            public string Is_Terminated { get; set; }
            public string Site_OU { get; set; }
            public string Level4_Department_Code { get; set; }
            public string Mobile { get; set; }
            public string Alternate_Email_Address { get; set; }
            public string Middle_Name { get; set; }
            public string Status { get; set; }
            public string Country { get; set; }
            public string Company { get; set; }
            public string Application_Name { get; set; }
            public string Account_Targets { get; set; }
            public string MGRUSER { get; set; }
            public string Vendor_ID { get; set; }
            public string Location_Name { get; set; }
            public string Company_Location_Name { get; set; }
            public string Company_Location { get; set; }
            public string Company_Department { get; set; }
            public string Registration_Mode { get; set; }
            public string Department_Entry_Date { get; set; }
            public string Benefit_Program { get; set; }
            public string Charge_Sys { get; set; }
            public string Compensation_Frequency { get; set; }
            public string Company_Department_ID { get; set; }
            public string Company_Job_Code { get; set; }
            public string EE_SVCS_USER { get; set; }
            public string Is_AuthoriaUser { get; set; }
            public string EECHGSYS { get; set; }
            public string Is_Employee { get; set; }
            public string Level5_Department_Code { get; set; }
            public string Emerge_DM { get; set; }
            public string Is_HRMS_User { get; set; }
            public string Emerge_RM { get; set; }
            public string IsEmployeeWithAID { get; set; }
            public string Emerge_RxSup { get; set; }
            public string UserHasANID { get; set; }
            public string FLSA_Status { get; set; }
            public string GL_Pay_Type { get; set; }
            public string Global_ID { get; set; }
            public string Home_City { get; set; }
            public string Home_Postal_Code { get; set; }
            public string Home_State { get; set; }
            public string Home_Street { get; set; }
            public string Job_Category_Code { get; set; }
            public string Job_Family_Code { get; set; }
            public string Job_Function { get; set; }
            public string Job_Grade { get; set; }
            public string Learnet_Compliance { get; set; }
            public string Manager_Level { get; set; }
            public string OPR_Class { get; set; }
            public string Pay_Group { get; set; }
            public string PBM_SOW_Contractor { get; set; }
            public string Personal_Phone { get; set; }
            public string PharmTech_License_ID { get; set; }
            public string PharmTech_License_State { get; set; }
            public string Reg_Temp { get; set; }
            public string RX_Zone { get; set; }
            public string RxNavigator_URL { get; set; }
            public string Standard_Hours { get; set; }
            public string Union_Code { get; set; }
            public string User_Category { get; set; }
            public string Vendor_DSD { get; set; }
            public string Vendor_DSD_Access { get; set; }
            public string Vendor_DSD_Invoice { get; set; }
            public string Vendor_Sup_Mail_1 { get; set; }
            public string Vendor_Sup_Mail_2 { get; set; }
            public string Work_Area { get; set; }
            public string Work_District { get; set; }
            public string Work_Region { get; set; }
            public string Reference_ID { get; set; }
            public string Work_Time { get; set; }
            public string Company_Business_Unit { get; set; }
            public string SSN { get; set; }
            public string Phone { get; set; }
            public string Date_Of_Birth { get; set; }
            public string Organization_ID { get; set; }
            public string User_Type { get; set; }
            public string City { get; set; }
            public string Empl_Pay_Type { get; set; }
            public string State { get; set; }
            public string Emp_Status { get; set; }
            public string Postal_Code { get; set; }
            public string Country_Code { get; set; }
            public string Cost_Center { get; set; }
            public string Department_ID { get; set; }
            public string Mail_Code { get; set; }
            public string Office_phone_Country_Prefix { get; set; }
            public string Identity_Type { get; set; }
            public string User_Name { get; set; }
            public string Pre_Conversion_ID { get; set; }
            public string Job_Description { get; set; }
            public string Preferred_Name { get; set; }
            public string Cost_Center_Name { get; set; }
            public string Description { get; set; }
            public string Address_Line { get; set; }
            public string Office_Name { get; set; }
            public string Support_Location { get; set; }
            public string Street { get; set; }
            public string Added_Date { get; set; }
            public string Modified_Date { get; set; }
            public Owner1 Owner1 { get; set; }
            public Owner2 Owner2 { get; set; }
            public Owner3 Owner3 { get; set; }
            public string Is_Executive { get; set; }
            public string Employee_Type { get; set; }
            public string[] owners { get; set; }
            public UserEntityOwners entityOwners { get; set; }
            public string[] tags { get; set; }
        }

        public class UserEntityOwners
        {
            public string id { get; set; }
            public string name { get; set; }
            public string entityTypeId { get; set; }
        }

        public class Owner1
        {
            public string id { get; set; }
        }

        public class Owner2
        {
            public string id { get; set; }
        }

        public class Owner3
        {
            public string id { get; set; }
        }

        public class UserSupervisor
        {
            public string Short_Name { get; set; }
            public string Email_Address { get; set; }
            public string id { get; set; }
        }

        public class RequestData
        {
            public string subRequestType { get; set; } //sub_request_type
            public string name { get; set; }
            public string newValue { get; set; }
            public string oldValue { get; set; }
            public string businessJustification { get; set; }
            public string type { get; set; }
            public Access access { get; set; }  
            public Account account { get; set; }
            public Asset asset { get; set; }
        }

        public class Access
        {
            public string Asset { get; set; } // asset
            public string Type { get; set; } // Access Column A
            public string Short_Name { get; set; } // Access Column B 
            public string Creation_Date { get; set; }   // mail_sent_on

            public long id { get; set; }
            public string entityTypeId { get; set; }
            public string entityTypeName { get; set; }
            public string assetId { get; set; }
            public string type { get; set; }
            public string Tag_Medium_Risk_Access { get; set; }
            public string Tag_User_Account { get; set; }
            public string Tag_Derived_Owner_Access { get; set; }
            public string Tag_UnApprovedAccess { get; set; }
            public string Tag_Reviewed_Medium_Risk_Access { get; set; }
            public string Tag_Cloud_Access { get; set; }
            public string Tag_Temporary_Account { get; set; }
            public string Tag_Low_Risk_Access { get; set; }
            public string Tag_Regular_Access { get; set; }
            public string Tag_Anomaly_Access { get; set; }
            public string Tag_Access_Groups { get; set; }
            public string Tag_Test_Account { get; set; }
            public string Tag_Risk_Not_Reviewed_Access { get; set; }
            public string Tag_System_Account { get; set; }
            public string Tag_Derived_Administrator_Access { get; set; }
            public string Tag_High_Risk_Access { get; set; }
            public string Tag_Indefinite_Access { get; set; }
            public string Tag_Administrator_Access { get; set; }
            public string Tag_L1_Exclusion_PreAuthorized { get; set; }
            public string Tag_No_Risk_Access { get; set; }
            public string Tag_Unauthorized_Access { get; set; }
            public string Tag_Orphaned_Account { get; set; }
            public string Tag_Shared_Account { get; set; }
            public string Tag_Service_Account { get; set; }
            public string Tag_L2_Exclusion_PreAuthorized { get; set; }
            public string Tag_Expired_Access { get; set; }
            public string Tag_Deleted_Account { get; set; }
            public string Tag_Authorization_Groups { get; set; }
            public string Tag_Temporary_Access { get; set; }
            public string Tag_Locked_Account { get; set; }
            public string Tag_Privileged_Access { get; set; }
            public string Tag_Disabled_Account { get; set; }
            public string Tag_RejectedAccess { get; set; }
            public string Tag_Exceptional_Access { get; set; }
            public string Tag_Derived_Privileged_Access { get; set; }
            public string Tag_Unprotected_Resource { get; set; }
            public string Tag_Derived_Administrative_Access { get; set; }
            public string Tag_Privileged_Resource { get; set; }
            public string Tag_Auto_Approval { get; set; }
            public string Tag_Reviewed_High_Risk_Access { get; set; }
            public string Tag_Owner_Access { get; set; }
            public string Tag_Administrative_Access { get; set; }
            public string Tag_Expired_Account { get; set; }
            public string Tag_Owner_Approval { get; set; }
            public string Tag_ViolatingAccess { get; set; }
            public string Tag_Manager_Approval { get; set; }
            public string Tag_Reviewed_Low_Risk_Access { get; set; }
            public string Special_Consideration { get; set; }
            public string GID_Number { get; set; }
            public string Special_Instructions { get; set; }
            public string Business_Group_Type { get; set; }
            public string Group_Location { get; set; }
            public string Special_Delete_Instructions { get; set; }
            public string Reference_ID { get; set; }
            public string Linux_Enabled { get; set; }
            public string Company_IAM_Global_Identifier { get; set; }
            public string Company_IAM_Control_Data { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Present_In { get; set; }
            public string Unique_Id { get; set; }
            public string[] owners { get; set; }
            public string[] tags { get; set; }
            public List<AccessEntityOwners> entityOwners { get; set; }
        }

       public class AccessEntityOwners
        {
            public string id { get; set; } 
            public string name { get; set; }    
            public string type { get; set; }
            public string entityTypeId { get; set; }
        }

        public class RBAC_Model
        {
            public string Short_Name { get; set; }
            public string id { get; set; }
            public string entityTypeId { get; set; }
        }

        public class RequestDataName
        {
            public string Short_Name { get; set; }
            public string name { get; set; }
            public string Asset { get; set; }
            public string Business_Group_Type { get; set; }
            public string Company_IAM_Control_Data { get; set; }
            public string Description { get; set; }
            public string GID_Number { get; set; }
            public string Reference_ID { get; set; }
            public string assetId { get; set; }
            public string entityTypeId { get; set; }
            public string entityTypeName { get; set; }
            public long id { get; set; }
            public string[] owners { get; set; }
        }
    }
}
