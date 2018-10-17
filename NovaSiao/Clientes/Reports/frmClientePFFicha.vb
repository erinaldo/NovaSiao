﻿Imports CamadaDTO
'
Public Class frmClientePFFicha
    Private _ClientePF As clClientePF
    '
    Private Sub btnFechar_Click(sender As Object, e As EventArgs) Handles btnFechar.Click
        Close()
    End Sub
    '
    Sub New(myClientePF As clClientePF)
        '
        ' This call is required by the designer.
        InitializeComponent()
        '
        ' Add any initialization after the InitializeComponent() call.
        _ClientePF = myClientePF
        '
    End Sub
    '
    Private Sub frmClientePF_Ficha_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        '
        'Criar a lista de ClientePF
        Dim lista As New List(Of clClientePF)
        lista.Add(_ClientePF)
        '
        Dim ds As New Microsoft.Reporting.WinForms.ReportDataSource("dsClientePF", lista)
        '
        '--- define o relatório
        rptvClienteFicha.LocalReport.DataSources.Clear()
        rptvClienteFicha.LocalReport.DataSources.Add(ds)
        rptvClienteFicha.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout)
        rptvClienteFicha.RefreshReport()
        '
        '--- define o tamanho
        'Me.Width = Application.OpenForms("frmPrincipal").Width - 200
        Dim tamMaxH = Application.OpenForms("frmPrincipal").Height
        Height = tamMaxH - (tamMaxH * 20) / 100
        '
        '--- define a posicao
        Dim posX As Integer = Width / 2 - 30
        Location = New Point(posX, 50)
        '
    End Sub
    '
End Class
