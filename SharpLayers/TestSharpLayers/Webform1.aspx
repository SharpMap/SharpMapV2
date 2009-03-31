<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Webform1.aspx.cs" Inherits="SharpMap.Demo.SharpLayers.Webform1" %>

<%@ Register Assembly="SharpMap.Presentation.Web.SharpLayers" Namespace="SharpMap.Presentation.Web.SharpLayers.Layers"
    TagPrefix="cc1" %>
<%@ Register Assembly="SharpMap.Presentation.Web.SharpLayers" Namespace="SharpMap.Presentation.Web.SharpLayers.Layers.Tms"
    TagPrefix="tms" %>
<%@ Register Assembly="SharpMap.Presentation.Web.SharpLayers" Namespace="SharpMap.Presentation.Web.SharpLayers.Layers.Wms"
    TagPrefix="wms" %>
<%@ Register Assembly="SharpMap.Presentation.Web.SharpLayers" Namespace="SharpMap.Presentation.Web.SharpLayers.Layers.Vector"
    TagPrefix="vector" %>
<%@ Register Assembly="SharpMap.Presentation.Web.SharpLayers" Namespace="SharpMap.Presentation.Web.SharpLayers"
    TagPrefix="cc1" %>
<%@ Register Assembly="SharpMap.Presentation.Web.SharpLayers" Namespace="SharpMap.Presentation.Web.SharpLayers.Controls"
    TagPrefix="tools" %>
<%@ Register Assembly="SharpMap.Presentation.Web.SharpLayers" Namespace="SharpMap.Presentation.Web.SharpLayers.Controls.Edit"
    TagPrefix="edit" %>
<%@ Register Assembly="SharpMap.Presentation.Web.SharpLayers" Namespace="SharpMap.Presentation.Web.SharpLayers.Controls.LayerSwitcher"
    TagPrefix="switch" %>
<%@ Register Assembly="SharpMap.Presentation.Web.SharpLayers" Namespace="SharpMap.Presentation.Web.SharpLayers.Protocol.Http"
    TagPrefix="protocol" %>
<%@ Register Assembly="SharpMap.Presentation.Web.SharpLayers" Namespace="SharpMap.Presentation.Web.SharpLayers.Strategy"
    TagPrefix="strategy" %>
<%@ Register Assembly="SharpMap.Presentation.Web.SharpLayers" Namespace="SharpMap.Presentation.Web.SharpLayers.Format"
    TagPrefix="format" %>
<%@ Register Assembly="SharpMap.Presentation.Web.SharpLayers" Namespace="SharpMap.Presentation.Web.SharpLayers.Styles"
    TagPrefix="style" %>
<%@ Register Assembly="SharpMap.Presentation.Web.SharpLayers" Namespace="SharpMap.Presentation.Web.SharpLayers.Controls.Nav"
    TagPrefix="nav" %>
<%@ Register Assembly="SharpMap.Presentation.Web.SharpLayers" Namespace="SharpMap.Presentation.Web.SharpLayers.Controls"
    TagPrefix="gen" %>
<%@ Register Assembly="SharpMap.Presentation.Web.SharpLayers" Namespace="SharpMap.Presentation.Web.SharpLayers.Controls.Containers"
    TagPrefix="pnl" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="theme/default/style.css" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <asp:Panel ID="mapPanel1" runat="server" Height="700px">
        </asp:Panel>
        <style:SldComponent runat="server" ID="stylemap1">
            <BuilderParams SldDocumentPath="mysld.xml"/>
        </style:SldComponent>
        <cc1:MapHostExtender ID="Panel1_MapHostExtender" runat="server" Enabled="True" TargetControlID="mapPanel1">
            <BuilderParams FallThrough="true">
                <TileSize Height="256" Width="256" />
            </BuilderParams>
            <Tools>
                <edit:EditingTools runat="server" ID="editTool1">
                    <BuilderParams EditableLayerId="VectorLayerComponent1" />
                </edit:EditingTools>
                <switch:LayerSwitcherTool runat="server" ID="layerSwitcherTool1">
                    <BuilderParams />
                </switch:LayerSwitcherTool>
                <gen:GenericOLControl ID="GenericOLControl1" runat="server">
                    <BuilderParams OpenLayersClassName="OpenLayers.Control.Navigation" />
                </gen:GenericOLControl>
                <gen:GenericOLControl ID="GenericOLControl2" runat="server">
                    <BuilderParams OpenLayersClassName="OpenLayers.Control.PanZoom" />
                </gen:GenericOLControl>
            </Tools>
            <LayerComponents>
                <wms:WmsLayerComponent ID="WmsLayerComponent1" runat="server" Name="My Wms Layer">
                    <BuilderParams Attribution="Newgrove &copy; 2008" Visibility="true" Units="degrees"
                        DisplayInLayerSwitcher="true" IsBaseLayer="true" WrapDateLine="true">
                        <TileSize Height="256" Width="256" />
                        <MaxExtent Bottom="-90" Left="-180" Right="180" Top="90" />
                        <WmsParameters WmsVersion="1.3.0" Crs="EPSG:4326">
                            <WmsLayerNames>
                                <cc1:StringValue Value="Countries" />
                                <cc1:StringValue Value="Rivers" />
                                <cc1:StringValue Value="Cities" />
                            </WmsLayerNames>
                            <WmsServerUrls>
                                <cc1:UriValue Value="http://localhost:50322/Maps/Map.ashx" />
                                <%--This should be updated to the port number assigned by vs dev web server or IIS to the SharpMap.Demo.AspNet project --%>
                            </WmsServerUrls>
                        </WmsParameters>
                    </BuilderParams>
                </wms:WmsLayerComponent>
                <vector:VectorLayerComponent runat="server" ID="VectorLayerComponent1" Name="My Vector Layer">
                    <BuilderParams Attribution="Newgrove Vectors Inc" DisplayInLayerSwitcher="true" IsBaseLayer="false"
                        Units="m" Visibility="true" WrapDateLine="true">
                        <MaxExtent Bottom="0" Left="0" Right="2000000" Top="2000000" />
                    </BuilderParams>
                </vector:VectorLayerComponent>
            </LayerComponents>
        </cc1:MapHostExtender>
    </div>
    </form>
</body>
</html>
