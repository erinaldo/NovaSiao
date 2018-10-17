﻿Imports CamadaDTO
Imports CamadaBLL
'
Public Class frmFornecedorProcurar
    '
    Private WithEvents listForn As New List(Of clFornecedor)
    Private WithEvents bindF As New BindingSource
    Private _Procura As Boolean
    Private ImgInativo As Image = My.Resources.block
    Private ImgAtivo As Image = My.Resources.accept
    Private _formOrigem As Form
    '
    '--- PROPRIEDADES DE RESPOSTA
    Property propFornecedor_Escolha As clFornecedor
    '
    '--- LOAD
#Region "LOAD FORM"
    Sub New(Optional Procura As Boolean = False, Optional formOrigem As Form = Nothing)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        CarregaCmbAtivo()
        _Procura = Procura
        _formOrigem = formOrigem
        '
        '--- Verifica se o form foi aberto para procura ou para controle de fornecedores 
        If Procura = True Then '--- nesse caso foi aberto para procura
            btnEditar.Text = "&Escolher"
            btnEditar.Image = My.Resources.accept
            btnAdicionar.Enabled = False
            btnFechar.Text = "&Cancelar"
            lblTitulo.Text = "Escolher Fornecedor"
        Else
            btnEditar.Text = "&Editar"
            btnEditar.Image = My.Resources.editar
            btnAdicionar.Enabled = True
            btnFechar.Text = "&Fechar"
            lblTitulo.Text = "Procurar Fornecedor"
        End If
        '
    End Sub
    '
    Private Sub Me_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim fBLL As New FornecedorBLL
        '
        Try
            listForn = fBLL.GetFornecedores()
        Catch ex As Exception
            MessageBox.Show("Houve uma exceção ao obter a lista de Fornecedores..." & vbNewLine &
                            ex.Message, "Exceção", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        '
        dgvListagem.DataSource = listForn
        PreencheListagem()
        '
    End Sub
    '
#End Region
    '
#Region "PREENCHE LISTAGEM E COMBO"
    '
    Private Sub PreencheListagem()
        '
        With dgvListagem
            .Columns.Clear()
            .AutoGenerateColumns = False
            ' altera as propriedades importantes
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .MultiSelect = False
            .ColumnHeadersVisible = True
            .AllowUserToResizeRows = False
            .AllowUserToResizeColumns = False
            .RowHeadersWidth = 36
            .RowTemplate.Height = 30
            .StandardTab = True
        End With
        '
        ' (1) COLUNA REG
        dgvListagem.Columns.Add("clnID", "Reg.")
        With dgvListagem.Columns("clnID")
            .DataPropertyName = "IDPessoa"
            .Width = 70
            .Visible = True
            .ReadOnly = True
            .Resizable = False
            .SortMode = DataGridViewColumnSortMode.NotSortable
            Dim newPadding As New Padding(5, 0, 0, 0)
            .DefaultCellStyle.Padding = newPadding
        End With
        '
        ' (2) COLUNA FORNECEDOR | CADASTRO
        dgvListagem.Columns.Add("clnRazao", "Fornecedor")
        With dgvListagem.Columns("clnRazao")
            .DataPropertyName = "Cadastro"
            .Width = 300
            .Visible = True
            .ReadOnly = True
            .Resizable = False
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft
        End With
        '
        ' (3) Coluna da imagem
        Dim colImage As New DataGridViewImageColumn
        With colImage
            .Name = "Ativo"
            .Resizable = False
            .Width = 70
        End With
        dgvListagem.Columns.Add(colImage)
        '
        ' (4) COLUNA ATIVO
        dgvListagem.Columns.Add("FornAtivo", "Ativo")
        With dgvListagem.Columns("FornAtivo")
            .DataPropertyName = "Ativo"
            .Width = 50
            .Visible = False
            .ReadOnly = True
            .Resizable = False
            .SortMode = DataGridViewColumnSortMode.NotSortable
        End With
        '
        '--- Preenche o DataSource com um Filtro
        FiltrarListagem()
        '
    End Sub
    '
    Private Sub PreencheListaImagem()
        For Each r As DataGridViewRow In dgvListagem.Rows
            If r.Cells(3).Value = True Then
                r.Cells(2).Value = ImgAtivo
            Else
                r.Cells(2).Value = ImgInativo
            End If
        Next
    End Sub
    '
    '--- FILTRAR LISTAGEM
    Private Sub FiltrarListagem()
        '
        dgvListagem.DataSource = listForn.FindAll(AddressOf FindForn)
        '
        '--- Preenche as imagens do Ativo
        PreencheListaImagem()
        '
    End Sub
    '
    Private Function FindForn(ByVal T As clFornecedor) As Boolean
        '
        If T.Ativo = cmbAtivo.SelectedValue AndAlso T.Cadastro.ToLower Like "*" & txtProcura.Text.ToLower & "*" Then
            Return True
        Else
            Return False
        End If
        '
    End Function
    '
    '--- PREENCHE O COMBO ATIVO
    Private Sub CarregaCmbAtivo()
        Dim dtAtivo As New DataTable
        'Adiciona todas as possibilidades de instrucao
        dtAtivo.Columns.Add("Ativo")
        dtAtivo.Columns.Add("Texto")
        dtAtivo.Rows.Add(New Object() {False, "Inativo"})
        dtAtivo.Rows.Add(New Object() {True, "Ativo"})
        With cmbAtivo
            .DataSource = dtAtivo
            .ValueMember = "Ativo"
            .DisplayMember = "Texto"
            .SelectedValue = True
        End With
    End Sub
    '
#End Region
    '
#Region "ACAO DOS BOTOES"
    '
    Private Sub btnFechar_Click(sender As Object, e As EventArgs) Handles btnFechar.Click
        If _Procura = True Then '--- se for um form de PROCURA
            Me.DialogResult = DialogResult.Cancel
        Else '--- se for um form de CONTROLE EDICAO
            Close()
            MostraMenuPrincipal()
        End If
    End Sub
    '
    Private Sub btnAdicionar_Click(sender As Object, e As EventArgs) Handles btnAdicionar.Click
        Dim forn As New clFornecedor
        Dim frmF As New frmFornecedor(forn)
        frmF.MdiParent = Application.OpenForms.OfType(Of frmPrincipal).First
        Close()
        frmF.Show()
    End Sub
    '
    Private Sub btnEditar_Click(sender As Object, e As EventArgs) Handles btnEditar.Click
        '
        '--- verifica se existe algum item selecionado
        If dgvListagem.SelectedRows.Count = 0 Then
            MessageBox.Show("Não existe nenhum Fornecedor selecionado...", "Escolher",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            dgvListagem.Focus()
            Exit Sub
        End If
        '
        '--- Seleciona o Fornecedor
        Dim forn As clFornecedor = dgvListagem.SelectedRows(0).DataBoundItem
        '
        If _Procura = True Then '--- se for para escolha e procura
            'Dim ID As Integer = dgvListagem.SelectedRows(0).Cells("clnID").Value
            propFornecedor_Escolha = forn
            Me.DialogResult = DialogResult.OK
        Else '--- se for para EDICAO
            Dim frmF As New frmFornecedor(forn)
            frmF.MdiParent = Application.OpenForms.OfType(Of frmPrincipal).First
            Close()
            frmF.Show()
        End If
        '
    End Sub
    '
#End Region
    '
#Region "PROCURA"
    '
    Private Sub txtProcura_TextChanged(sender As Object, e As EventArgs) Handles txtProcura.TextChanged
        FiltrarListagem()
    End Sub
    '
    Private Sub cmbAtivo_SelectedValueChanged(sender As Object, e As EventArgs) Handles cmbAtivo.SelectedValueChanged
        FiltrarListagem()
    End Sub
    '
#End Region
    '
#Region "MENU SUSPENSO"
    ' CONTROLE DO MENU SUSPENSO
    Private Sub dgvListagem_MouseDown(sender As Object, e As MouseEventArgs) Handles dgvListagem.MouseDown
        If e.Button = MouseButtons.Right Then
            Dim c As Control = DirectCast(sender, Control)
            Dim hit As DataGridView.HitTestInfo = dgvListagem.HitTest(e.X, e.Y)
            dgvListagem.ClearSelection()
            '
            '---VERIFICAÇÕES NECESSÁRIAS
            If Not hit.Type = DataGridViewHitTestType.Cell Then Exit Sub
            '
            ' seleciona o ROW
            dgvListagem.Rows(hit.RowIndex).Cells(0).Selected = True
            dgvListagem.Rows(hit.RowIndex).Selected = True
            '
            ' mostra o MENU ativar e desativar
            If dgvListagem.Columns(hit.ColumnIndex).Name = "Ativo" Then
                If dgvListagem.Rows(hit.RowIndex).Cells("FornAtivo").Value = True Then
                    AtivarToolStripMenuItem.Enabled = False
                    DesativarToolStripMenuItem.Enabled = True
                Else
                    AtivarToolStripMenuItem.Enabled = True
                    DesativarToolStripMenuItem.Enabled = False
                End If
                ' revela menu
                MenuListagem.Show(c.PointToScreen(e.Location))
            End If
        End If
    End Sub
    '
    Private Sub AtivarToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AtivarToolStripMenuItem.Click
        '--- verifica se existe alguma cell 
        If dgvListagem.SelectedRows.Count = 0 Then Exit Sub
        '
        '--- Verifica o item
        Dim selID As Integer = dgvListagem.SelectedRows(0).Cells("clnID").Value
        Dim f As clFornecedor = listForn.Find(Function(x) x.IDPessoa = selID)
        '
        '---pergunta ao usuário
        If MessageBox.Show("Deseja realmente ATIVAR esse Fornecedor?" & vbNewLine &
                           f.Cadastro.ToUpper, "Ativar",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) = vbNo Then Exit Sub
        '
        '--- altera o valor
        f.Ativo = True
        '
        '--- Salvar o Registro
        Dim fBLL As New FornecedorBLL
        fBLL.AtualizaFornecedor(f)
        '
        '--- altera a imagem
        FiltrarListagem()
        '
    End Sub
    '
    Private Sub DesativarToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DesativarToolStripMenuItem.Click
        '--- verifica se existe alguma cell 
        If dgvListagem.SelectedRows.Count = 0 Then Exit Sub
        '
        '--- Verifica o item
        Dim selID As Integer = dgvListagem.SelectedRows(0).Cells("clnID").Value
        Dim f As clFornecedor = listForn.Find(Function(x) x.IDPessoa = selID)
        '
        '---pergunta ao usuário
        If MessageBox.Show("Deseja realmente INATIVAR esse Fornecedor?" & vbNewLine &
                           f.Cadastro.ToUpper, "Inativar",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) = vbNo Then Exit Sub
        '
        '--- altera o valor
        f.Ativo = False
        '
        '--- Salvar o Registro
        Dim fBLL As New FornecedorBLL
        fBLL.AtualizaFornecedor(f)
        '
        '--- altera a imagem
        FiltrarListagem()
        '
    End Sub
#End Region
    '
#Region "OUTRAS FUNCOES"
    '
    '-------------------------------------------------------------------------------------------------
    '--- SELECIONAR ITEM QUANDO PRESSIONA A TECLA (ENTER)
    '-------------------------------------------------------------------------------------------------
    Private Sub dgv_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvListagem.CellDoubleClick
        btnEditar_Click(New Object, New EventArgs)
    End Sub
    '
    Private Sub dgv_KeyDown(sender As Object, e As KeyEventArgs) Handles dgvListagem.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.Handled = True
            '
            btnEditar_Click(New Object, New EventArgs)
        End If
    End Sub
    '
    '-------------------------------------------------------------------------------------------------
    '--- QUANDO PRESSIONA A TECLA ESC FECHA O FORMULARIO
    '--- QUANDO A TECLA CIMA E BAIXO NAVEGA ENTRE OS ITENS DA LISTAGEM
    '-------------------------------------------------------------------------------------------------
    Private Sub Me_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Escape Then
            e.Handled = True
            btnFechar_Click(New Object, New EventArgs)
            '
        ElseIf e.KeyCode = Keys.Up Then
            '
            '--- garante que o combo não está aberto
            If cmbAtivo.DroppedDown = True Then Exit Sub
            '
            e.Handled = True
            '
            If dgvListagem.Rows.Count > 0 Then
                If dgvListagem.SelectedRows.Count > 0 Then
                    Dim i As Integer = dgvListagem.SelectedRows(0).Index
                    '
                    dgvListagem.Rows(i).Selected = False
                    '
                    If i = 0 Then
                        i = dgvListagem.Rows.Count
                    End If
                    '
                    dgvListagem.Rows(i - 1).Selected = True
                    dgvListagem.Rows(i - 1).Visible = True
                Else
                    dgvListagem.Rows(0).Selected = True
                    dgvListagem.Rows(0).Visible = True
                End If
            End If
            '
        ElseIf e.KeyCode = Keys.Down Then
            '
            '--- garante que o combo não está aberto
            If cmbAtivo.DroppedDown = True Then Exit Sub
            '
            e.Handled = True
            '
            If dgvListagem.Rows.Count > 0 Then
                If dgvListagem.SelectedRows.Count > 0 Then
                    Dim i As Integer = dgvListagem.SelectedRows(0).Index
                    '
                    dgvListagem.Rows(i).Selected = False
                    '
                    If i = dgvListagem.Rows.Count - 1 Then
                        i = -1
                    End If
                    '
                    dgvListagem.Rows(i + 1).Selected = True
                    dgvListagem.Rows(i + 1).Visible = True
                Else
                    dgvListagem.Rows(0).Selected = True
                End If
            End If
            '
        End If
    End Sub
    '
    Private Sub txtProcurar_KeyDown(sender As Object, e As KeyEventArgs) Handles txtProcura.KeyDown
        '
        If e.KeyCode = Keys.Enter Then
            If dgvListagem.SelectedRows.Count > 0 Then
                btnEditar_Click(New Object, New System.EventArgs)
            Else
                e.Handled = True
                SendKeys.Send("{Tab}")
            End If
        ElseIf e.KeyCode = Keys.Tab Then
            e.Handled = True
            SendKeys.Send("{Tab}")
        End If
        '
    End Sub
    '



#End Region
    '
#Region "EFEITOS VISUAIS"
    '
    '-------------------------------------------------------------------------------------------------
    ' QUANDO O DGVLISTAGEM ESTA SELECIONADO MUDA DE COR FUNDO
    '-------------------------------------------------------------------------------------------------
    Private Sub dgvListagem_Focus(sender As Object, e As EventArgs) Handles dgvListagem.GotFocus
        dgvListagem.BackgroundColor = Color.LightSteelBlue
    End Sub
    Private Sub dgvListagem_LostFocus(sender As Object, e As EventArgs) Handles dgvListagem.LostFocus
        dgvListagem.BackgroundColor = Color.DarkGray
    End Sub
    '
    '-------------------------------------------------------------------------------------------------
    ' CRIAR EFEITO VISUAL DE FORM SELECIONADO
    '-------------------------------------------------------------------------------------------------
    Private Sub form_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        If Not IsNothing(_formOrigem) Then
            Dim pnl As Panel = _formOrigem.Controls("Panel1")
            pnl.BackColor = Color.Silver
        End If
    End Sub
    '
    Private Sub frmProdutoProcurar_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        If Not IsNothing(_formOrigem) Then
            Dim pnl As Panel = _formOrigem.Controls("Panel1")
            pnl.BackColor = Color.SlateGray
        End If
    End Sub
    '
#End Region
    '
End Class
