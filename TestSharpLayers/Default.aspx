<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TestSharpLayers._Default" %>

<%@ Register Assembly="SharpLayers" Namespace="SharpLayers" TagPrefix="cc1" %>
<%@ Register Assembly="SharpLayers" Namespace="SharpLayers.Controls" TagPrefix="cc1" %>
<%@ Register Assembly="SharpLayers" Namespace="SharpLayers.Layers" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
    <!-- <script src="http://maps.google.com/maps?file=api&amp;v=2&amp;key=ABQIAAAAQHrNIFOT6kTymwvVIRjdoBT2yXp_ZAY8_ufC3CFXhHIE1NvwkxSMLHmV-xOthJlgViFGVaYz1zHowg"
        type="text/javascript"></script> -->
    <style type="text/css">
        .myClass
        {
            height: 30px;
            background-color: Yellow;
            z-index: 100000;
        }
        .olLayerGooglePoweredBy_gmnoprint
        {
            position: absolute;
            bottom: 20px;
        }
        .olLayerGoogleCopyright
        {
            position: absolute;
            bottom: 5px;
        }
        #Map1
        {
            left: 0px;
            right: 0px;
            bottom: 0px;
            top: 0px;
            width: auto;
            height: auto;
        }
        Body
        {
            padding: 0;
            margin: 0;
        }
        #navtoolbar1
        {
            background-color: Red;
            height: 20px;
        }
        #navtoolbar1 div
        {
            width: 20px;
            height: 20px;
            margin-right: 5px;
            background-color: Green;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div id="scalediv1" style="position: absolute; top: 5px; left: 5px; z-index: 10000">
    </div>
    <cc1:Map ID="Map1" runat="server" Width="100%" Height="700px" />
    <div id="layerSwitcher1" style="position: absolute; right: 5px; width: 200px">
    </div>
    <div id="attribution1">
    </div>
    <div id="navtoolbar1">
    </div>
    <div id="scaleDiv">
    </div>
    <div id="coordDiv">
    </div>
    </form>
</body>
</html>
