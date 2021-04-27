<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BOD._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-sm">
            <h3>BOD</h3>
            <asp:Button ID="btnProcess" runat="server" Text="Process" OnClick="btnProcess_Click" CssClass="btn btn-primary" />
            <asp:Label ID="lblTxt" runat="server" Font-Bold="true" Font-Size="Large"></asp:Label>
        </div>
    </div>

</asp:Content>
