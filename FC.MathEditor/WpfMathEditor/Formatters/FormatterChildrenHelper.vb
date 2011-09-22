
Public Class FormatterChildrenHelper : Inherits ChildrenHelper

    Public Sub New(ByVal El As MathElement)
        MyBase.New(El)
    End Sub

    Protected Frozen As Boolean = False
    Public ReadOnly Property IsFrozen As Boolean
        Get
            Return Frozen
        End Get
    End Property

    Public ReadOnly Property IsNotFrozen As Boolean
        Get
            Return Not Frozen
        End Get
    End Property

    Public Sub Freeze()
        Frozen = True
    End Sub

    Public Overrides ReadOnly Property CanHave As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property CanInsert As Boolean
        Get
            Return IsNotFrozen
        End Get
    End Property

    Public Overrides ReadOnly Property CanRemove As Boolean
        Get
            Return IsNotFrozen
        End Get
    End Property

    Public Overrides ReadOnly Property CanReplace As Boolean
        Get
            Return True
        End Get
    End Property

    Protected Overrides Sub ValidateNewElement_Internal(ByVal NewElement As MathElement)
        MyBase.ValidateNewElement_Internal(NewElement)
        If Not (TypeOf NewElement Is RowLayoutEngine) Then Throw New ArgumentException("Une fraction ne peut avoir que des RowLayoutEngine pour enfant.")
    End Sub

    Public Overrides ReadOnly Property CanSwap As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property ElementType As MathElement.Type
        Get
            Return MathElement.Type.Formatter
        End Get
    End Property
End Class
