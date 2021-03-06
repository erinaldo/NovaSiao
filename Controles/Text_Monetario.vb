﻿Imports System.Windows.Forms
Public Class Text_Monetario
    Inherits TextBox

    Sub New()
        Me.TextAlign = LeftRightAlignment.Right
    End Sub

    Private Sub Text_Monetario_GotFocus(sender As Object, e As EventArgs) Handles Me.GotFocus
        Me.SelectAll()
    End Sub

    Private Sub Text_Monetario_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Me.KeyPress
        If e.KeyChar = ","c Then
            e.Handled = False
        ElseIf e.KeyChar = "."c Then
            SelectedText = ","
            e.Handled = True
        ElseIf Char.IsNumber(e.KeyChar) Then
            e.Handled = False
        ElseIf e.KeyChar = vbBack Then
            e.Handled = False
        Else
            e.Handled = True
        End If
    End Sub

    Private Sub Text_Monetario_LostFocus(sender As Object, e As EventArgs) Handles Me.LostFocus
        If Me.TextLength > 0 Then
            If IsNumeric(Me.Text) = False OrElse CDbl(Me.Text) < 0 Then
                MessageBox.Show("Valor númerico incorreto..." &
                                vbNewLine & "Digite um valor numérico maior ou igual a 0 (zero)",
                                "Valor Incorreto", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Me.Focus()
            Else
                If Me.Text = String.Empty Then
                    Me.Text = 0
                End If
                Me.Text = FormatCurrency(Me.Text, 2, TriState.True, TriState.True,
                                            TriState.True)
            End If
        End If
    End Sub


End Class
