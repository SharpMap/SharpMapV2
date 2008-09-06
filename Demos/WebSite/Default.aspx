<%@ Page Language="C#" MasterPageFile="~/DemoMaster.Master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="SharpMap.Presentation.AspNet.Demo.Default"
    Title="Untitled Page" %>

<%@ Import Namespace="System.IO" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <ul>
        <li>All the handlers respond to the querystring parameter BBOX (e.g BBOX=-180,-90,180,90)</li>
        <li>Map and caching map respond to the param mimeType (e.g mimeType=image/jpeg)</li>
        <li>Wms responds to all the same params as the WmsDemo in the DemoWebSite Project</li>
    </ul>
    <asp:Repeater ID="repDemos" runat="server">
        <ItemTemplate>
            <div>
                <asp:HyperLink runat="server" ID="hlDemo" NavigateUrl='<%# string.Format("~/Maps/{0}", ((FileInfo)Container.DataItem).Name) %>'
                    Text='<%# ((FileInfo)Container.DataItem).Name %>' />
            </div>
        </ItemTemplate>
    </asp:Repeater>
</asp:Content>
