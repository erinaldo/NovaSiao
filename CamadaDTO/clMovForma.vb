﻿Imports System.ComponentModel
'
'=======================================================================================
' clMOVFORMA
'=======================================================================================
Public Class clMovForma : Implements IEditableObject
    '
#Region "ESTRUTURA DOS DADOS"
    Structure FormaStructure ' alguns usam FRIEND em vez de DIM
        Dim _IDMovForma As Int16?
        Dim _MovForma As String
        Dim _IDMovTipo As Int16?
        Dim _MovTipo As String
        Dim _IDOperadora As Int16?
        Dim _Operadora As String
        Dim _IDCartao As Int16?
        Dim _Cartao As String
        Dim _Parcelas As Byte?
        Dim _Comissao As Decimal?
        Dim _NoDias As Byte?
        Dim _Ativo As Boolean
    End Structure
#End Region
    '
#Region "PRIVATE VARIABLES"
    Private FData As FormaStructure
    Private backupData As FormaStructure
    Private inTxn As Boolean = False
    '
    Enum Origem_APagar
        Compra = 1
        Despesa = 2
    End Enum
    '
#End Region
    '
#Region "IMPLEMENTS EVENTS"
    Public Sub New()
        FData = New FormaStructure()
        With FData
            ._Ativo = True
        End With
    End Sub
    '
    '-- IMPLEMENTS IEditableObject
    Public Sub BeginEdit() Implements IEditableObject.BeginEdit
        '
        If Not inTxn Then
            Me.backupData = FData
            inTxn = True
        End If
        '
    End Sub
    '
    Public Sub CancelEdit() Implements IEditableObject.CancelEdit
        If inTxn Then
            Me.FData = backupData
            inTxn = False
        End If
    End Sub
    '
    Public Sub EndEdit() Implements IEditableObject.EndEdit
        If inTxn Then
            backupData = New FormaStructure()
            inTxn = False
        End If
    End Sub
    '
    '-- _EVENTO PUBLIC AOALTERAR
    Public Event AoAlterar()
    '
    Public Property RegistroAlterado As Boolean
        Get
            Return inTxn
        End Get
        Set(value As Boolean)
            inTxn = value
        End Set
    End Property
    '
#End Region
    '
#Region "PROPRIEDADES"
    '
    '--- Propriedade IDMovForma
    Public Property IDMovForma() As Int16?
        Get
            Return FData._IDMovForma
        End Get
        Set(ByVal value As Int16?)
            If value <> FData._IDMovForma Then
                RaiseEvent AoAlterar()
            End If
            FData._IDMovForma = value
        End Set
    End Property
    '
    '--- Propriedade MovForma
    Public Property MovForma() As String
        Get
            Return FData._MovForma
        End Get
        Set(ByVal value As String)
            If value <> FData._MovForma Then
                RaiseEvent AoAlterar()
            End If
            FData._MovForma = value
        End Set
    End Property
    '
    '--- Propriedade IDMovTipo
    Public Property IDMovTipo() As Int16?
        Get
            Return FData._IDMovTipo
        End Get
        Set(ByVal value As Int16?)
            If value <> FData._IDMovTipo Then
                RaiseEvent AoAlterar()
            End If
            FData._IDMovTipo = value
        End Set
    End Property
    '
    '--- Propriedade MovTipo
    Public Property MovTipo() As String
        Get
            Return FData._MovTipo
        End Get
        Set(ByVal value As String)
            If value <> FData._MovTipo Then
                RaiseEvent AoAlterar()
            End If
            FData._MovTipo = value
        End Set
    End Property
    '
    '--- Propriedade IDOperadora
    Public Property IDOperadora() As Int16?
        Get
            Return FData._IDOperadora
        End Get
        Set(ByVal value As Int16?)
            If value <> FData._IDOperadora Then
                RaiseEvent AoAlterar()
            End If
            FData._IDOperadora = value
        End Set
    End Property
    '
    '--- Propriedade Operadora
    Public Property Operadora() As String
        Get
            Return FData._Operadora
        End Get
        Set(ByVal value As String)
            If value <> FData._Operadora Then
                RaiseEvent AoAlterar()
            End If
            FData._Operadora = value
        End Set
    End Property
    '
    '--- Propriedade IDCartao
    Public Property IDCartao() As Int16?
        Get
            Return FData._IDCartao
        End Get
        Set(ByVal value As Int16?)
            If value <> FData._IDCartao Then
                RaiseEvent AoAlterar()
            End If
            FData._IDCartao = value
        End Set
    End Property
    '
    '--- Propriedade Cartao
    Public Property Cartao() As String
        Get
            Return FData._Cartao
        End Get
        Set(ByVal value As String)
            If value <> FData._Cartao Then
                RaiseEvent AoAlterar()
            End If
            FData._Cartao = value
        End Set
    End Property
    '
    '--- Propriedade Parcelas
    Public Property Parcelas() As Byte?
        Get
            Return FData._Parcelas
        End Get
        Set(ByVal value As Byte?)
            If value <> IIf(IsNothing(FData._Parcelas), -1, FData._Parcelas) Then
                RaiseEvent AoAlterar()
            End If
            FData._Parcelas = value
        End Set
    End Property
    '
    '--- Propriedade Comissao
    Public Property Comissao() As Decimal?
        Get
            Return FData._Comissao
        End Get
        Set(ByVal value As Decimal?)
            If value <> IIf(IsNothing(FData._Comissao), -1, FData._Comissao) Then
                RaiseEvent AoAlterar()
            End If
            FData._Comissao = value
        End Set
    End Property
    '
    '--- Propriedade NoDias
    Public Property NoDias() As Byte?
        Get
            Return FData._NoDias
        End Get
        Set(ByVal value As Byte?)
            If value <> IIf(IsNothing(FData._NoDias), -1, FData._NoDias) Then
                RaiseEvent AoAlterar()
            End If
            FData._NoDias = value
        End Set
    End Property
    '
    '--- Propriedade Ativo
    Public Property Ativo() As Boolean
        Get
            Return FData._Ativo
        End Get
        Set(ByVal value As Boolean)
            If value <> FData._Ativo Then
                RaiseEvent AoAlterar()
            End If
            FData._Ativo = value
        End Set
    End Property
    '
#End Region
    '
End Class