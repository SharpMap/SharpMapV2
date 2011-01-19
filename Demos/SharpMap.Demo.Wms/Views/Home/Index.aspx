<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="cssContent" ContentPlaceHolderID="CustomCssContent" runat="server">
    <%--<link rel="stylesheet" href="<%=Url.Content("~/Content/polymaps.css?v=1")%>" />--%>
    <link rel="stylesheet" href="http://openlayers.org/dev/theme/default/style.css?v=1"
        type="text/css" />
    <link rel="stylesheet" href="http://openlayers.org/dev/examples/style.css?v=1" type="text/css" />
</asp:Content>
<asp:Content ID="jsContent" ContentPlaceHolderID="CustomJsContent" runat="server">
    <%--<script type="text/javascript" src="<%=Url.Content("~/Scripts/polymaps.js?v=1")%>"></script>--%>
    <%--<script type="text/javascript" src="<%=Url.Content("~/Scripts/globalmaptiles.js?v=1")%>"></script>--%>
    <%--<script type="text/javascript" src="<%=Url.Content("~/Scripts/poly_script.js?v=1")%>"></script>--%>

    <script type="text/javascript" src="http://openlayers.org/dev/OpenLayers.js"></script>

    <script type="text/javascript" src="<%=Url.Content("~/Scripts/ol_script.js?v=1")%>"></script>

</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <div id="map" class="smallmap">
    </div>
</asp:Content>
