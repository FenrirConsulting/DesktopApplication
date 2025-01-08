using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using IAMHeimdall.MVVM.Model;
using IAMHeimdall.MVVM.ViewModel;
using Microsoft.SharePoint.Client;
using PnP.Framework;
using SharePointPnP;

namespace IAMHeimdall.Core
{
    public class SharePointMethods
    {
        static string TenantId = "fabb61b8-3afe-4e75-b934-a47f782b8cd7";

        #region Functions
        // SharePoint List Insert Method, Updates all Items to passed DataTable
        public async Task<bool> Insert(DataTable passedTable, bool updateAll)
        {
            bool returnFunction = false;

            if (passedTable.Rows.Count > 0)
            {
                await Task.Run(() => {
                    try
                    {
                        string siteUrl = ConfigurationProperties.ServiceCatalogSPSite;
                        var authManager = new OfficeDevPnP.Core.AuthenticationManager();
                        using (var clientContext = authManager.GetAppOnlyAuthenticatedContext(siteUrl, ServiceCatalogMainViewModel.SPClientId, ServiceCatalogMainViewModel.SPClientSecret, TenantId))
                        {
                            Web oWebsite = clientContext.Web;
                            List scList = clientContext.Web.Lists.GetByTitle("Service Catalog");
                            ListCollection collList = oWebsite.Lists;
                            clientContext.Load(collList);
                            clientContext.ExecuteQuery();

                            if (updateAll == true)
                            {
                                DeleteItems(clientContext, scList);
                            }

                            foreach (DataRow row in passedTable.Rows)
                            {
                                ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                                ListItem newItem = scList.AddItem(itemCreateInfo);
                                var charsToRemove = new string[] { ";" };

                                newItem["SQLID"] = row["ID"].ToString();
                                string cleanedServices = row["Services"].ToString();
                                foreach (var c in charsToRemove)
                                {
                                    cleanedServices = cleanedServices.Replace(c, string.Empty);
                                }
                                newItem["Services"] = cleanedServices;
                                newItem["SubSystem"] = row["SubSystem"].ToString();
                                newItem["Description"] = row["Description"].ToString();
                                newItem["RequestSource"] = row["RequestSource"].ToString();
                                newItem["AssignmentGroup"] = row["AssignmentGroup"].ToString();
                                newItem["ExpediteProcess"] = row["ExpediteProcess"].ToString();
                                newItem["SLA"] = row["SLA"].ToString();
                                newItem["ArcherAppID"] = row["ArcherAppID"].ToString();
                                newItem["ITPMAppID"] = row["ITPMAppID"].ToString();
                                newItem["Hints"] = row["Hints"].ToString();
                                newItem["Tasks"] = row["Tasks"].ToString();
                                newItem["Domain"] = row["Domain"].ToString();
                                newItem["Article"] = row["Article"].ToString();
                                newItem["ProvisioningTeam"] = row["ProvisioningTeam"].ToString();
                                newItem["URL"] = row["URL"].ToString();
                                newItem["Environment"] = row["Environment"].ToString();
                                newItem["AutomationStatus"] = row["AutomationStatus"].ToString();
                                newItem["LastModifiedDate"] = DateTime.Now.ToShortDateString();
                                newItem["LastModifiedBy"] = App.GlobalUserInfo.DisplayName;
                                newItem.Update();
                                clientContext.ExecuteQuery();
                            }

                            
                            returnFunction = true;
                        };
                    }
                    catch (Exception ex)
                    {
                        ExceptionOutput.Output(ex.ToString());
                    }
                });
            }
            return returnFunction;
        }

        // Remove all Items from passed SharePoint List
        public void DeleteItems(ClientContext passedContext, List passedList)
        {
            try
            {
                CamlQuery query = CamlQuery.CreateAllItemsQuery(4000);
                ListItemCollection items = passedList.GetItems(query);
                passedContext.Load(items);
                passedContext.ExecuteQuery();
                for (int i = items.Count - 1; i >= 0; i--)
                {
                    items[i].DeleteObject();
                }
                passedContext.ExecuteQuery();
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }   
        }

        // Update Passed SharePoint List Item
        public async Task<bool> UpdateItem(ServiceCatalogItemModel CurrentItem)
        {
            bool functionReturned = false;
            await Task.Run(() => {
                try
                {
                    string siteUrl = ConfigurationProperties.ServiceCatalogSPSite;
                    var authManager = new OfficeDevPnP.Core.AuthenticationManager();
                    using (var clientContext = authManager.GetAppOnlyAuthenticatedContext(siteUrl, ServiceCatalogMainViewModel.SPClientId, ServiceCatalogMainViewModel.SPClientSecret, TenantId))
                    {
                        Web oWebsite = clientContext.Web;
                        List scList = clientContext.Web.Lists.GetByTitle("Service Catalog");
                        ListCollection collList = oWebsite.Lists;
                        clientContext.Load(collList);
                        clientContext.ExecuteQuery();

                        CamlQuery query = new();
                        query.ViewXml = "<View><Query><Where><Eq><FieldRef Name='SQLID'/>" +
                        "<Value Type='Number'> " + CurrentItem.ID.ToString() + "</Value></Eq></Where></Query><RowLimit>1000</RowLimit></View>";
                        ListItemCollection foundItems = scList.GetItems(query);
                        clientContext.Load(foundItems);
                        clientContext.ExecuteQuery();
                        ListItem selectedItem = foundItems[0];

                        string joinedDomains = string.Join(";", CurrentItem.Domain);

                        var charsToRemove = new string[] { ";" };
                        string cleanedServices = CurrentItem.Services;
                        foreach (var c in charsToRemove)
                        {
                            cleanedServices = cleanedServices.Replace(c, string.Empty);
                        }

                        selectedItem["Services"] = cleanedServices;
                        selectedItem["SubSystem"] = CurrentItem.SubSystem;
                        selectedItem["Description"] = CurrentItem.Description;
                        selectedItem["RequestSource"] = CurrentItem.RequestSource;
                        selectedItem["AssignmentGroup"] = CurrentItem.AssignmentGroup;
                        selectedItem["ExpediteProcess"] = CurrentItem.ExpediteProcess;
                        selectedItem["SLA"] = CurrentItem.SLA;
                        selectedItem["ArcherAppID"] = CurrentItem.ArcherAppID;
                        selectedItem["ITPMAppID"] = CurrentItem.ITPMAppID;
                        selectedItem["Hints"] = CurrentItem.Hints;
                        selectedItem["Tasks"] = CurrentItem.Tasks;
                        selectedItem["Domain"] = joinedDomains;
                        selectedItem["Article"] = CurrentItem.Article;
                        selectedItem["ProvisioningTeam"] = CurrentItem.ProvisioningTeam;
                        selectedItem["URL"] = CurrentItem.URL;
                        selectedItem["Environment"] = CurrentItem.Environment;
                        selectedItem["AutomationStatus"] = CurrentItem.AutomationStatus;
                        selectedItem["LastModifiedDate"] = DateTime.Now.ToShortDateString();
                        selectedItem["LastModifiedBy"] = App.GlobalUserInfo.DisplayName;

                        selectedItem.Update();

                        clientContext.ExecuteQuery();
                    };
                }
                catch (Exception ex)
                {
                    ExceptionOutput.Output(ex.ToString());
                }
            });
            functionReturned = true;
            return functionReturned;
        }

        // Update Passed SharePoint List Item
        public async Task<bool> UpdateAllItems(DataTable passedTable)
        {
            bool functionReturned = false;
            await Task.Run(() => {
                try
                {
                    string siteUrl = ConfigurationProperties.ServiceCatalogSPSite;
                    var authManager = new OfficeDevPnP.Core.AuthenticationManager();
                    using (var clientContext = authManager.GetAppOnlyAuthenticatedContext(siteUrl, ServiceCatalogMainViewModel.SPClientId, ServiceCatalogMainViewModel.SPClientSecret, TenantId))
                    {
                        Web oWebsite = clientContext.Web;
                        List scList = clientContext.Web.Lists.GetByTitle("Service Catalog");
                        ListCollection collList = oWebsite.Lists;
                        clientContext.Load(collList);
                        clientContext.ExecuteQuery();

                        foreach (DataRow row in passedTable.Rows)
                        {
                            
                            CamlQuery query = new();
                            query.ViewXml = "<View><Query><Where><Eq><FieldRef Name='SQLID'/>" +
                            "<Value Type='Number'> " + row["ID"].ToString() + "</Value></Eq></Where></Query><RowLimit>1000</RowLimit></View>";
                            ListItemCollection foundItems = scList.GetItems(query);
                            clientContext.Load(foundItems);
                            clientContext.ExecuteQuery();

                            ListItem selectedItem = foundItems[0];
                            string joinedDomains = string.Join(";", row["Domain"]);
                            var charsToRemove = new string[] { ";" };

                            string cleanedServices = row["Services"].ToString();
                            foreach (var c in charsToRemove)
                            {
                                cleanedServices = cleanedServices.Replace(c, string.Empty);
                            }
                            selectedItem["Services"] = cleanedServices;
                            selectedItem["SubSystem"] = row["SubSystem"].ToString();
                            selectedItem["Description"] = row["Description"].ToString();
                            selectedItem["RequestSource"] = row["RequestSource"].ToString();
                            selectedItem["AssignmentGroup"] = row["AssignmentGroup"].ToString();
                            selectedItem["ExpediteProcess"] = row["ExpediteProcess"].ToString();
                            selectedItem["SLA"] = row["SLA"].ToString();
                            selectedItem["ArcherAppID"] = row["ArcherAppID"].ToString();
                            selectedItem["ITPMAppID"] = row["ITPMAppID"].ToString();
                            selectedItem["Hints"] = row["Hints"].ToString();
                            selectedItem["Tasks"] = row["Tasks"].ToString();
                            selectedItem["Domain"] = row["Domain"].ToString();
                            selectedItem["Article"] = row["Article"].ToString();
                            selectedItem["ProvisioningTeam"] = row["ProvisioningTeam"].ToString();
                            selectedItem["URL"] = row["URL"].ToString();
                            selectedItem["Environment"] = row["Environment"].ToString();
                            selectedItem["AutomationStatus"] = row["AutomationStatus"].ToString();
                            selectedItem["LastModifiedDate"] = DateTime.Now.ToShortDateString();
                            selectedItem["LastModifiedBy"] = App.GlobalUserInfo.DisplayName;
                            selectedItem.Update();
                            clientContext.ExecuteQuery();
                        }
                    };
                }
                catch (Exception ex)
                {
                    ExceptionOutput.Output(ex.ToString());
                }
            });
            functionReturned = true;
            return functionReturned;
        }

        // Function to Delete a single Item given passed ID value
        public async Task<bool> DeleteSingleItem(string passedID)
        {
            bool returnFunction = false;
            await Task.Run(() => {
                try
                {
                    string siteUrl = ConfigurationProperties.ServiceCatalogSPSite;
                    var authManager = new OfficeDevPnP.Core.AuthenticationManager();
                    using (var clientContext = authManager.GetAppOnlyAuthenticatedContext(siteUrl, ServiceCatalogMainViewModel.SPClientId, ServiceCatalogMainViewModel.SPClientSecret, TenantId))
                    {
                        Web oWebsite = clientContext.Web;
                        List scList = clientContext.Web.Lists.GetByTitle("Service Catalog");
                        ListCollection collList = oWebsite.Lists;
                        clientContext.Load(collList);
                        clientContext.ExecuteQuery();

                        CamlQuery query = new();
                        query.ViewXml = "<View><Query><Where><Geq><FieldRef Name='SQLID'/>" +
                        "<Value Type='Number'> " + passedID + "</Value></Geq></Where></Query><RowLimit>100</RowLimit></View>";
                        ListItemCollection foundItems = scList.GetItems(query);
                        clientContext.Load(foundItems);
                        clientContext.ExecuteQuery();
                        ListItem selectedItem = foundItems[0];
                        selectedItem.DeleteObject();
                        clientContext.ExecuteQuery();
                    }
                }
                catch (Exception ex)
                {
                    ExceptionOutput.Output(ex.ToString());
                }
            });
            return returnFunction;
        }
        #endregion
    }
}
