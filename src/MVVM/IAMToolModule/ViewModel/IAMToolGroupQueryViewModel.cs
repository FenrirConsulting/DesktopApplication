using IAMHeimdall.Core;
using IAMHeimdall.MVVM.Model;
using IAMHeimdall.Resources.Commands;
using SimpleImpersonation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.DirectoryServices;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MSG = GalaSoft.MvvmLight.Messaging;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class IAMToolGroupQueryViewModel : BaseViewModel
    {
        #region Delegates
        public RelayCommand GetMembersCommand { get; set; }
        public RelayCommand ClearFormCommand { get; set; }
        public RelayCommand UserQueryCommand { get; set; }

        private const string V = "";
        #endregion Delegates

        #region Properties
        // Loads Group Query Attributes into String Array
        public static string[] GroupQueryAttributesArray = ConfigurationProperties.GroupQueryAttributes.ToArray();

        // Binds Group Name Entry Box
        private string groupNamesEntryBox;
        public string GroupNamesEntryBox
        {
            get { return groupNamesEntryBox; }
            set
            {
                if (groupNamesEntryBox != value)
                {
                    groupNamesEntryBox = value;
                    if (groupNamesEntryBox.Length > 3)
                    {
                        GetMembersEnabled = true;
                    }
                    else
                    {
                        GetMembersEnabled = false;
                    }
                    OnPropertyChanged();
                }
            }
        }

        // Finds selected Cell Value to open Record
        private DataGridCellInfo cellInfo;
        public DataGridCellInfo CellInfo
        {
            get { return cellInfo; }
            set { cellInfo = value; OnPropertyChanged(); }
        }

        // Holds String to send to User Query Lookup Box
        public static string SelectedUserQuery = V;

        // Binds Loading Spinner to Bool
        private bool isLoadBool;
        public bool IsLoadBool
        {
            get { return isLoadBool; }
            set { if (isLoadBool != value) { isLoadBool = value; OnPropertyChanged(); } }
        }

        // Binds Data Buttons Enabled Bool
        private bool getMembersEnabled;
        public bool GetMembersEnabled
        {
            get { return getMembersEnabled; }
            set { if (getMembersEnabled != value) { getMembersEnabled = value; OnPropertyChanged(); } }
        }

        // Binds Nested Groups Checkbox
        private bool nestedGroupChecked;
        public bool NestedGroupChecked
        {
            get { return nestedGroupChecked; }
            set { if (nestedGroupChecked != value) { nestedGroupChecked = value; OnPropertyChanged(); } }
        }

        // Binds Centrify Groups Checkboox
        private bool centrifyGroupsChecked;
        public bool CentrifyGroupsChecked
        {
            get { return centrifyGroupsChecked; }
            set { if (centrifyGroupsChecked != value) { centrifyGroupsChecked = value; OnPropertyChanged(); } }
        }

        // Search Terms for ComboBox
        private readonly ObservableCollection<ComboBoxListItem> domainSearchTerms = new()
        {
        };
        // Basis for Search Terms Collection
        public IEnumerable<ComboBoxListItem> DomainSearchTerms
        {
            get { return domainSearchTerms; }
        }

        // Selected Term for ComboBox
        private ComboBoxListItem selectedDomainTerm = new();
        public ComboBoxListItem SelectedDomainTerm
        {
            get { return selectedDomainTerm; }
            set { selectedDomainTerm = value; OnPropertyChanged(); }
        }

        // Binds Display Data Table
        private DataTable membersDisplayTable;
        public DataTable MembersDisplayTable
        {
            get { return membersDisplayTable; }
            set { membersDisplayTable = value; OnPropertyChanged(); }
        }

        // View Used to Display DisplayTable
        private DataView membersDisplayView;
        public DataView MembersDisplayView
        {
            get { return membersDisplayView; }
            set { membersDisplayView = value; OnPropertyChanged(); }
        }

        // Binds Status Label String
        private string statusString;
        public string StatusString
        {
            get { return statusString; }
            set
            {
                if (statusString != value)
                {
                    statusString = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion Properties

        #region Methods
        public IAMToolGroupQueryViewModel()
        {
            // Initialize Properties
            MembersDisplayTable = new();
            MembersDisplayView = new();
            IsLoadBool = false;
            NestedGroupChecked = false;
            CentrifyGroupsChecked = false;
            StatusString = "";
            MSG.Messenger.Default.Register<String>(this, SentMessage);

            // Initialize Relay Commands
            GetMembersCommand = new RelayCommand(o => { GetMembersButton(); });
            ClearFormCommand = new RelayCommand(o => { ClearForm(); });
            UserQueryCommand = new RelayCommand(o => { SelectUserQuery(); });
            
            MembersDisplayTable = new("Temporary Table");

            foreach (string s in ConfigurationProperties.ADDomains)
            {
                domainSearchTerms.Add(new ComboBoxListItem { BoxItem = s });
            }
        }
        #endregion Methods

        #region Functions
        // On Load Method
        public void LoadData()
        {
            try
            {

            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }

        // Retrieve Members from Group Name Box Input
        public async void GetMembersButton()
        {
            try 
            {
                MembersDisplayTable = new();
                IsLoadBool = true;
                MembersDisplayTable.Columns.Clear();
                MembersDisplayView = null;
                SetColumns(MembersDisplayTable);
                StatusString = "Searching...";
                await Task.Run(() => 
                {
                    try
                    {
                        Thread.CurrentPrincipal = new System.Security.Principal.WindowsPrincipal(WindowsIdentity.GetCurrent());
                        MembersDisplayTable = GetMembers();
                    }
                    catch (Exception ex)
                    {
                        ExceptionOutput.Output(ex.ToString());
                    }
                });
                MembersDisplayView = MembersDisplayTable.DefaultView;
                StatusString = "Found : " + MembersDisplayTable.Rows.Count.ToString() + " Groups and Members in AD.";
                IsLoadBool = false;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Main Member Fetch Function
        public DataTable GetMembers()
        {
            DataTable tempTable = new();
            SetColumns(tempTable);
            try 
            {
                SetColumns(tempTable);
                StatusString = "";
                string groupNamesText = GroupNamesEntryBox;
                string selectedDomain = SelectedDomainTerm.BoxItem.ToString();
                string[] groups = groupNamesText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                // Clean Input and Remove Duplicates
                for (int i = 0; i < groups.Length; i++)
                {
                    //clean up the line
                    groups[i] = groups[i].Trim();
                    groups[i] = groups[i].ToLower();
                    groups[i] = Regex.Replace(groups[i], "[^ -~]*", "");
                    // If there is a / in group string strips out all characters up to and including it.
                    if (groups[i].ToLower().Contains('\\')) { string str = groups[i][(groups[i].IndexOf('\\') + 1)..]; groups[i] = str; }
                }
                var dgroups = groups.Distinct();
                var rebuild = string.Join(Environment.NewLine, dgroups);
                GroupNamesEntryBox = rebuild;

                foreach (var group in dgroups)
                {
                    if (group != null && group != "" && group != "*") // Avoid Searching Entire AD by removing * as an option to search
                    {
                        List<string> DNsofGroups = new();
                        DNsofGroups = GetGroupDNFromName(group, selectedDomain);
                        //No groups were found that match
                        if (DNsofGroups.Count == 0)
                        {
                            StatusString = "This group name does not exist on this domain";
                        }
                        else
                        {
                            foreach (string DNofGroup in DNsofGroups)
                            {
                                if (CentrifyGroupsChecked == true)
                                {
                                    //Only want to query for groups that have *cfy* in the name when the Centrify checkbox is checked

                                    if (DNofGroup.ToLower().Contains("cfy"))
                                    {
                                        //Call the function to get the members of this group
                                        tempTable = GetADGroupMembers(selectedDomain, DNofGroup, group);
                                    }
                                }
                                else
                                {
                                    //Call the function to get the members of this group
                                    tempTable = GetADGroupMembers(selectedDomain, DNofGroup, group);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
            return tempTable;
        }

        // Main Fetch AD Memebers Function
        public DataTable GetADGroupMembers(string fullDomainName, string DNOfGroupToQuery, string OriginalSearchTerm)
        {
            DataTable tempTable = new();
            SetColumns(tempTable);
            try 
            {
                DirectoryEntry groupEntry = new("LDAP://" + fullDomainName);

                //This search is used to get local domain members.
                DirectorySearcher mySearcher = new(groupEntry)
                {
                    Filter = "(memberOf=" + DNOfGroupToQuery + ")",
                    PageSize = 1000
                };
                mySearcher.PropertiesToLoad.AddRange(GroupQueryAttributesArray);
                SearchResultCollection mySearchResults = mySearcher.FindAll();

                //This search is used to get foreign domain members (stored as a foreign SID in the 'member' attribute of the group).
                DirectorySearcher groupSearcher = new(groupEntry)
                {
                    Filter = "(distinguishedname=" + DNOfGroupToQuery + ")"
                };
                groupSearcher.PropertiesToLoad.Add("name");
                groupSearcher.PropertiesToLoad.Add("member");
                groupSearcher.PropertiesToLoad.Add("objectcategory");
                groupSearcher.PropertiesToLoad.Add("distinguishedname");
                SearchResult groupSearchResult = groupSearcher.FindOne();

                String ThisGroupsDomain = fullDomainName;
                String ThisGroupsName = groupSearchResult.Properties["name"][0].ToString();

                //If the group entered is domain users or domain computers, don't do it. Those groups contain every object.
                if (ThisGroupsName.ToLower().Contains("domain users") || ThisGroupsName.ToLower().Contains("domain computers"))
                {
                    StatusString = "This group is not supported by Heimdall Tool";
                    groupEntry.Dispose();
                    mySearcher.Dispose();
                    mySearchResults.Dispose();
                    groupSearcher.Dispose();
                    return tempTable;
                }

                List<SearchResult> MasterListOfMembersForThisGroup = new();

                //Grabs foreign object from Group Member Attribute.
                foreach (var member in groupSearchResult.Properties["member"])
                {
                    if (member.ToString().Contains("CN=S-1-5-21"))
                    {
                        //This is a foreign security principal. Need to query another domain to get this object's details.
                        String ForeginSidToQuery = member.ToString();

                        //Chop up the string to get only the ObjectSID portion of it
                        ForeginSidToQuery = ForeginSidToQuery.Replace("CN=", "");
                        ForeginSidToQuery = ForeginSidToQuery.Substring(0, ForeginSidToQuery.IndexOf(","));

                        SearchResult ForignSearchResult = GetForeignADObject(ForeginSidToQuery);

                        if (ForignSearchResult == null)
                        {
                            //Foreign object was not found.
                            StatusString = "Group or Member object not found in AD: " + ForeginSidToQuery;
                        }
                        else
                        {
                            //Foreign object was found. Add it to the master list of searchresult objects to be processed later.
                            MasterListOfMembersForThisGroup.Add(ForignSearchResult);
                        }
                    }
                }

                foreach (SearchResult mySearchResult in mySearchResults)
                {
                    // These are the objects that are NOT foreign objects. Add it to the master list of searchresult objects to be processed later.
                    MasterListOfMembersForThisGroup.Add(mySearchResult);
                }

                if (MasterListOfMembersForThisGroup.Count == 0)
                {
                    //There are no members in this group.
                    StatusString = "N/A - There are no members in this group";

                    groupEntry.Dispose();
                    mySearcher.Dispose();
                    mySearchResults.Dispose();
                    groupSearcher.Dispose();
                    return tempTable;
                }

                //loop through the master list of searchresult objects, containing both the foreign and non-foreign objects, and write values to listview
                foreach (SearchResult mySearchResult in MasterListOfMembersForThisGroup)
                {
                    //track if this object is a group or not, for nested searching purposes
                    Boolean IsAGroup = false;

                    //build a dictionary of attribute name = value
                    var MemberDictionary = new Dictionary<string, string>();

                    //loop throuh each attribute name in the global list of attributes to return, and add it to the dictionary of attributes / values
                    foreach (string AttributeName in GroupQueryAttributesArray)
                    {
                        MemberDictionary.Add(AttributeName.ToLower(), "");
                    }

                    //loop through the properties in the search result
                    foreach (string strProperty in mySearchResult.Properties.PropertyNames)
                    {
                        //if the property name matches one of the dictionary keys, update the value of that dictionary key with the search result's value
                        if (MemberDictionary.ContainsKey(strProperty.ToLower()))
                        {
                            //Clean up values of attributes so that they are nice and human-readable for output
                            //Making dates look like dates, clean up useraccountcontrol into enable/disable values, etc
                            string CleanedValue = CleanUpADValue(strProperty.ToLower(), mySearchResult);
                            MemberDictionary[strProperty.ToLower()] = CleanedValue;
                        }
                    }

                    //now we have a memberdictionary populated with attribute name / value pairs, and the values have been cleaned into a human readable format

                    //what domain is the group member object from? (could be a foreign object from another domain)
                    String ThisMembersDomain = mySearchResult.Path;
                    ThisMembersDomain = ThisMembersDomain.Replace("LDAP://", "");
                    ThisMembersDomain = ThisMembersDomain.Substring(0, ThisMembersDomain.IndexOf("/"));

                    //is this a Group? need to know if we are grabbing nested members
                    if (MemberDictionary["objectcategory"].Contains("group"))
                    {
                        IsAGroup = true;
                    }

                    DataRow NewRow;
                    NewRow = tempTable.NewRow();
                    NewRow["GroupName"] = ThisGroupsName;
                    NewRow["GroupDomain"] = ThisGroupsDomain;
                    NewRow["MemberDomain"] = ThisMembersDomain;
                    NewRow["Name"] = MemberDictionary[GroupQueryAttributesArray[0].ToLower()];
                    NewRow["ObjectCategory"] = MemberDictionary[GroupQueryAttributesArray[1].ToLower()];
                    NewRow["SamAccountName"] = MemberDictionary[GroupQueryAttributesArray[2].ToLower()];
                    NewRow["EmployeeNumber"] = MemberDictionary[GroupQueryAttributesArray[3].ToLower()];
                    NewRow["UserAccountControl"] = MemberDictionary[GroupQueryAttributesArray[4].ToLower()];
                    NewRow["Mail"] = MemberDictionary[GroupQueryAttributesArray[5].ToLower()];
                    NewRow["DistinguishedName"] = MemberDictionary[GroupQueryAttributesArray[6].ToLower()];
                    NewRow["LastLogon"] = MemberDictionary[GroupQueryAttributesArray[7].ToLower()];
                    tempTable.Rows.Add(NewRow);

                    //if this object is a group, and the nested box is checked, need to call this funciton recursively to get the nested members
                    //this will loop recursively until there are no more groups within groups
                    //could run into trouble here if we have any bad nesting in AD. eg two groups that are nested in each other...
                    if (IsAGroup && NestedGroupChecked == true)
                    {
                        tempTable = GetADGroupMembers(ThisMembersDomain, MemberDictionary["distinguishedname"], OriginalSearchTerm);
                    }
                }

                groupEntry.Dispose();
                mySearcher.Dispose();
                mySearchResults.Dispose();
                groupSearcher.Dispose();
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
            return tempTable;
        }

        // Get DN From Strings Passed In.
        public static List<string> GetGroupDNFromName(String nameOfGroupToQuery, String fullDomainName)
        {
            List<string> DnsOfGroups = new() { };
            try
            {
                
                DirectoryEntry groupEntry = new("LDAP://" + fullDomainName);
                DirectorySearcher mySearcher = new(groupEntry);
                String LDAPFilter = "(&(name=" + nameOfGroupToQuery + ")(objectCategory=group))";
                mySearcher.Filter = LDAPFilter;
                mySearcher.PropertiesToLoad.Add("distinguishedname");
                SearchResultCollection mySearchResults = null;

                try
                {
                    mySearchResults = mySearcher.FindAll();
                }
                catch (Exception ex)
                {
                    ExceptionOutput.Output(ex.ToString());
                    MessageBox.Show("Failed connecting to domain '" + fullDomainName + "'. Potential causes for this error:" + Environment.NewLine + Environment.NewLine +
                    "-Domain name entered is incorrect" + Environment.NewLine +
                    "-Domain does not allow non-authenticated queries" + Environment.NewLine +
                    "-No trust in place for this domain" + Environment.NewLine +
                    "-Network / Firewall issue blocking connection to the domain", "Error: Unable to connect to domain", MessageBoxButton.OK, MessageBoxImage.Error);
                    return DnsOfGroups;
                }

                if (mySearchResults != null)
                {
                    //loop through the results and add the DNS to the string list
                    foreach (SearchResult mySearchResult in mySearchResults)
                    {
                        DnsOfGroups.Add(mySearchResult.Properties["distinguishedname"][0].ToString());
                    }
                    mySearcher.Dispose();
                    groupEntry.Dispose();
                    mySearchResults.Dispose();
                    return DnsOfGroups;
                }
                else
                {
                    mySearcher.Dispose();
                    groupEntry.Dispose();
                    mySearchResults.Dispose();
                    return DnsOfGroups;
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
            return DnsOfGroups;
        }

        //Clean up the value of the AD attribute, and send a String back, from AD Group Reader
        public static string CleanUpADValue(string AttributeName, SearchResult searchResult)
        {
            string cleanedUpReturnValue = "";

            try 
            {
                if (searchResult.Properties[AttributeName].Count > 1)
                {
                    //This is a multi-valued attribute. Need to loop through and concatenate the values
                    //For attributes like memberof, proxyaddresses
                    String combinedDelimitedString = "";

                    foreach (string singleValue in searchResult.Properties[AttributeName])
                    {
                        combinedDelimitedString += singleValue + ";";
                    }

                    return combinedDelimitedString;
                }
                if (AttributeName.ToLower().Equals("objectcategory"))
                {
                    if (searchResult.Properties["objectcategory"][0].ToString().Contains("Person"))
                    {
                        cleanedUpReturnValue = "person";
                    }
                    else if (searchResult.Properties["objectcategory"][0].ToString().Contains("Group"))
                    {
                        cleanedUpReturnValue = "group";
                    }
                    else if (searchResult.Properties["objectcategory"][0].ToString().Contains("Computer"))
                    {
                        cleanedUpReturnValue = "computer";
                    }
                    else if (searchResult.Properties["objectcategory"][0].ToString().Contains("Foreign"))
                    {
                        cleanedUpReturnValue = "foreign security principal";
                    }
                    else
                    {
                        cleanedUpReturnValue = searchResult.Properties[AttributeName.ToLower()][0].ToString();
                    }
                }
                else if (AttributeName.ToLower().Equals("accountexpires") ||
                AttributeName.ToLower().Equals("badpasswordtime") ||
                AttributeName.ToLower().Equals("lastlogoff") ||
                AttributeName.ToLower().Equals("lastlogon") ||
                AttributeName.ToLower().Equals("lastlogontimestamp") ||
                AttributeName.ToLower().Equals("lockouttime") ||
                AttributeName.ToLower().Equals("pwdlastset"))
                {
                    if ((long)searchResult.Properties[AttributeName][0] == 9223372036854775807 ||
                        (long)searchResult.Properties[AttributeName][0] == 0 ||
                        searchResult.Properties[AttributeName][0] == null ||
                        searchResult.Properties[AttributeName][0].ToString().Equals(""))
                    {
                        cleanedUpReturnValue = "never";
                    }
                    else
                    {
                        //convert from filetime to datetime
                        System.DateTime datetimevalue = System.DateTime.FromFileTime((long)searchResult.Properties[AttributeName][0]);
                        cleanedUpReturnValue = datetimevalue.ToString("MM/dd/yyyy hh:mm");
                    }
                }
                else if (AttributeName.ToLower().Equals("objectsid") ||
                 AttributeName.ToLower().Equals("msRTCSIP-OriginatorSid".ToLower()) ||
                 AttributeName.ToLower().Equals("msExchMasterAccountSid".ToLower()) ||
                 AttributeName.ToLower().Equals("sIDHistory".ToLower()))
                {
                    //convert to a human-readable SID value
                    SecurityIdentifier si = new((byte[])searchResult.Properties[AttributeName][0], 0);
                    cleanedUpReturnValue = si.ToString();
                }
                else if (AttributeName.ToLower().Equals("useraccountcontrol"))
                {
                    cleanedUpReturnValue = searchResult.Properties[AttributeName][0] switch
                    {
                        512 or 544 or 1049088 or 1049120 or 2097696 or 2097664 or 66048 or 66080 or 262656 or 262688 or 328192 or 328224 or 1114624 or 131616 => "Enabled - " + searchResult.Properties[AttributeName][0].ToString(),
                        514 or 546 or 1049122 or 66050 or 66082 or 262658 or 262690 or 328194 or 328226 or 1049090 or 131618 => "Disabled - " + searchResult.Properties[AttributeName][0].ToString(),
                        _ => "Unknown - " + searchResult.Properties[AttributeName][0].ToString(),
                    };
                }
                else if (AttributeName.ToLower().Equals("objectguid") ||
                   AttributeName.ToLower().Equals("msExchMailboxGuid"))
                {
                    cleanedUpReturnValue = new Guid((System.Byte[])searchResult.Properties[AttributeName][0]).ToString();
                }
                else
                {
                    cleanedUpReturnValue = searchResult.Properties[AttributeName.ToLower()][0].ToString();
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }

            return cleanedUpReturnValue;
        }

        // Send SID to find on Known Domains
        public static SearchResult GetForeignADObject(string ForeignSidToQuery)
        {
            SearchResult sr = null;
            try 
            {
                string fullDomainName = "";
                //List of Known Domain SIDs
                if (ForeignSidToQuery.Contains("S-1-5-21-2130522478-1725188094-786498627"))
                {
                    fullDomainName = "corp.Company.com";
                }
                else if (ForeignSidToQuery.Contains("S-1-5-21-4258776639-1271169360-244890835"))
                {
                    fullDomainName = "rx.net";
                }
                else if (ForeignSidToQuery.Contains("S-1-5-21-1715567821-2111687655-682003330"))
                {
                    fullDomainName = "procare.pcare.ltd";
                }
                else if (ForeignSidToQuery.Contains("S-1-5-21-1139409664-453791681-928725530"))
                {
                    fullDomainName = "pharmacare.pcare.ltd";
                }
                else if (ForeignSidToQuery.Contains("S-1-5-21-238693470-80480483-1541874228"))
                {
                    fullDomainName = "ridc.pcare.ltd";
                }
                else if (ForeignSidToQuery.Contains("S-1-5-21-1808345313-2269461769-1081395210"))
                {
                    fullDomainName = "minclinic.local";
                }
                else if (ForeignSidToQuery.Contains("S-1-5-21-2702892183-608350096-274610827"))
                {
                    fullDomainName = "corp.Company.com";
                }
                else if (ForeignSidToQuery.Contains("S-1-5-21-2396041630-3241229156-1003068976"))
                {
                    fullDomainName = "corporate.mhrx.local";
                }
                else if (ForeignSidToQuery.Contains("S-1-5-21-3413624531-4195256222-3378527014"))
                {
                    fullDomainName = "storz.Company.com";
                }
                else if (ForeignSidToQuery.Contains("S-1-5-21-2480620504-3585254612-2290200040"))
                {
                    fullDomainName = "coe.rx.net";
                }
                else if (ForeignSidToQuery.Contains("S-1-5-21-606747145-1303643608-682003330"))
                {
                    fullDomainName = "pcare.ltd";
                }
                else if (ForeignSidToQuery.Contains("S-1-5-21-2326735829-1461535259-1004279218"))
                {
                    fullDomainName = "corp.omnicare.com";
                }
                if (fullDomainName == "")
                {
                    //This foreign sid is coming from an unknown / unsupported domain. Return nothing
                    return null;
                }
                //Search the Domain for the object SID
                DirectoryEntry de = new("LDAP://" + fullDomainName);
                DirectorySearcher ds = new(de)
                {
                    Filter = "(objectsid=" + ForeignSidToQuery + ")"
                };

                ds.PropertiesToLoad.AddRange(GroupQueryAttributesArray);
                sr = ds.FindOne();
                de.Dispose();
                ds.Dispose();
            }
             catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
            return sr;
        }

        // Select Group To Lookup in 
        public void SelectUserQuery()
        {
            try
            {
                DataRowView drv = (DataRowView)CellInfo.Item;
                if (drv != null)
                {
                    DataRow row = drv.Row;
                    if (row != null)
                    {
                        string record = row["SamAccountName"].ToString();
                        SelectedUserQuery = record;
                        MSG.Messenger.Default.Send("Select IAM Tool Tab 3");
                        MSG.Messenger.Default.Send("IAM Tool User Query Selected");
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }

        }

        // Reset Form
        public void ClearForm()
        {
            GroupNamesEntryBox = "";
            MembersDisplayTable.Clear();
            MembersDisplayView = MembersDisplayTable.DefaultView;
            NestedGroupChecked = false;
            CentrifyGroupsChecked = false;
            SelectedDomainTerm = DomainSearchTerms.ElementAt(0);
        }

        // Reset Column Names and Values
        public static void SetColumns(DataTable SentColumn)
        {
            SentColumn.Columns.Clear();
            SentColumn.Columns.Add("GroupName", typeof(String));
            SentColumn.Columns.Add("GroupDomain", typeof(String));
            SentColumn.Columns.Add("MemberDomain", typeof(String));
            SentColumn.Columns.Add("Name", typeof(String));
            SentColumn.Columns.Add("ObjectCategory", typeof(String));
            SentColumn.Columns.Add("SamAccountName", typeof(String));
            SentColumn.Columns.Add("EmployeeNumber", typeof(String));
            SentColumn.Columns.Add("UserAccountControl", typeof(String));
            SentColumn.Columns.Add("Mail", typeof(String));
            SentColumn.Columns.Add("DistinguishedName", typeof(String));
            SentColumn.Columns.Add("LastLogon", typeof(String));
        }
        #endregion Functions

        #region Messages
        // Listen to New Record for Database Reload
        public void SentMessage(string passedMessage)
        {
            if (passedMessage != null)
            {
                if (passedMessage == "IAM Tool Group Query Selected")
                {
                    GroupNamesEntryBox = IAMToolGetComputerRolesViewModel.SelectedGroupQuery;
                }
            }
        }
        #endregion
    }
}