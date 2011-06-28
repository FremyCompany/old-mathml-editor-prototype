Public Class GlyphChildrenHelper : Inherits ChildrenHelper

    Public Sub New(This As MathElement)
        MyBase.New(This)
    End Sub

    Public Function InvalidCall() As Object
        Throw New NotSupportedException("This element don't accept children. Please refer to the CanHave property before calling the Children methods.")
    End Function

    Protected Overrides Sub Add_Internal(NewChild As MathElement)
        InvalidCall()
    End Sub

    Public Overrides Function After(OldChild As MathElement) As MathElement
        Return InvalidCall()
    End Function

    Public Overrides Function Before(OldChild As MathElement) As MathElement
        Return InvalidCall()
    End Function

    Public Overrides Function IndexOf(Element As MathElement) As Integer
        Return -1
    End Function

    Public Overrides ReadOnly Property Count As Integer
        Get
            Return 0
        End Get
    End Property

    Public Overrides ReadOnly Property CanAdd As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property CanHave As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property CanRemove As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property ElementType As MathElement.Type
        Get
            Return MathElement.Type.Glyph
        End Get
    End Property

    Public Overrides ReadOnly Property First As MathElement
        Get
            Return InvalidCall()
        End Get
    End Property

    Protected Overrides Sub InsertAfter_Internal(NewChild As MathElement, OldChild As MathElement)
        InvalidCall()
    End Sub

    Protected Overrides Sub Remove_Internal(OldChild As MathElement)
        InvalidCall()
    End Sub

    Protected Overrides Function Contains_Internal(Element As MathElement) As Boolean
        Return False
    End Function

    Public Overrides ReadOnly Property Last As MathElement
        Get
            Return Nothing
        End Get
    End Property

End Class