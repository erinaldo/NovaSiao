﻿Imports CamadaBLL
Imports CamadaDTO

Public Class frmVendaEntrada
    Private _formOrigem As Form
    Private _vlMaximo As Double
    Private _VerAlteracao As Boolean
    Private _Pag As clMovimentacao
    Private _Acao As FlagAcao
    Private bindPag As New BindingSource
    '
    '-------------------------------------------------------------------------------------------------
    ' SUB NEW
    '-------------------------------------------------------------------------------------------------
    Sub New(fOrigem As Form, TranVlTotal As Double,
            Pag As clMovimentacao, Acao As FlagAcao, Optional Posicao As Point = Nothing)
        '
        ' This call is required by the designer.
        InitializeComponent()
        '
        ' Add any initialization after the InitializeComponent() call.
        _formOrigem = fOrigem
        _VerAlteracao = False
        _vlMaximo = TranVlTotal
        _Pag = Pag
        _Acao = Acao
        '
        bindPag.DataSource = _Pag
        PreencheDataBinding()
        '
        ' Verifica valor do Combo MovTipo pelo MovForma
        If IsNothing(_Pag.IDMovForma) Then
            cmbFormaTipo.SelectedValue = -1
        Else
            Dim dtForma As DataTable = cmbForma.DataSource
            For Each r As DataRow In dtForma.Rows
                If r("IDMovForma") = _Pag.IDMovForma Then
                    cmbFormaTipo.SelectedValue = r("IDMovTipo")
                End If
            Next
        End If
        '
        lblConta.Text = ObterDefault("Conta")
        lblFilial.Text = ObterDefault("Filial")
        '
        '--- Define a posicao do form
        If Not IsNothing(Posicao) Then
            StartPosition = FormStartPosition.Manual
            Location = Posicao
        End If
        '
        _VerAlteracao = True
        '
    End Sub
    '
    '------------------------------------------------------------------------------------------
    ' PREENCHE O DATABINDING
    '------------------------------------------------------------------------------------------
    Private Sub PreencheDataBinding()
        '
        txtValor.DataBindings.Add("Text", bindPag, "EntradaValor", True, DataSourceUpdateMode.OnPropertyChanged)
        '
        ' CARREGA OS COMBOBOX
        CarregaCmbTipo()
        CarregaCmbForma()
        '
        ' FORMATA OS VALORES DO DATABINDING
        AddHandler txtValor.DataBindings("Text").Format, AddressOf FormatCUR
        '
    End Sub
    '
    Private Sub FormatCUR(sender As Object, e As System.Windows.Forms.ConvertEventArgs)
        e.Value = FormatCurrency(e.Value, 2)
    End Sub
    '
    '------------------------------------------------------------------------------------------
    ' CARREGAR OS COMBOBOX
    '------------------------------------------------------------------------------------------
    Private Sub CarregaCmbTipo()
        Dim TipoBLL As New MovimentacaoBLL
        Dim dtTipo As DataTable = TipoBLL.MovTipo_GET
        '
        With cmbFormaTipo
            .DataSource = dtTipo
            .DisplayMember = "MovTipo"
            .ValueMember = "IDMovTipo"
            .DataBindings.Add("SelectedValue", bindPag, "IDMovTipo", True, DataSourceUpdateMode.OnPropertyChanged)
        End With
        '
    End Sub
    '
    Private Sub CarregaCmbForma()
        Dim TipoBLL As New MovimentacaoBLL
        Dim dtForma As DataTable = TipoBLL.MovForma_GET_DT
        '
        With cmbForma
            .DataSource = dtForma
            .DisplayMember = "MovForma"
            .ValueMember = "IDMovForma"
            .DataBindings.Add("SelectedValue", bindPag, "IDMovForma", True, DataSourceUpdateMode.OnPropertyChanged)
        End With
        '
    End Sub
    '
    '------------------------------------------------------------------------------------------
    ' CRIA UM ATALHO PARA OS COMBO BOX
    '------------------------------------------------------------------------------------------
    Private Sub cmbFormaTipo_KeyPress(sender As Object, e As KeyPressEventArgs) Handles cmbFormaTipo.KeyPress
        If Char.IsNumber(e.KeyChar) Then
            e.Handled = True
            '
            Dim dt As DataTable = cmbFormaTipo.DataSource
            Dim rCount As Integer = dt.Rows.Count
            Dim item As Integer = e.KeyChar.ToString
            '
            If item <= rCount And item > 0 Then
                Dim Valor As Integer = dt.Rows(e.KeyChar.ToString - 1)("IDMovTipo")
                '
                _Pag.IDMovTipo = Valor
                cmbFormaTipo.SelectedValue = Valor
                '
            End If
        End If
    End Sub
    '
    '-------------------------------------------------------------------------------------------------
    ' SELECIONA A FORMA DE PAGAMENTO CRIANDO ATALHO NUMERICO
    '-------------------------------------------------------------------------------------------------
    Private Sub cmbForma_KeyPress(sender As Object, e As KeyPressEventArgs) Handles cmbForma.KeyPress
        If Char.IsNumber(e.KeyChar) Then
            e.Handled = True
            '
            Dim num As Integer = e.KeyChar.ToString
            Dim dtF As DataTable = cmbForma.DataSource
            Dim i As Integer = 1
            '
            For Each r As DataRow In dtF.Rows
                If r("IDMovTipo") = cmbFormaTipo.SelectedValue Then
                    If num = i Then
                        _Pag.IDMovForma = r("IDMovForma")
                        cmbForma.DataBindings("SelectedValue").ReadValue()
                        Exit For
                    Else
                        i += 1
                    End If
                End If
            Next
        End If
        '
    End Sub
    '
    '-------------------------------------------------------------------------------------------------
    ' CONSTRUIR UMA BORDA NO FORMULÁRIO
    '-------------------------------------------------------------------------------------------------
    Protected Overrides Sub OnPaintBackground(ByVal e As PaintEventArgs)
        MyBase.OnPaintBackground(e)

        Dim rect As New Rectangle(0, 0, Me.ClientSize.Width - 1, Me.ClientSize.Height - 1)
        Dim pen As New Pen(Color.SlateGray, 3)

        e.Graphics.DrawRectangle(pen, rect)
    End Sub
    '
    Private Sub Me_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Escape Then
            e.Handled = True
            '
            btnCancelar_Click(New Object, New EventArgs)
        End If
    End Sub
    '
    '------------------------------------------------------------------------------------------
    ' ACAO DOS BOTOES
    '------------------------------------------------------------------------------------------
    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        '
        '--- Verifica campos/valores
        If IsNothing(_Pag.IDMovTipo) Then
            MessageBox.Show("O campo TIPO de Entrada não pode ficar vazio..." & vbNewLine &
                            "Favor escolher um valor para esse campo.", "Campo Vazio",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            cmbFormaTipo.Focus()
            cmbFormaTipo.DroppedDown = True
            Exit Sub
        End If
        '
        If IsNothing(_Pag.IDMovForma) Then
            MessageBox.Show("O campo FORMA de Entrada não pode ficar vazio..." & vbNewLine &
                            "Favor escolher um valor para esse campo.", "Campo Vazio",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            cmbForma.Focus()
            cmbForma.DroppedDown = True
            Exit Sub
        End If
        '
        If IsNothing(_Pag.MovValor) OrElse _Pag.MovValor <= 0 OrElse _Pag.MovValor > _vlMaximo Then
            MessageBox.Show("O VALOR da Entrada não pode ser igual ou menor que Zero..." & vbNewLine &
                            "bem como também não pode ser maior que o total da venda..." & vbNewLine &
                            "Favor escolher um valor para esse campo.", "Campo Vazio",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            txtValor.Focus()
            txtValor.SelectAll()
            Exit Sub
        End If
        '
        '--- Devolve o pagamento para o formOrigem
        Select Case _formOrigem.Name
            Case "frmVendaVista"
                _Pag.MovForma = cmbFormaTipo.Text
                _Pag.IDMovTipo = cmbFormaTipo.SelectedValue
                _Pag.IDMovForma = cmbForma.SelectedValue
                '
                If _Acao = FlagAcao.INSERIR Then
                    DirectCast(_formOrigem, frmVendaVista).Pagamentos_Manipulacao(_Pag, FlagAcao.INSERIR)
                ElseIf _Acao = FlagAcao.EDITAR Then

                End If
                '
            Case Else
                MessageBox.Show("Ainda não implementado")
        End Select
        Close()
    End Sub
    '
    Private Sub btnCancelar_Click(sender As Object, e As EventArgs) Handles btnCancelar.Click
        Close()
    End Sub
    '
    '------------------------------------------------------------------------------------------
    ' ALTERA A FORMA DE PAGAMENTO PELA ALTERACAO DO TIPO DE ENTRADA
    '------------------------------------------------------------------------------------------
    Private Sub cmbFormaTipo_Validated(sender As Object, e As EventArgs) Handles cmbFormaTipo.Validated
        If _VerAlteracao = False Then Exit Sub
        If IsNothing(cmbFormaTipo.SelectedValue) Then Exit Sub
        '
        Dim dtForma As DataTable = cmbForma.DataSource
        dtForma.DefaultView.RowFilter = "IDMovTipo = " & cmbFormaTipo.SelectedValue
        '
        _VerAlteracao = False
        If cmbForma.Items.Count = 1 Then
            cmbForma.SelectedIndex = 0
            _Pag.IDMovForma = dtForma.Rows(0).Item("IDMovForma")
        Else
            cmbForma.SelectedIndex = -1
        End If
        _VerAlteracao = True
        '
    End Sub
    '
    '------------------------------------------------------------------------------------------
    ' USAR TAB AO KEYPRESS ENTER
    '------------------------------------------------------------------------------------------
    Private Sub txtValor_KeyDown(sender As Object, e As KeyEventArgs) Handles txtValor.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            SendKeys.Send("{Tab}")
        End If
    End Sub
    '
    '-------------------------------------------------------------------------------------------------
    ' CRIAR EFEITO VISUAL DE FORM SELECIONADO
    '-------------------------------------------------------------------------------------------------
    Private Sub frmAReceberItem_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        If IsNothing(_formOrigem) Then Exit Sub
        '
        For Each c As Control In _formOrigem.Controls
            If c.Name = "Panel1" Then
                c.BackColor = Color.Silver
            End If
        Next
    End Sub
    '
    Private Sub frmVendaPagamento_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        If IsNothing(_formOrigem) Then Exit Sub
        '
        For Each c As Control In _formOrigem.Controls
            If c.Name = "Panel1" Then
                c.BackColor = Color.SlateGray
            End If
        Next
    End Sub
    '
End Class
