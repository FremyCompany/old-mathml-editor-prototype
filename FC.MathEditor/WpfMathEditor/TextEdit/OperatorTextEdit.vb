﻿Public Class OperatorTextEdit : Inherits TextEdit

    Public Sub New()
        Me.FontStyle = DefaultFontStyle
    End Sub

    Public Sub New(Children As IEnumerable(Of MathElement))
        Call Me.New()
        For Each C In Children
            Me.Children.Add(C)
        Next
    End Sub

    Public Overrides Function Clone_Internal(Optional ByVal CloneChildren As Boolean = True) As MathElement
        Dim Clone As New OperatorTextEdit()

        If CloneChildren Then
            For Each C In Children
                Clone.Children.Add(C.Clone())
            Next
        End If

        Return Clone
    End Function

    Public Overrides ReadOnly Property ElementName As String
        Get
            Return "mo"
        End Get
    End Property

    Public Overrides Function IsAccepted(C As Integer, IsFirst As Boolean) As Boolean
        Return IsFirst
    End Function

    Public Overrides ReadOnly Property EatInputByDefault As Boolean
        Get
            Return False
        End Get
    End Property

End Class
