<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Webform1.aspx.cs" Inherits="TestSharpLayers.Webform1" %>

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
        <asp:Panel ID="Panel1" runat="server" Height="623px">
        </asp:Panel>
        <div style="background-color: Green;">
            <asp:Panel ID="toolBar1" runat="server" Height="100px" BackColor="Black" CssClass="olControlEditingToolbar" />
        </div>
        <cc1:MapHostExtender ID="Panel1_MapHostExtender" runat="server" Enabled="True" TargetControlID="Panel1">
            <BuilderParams FallThrough="true">
                <TileSize Height="256" Width="256" />
            </BuilderParams>
            <Tools>
                <edit:EditingTools runat="server" ID="editTool1">
                    <BuilderParams EditableLayerId="VectorLayerComponent1" TargetElementId="toolBar1" />
                </edit:EditingTools>
                <switch:LayerSwitcherTool runat="server" ID="layerSwitcherTool1">
                    <BuilderParams />
                </switch:LayerSwitcherTool>
            </Tools>
            <LayerComponents>
                <tms:TmsLayerComponent ID="TmsLayerComponent1" runat="server" Name="My Tms Layer">
                    <BuilderParams Attribution="2008 &copy; John Diss" />
                </tms:TmsLayerComponent>
                <wms:WmsLayerComponent ID="WmsLayerComponent1" runat="server" Name="My Wms Layer">
                    <BuilderParams Attribution="Newgrove &copy; 2008" Visibility="true" Units="m" DisplayInLayerSwitcher="true"
                        IsBaseLayer="false">
                        <TileSize Height="256" Width="256" />
                        <MaxExtent Bottom="0" Left="0" Right="2000000" Top="2000000" />
                        <WmsParameters WmsVersion="1.3.0" Crs="EPSG:27700">
                            <WmsLayerNames>
                                <wms:StringValue Value="ARoads" />
                                <wms:StringValue Value="BRoads" />
                                <wms:StringValue Value="UnclassifiedRoads" />
                                <wms:StringValue Value="PrimaryRoads" />
                                <wms:StringValue Value="Streets" />
                            </WmsLayerNames>
                            <WmsServerUrls>
                                <cc1:UriValue Value="http://a.sharplayers.newgrove.com/sharpmapv2demo/Maps/Wms.ashx" />
                                <cc1:UriValue Value="http://b.sharplayers.newgrove.com/sharpmapv2demo/Maps/Wms.ashx" />
                                <cc1:UriValue Value="http://c.sharplayers.newgrove.com/sharpmapv2demo/Maps/Wms.ashx" />
                                <cc1:UriValue Value="http://d.sharplayers.newgrove.com/sharpmapv2demo/Maps/Wms.ashx" />
                                <cc1:UriValue Value="http://e.sharplayers.newgrove.com/sharpmapv2demo/Maps/Wms.ashx" />
                                <cc1:UriValue Value="http://f.sharplayers.newgrove.com/sharpmapv2demo/Maps/Wms.ashx" />
                                <cc1:UriValue Value="http://g.sharplayers.newgrove.com/sharpmapv2demo/Maps/Wms.ashx" />
                            </WmsServerUrls>
                        </WmsParameters>
                    </BuilderParams>
                </wms:WmsLayerComponent>
                <vector:VectorLayerComponent runat="server" ID="VectorLayerComponent1" Name="My Vector Layer">
                    <BuilderParams Attribution="Newgrove Vectors Inc" DisplayInLayerSwitcher="true" IsBaseLayer="false"
                        Units="m" Visibility="true">
                        <MaxExtent Bottom="0" Left="0" Right="2000000" Top="2000000" />
                    </BuilderParams>
                </vector:VectorLayerComponent>
                <vector:VectorLayerComponent runat="server" ID="vector2">
                    <BuilderParams Attribution="aaa" Protocol="httpProtocol">
                        <Strategies>
                            <strategy:BBoxStrategy runat="server" />
                        </Strategies>
                    </BuilderParams>
                </vector:VectorLayerComponent>
            </LayerComponents>
        </cc1:MapHostExtender>
        <protocol:HttpProtocolComponent FormatType="GeoJsonFormat" runat="server" ID="httpProtocol">
            <BuilderParams Url="http://localhost">
                <Formats>
                    <format:GeoJsonFormat />
                </Formats>
            </BuilderParams>
        </protocol:HttpProtocolComponent>
    </div>
    </form>
</body>
</html>
