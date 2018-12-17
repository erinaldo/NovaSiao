﻿Imports CamadaBLL
Imports CamadaDTO
Imports ComponentOwl.BetterListView
Imports System.Drawing.Drawing2D
'
Public Class frmMovFormas
    Private WithEvents listMovFormas As New List(Of clMovForma)
    Private WithEvents bindMovForma As New BindingSource
    Private _Sit As FlagEstado '= 1:Registro Salvo; 2:Registro Alterado; 3:Novo registro
    '
    Private AtivarImage As Image = My.Resources.Switch_ON_PEQ
    Private DesativarImage As Image = My.Resources.Switch_OFF_PEQ
    '
    Private _IDFilial As Integer
    Private _IDConta As Int16
    '
#Region "EVENTO LOAD E PROPRIEDADE SIT"
    '
    Private Property Sit As FlagEstado
        Get
            Return _Sit
        End Get
        Set(value As FlagEstado)
            _Sit = value
            If _Sit = FlagEstado.RegistroSalvo Then
                btnSalvar.Enabled = False
                btnNovo.Enabled = True
                btnExcluir.Enabled = True
                btnCancelar.Enabled = False
                AtivoButtonImage()
                lstFormas.ReadOnly = False
                '
            ElseIf _Sit = FlagEstado.Alterado Then
                btnSalvar.Enabled = True
                btnNovo.Enabled = False
                btnExcluir.Enabled = True
                btnCancelar.Enabled = True
                AtivoButtonImage()
                lstFormas.ReadOnly = True
                '
            ElseIf _Sit = FlagEstado.NovoRegistro Then
                txtMovForma.Select()
                btnSalvar.Enabled = True
                btnNovo.Enabled = False
                btnExcluir.Enabled = False
                btnCancelar.Enabled = True
                AtivoButtonImage()
                lstFormas.ReadOnly = True
                '
            End If
        End Set
    End Property
    '
    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        '
        '--- define a FilialPadrao
        _IDFilial = Obter_FilialPadrao()
        lblFilial.Text = ObterDefault("FilialDescricao")
        '
    End Sub
    '
    ' EVENTO LOAD
    Private Sub frmPagamentoFormas_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        '
        Dim fBLL As New MovimentacaoBLL
        '
        Try
            ListMovFormas = fBLL.MovForma_GET_List
        Catch ex As Exception
            MessageBox.Show("Uma exceção ocorreu ao obter lista da Formas...", "Obter Lista", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        '
        bindMovForma.DataSource = listMovFormas
        '
        If listMovFormas.Count > 0 Then
            bindMovForma.MoveFirst()
            PreencheDataBindings()
            Sit = FlagEstado.RegistroSalvo
            '
        Else
            bindMovForma.AddNew()
            Sit = FlagEstado.NovoRegistro
            PreencheDataBindings()
        End If
        '
        PreencheListagem()
        '
        DirectCast(bindMovForma.Current, clMovForma).RegistroAlterado = False
        '
    End Sub
    '
#End Region
    '
#Region "BINDIGNS"
    '
    Private Sub PreencheDataBindings()
        ' ADICIONANDO O DATABINDINGS AOS CONTROLES TEXT
        ' OS COMBOS JA SÃO ADICIONADOS DATABINDINGS QUANDO CARREGA
        '
        lblIDMovForma.DataBindings.Add("Text", bindMovForma, "IDMovForma", True, DataSourceUpdateMode.OnPropertyChanged)
        txtMovForma.DataBindings.Add("Text", bindMovForma, "MovForma", True, DataSourceUpdateMode.OnPropertyChanged)
        txtComissao.DataBindings.Add("Text", bindMovForma, "Comissao", True, DataSourceUpdateMode.OnPropertyChanged)
        txtParcelas.DataBindings.Add("Text", bindMovForma, "Parcelas", True, DataSourceUpdateMode.OnPropertyChanged)
        txtNoDias.DataBindings.Add("Text", bindMovForma, "NoDias", True, DataSourceUpdateMode.OnPropertyChanged)
        txtMovTipo.DataBindings.Add("Text", bindMovForma, "MovTipo", True, DataSourceUpdateMode.OnPropertyChanged)
        txtCartao.DataBindings.Add("Text", bindMovForma, "Cartao", True, DataSourceUpdateMode.OnPropertyChanged)
        txtConta.DataBindings.Add("Text", bindMovForma, "ContaPadrao", True, DataSourceUpdateMode.OnPropertyChanged)
        '
        ' FORMATA OS VALORES DO DATABINDING
        AddHandler lblIDMovForma.DataBindings("Text").Format, AddressOf idFormatRG
        AddHandler txtParcelas.DataBindings("Text").Format, AddressOf idFormatRG
        AddHandler txtNoDias.DataBindings("Text").Format, AddressOf idFormatRG
        AddHandler txtComissao.DataBindings("Text").Format, AddressOf idFormatCur
        '
        ' ADD HANDLER PARA DATABINGS
        AddHandler DirectCast(bindMovForma.CurrencyManager.Current, clMovForma).AoAlterar, AddressOf Handler_AoAlterar
        AddHandler bindMovForma.CurrentChanged, AddressOf handler_CurrentChanged
        '
    End Sub
    '
    Private Sub idFormatRG(sender As Object, e As System.Windows.Forms.ConvertEventArgs)
        e.Value = Format(IIf(IsDBNull(e.Value), Nothing, e.Value), "00")
    End Sub
    '
    Private Sub idFormatCur(sender As Object, e As System.Windows.Forms.ConvertEventArgs)
        e.Value = Format(IIf(IsDBNull(e.Value), Nothing, e.Value), "0.00")
    End Sub
    '
    Private Sub Handler_AoAlterar()
        '
        If Sit = FlagEstado.RegistroSalvo Then
            Sit = FlagEstado.Alterado
        End If
        '
    End Sub
    '
    Private Sub handler_CurrentChanged()
        '
        ' ADD HANDLER PARA DATABINGS
        AddHandler DirectCast(bindMovForma.CurrencyManager.Current, clMovForma).AoAlterar, AddressOf Handler_AoAlterar
        '
        '--- Nesse caso é um novo registro
        If Not IsNothing(DirectCast(bindMovForma.Current, clMovForma).IDMovForma) Then
            '
            ' LER O ID
            lblIDMovForma.DataBindings.Item("text").ReadValue()
            '
            ' ALTERAR PARA REGISTRO SALVO
            Sit = FlagEstado.RegistroSalvo
            '
            DirectCast(bindMovForma.Current, clMovForma).RegistroAlterado = False
            '
        End If
        '
    End Sub
    '
#End Region
    '
#Region "LISTAGEM FORMATACAO"
    '
    ' FORMATAR A LISTAGEM
    Private Sub PreencheListagem()
        lstFormas.DataSource = bindMovForma
        '
        With lstFormas.Columns("clnIDMovForma") ' column 0
            .Width = 50
            .DisplayMember = "IDMovForma"
        End With
        '
        With lstFormas.Columns("clnMovForma") ' column 1
            .Width = 220
            .DisplayMember = "MovForma"
        End With
        '
    End Sub
    '
    ' ALTERA A COR DO HEADER DO LISTBOX
    Private Sub lstFormas_DrawColumnHeader(sender As Object, eventArgs As BetterListViewDrawColumnHeaderEventArgs) Handles lstFormas.DrawColumnHeader
        '
        If eventArgs.ColumnHeaderBounds.BoundsOuter.Width > 0 AndAlso
            eventArgs.ColumnHeaderBounds.BoundsOuter.Height > 0 Then
            ' Pode Colocar: AndAlso eventArgs.ColumnHeader.Index = 1 AndAlso
            '
            Dim brush As Brush = New LinearGradientBrush(eventArgs.ColumnHeaderBounds.BoundsOuter, Color.Transparent, Color.FromArgb(100, Color.LightSlateGray), LinearGradientMode.Vertical)
            Dim p As Pen = New Pen(Color.SlateGray, 2)
            '
            eventArgs.Graphics.FillRectangle(brush, eventArgs.ColumnHeaderBounds.BoundsOuter)
            '
            eventArgs.Graphics.DrawLine(p, eventArgs.ColumnHeaderBounds.BoundsOuter.X, 'x1
                                        eventArgs.ColumnHeaderBounds.BoundsOuter.Height, 'y1
                                        eventArgs.ColumnHeaderBounds.BoundsOuter.Width + eventArgs.ColumnHeaderBounds.BoundsOuter.X, 'x2
                                        eventArgs.ColumnHeaderBounds.BoundsOuter.Height) 'y2
            brush.Dispose()
            p.Dispose()
        End If
        '
    End Sub
    '
    ' ESCREVE OS TEXTOS DE TIPO DE ACESSO NA LISTAGEM
    Private Sub lstFormas_DrawItem(sender As Object, eventArgs As BetterListViewDrawItemEventArgs) Handles lstFormas.DrawItem
        If IsNumeric(eventArgs.Item.Text) Then
            eventArgs.Item.Text = Format(CInt(eventArgs.Item.Text), "00")
        End If
    End Sub
#End Region
    '
#Region "ACAO DOS BOTOES"
    '
    ' ATIVAR OU DESATIVAR USUARIO BOTÃO
    Private Sub btnAtivo_Click(sender As Object, e As EventArgs) Handles btnAtivo.Click
        '
        If Sit = FlagEstado.NovoRegistro Then
            MessageBox.Show("Você não pode DESATIVAR uma Nova Forma de Entrada", "Desativar Forma",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If
        '
        Dim r As clMovForma = DirectCast(bindMovForma.Current, clMovForma)
        '
        If r.Ativo = True Then '---usuario ativo
            If MessageBox.Show("Você deseja realmente DESATIVAR a Forma de Entrada:" & vbNewLine &
            txtMovForma.Text.ToUpper, "Desativar Forma", MessageBoxButtons.YesNo,
            MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = DialogResult.No Then Exit Sub
        Else '---Usuário Inativo
            If MessageBox.Show("Você deseja realmente ATIVAR a Forma de Entrada:" & vbNewLine &
            txtMovForma.Text.ToUpper, "Desativar Forma", MessageBoxButtons.YesNo,
            MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = DialogResult.No Then Exit Sub
        End If
        '
        DirectCast(bindMovForma.Current, clMovForma).BeginEdit()
        r.Ativo = Not r.Ativo
        '
        If Sit = FlagEstado.RegistroSalvo Then
            Sit = FlagEstado.Alterado
        End If
        '
        AtivoButtonImage()
        '
    End Sub
    '
    ' BOTÃO CANCELAR
    Private Sub btnCancelar_Click(sender As Object, e As EventArgs) Handles btnCancelar.Click
        '
        If Sit = FlagEstado.Alterado Then ' REGISTRO ALTERADO
            If MessageBox.Show("Deseja cancelar todas as alterações feitas no registro atual?",
                               "Cancelar Alterações", MessageBoxButtons.YesNo,
                               MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = DialogResult.Yes Then
                '
                bindMovForma.CancelEdit()
                txtMovForma.Focus()
                Sit = FlagEstado.RegistroSalvo
                '
            End If
        ElseIf Sit = FlagEstado.NovoRegistro Then ' REGISTRO NOVO
            If lstFormas.Items.Count = 1 Then
                MessageBox.Show("Não é possível cancelar porque não existe outro registro." & vbNewLine &
                                "Se deseja sair apenas feche a janela!", "Cancelar Edição", MessageBoxButtons.OK,
                                MessageBoxIcon.Information)
                Exit Sub
            End If
            '
            Sit = FlagEstado.RegistroSalvo
            bindMovForma.RemoveCurrent()
            '
        End If
        '
    End Sub
    '
    ' BOTÃO NOVO REGISTRO
    Private Sub btnNovo_Click(sender As Object, e As EventArgs) Handles btnNovo.Click
        '
        lstFormas.SelectedItems.Clear()
        '
        '---cria um novo DataRow com valores padrão
        Dim row As New clMovForma
        row.Ativo = True
        row.Parcelas = 0
        row.Comissao = Format(0, "0.00")
        row.NoDias = Format(0, "00")
        '
        '---adiciona o NewROW
        listMovFormas.Add(row)
        bindMovForma.MoveLast()
        '
        '---altera o SIT
        Sit = FlagEstado.NovoRegistro
        '
    End Sub
    '
    ' BOTÃO FECHAR
    Private Sub btnFechar_Click(sender As Object, e As EventArgs) Handles btnFechar.Click, btnClose.Click
        Close()
        MostraMenuPrincipal()
    End Sub
    '
    ' BOTÃO EXCLUIR
    Private Sub btnExcluir_Click(sender As Object, e As EventArgs) Handles btnExcluir.Click
        MsgBox("Ainda não implementado")
    End Sub
    '
    ' BUTON ABRIR FORMA TIPOS
    Private Sub btnMovTipos_Click(sender As Object, e As EventArgs) Handles btnMovTipos.Click
        '
        Dim clF As clMovForma = DirectCast(bindMovForma.CurrencyManager.Current, clMovForma)
        '    
        '---abre o formTipos
        Dim frmTipo As New frmMovTipos(frmMovTipos.DadosOrigem.MovTipo, True, Me, IIf(IsNothing(clF.IDMovTipo), Nothing, clF.IDMovTipo))
        frmTipo.ShowDialog()
        '
        '---verifica os valores
        If frmTipo.DialogResult <> DialogResult.OK Then
            txtMovTipo.Focus()
            Exit Sub
        End If
        '
        '--- grava os novos valores
        txtMovTipo.Text = frmTipo.OrigemDesc_Escolhido
        clF.IDMovTipo = frmTipo.IDOrigem_Escolhido
        txtMovTipo.Focus()
        txtMovTipo.SelectAll()
        '
    End Sub
    '
    ' BUTON ABRIR CARTAO PROCURA
    Private Sub btnCartao_Click(sender As Object, e As EventArgs) Handles btnCartao.Click
        '
        Dim clF As clMovForma = DirectCast(bindMovForma.CurrencyManager.Current, clMovForma)
        '    
        '---abre o formTipos
        Dim frmTipo As New frmMovTipos(frmMovTipos.DadosOrigem.Cartao, True, Me, IIf(IsNothing(clF.IDCartao), Nothing, clF.IDCartao))
        frmTipo.ShowDialog()
        '
        '---verifica os valores
        If frmTipo.DialogResult <> DialogResult.OK Then
            txtCartao.Focus()
            Exit Sub
        End If
        '
        '--- grava os novos valores
        txtCartao.Text = frmTipo.OrigemDesc_Escolhido
        clF.IDCartao = frmTipo.IDOrigem_Escolhido
        txtCartao.Focus()
        txtCartao.SelectAll()
        '
    End Sub
    '
    Private Sub btnContaEscolher_Click(sender As Object, e As EventArgs) Handles btnContaEscolher.Click
        '
        '--- Verifica se existe uma filial definida
        If IsNothing(_IDFilial) Then
            MessageBox.Show("É necessário definir a Filial para escolher a conta a partir dela..." & vbNewLine &
                            "Favor escolher uma Filial Padrão...",
                            "Escolher Filial",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information)
            Return
        End If
        '
        Dim clF As clMovForma = DirectCast(bindMovForma.CurrencyManager.Current, clMovForma)
        '
        '--- Abre o frmContas
        Dim frmConta As New frmContaProcurar(Me, _IDFilial, _IDConta)
        '
        frmConta.ShowDialog()
        '
        If frmConta.DialogResult = DialogResult.Cancel Then Exit Sub
        '
        txtConta.Text = frmConta.propConta_Escolha
        _IDConta = frmConta.propIdConta_Escolha
        clF.IDContaPadrao = _IDConta
        '
    End Sub
    '
#End Region
    '
#Region "SALVA REGISTRO"
    '
    '--- BTN SALVAR REGISTRO
    Private Sub btnSalvar_Click(sender As Object, e As EventArgs) Handles btnSalvar.Click
        If VerificaDados() = False Then Exit Sub
        '
        Dim clF As clMovForma = DirectCast(bindMovForma.CurrencyManager.Current, clMovForma)
        Dim movBLL As New MovimentacaoBLL
        '
        If Sit = FlagEstado.NovoRegistro Then
            Try
                Dim newID As Int16 = movBLL.MovForma_Inserir(clF)
                '
                clF.IDMovForma = newID
                lblIDMovForma.DataBindings("Text").ReadValue()
                '
                Sit = FlagEstado.RegistroSalvo
                bindMovForma.EndEdit()
                bindMovForma.ResetBindings(True)
                '
                '---informa o usuário
                MessageBox.Show("Registro Inserido com sucesso!", "Registro Salvo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information)
                '
            Catch ex As Exception
                MessageBox.Show("Esse Registro NÃO foi salvo com sucesso!" & vbNewLine &
                                ex.Message, "Exceção Inesperada",
                                MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
            '
        ElseIf Sit = FlagEstado.Alterado Then
            Try
                movBLL.MovForma_Update(clF)
                '
                Sit = FlagEstado.RegistroSalvo
                bindMovForma.EndEdit()
                '
                '---informa o usuário
                MessageBox.Show("Registro Atualizado com sucesso!", "Registro Salvo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information)
                '
            Catch ex As Exception
                MessageBox.Show("Esse Registro NÃO foi salvo com sucesso!" & vbNewLine &
                                ex.Message, "Exceção Inesperada",
                                MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
        '
    End Sub
    '
    ' VERIFICA OS DADOS ANTES DE SALVAR
    Private Function VerificaDados() As Boolean
        '
        Dim f As New FuncoesUtilitarias
        Dim clF As clMovForma = DirectCast(bindMovForma.CurrencyManager.Current, clMovForma)
        '
        If Not f.VerificaDadosClasse(txtMovForma, "Descrição da Forma de Movimento", clF, epValida) Then
            Return False
        End If
        '
        If Not f.VerificaDadosClasse(txtMovTipo, "Tipo da Forma de Pagamento", clF, epValida) Then
            Return False
        End If
        '
        If Not IsNothing(clF.IDCartao) Then
            '
            If Not f.VerificaDadosClasse(txtNoDias, "Intervalo/Prazo de pagamento do Cartão", clF, epValida) Then
                Return False
            End If
            '
            If clF.NoDias <= 0 Then
                MessageBox.Show("O número de dias não pode ser igual ou menor que Zero...", "Aviso",
                                MessageBoxButtons.OK, MessageBoxIcon.Information)
                txtNoDias.Focus()
                Return False
            End If
            '
            If Not f.VerificaDadosClasse(txtParcelas, "Quantidade de Parcelas de pagamento", clF, epValida) Then
                Return False
            End If
            '
            If clF.Parcelas < 0 Then
                MessageBox.Show("O número de parcelas não pode ser menor que Zero...", "Aviso",
                                MessageBoxButtons.OK, MessageBoxIcon.Information)
                txtParcelas.Focus()
                Return False
            End If
            '
            If Not f.VerificaDadosClasse(txtComissao, "Comissão da Operadora de Cartão", clF, epValida) Then
                Return False
            End If
            '
            If clF.Comissao < 0 Or clF.Comissao > 99 Then
                MessageBox.Show("A taxa de comissão não pode ser menor que Zero ou maior que 100%", "Aviso",
                                MessageBoxButtons.OK, MessageBoxIcon.Information)
                txtComissao.Focus()
                Return False
            End If
            '
        Else
            clF.NoDias = Nothing
            clF.Comissao = Nothing
            clF.Parcelas = Nothing
        End If
        '
        Return True
        '
    End Function
    '
#End Region
    '
#Region "ATALHOS E FUNCOES UTILITARIAS"
    '
    ' ALTERA A IMAGEM E O TEXTO DO BOTÃO ATIVO E DESATIVO
    Private Sub AtivoButtonImage()
        Try
            If DirectCast(bindMovForma.Current, clMovForma).Ativo = True Then ' Nesse caso é Forma Ativo
                btnAtivo.Image = AtivarImage
                btnAtivo.Text = "Forma Ativa"
            ElseIf DirectCast(bindMovForma.Current, clMovForma).Ativo = False Then ' Nesse caso é Forma Inativo
                btnAtivo.Image = DesativarImage
                btnAtivo.Text = "Forma Inativa"
            End If
        Catch ex As System.IndexOutOfRangeException
            btnAtivo.Image = AtivarImage
            btnAtivo.Text = "Forma Ativa"
        End Try
    End Sub

    '--- QUANDO PRESSIONA DELETE APAGA LIMPA O CONTA
    ????
    '
    '--- EXECUTAR A FUNCAO DO BOTAO QUANDO PRESSIONA A TECLA (+) NO CONTROLE
    Private Sub Control_KeyDown(sender As Object, e As KeyEventArgs) Handles txtMovTipo.KeyDown, txtCartao.KeyDown, txtConta.KeyDown
        '
        Dim ctr As Control = DirectCast(sender, Control)
        '
        If e.KeyCode = Keys.Add Then
            e.Handled = True
            Select Case ctr.Name
                Case "txtConta"
                    btnContaEscolher_Click(New Object, New EventArgs)
                Case "txtMovTipo"
                    btnMovTipos_Click(New Object, New EventArgs)
                Case "txtCartao"
                    btnCartao_Click(New Object, New EventArgs)
            End Select
        ElseIf e.KeyCode = Keys.Enter OrElse e.KeyCode = Keys.Tab Then
            e.Handled = True
            'e.SuppressKeyPress = True
            SendKeys.Send("{Tab}")
        Else
            e.Handled = True
            e.SuppressKeyPress = True
        End If
        '
    End Sub
    '
    '--- BLOQUEIA PRESS A TECLA (+)
    Private Sub Me_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Me.KeyPress
        If e.KeyChar = "+" Then
            e.Handled = True
        End If
    End Sub
    '
#End Region
    '
End Class
