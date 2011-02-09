Public Class GlyphChildrenHelper : Inherits ChildrenHelper

    Public Function InvalidCall() As Object
        Throw New NotSupportedException("This element don't accept children. Please refer to the CanHave property before calling the Children methods.")
    End Function

    Public Overrides Sub Add(ByVal NewChild As MathElement)
        InvalidCall()
    End Sub

    Public Overrides Function After(ByVal OldChild As MathElement) As MathElement
        Return InvalidCall()
    End Function

    Public Overrides Function Before(ByVal OldChild As MathElement) As MathElement
        Return InvalidCall()
    End Function

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

    Public Overrides Sub InsertAfter(ByVal NewChild As MathElement, ByVal OldChild As MathElement)
        InvalidCall()
    End Sub

    Public Overrides Sub Remove_Internal(ByVal OldChild As MathElement)
        InvalidCall()
    End Sub

End Class