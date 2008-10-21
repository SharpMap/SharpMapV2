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
                                <wms:WmsLayer Name="ARoads" />
                                <wms:WmsLayer Name="BRoads" />
                                <wms:WmsLayer Name="UnclassifiedRoads" />
                                <wms:WmsLayer Name="PrimaryRoads" />
                                <wms:WmsLayer Name="Streets" />
                            </WmsLayerNames>
                            <WmsServerUrls>
                                <cc1:ResourceUri Uri="http://a.sharplayers.newgrove.com/sharpmapv2demo/Maps/Wms.ashx" />
                                <cc1:ResourceUri Uri="http://b.sharplayers.newgrove.com/sharpmapv2demo/Maps/Wms.ashx" />
                                <cc1:ResourceUri Uri="http://c.sharplayers.newgrove.com/sharpmapv2demo/Maps/Wms.ashx" />
                                <cc1:ResourceUri Uri="http://d.sharplayers.newgrove.com/sharpmapv2demo/Maps/Wms.ashx" />
                                <cc1:ResourceUri Uri="http://e.sharplayers.newgrove.com/sharpmapv2demo/Maps/Wms.ashx" />
                                <cc1:ResourceUri Uri="http://f.sharplayers.newgrove.com/sharpmapv2demo/Maps/Wms.ashx" />
                                <cc1:ResourceUri Uri="http://g.sharplayers.newgrove.com/sharpmapv2demo/Maps/Wms.ashx" />
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
            </LayerComponents>
        </cc1:MapHostExtender>
    </div>
    </form>
</body>
</html>
