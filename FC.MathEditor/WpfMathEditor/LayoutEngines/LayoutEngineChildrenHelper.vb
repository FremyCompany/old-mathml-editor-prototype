Public Class LayoutEngineChildrenHelper : Inherits ChildrenHelper

    Public Sub New(This As MathElement)
        MyBase.New(This)
    End Sub

    Public Overrides ReadOnly Property CanHave As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property CanInsert As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property CanRemove As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property CanSwap As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property ElementType As MathElement.Type
        Get
            Return MathElement.Type.LayoutEngine
        End Get
    End Property

End Class
