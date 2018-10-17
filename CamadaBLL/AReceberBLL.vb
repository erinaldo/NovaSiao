﻿Imports CamadaDAL
Imports CamadaDTO
'
Public Class AReceberBLL
    '
    '===================================================================================================
    ' OBTER ARECEBER POR IDORIGEM E ORIGEM (TRANSACAO OU FINANCIAMENTO)
    '===================================================================================================
    Public Function AReceber_GET_PorOrigem(IDOrigem As Integer, Origem As clAReceber.AReceberOrigem) As clAReceber
        Dim db As New AcessoDados
        Dim dt As New DataTable
        Dim clRec As New clAReceber
        '
        Try
            db.LimparParametros()
            db.AdicionarParametros("@IDVenda", IDOrigem)
            db.AdicionarParametros("@Origem", Origem)
            '
            dt = db.ExecutarConsulta(CommandType.StoredProcedure, "uspAReceber_GET_PorIDOrigem")
            '
            If dt.Rows.Count = 0 Then Return clRec
            '
            Dim r As DataRow = dt.Rows(0)

            '
            With clRec
                '--- tblAReceber
                .IDAReceber = IIf(IsDBNull(r("IDAReceber")), Nothing, r("IDAReceber"))
                .IDOrigem = IDOrigem
                .Origem = Origem
                .IDPessoa = IIf(IsDBNull(r("IDPessoa")), Nothing, r("IDPessoa"))
                .AReceberValor = IIf(IsDBNull(r("AReceberValor")), 0, r("AReceberValor"))
                .ValorPagoTotal = IIf(IsDBNull(r("ValorPagoTotal")), 0, r("ValorPagoTotal"))
                .SituacaoAReceber = IIf(IsDBNull(r("SituacaoAReceber")), Nothing, r("SituacaoAReceber"))
            End With
            '
            Return clRec
            '
        Catch ex As Exception
            Throw ex
        End Try
        '
    End Function
    '
    '===================================================================================================
    ' SALVAR ITEM ARECEBER DA TRANSACAO OU REFINANCIAMENTO
    '===================================================================================================
    Public Function InserirNovo_AReceber(clAReceber As clAReceber) As Integer
        Dim db As New AcessoDados
        Dim obj As Object = Nothing
        '
        Try
            db.LimparParametros()
            '
            With clAReceber
                db.AdicionarParametros("@IDOrigem", .IDOrigem)
                db.AdicionarParametros("@Origem", .Origem)
                db.AdicionarParametros("@IDPessoa", .IDPessoa)
                db.AdicionarParametros("@AReceberValor", .AReceberValor)
            End With
            '
            obj = db.ExecutarManipulacao(CommandType.StoredProcedure, "uspAReceber_Inserir")
            '
            If IsNumeric(obj) Then
                Return obj
            Else
                Throw New Exception(obj.ToString)
            End If
            '
        Catch ex As Exception
            Throw ex
        End Try
        '
    End Function
    '
    '===================================================================================================
    ' QUITAR ARECEBER DA VENDA A VISTA
    '===================================================================================================
    Public Function Quitar_AReceber_AVista(IDTransacao As Integer, vlPago As Decimal) As Boolean
        Dim sql As New SQLControl
        '
        Dim myQry As String = String.Format(New Globalization.CultureInfo("en-US"),
                                            "UPDATE tblAReceber SET ValorPagoTotal = {0:#.####}, SituacaoAReceber = 1 WHERE IDTransacao = {1}",
                                            vlPago, IDTransacao)
        '
        Try
            sql.ExecQuery(myQry)
            '
            If sql.HasException Then
                Throw New Exception(sql.Exception)
                Return False
            End If
            '
            Return True
            '
        Catch ex As Exception
            Throw ex
        End Try
        '
    End Function
    '
    '===================================================================================================
    ' EXCLUIR TODOS ARECEBER E PARCELAR DE UMA TRANSACAO PELO ID
    '===================================================================================================
    Public Function Excluir_AReceber_Transacao(IDTransacao As Integer) As Boolean
        Dim db As New AcessoDados
        '
        '--- Limpa os paramentros
        db.LimparParametros()
        '
        '--- Adiciona os paramentros
        db.AdicionarParametros("IDTransacao", IDTransacao)
        '
        Try
            Dim obj As Object = db.ExecutarManipulacao(CommandType.StoredProcedure, "uspAReceber_Excluir_PorIDTransacao")
            Return CBool(obj)
        Catch ex As Exception
            Throw ex
        End Try
        '
    End Function
    '
    '===================================================================================================
    ' GET COBRANCA FORMAS DE ARECEBER
    '===================================================================================================
    Public Function CobrancaFormas_AReceber_GET() As DataTable
        Dim SQL As New SQLControl
        '
        Dim strSQL As String = "SELECT IDCobrancaForma, CobrancaForma FROM tblCobrancaForma WHERE Entradas = 'TRUE'"
        Try
            SQL.ExecQuery(strSQL)
            '
            If SQL.HasException Then
                Throw New Exception(SQL.Exception)
            End If
            '
            Return SQL.DBDT
        Catch ex As Exception
            Throw ex
        End Try
        '
    End Function
    '
    '===================================================================================================
    ' VERIFICA SITUACAO DA ORIGEM DO ARECEBER
    '===================================================================================================
    Public Function AReceber_VerificaSituacaoOrigem(myIDAReceber As Integer) As Byte
        '
        Dim db As New AcessoDados
        '
        db.AdicionarParametros("@IDAReceber", myIDAReceber)
        '
        Try
            Dim Sit As Byte
            Dim dt As DataTable = db.ExecutarConsulta(CommandType.StoredProcedure, "uspAReceber_VerificaSituacaoOrigem")
            '
            If dt.Rows.Count > 0 Then
                Sit = dt.Rows(0)("Retorno")
                Return Sit
            Else
                Throw New Exception("Não foi retornado nenhum resultado...")
            End If
            '
        Catch ex As Exception
            Throw ex
        End Try
        '
    End Function
    '
End Class

Public Class ParcelaBLL
    '
    '===================================================================================================
    ' OBTER PARCELAS POR IDORIGEM (IDTRANSACAO OU PARCELAMENTO)
    '===================================================================================================
    Public Function Parcela_GET_PorIDOrigem(Origem As Byte, IDOrigem As Integer) As List(Of clAReceberParcela)
        Dim db As New AcessoDados
        Dim dt As New DataTable
        Dim ParcList As New List(Of clAReceberParcela)
        '
        Try
            db.LimparParametros()
            db.AdicionarParametros("@IDOrigem", IDOrigem)
            db.AdicionarParametros("@Origem", Origem)
            '
            dt = db.ExecutarConsulta(CommandType.StoredProcedure, "uspAReceberParcela_GET_PorIDOrigem")
            '
            If dt.Rows.Count = 0 Then Return ParcList
            '
            For Each r As DataRow In dt.Rows
                Dim clPar As New clAReceberParcela
                '
                With clPar
                    '--- tblAReceber
                    .IDAReceber = IIf(IsDBNull(r("IDAReceber")), Nothing, r("IDAReceber"))
                    .IDOrigem = IIf(IsDBNull(r("IDOrigem")), Nothing, r("IDOrigem"))
                    .Origem = IIf(IsDBNull(r("Origem")), Nothing, r("Origem"))
                    .IDPessoa = IIf(IsDBNull(r("IDPessoa")), Nothing, r("IDPessoa"))
                    .AReceberValor = IIf(IsDBNull(r("AReceberValor")), Nothing, r("AReceberValor"))
                    .ValorPagoTotal = IIf(IsDBNull(r("ValorPagoTotal")), 0, r("ValorPagoTotal"))
                    .SituacaoAReceber = IIf(IsDBNull(r("SituacaoAReceber")), Nothing, r("SituacaoAReceber"))
                    .IDCobrancaForma = IIf(IsDBNull(r("IDCobrancaForma")), Nothing, r("IDCobrancaForma"))
                    .IDPlano = IIf(IsDBNull(r("IDPlano")), Nothing, r("IDPlano"))
                    '--- tblAReceberParcela
                    .IDAReceberParcela = IIf(IsDBNull(r("IDAReceberParcela")), Nothing, r("IDAReceberParcela"))
                    .Letra = IIf(IsDBNull(r("Letra")), String.Empty, r("Letra"))
                    .Vencimento = IIf(IsDBNull(r("Vencimento")), Nothing, r("Vencimento"))
                    .ParcelaValor = IIf(IsDBNull(r("ParcelaValor")), 0, r("ParcelaValor"))
                    .PermanenciaTaxa = IIf(IsDBNull(r("PermanenciaTaxa")), 0, r("PermanenciaTaxa"))
                    .ValorPagoParcela = IIf(IsDBNull(r("ValorPagoParcela")), 0, r("ValorPagoParcela"))
                    .ValorJuros = IIf(IsDBNull(r("ValorJuros")), Nothing, r("ValorJuros"))
                    .SituacaoParcela = IIf(IsDBNull(r("SituacaoParcela")), Nothing, r("SituacaoParcela"))
                    .PagamentoData = IIf(IsDBNull(r("PagamentoData")), Nothing, r("PagamentoData"))
                    '--- qryPessoaFisicaJuridica
                    .Cadastro = IIf(IsDBNull(r("Cadastro")), String.Empty, r("Cadastro"))
                    .CNP = IIf(IsDBNull(r("CNP")), String.Empty, r("CNP"))
                End With
                '
                ParcList.Add(clPar)
                '
            Next
            '
            Return ParcList
            '
        Catch ex As Exception
            Throw ex
        End Try
        '
    End Function
    '
    '===================================================================================================
    ' SALVAR NOVA PARCELA DA VENDA / TRANSACAO
    '===================================================================================================
    Public Function InserirNova_Parcela(clParcela As clAReceberParcela) As Integer
        Dim db As New AcessoDados
        Dim obj As Object = Nothing
        '
        db.LimparParametros()
        '
        With clParcela
            db.AdicionarParametros("@IDAReceber", .IDAReceber)
            db.AdicionarParametros("@IDPessoa", .IDPessoa)
            db.AdicionarParametros("@Letra", .Letra)
            db.AdicionarParametros("@Vencimento", .Vencimento)
            db.AdicionarParametros("@ParcelaValor", .ParcelaValor)
            db.AdicionarParametros("@PermanenciaTaxa", .PermanenciaTaxa)
        End With
        '
        Try
            obj = db.ExecutarManipulacao(CommandType.StoredProcedure, "uspAReceberParcela_Inserir")
            '
            If IsNumeric(obj) Then
                Return obj
            Else
                Throw New Exception(obj.ToString)
            End If
            '
        Catch ex As Exception
            Throw ex
        End Try
        '
    End Function
    '
    '===================================================================================================
    ' OBTER LISTA DE PARCELAS POR IDPESSOA
    '===================================================================================================
    Public Function Parcela_GET_PorIDPessoa(IDPessoa As Integer, Optional Situacao As Byte? = Nothing) As List(Of clAReceberParcela)
        Dim db As New AcessoDados
        Dim dt As New DataTable
        Dim ParcList As New List(Of clAReceberParcela)
        '
        Try
            db.LimparParametros()
            db.AdicionarParametros("@IDPessoa", IDPessoa)
            '
            If Not IsNothing(Situacao) Then
                db.AdicionarParametros("@Situacao", Situacao)
            End If
            '
            dt = db.ExecutarConsulta(CommandType.StoredProcedure, "uspAReceber_GET_PorIDPessoa")
            '
            If dt.Rows.Count = 0 Then Return ParcList
            '
            For Each r As DataRow In dt.Rows
                Dim clPar As New clAReceberParcela
                '
                With clPar
                    '--- tblAReceber
                    .IDAReceber = IIf(IsDBNull(r("IDAReceber")), Nothing, r("IDAReceber"))
                    .IDOrigem = IIf(IsDBNull(r("IDOrigem")), Nothing, r("IDOrigem"))
                    .Origem = IIf(IsDBNull(r("Origem")), Nothing, r("Origem"))
                    .IDPessoa = IIf(IsDBNull(r("IDPessoa")), Nothing, r("IDPessoa"))
                    .AReceberValor = IIf(IsDBNull(r("AReceberValor")), Nothing, r("AReceberValor"))
                    .ValorPagoTotal = IIf(IsDBNull(r("ValorPagoTotal")), 0, r("ValorPagoTotal"))
                    .SituacaoAReceber = IIf(IsDBNull(r("SituacaoAReceber")), Nothing, r("SituacaoAReceber"))
                    .IDCobrancaForma = IIf(IsDBNull(r("IDCobrancaForma")), Nothing, r("IDCobrancaForma"))
                    .IDPlano = IIf(IsDBNull(r("IDPlano")), Nothing, r("IDPlano"))
                    '--- tblAReceberParcela
                    .IDAReceberParcela = IIf(IsDBNull(r("IDAReceberParcela")), Nothing, r("IDAReceberParcela"))
                    .Letra = IIf(IsDBNull(r("Letra")), String.Empty, r("Letra"))
                    .Vencimento = IIf(IsDBNull(r("Vencimento")), Nothing, r("Vencimento"))
                    .ParcelaValor = IIf(IsDBNull(r("ParcelaValor")), 0, r("ParcelaValor"))
                    .PermanenciaTaxa = IIf(IsDBNull(r("PermanenciaTaxa")), 0, r("PermanenciaTaxa"))
                    .ValorPagoParcela = IIf(IsDBNull(r("ValorPagoParcela")), 0, r("ValorPagoParcela"))
                    .ValorJuros = IIf(IsDBNull(r("ValorJuros")), Nothing, r("ValorJuros"))
                    .SituacaoParcela = IIf(IsDBNull(r("SituacaoParcela")), Nothing, r("SituacaoParcela"))
                    .PagamentoData = IIf(IsDBNull(r("PagamentoData")), Nothing, r("PagamentoData"))
                    '--- qryPessoaFisicaJuridica
                    .Cadastro = IIf(IsDBNull(r("Cadastro")), String.Empty, r("Cadastro"))
                    .CNP = IIf(IsDBNull(r("CNP")), String.Empty, r("CNP"))
                End With
                '
                ParcList.Add(clPar)
                '
            Next
            '
            Return ParcList
            '
        Catch ex As Exception
            Throw ex
        End Try
        '
    End Function
    '
    '===================================================================================================
    ' EXCLUIR TODOS PARCELAS DE UM ARECEBER PELO IDARECEBER
    '===================================================================================================
    Public Function Excluir_Parcelas_AReceber(IDAReceber As Integer) As Boolean
        Dim db As New AcessoDados
        '
        '--- Limpa os paramentros
        db.LimparParametros()
        '
        '--- Adiciona os paramentros
        db.AdicionarParametros("@IDAReceber", IDAReceber)
        '
        Try
            Dim obj As Object = db.ExecutarManipulacao(CommandType.StoredProcedure, "uspAReceberParcela_Excluir_PorIDAReceber")
            Return CBool(obj)
        Catch ex As Exception
            Throw ex
        End Try
        '
    End Function
    '
    '===================================================================================================
    ' QUITAR PARCELA TRANSACAO
    '===================================================================================================
    Public Function Quitar_Parcela(IDParcela As Integer, vlRecebido As Double, vlJuros As Double, EntradaData As Date) As Integer
        Dim db As New AcessoDados
        Dim obj As Object = Nothing
        '
        Try
            db.LimparParametros()
            '
            db.AdicionarParametros("@IDAReceberParcela", IDParcela)
            db.AdicionarParametros("@ValorRecebidoParcela", vlRecebido)
            db.AdicionarParametros("@ValorRecebidoJuros", vlJuros)
            db.AdicionarParametros("@EntradaData", EntradaData)
            '
            obj = db.ExecutarManipulacao(CommandType.StoredProcedure, "uspAReceberParcela_Quitar")
            '
            If IsNumeric(obj) Then
                Return obj
            Else
                Throw New Exception(obj.ToString)
            End If
            '
        Catch ex As Exception
            Throw ex
        End Try
        '
    End Function
    '
    '===================================================================================================
    ' CANCELAR PARCELA TRANSACAO | VENDA
    '===================================================================================================
    Public Function Cancelar_Parcela(IDParcela As Integer) As Integer
        Dim db As New AcessoDados
        Dim obj As Object = Nothing
        '
        Try
            db.LimparParametros()
            '
            db.AdicionarParametros("@IDAReceberParcela", IDParcela)
            db.AdicionarParametros("@NovaSituacao", 2) '-- 2 : Situacao CANCELADA
            '
            obj = db.ExecutarManipulacao(CommandType.StoredProcedure, "uspAReceberParcela_AlterarSituacao")
            '
            If IsNumeric(obj) Then
                Return obj
            Else
                Throw New Exception(obj.ToString)
            End If
            '
        Catch ex As Exception
            Throw ex
        End Try
        '
    End Function
    '
    '===================================================================================================
    ' NORMALIZAR PARCELA TRANSACAO | VENDA
    '===================================================================================================
    Public Function Normalizar_Parcela(IDParcela As Integer) As Integer
        Dim db As New AcessoDados
        Dim obj As Object = Nothing
        '
        Try
            db.LimparParametros()
            '
            db.AdicionarParametros("@IDAReceberParcela", IDParcela)
            db.AdicionarParametros("@NovaSituacao", 0) '-- 2 : Situacao CANCELADA
            '
            obj = db.ExecutarManipulacao(CommandType.StoredProcedure, "uspAReceberParcela_AlterarSituacao")
            '
            If IsNumeric(obj) Then
                Return obj
            Else
                Throw New Exception(obj.ToString)
            End If
            '
        Catch ex As Exception
            Throw ex
        End Try
        '
    End Function
    '
    '===================================================================================================
    ' ALTERAR VENCIMENTO PARCELA TRANSACAO | VENDA
    '===================================================================================================
    Public Function AlteraVencimento_Parcela(IDParcela As Integer, Vencimento As Date,
                                             Optional IDTransacao As Integer? = Nothing) As Boolean
        Dim SQL As New SQLControl
        Dim myStr As String
        '
        '--- verifica se a data da transação não é posterior à nova data
        '--- a parcela precisa ter uma data de vencimento após a data da transacao
        If Not IsNothing(IDTransacao) Then
            myStr = "SELECT TransacaoData FROM tblTransacao WHERE IDTransacao = " & IDTransacao
            SQL.ExecQuery(myStr)
            If SQL.DBDT.Rows.Count > 0 Then
                Dim TranData As Date = SQL.DBDT.Rows(0)("TransacaoData")
                '
                If TranData > Vencimento Then
                    Throw New Exception("A Data de Vencimento não pode ser ANTERIOR à" & vbNewLine &
                                        "DATA DA TRANSAÇÃO/VENDA: " & TranData.ToShortDateString)
                End If
                '
            End If
        End If
        '
        myStr = String.Format("UPDATE tblAReceberParcela SET Vencimento = '{0}' WHERE IDAReceberParcela = {1}",
                               Vencimento.Date.ToString("yyyy-MM-dd"),
                               IDParcela)
        '
        SQL.ExecQuery(myStr)
        '

        If SQL.HasException() Then
            Throw New Exception(SQL.Exception)
        Else
            Return True
        End If
        '
    End Function
    '
    '===================================================================================================
    ' ESTORNAR ENTRADA PARCELA TRANSACAO | VENDA
    '===================================================================================================
    Public Function EstornarEntradaParcela(myIDParcela As Integer, myIDEntrada As Integer) As clAReceberParcela
        Dim db As New AcessoDados
        Dim dtPar As DataTable
        '
        Try
            db.LimparParametros()
            '
            db.AdicionarParametros("@IDAReceberParcela", myIDParcela)
            db.AdicionarParametros("@IDEntrada", myIDEntrada)
            '
            dtPar = db.ExecutarConsulta(CommandType.StoredProcedure, "uspAReceberParcela_Estornar")
            '
            '--- verifica se retornou alguma ROW
            If dtPar.Rows.Count = 0 Then
                Throw New Exception("Uma exceção inesperada ocorreu ao Estornar a Entrada/Parcela...")
            End If
            '
            '--- seleciona a ROW
            Dim r As DataRow = dtPar.Rows(0)
            '
            '--- verifica se o RETORNO é uma clParcela
            If dtPar.Columns.Count = 1 Then '--- se tem mais de uma coluna então é uma clParcela
                Throw New Exception(r(0))
            End If
            '
            '--- Preenche Nova clParcela com a DataROW
            '---------------------------------------------------------------------
            Dim clPar As New clAReceberParcela
            '--- tblAReceber
            clPar.IDAReceber = IIf(IsDBNull(r("IDAReceber")), Nothing, r("IDAReceber"))
            clPar.IDOrigem = IIf(IsDBNull(r("IDOrigem")), Nothing, r("IDOrigem"))
            clPar.Origem = IIf(IsDBNull(r("Origem")), Nothing, r("Origem"))
            clPar.IDPessoa = IIf(IsDBNull(r("IDPessoa")), Nothing, r("IDPessoa"))
            clPar.AReceberValor = IIf(IsDBNull(r("AReceberValor")), Nothing, r("AReceberValor"))
            clPar.ValorPagoTotal = IIf(IsDBNull(r("ValorPagoTotal")), 0, r("ValorPagoTotal"))
            clPar.SituacaoAReceber = IIf(IsDBNull(r("SituacaoAReceber")), Nothing, r("SituacaoAReceber"))
            clPar.IDCobrancaForma = IIf(IsDBNull(r("IDCobrancaForma")), Nothing, r("IDCobrancaForma"))
            clPar.IDPlano = IIf(IsDBNull(r("IDPlano")), Nothing, r("IDPlano"))
            '--- tblAReceberParcela
            clPar.IDAReceberParcela = IIf(IsDBNull(r("IDAReceberParcela")), Nothing, r("IDAReceberParcela"))
            clPar.Letra = IIf(IsDBNull(r("Letra")), String.Empty, r("Letra"))
            clPar.Vencimento = IIf(IsDBNull(r("Vencimento")), Nothing, r("Vencimento"))
            clPar.ParcelaValor = IIf(IsDBNull(r("ParcelaValor")), 0, r("ParcelaValor"))
            clPar.PermanenciaTaxa = IIf(IsDBNull(r("PermanenciaTaxa")), 0, r("PermanenciaTaxa"))
            clPar.ValorPagoParcela = IIf(IsDBNull(r("ValorPagoParcela")), 0, r("ValorPagoParcela"))
            clPar.ValorJuros = IIf(IsDBNull(r("ValorJuros")), Nothing, r("ValorJuros"))
            clPar.SituacaoParcela = IIf(IsDBNull(r("SituacaoParcela")), Nothing, r("SituacaoParcela"))
            clPar.PagamentoData = IIf(IsDBNull(r("PagamentoData")), Nothing, r("PagamentoData"))
            '--- qryPessoaFisicaJuridica
            clPar.Cadastro = IIf(IsDBNull(r("Cadastro")), String.Empty, r("Cadastro"))
            clPar.CNP = IIf(IsDBNull(r("CNP")), String.Empty, r("CNP"))
            '
            '--- Retorna
            Return clPar
            '
        Catch ex As Exception
            Throw ex
        End Try
        '
    End Function
    '
End Class
