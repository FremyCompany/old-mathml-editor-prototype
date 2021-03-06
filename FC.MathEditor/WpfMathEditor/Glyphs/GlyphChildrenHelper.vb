﻿Public Class GlyphChildrenHelper : Inherits ChildrenHelper

    Public Sub New(This As MathElement)
        MyBase.New(This)
    End Sub

    Public Function InvalidCall() As Object
        Throw New NotSupportedException("This element don't accept children. Please refer to the CanHave property before calling the Children methods.")
    End Function

    Public Overrides ReadOnly Property CanHave As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property CanInsert As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property CanRemove As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property CanSwap As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property ElementType As MathElement.Type
        Get
            Return MathElement.Type.Glyph
        End Get
    End Property

End Class